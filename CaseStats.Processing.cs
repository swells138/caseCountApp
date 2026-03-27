using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JiraTicketStats
{
    public partial class CaseStats
    {
        // detect whether a file is CSV-like: either extension .csv or header contains Created & Resolved
        private bool IsCsvLike(string path)
        {
            try
            {
                if (string.Equals(Path.GetExtension(path), ".csv", StringComparison.OrdinalIgnoreCase))
                    return true;

                // read only the first non-empty line
                var first = File.ReadLines(path).FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));
                if (string.IsNullOrEmpty(first))
                    return false;

                return first.IndexOf("Created", StringComparison.OrdinalIgnoreCase) >= 0
                    && first.IndexOf("Resolved", StringComparison.OrdinalIgnoreCase) >= 0;
            }
            catch
            {
                return false;
            }
        }

        //  build stats text for a CSV file path using supplied filters.
        // Returns the formatted stats string (same format as existing txtResults).
        private async Task<string> BuildStatsTextAsync(string csvPath, bool excludeReopened, bool excludeIncidents, bool excludeNoActionDuplicate)
        {
            if (string.IsNullOrWhiteSpace(csvPath) || !File.Exists(csvPath))
                throw new FileNotFoundException("CSV file not found.", csvPath);

            // Show marquee while the CSV is being read (parsing can take time).
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;
            progressBar1.Visible = true;

            // Load CSV on background thread
            List<TicketRecord> tickets = await Task.Run(() => LoadCsv(csvPath));

            if (tickets == null || tickets.Count == 0)
            {
                // reset progress bar
                try
                {
                    progressBar1.Value = 0;
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    progressBar1.MarqueeAnimationSpeed = 0;
                    progressBar1.Visible = false;
                }
                catch { }

                return "No rows were found in the file.";
            }

            // Prepare progress reporting
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = Math.Max(1, tickets.Count);
            progressBar1.Value = 0;

            var progress = new Progress<int>(inc =>
            {
                int newValue = progressBar1.Value + inc;
                progressBar1.Value = Math.Min(progressBar1.Maximum, newValue);
            });

            // Run the processing on a background thread and return the formatted string
            var result = await Task.Run(() =>
            {
                int skippedBadDates = 0;
                int skippedIncidents = 0;
                int skippedDuplicateNoAction = 0;
                int skippedReopened = 0;
                int totalReopened = 0;

                List<double> closeHours = new List<double>();

                DateTime startOfYear = new DateTime(DateTime.Now.Year, 1, 1);

                foreach (TicketRecord ticket in tickets)
                {
                    if (string.IsNullOrWhiteSpace(ticket.ResolvedRaw))
                    {
                        ((IProgress<int>)progress).Report(1);
                        continue;
                    }

                    DateTime createdDate;
                    DateTime resolvedDate;

                    bool createdOk = TryParseJiraDate(ticket.CreatedRaw, out createdDate);
                    bool resolvedOk = TryParseJiraDate(ticket.ResolvedRaw, out resolvedDate);

                    if (!createdOk || !resolvedOk)
                    {
                        skippedBadDates++;
                        ((IProgress<int>)progress).Report(1);
                        continue;
                    }

                    bool reopened = IsReopened(ticket);

                    if (reopened)
                    {
                        totalReopened++;
                        if (excludeReopened)
                        {
                            skippedReopened++;
                            ((IProgress<int>)progress).Report(1);
                            continue;
                        }
                    }

                    if (createdDate < startOfYear)
                    {
                        ((IProgress<int>)progress).Report(1);
                        continue;
                    }

                    if (excludeIncidents && IsIncident(ticket))
                    {
                        skippedIncidents++;
                        ((IProgress<int>)progress).Report(1);
                        continue;
                    }

                    if (excludeNoActionDuplicate && IsDuplicateOrNoAction(ticket))
                    {
                        skippedDuplicateNoAction++;
                        ((IProgress<int>)progress).Report(1);
                        continue;
                    }

                    double hours = (resolvedDate - createdDate).TotalHours;

                    if (hours < 0)
                    {
                        skippedBadDates++;
                        ((IProgress<int>)progress).Report(1);
                        continue;
                    }

                    closeHours.Add(hours);
                    ((IProgress<int>)progress).Report(1);
                }

                closeHours.Sort();

                double avg = closeHours.Count > 0 ? closeHours.Average() : 0;
                double median = closeHours.Count > 0 ? GetMedian(closeHours) : 0;
                double min = closeHours.Count > 0 ? closeHours.Min() : 0;
                double max = closeHours.Count > 0 ? closeHours.Max() : 0;

                // Build results string exactly as original
                if (closeHours.Count == 0)
                {
                    return "No valid resolved tickets were found after filtering.";
                }
                else
                {
                    return
                        "Ticket Close Time Results" + Environment.NewLine +
                        "-----------------------------" + Environment.NewLine +
                        "Total Tickets Loaded: " + tickets.Count + Environment.NewLine +
                        "Average Close Time: " + FormatTime(avg) + Environment.NewLine +
                        "Median Close Time: " + FormatTime(median) + Environment.NewLine +
                        "Fastest Close Time: " + FormatTime(min) + Environment.NewLine +
                        "Slowest Close Time: " + FormatTime(max) + Environment.NewLine +
                        Environment.NewLine +
                        "Total Reopened Tickets: " + totalReopened + Environment.NewLine +
                        Environment.NewLine +
                        "Rows Skipped" + Environment.NewLine +
                        "-----------" + Environment.NewLine +
                        "Bad/Missing Dates: " + skippedBadDates + Environment.NewLine +
                        "Reopened Excluded: " + skippedReopened + Environment.NewLine +
                        "Incidents Excluded: " + skippedIncidents + Environment.NewLine +
                        "Duplicate / No Action Excluded: " + skippedDuplicateNoAction;
                }
            });

            // reset progress bar
            try
            {
                progressBar1.Value = 0;
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.MarqueeAnimationSpeed = 0;
                progressBar1.Visible = false;
            }
            catch { }

            return result;
        }

        private static string FormatTime(double hours)
        {
            if (hours >= 24)
                return (hours / 24.0).ToString("F2", CultureInfo.CurrentCulture) + " days";

            return hours.ToString("F2", CultureInfo.CurrentCulture) + " hours";
        }

        private static double GetMedian(List<double> values)
        {
            int count = values.Count;

            if (count == 0)
                return 0.0;

            if ((count & 1) == 1)
                return values[count / 2];

            return (values[(count / 2) - 1] + values[count / 2]) / 2.0;
        }

        private static bool TryParseJiraDate(string rawDate, out DateTime parsedDate)
        {
            parsedDate = default(DateTime);

            if (string.IsNullOrWhiteSpace(rawDate))
                return false;

            string[] formats =
            {
                "dd/MMM/yy h:mm tt",
                "d/MMM/yy h:mm tt",
                "M/d/yyyy h:mm tt",
                "MM/dd/yyyy h:mm tt",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-ddTHH:mm:ss.fffK",
                "yyyy-MM-ddTHH:mm:ssK",
                "yyyy-MM-ddTHH:mm:ss.fff'Z'",
                "yyyy-MM-ddTHH:mm:ss'Z'"
            };

            // First try exact known formats
            if (DateTime.TryParseExact(rawDate, formats, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out parsedDate))
            {
                return true;
            }

            // Fallback to general parse using current culture as a last resort
            return DateTime.TryParse(rawDate, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out parsedDate);
        }

        private static bool IsReopened(TicketRecord ticket)
        {
            return SafeLower(ticket.Reopened) == "yes";
        }

        private static bool IsIncident(TicketRecord ticket)
        {
            string requestType = SafeLower(ticket.RequestType);
            string component = SafeLower(ticket.ServiceRequestComponent);

            return (!string.IsNullOrEmpty(requestType) && requestType.Contains("incident"))
                || (!string.IsNullOrEmpty(component) && component.Contains("incident"));
        }

        private static bool IsDuplicateOrNoAction(TicketRecord ticket)
        {
            string component = SafeLower(ticket.ServiceRequestComponent);
            return (!string.IsNullOrEmpty(component) && (
                   component.Contains("duplicate/no action needed")
                || component.Contains("duplicate")
                || component.Contains("no action")));
        }

        private static string SafeLower(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return value.Trim().ToLowerInvariant();
        }
    }
}