using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace JiraTicketStats
{
    public partial class CaseStats : Form
    {
        // stores the optional second CSV path (used to recalculate the right-side stats)
        private string _secondCsvPath;

        public CaseStats()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtResults.ReadOnly = true;
            txtResults.Multiline = true;
            txtResults.ScrollBars = ScrollBars.Vertical;

            // Optional side textboxes (if present in the Designer) are set up safely
            var saved = FindTextBox("txtSavedResults");
            if (saved != null)
            {
                saved.ReadOnly = true;
                saved.Multiline = true;
                saved.ScrollBars = ScrollBars.Vertical;
            }

            var compare = FindTextBox("txtCompareResults");
            if (compare != null)
            {
                compare.ReadOnly = true;
                compare.Multiline = true;
                compare.ScrollBars = ScrollBars.Vertical;
            }

            chkExcludeIncidents.Checked = true;
            chkExcludeNoActionDuplicate.Checked = true;
            chkExcludeReopened.Checked = true;

            // Ensure progress bar is hidden until work starts
            progressBar1.Visible = false;
            progressBar1.Style = ProgressBarStyle.Blocks;

            // ensure second path cleared on startup
            _secondCsvPath = null;
            txtSavedResults2.Text = string.Empty;
        }

        private async void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Jira CSV File";
                ofd.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = ofd.FileName;

                    // Automatically calculate stats after selecting a file.
                    try
                    {
                        btnCalculate_Click(this, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to start calculation: " + ex.Message);
                    }
                }
            }
        }

        private async void btnCalculate_Click(object sender, EventArgs e)
        {
            txtResults.Clear();

            if (string.IsNullOrWhiteSpace(txtFilePath.Text) || !File.Exists(txtFilePath.Text))
            {
                MessageBox.Show("Please choose a valid CSV file first.");
                return;
            }

            // If file isn't CSV-like, treat it as a saved-stats/text file and load its content instead of parsing CSV.
            if (!IsCsvLike(txtFilePath.Text))
            {
                try
                {
                    string content = File.ReadAllText(txtFilePath.Text);
                    txtResults.Text = content ?? string.Empty;
                    // Clear any stored second CSV path since this is a raw stats file
                    _secondCsvPath = null;
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to read file: " + ex.Message);
                    return;
                }
            }

            // Capture the checkbox settings and call the reusable builder
            bool excludeReopened = chkExcludeReopened.Checked;
            bool excludeIncidents = chkExcludeIncidents.Checked;
            bool excludeNoActionDuplicate = chkExcludeNoActionDuplicate.Checked;

            try
            {
                // Recalculate main (left) results
                string resultText;
                try
                {
                    resultText = await BuildStatsTextAsync(txtFilePath.Text, excludeReopened, excludeIncidents, excludeNoActionDuplicate);
                    txtResults.Text = resultText;
                }
                catch (InvalidDataException)
                {
                    // IsCsvLike gave a false positive; treat file as saved-stats/text file
                    try
                    {
                        string content = File.ReadAllText(txtFilePath.Text);
                        txtResults.Text = content ?? string.Empty;
                        _secondCsvPath = null;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to read file after failed CSV parse: " + ex.Message);
                        return;
                    }
                }

                // Decide which second-file path to use. Only accept .csv for recalculation.
                string candidate = (txtSavedResults2.Text ?? string.Empty).Trim();

                // allow quoted paths
                if (candidate.Length >= 2 && candidate[0] == '"' && candidate[candidate.Length - 1] == '"')
                    candidate = candidate.Substring(1, candidate.Length - 2).Trim();

                string pathToUse = null;

                // Accept candidate only if it exists and has .csv extension
                if (!string.IsNullOrEmpty(candidate) && File.Exists(candidate)
                    && string.Equals(Path.GetExtension(candidate), ".csv", StringComparison.OrdinalIgnoreCase))
                {
                    pathToUse = candidate;
                    _secondCsvPath = candidate; // persist choice
                    txtSavedResults2.Text = candidate;
                }
                // Otherwise fall back to previously stored second CSV path (must also be .csv)
                else if (!string.IsNullOrWhiteSpace(_secondCsvPath) && File.Exists(_secondCsvPath)
                    && string.Equals(Path.GetExtension(_secondCsvPath), ".csv", StringComparison.OrdinalIgnoreCase))
                {
                    pathToUse = _secondCsvPath;
                    txtSavedResults2.Text = _secondCsvPath;
                }
                else
                {
                    // Not a CSV — ensure we do not treat .txt/.stats as CSV
                    // Clear stored CSV path so future attempts re-evaluate txtSavedResults2
                    _secondCsvPath = null;
                }

                // Recalculate right-side results if we have a valid CSV path
                if (!string.IsNullOrEmpty(pathToUse))
                {
                    try
                    {
                        var savedResultText = await BuildStatsTextAsync(pathToUse, excludeReopened, excludeIncidents, excludeNoActionDuplicate);
                        var savedBox = FindTextBox("txtSavedResults");
                        if (savedBox != null)
                            savedBox.Text = savedResultText;
                        else
                            ShowLongTextInDialog("Calculated Saved Results", savedResultText);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to recalculate saved results: " + ex.Message);
                    }
                }
                else
                {
                    // If the user pointed the small textbox at a .txt/.stats snapshot, load it as raw text (no recalculation).
                    if (!string.IsNullOrEmpty(candidate) && File.Exists(candidate))
                    {
                        var ext = Path.GetExtension(candidate);
                        if (string.Equals(ext, ".txt", StringComparison.OrdinalIgnoreCase) || string.Equals(ext, ".stats", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                string content = File.ReadAllText(candidate);
                                var savedBox = FindTextBox("txtSavedResults");
                                if (savedBox != null)
                                    savedBox.Text = content;
                                else
                                    ShowLongTextInDialog("Loaded Saved Results", content);

                                // keep the small textbox showing the snapshot path but do not persist as CSV
                                _secondCsvPath = null;
                                txtSavedResults2.Text = candidate;
                                // After loading a saved snapshot, if a comparison exists, update it
                                UpdateComparisonIfPresent();
                                return;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Failed to load saved stats text: " + ex.Message);
                            }
                        }
                    }

                    // Final fallback: nothing to recalc
                }

                // If a comparison was already present, update it to reflect the new recalculated stats.
                try
                {
                    UpdateComparisonIfPresent();
                }
                catch
                {
                    // Non-fatal; don't block the main workflow for comparison update failures.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading file: " + ex.Message);
            }
            finally
            {
                // reset progress bar and hide after completion (also handled in builder, but keep safe)
                try
                {
                    progressBar1.Value = 0;
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    progressBar1.MarqueeAnimationSpeed = 0;
                    progressBar1.Visible = false;
                }
                catch { }
            }
        }

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

        // Reusable: build stats text for a CSV file path using supplied filters.
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

        private List<TicketRecord> LoadCsv(string filePath)
        {
            var records = new List<TicketRecord>();

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;

                string[] headers = parser.ReadFields();

                if (headers == null || headers.Length == 0)
                    throw new InvalidDataException("CSV file contains no header row.");

                // Trim headers and use them for index lookup
                for (int i = 0; i < headers.Length; i++)
                {
                    if (headers[i] != null)
                        headers[i] = headers[i].Trim();
                }

                int createdIndex = FindColumnIndex(headers, "Created");
                int resolvedIndex = FindColumnIndex(headers, "Resolved");
                int assigneeIndex = FindColumnIndex(headers, "Assignee");
                int requestTypeIndex = FindColumnIndex(headers, "Custom Field (Request Type)");
                int serviceRequestIndex = FindColumnIndex(headers, "Custom Field (Service Request)");
                int componentIndex = FindColumnIndex(headers, "Service Request Component");
                int reopenedIndex = FindColumnIndex(headers, "Re-Opened");

                // It's reasonable to require Created and Resolved columns
                // if (createdIndex < 0 || resolvedIndex < 0)
                //   throw new InvalidDataException("CSV must contain 'Created' and 'Resolved' columns.");
                while (!parser.EndOfData)
                {
                    string[] fields = null;
                    try
                    {
                        fields = parser.ReadFields();
                    }
                    catch
                    {
                        // Malformed line - skip it
                        continue;
                    }

                    if (fields == null)
                        continue;

                    var record = new TicketRecord
                    {
                        CreatedRaw = GetField(fields, createdIndex),
                        ResolvedRaw = GetField(fields, resolvedIndex),
                        Assignee = GetField(fields, assigneeIndex),
                        RequestType = GetField(fields, requestTypeIndex),
                        ServiceRequestComponent = GetField(fields, componentIndex),
                        ServiceRequest = GetField(fields, serviceRequestIndex),
                        Reopened = GetField(fields, reopenedIndex)
                    };

                    records.Add(record);
                }
            }

            return records;
        }

        private static bool IsReopened(TicketRecord ticket)
        {
            return SafeLower(ticket.Reopened) == "yes";
        }

        private static bool IsIncident(TicketRecord ticket)
        {
            string requestType = SafeLower(ticket.RequestType);
            string serviceRequest = SafeLower(ticket.ServiceRequest);
            string component = SafeLower(ticket.ServiceRequestComponent);

            return (!string.IsNullOrEmpty(requestType) && requestType.Contains("incident"))
                || (!string.IsNullOrEmpty(serviceRequest) && serviceRequest.Contains("incident"))
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

        private static int FindColumnIndex(string[] headers, string name)
        {
            if (headers == null || name == null)
                return -1;

            for (int i = 0; i < headers.Length; i++)
            {
                if (!string.IsNullOrEmpty(headers[i]) &&
                    headers[i].IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                    return i;
            }

            return -1;
        }

        private static string GetField(string[] fields, int index)
        {
            if (index >= 0 && index < fields.Length)
                return (fields[index] ?? string.Empty).Trim();

            return string.Empty;
        }

        private static string SafeLower(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return value.Trim().ToLowerInvariant();
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

            // First try exact known formats (invariant)
            if (DateTime.TryParseExact(rawDate, formats, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out parsedDate))
            {
                return true;
            }

            // Fallback to general parse using current culture as a last resort
            return DateTime.TryParse(rawDate, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out parsedDate);
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

        // Updated: btnClear_Click now resets the entire form to its initial state.
        private void btnClear_Click(object sender, EventArgs e)
        {
            // Clear main inputs and outputs
            txtFilePath.Text = string.Empty;
            txtResults.Clear();

            // Safely clear optional side textboxes if present
            var saved = FindTextBox("txtSavedResults");
            if (saved != null)
                saved.Clear();

            var compare = FindTextBox("txtCompareResults");
            if (compare != null)
                compare.Clear();

            // Clear stored second-CSV path and its textbox
            _secondCsvPath = null;
            txtSavedResults2.Text = string.Empty;

            // Reset checkboxes to their default state (matches Form1_Load)
            chkExcludeIncidents.Checked = true;
            chkExcludeNoActionDuplicate.Checked = true;
            chkExcludeReopened.Checked = true;

            // Reset and hide progress bar
            try
            {
                progressBar1.Value = 0;
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.MarqueeAnimationSpeed = 0;
                progressBar1.Visible = false;
            }
            catch
            {
                // Ignore if progress bar not initialized yet
            }

            // Return focus to file path textbox
            txtFilePath.Focus();
        }

        // New: Save current displayed stats to a file
        private void btnSaveStats_Click(object sender, EventArgs e)
        {
            // Collect what's visible in the UI
            string current = txtResults.Text ?? string.Empty;
            var savedBox = FindTextBox("txtSavedResults");
            string saved = savedBox != null ? savedBox.Text : string.Empty;
            var compareBox = FindTextBox("txtCompareResults");
            string compare = compareBox != null ? compareBox.Text : string.Empty;

            if (string.IsNullOrWhiteSpace(current) && string.IsNullOrWhiteSpace(saved) && string.IsNullOrWhiteSpace(compare))
            {
                MessageBox.Show("There are no results to save. Calculate or load stats first.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Export Results";
                sfd.Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                sfd.DefaultExt = "txt";
                sfd.FileName = "JiraStats.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        bool asCsv = string.Equals(Path.GetExtension(sfd.FileName), ".csv", StringComparison.OrdinalIgnoreCase);
                        ExportStats(sfd.FileName, asCsv);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to save/export stats: " + ex.Message);
                    }
                }
            }
        }

        // Exports all visible box content and some context either as a readable text file or as a CSV.
        private void ExportStats(string filePath, bool asCsv)
        {
            // Gather current UI contents
            string current = txtResults.Text ?? string.Empty;
            var savedBox = FindTextBox("txtSavedResults");
            string saved = savedBox != null ? savedBox.Text : string.Empty;
            var compareBox = FindTextBox("txtCompareResults");
            string compare = compareBox != null ? compareBox.Text : string.Empty;

            string mainFilePath = txtFilePath.Text ?? string.Empty;
            string secondPath = txtSavedResults2.Text ?? string.Empty;

            string excludeReopened = chkExcludeReopened.Checked ? "True" : "False";
            string excludeIncidents = chkExcludeIncidents.Checked ? "True" : "False";
            string excludeDuplicates = chkExcludeNoActionDuplicate.Checked ? "True" : "False";

            if (asCsv)
            {
                // Simple CSV with Section,Value. Values are quoted and internal quotes are doubled.
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("Section,Value");

                sb.AppendLine("MainFilePath," + EscapeCsv(mainFilePath));
                sb.AppendLine("SecondFilePath," + EscapeCsv(secondPath));
                sb.AppendLine("ExcludeReopened," + EscapeCsv(excludeReopened));
                sb.AppendLine("ExcludeIncidents," + EscapeCsv(excludeIncidents));
                sb.AppendLine("ExcludeDuplicateNoAction," + EscapeCsv(excludeDuplicates));
                sb.AppendLine(); // spacer

                sb.AppendLine("CurrentResults," + EscapeCsv(current));
                sb.AppendLine("SavedResults," + EscapeCsv(saved));
                sb.AppendLine("ComparisonResults," + EscapeCsv(compare));

                File.WriteAllText(filePath, sb.ToString(), System.Text.Encoding.UTF8);
            }
            else
            {
                // Human readable text export with sections
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                sb.AppendLine("Jira Ticket Stats Export");
                sb.AppendLine("------------------------");
                sb.AppendLine("Export Time: " + DateTime.Now.ToString("u"));
                sb.AppendLine();
                sb.AppendLine("Main File Path: " + mainFilePath);
                sb.AppendLine("Second File Path: " + secondPath);
                sb.AppendLine();
                sb.AppendLine("Filters:");
                sb.AppendLine(" - Exclude Reopened: " + excludeReopened);
                sb.AppendLine(" - Exclude Incidents: " + excludeIncidents);
                sb.AppendLine(" - Exclude Duplicate/No-Action: " + excludeDuplicates);
                sb.AppendLine();
                sb.AppendLine("=== Current Results ===");
                sb.AppendLine(current);
                sb.AppendLine();
                sb.AppendLine("=== Saved Results ===");
                sb.AppendLine(saved);
                sb.AppendLine();
                sb.AppendLine("=== Comparison ===");
                sb.AppendLine(compare);

                File.WriteAllText(filePath, sb.ToString(), System.Text.Encoding.UTF8);
            }
        }

        // Helper to escape CSV fields (wrap in double quotes, double any internal quotes)
        private static string EscapeCsv(string value)
        {
            if (value == null)
                value = string.Empty;

            // Double up existing quotes
            string escaped = value.Replace("\"", "\"\"");

            // Wrap in quotes to preserve newlines and commas
            return "\"" + escaped + "\"";
        }

        // Load: supports saved stats (.stats/.txt) and CSV-like files.
        // If file is a CSV (or looks like CSV because header contains 'Created' and 'Resolved')
        // it's calculated using current checkbox filters and shown in txtSavedResults and the path stored.
        private async void btnLoadStats_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select CSV File for Saved Results";
                ofd.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                ofd.CheckFileExists = true;
                ofd.Multiselect = false;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                string path = ofd.FileName;

                // Enforce CSV-only for the second file
                if (!string.Equals(Path.GetExtension(path), ".csv", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Please select a .csv file for the second file. .txt/.stats snapshots are not accepted for recalculation.", "Select CSV Only", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                try
                {
                    bool excludeReopened = chkExcludeReopened.Checked;
                    bool excludeIncidents = chkExcludeIncidents.Checked;
                    bool excludeNoActionDuplicate = chkExcludeNoActionDuplicate.Checked;

                    var savedBox = FindTextBox("txtSavedResults");
                    var txt = await BuildStatsTextAsync(path, excludeReopened, excludeIncidents, excludeNoActionDuplicate);

                    if (savedBox == null)
                        ShowLongTextInDialog("Calculated Stats (loaded CSV)", txt);
                    else
                        savedBox.Text = txt;

                    // store the CSV path so Recalculate will update this saved-results file as well
                    _secondCsvPath = path;
                    txtSavedResults2.Text = path;

                    // If a comparison is present, update it to reflect the newly loaded saved results.
                    try
                    {
                        UpdateComparisonIfPresent();
                    }
                    catch
                    {
                        // ignore
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to calculate stats from CSV: " + ex.Message);
                }
            }
        }

        // Compare current results (txtResults) to loaded saved results (txtSavedResults).
        private void btnCompare_Click(object sender, EventArgs e)
        {
            string current = txtResults.Text;
            var savedBox = FindTextBox("txtSavedResults");
            string saved = savedBox != null ? savedBox.Text : string.Empty;

            if (string.IsNullOrWhiteSpace(current))
            {
                MessageBox.Show("No current stats to compare. Calculate stats first.");
                return;
            }

            if (string.IsNullOrWhiteSpace(saved))
            {
                MessageBox.Show("No saved stats loaded. Load a saved stats file first.");
                return;
            }

            var curSummary = ParseStats(current);
            var savedSummary = ParseStats(saved);

            if (curSummary == null || savedSummary == null)
            {
                MessageBox.Show("Unable to parse one or both stats blocks. Make sure they are produced by this tool.");
                return;
            }

            string compare = BuildComparisonText(curSummary, savedSummary);

            var compareBox = FindTextBox("txtCompareResults");
            if (compareBox != null)
            {
                compareBox.Text = compare;
            }
            else
            {
                ShowLongTextInDialog("Stats Comparison", compare);
            }
        }

        // Helper: locate textbox by name safely (works even if controls are added in Designer)
        private TextBox FindTextBox(string name)
        {
            var found = this.Controls.Find(name, true);
            foreach (var c in found)
            {
                var tb = c as TextBox;
                if (tb != null)
                    return tb;
            }
            return null;
        }

        // Helper: show long text in modal dialog
        private void ShowLongTextInDialog(string title, string text)
        {
            using (Form dlg = new Form())
            {
                dlg.Text = title;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.Width = 800;
                dlg.Height = 600;

                var tb = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Both,
                    Dock = DockStyle.Fill,
                    Text = text
                };

                dlg.Controls.Add(tb);
                dlg.ShowDialog(this);
            }
        }

        // Parsing a result block produced by this tool into a simple summary object.
        private StatsSummary ParseStats(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var s = new StatsSummary();

            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (line.StartsWith("Total Tickets Loaded:", StringComparison.OrdinalIgnoreCase))
                {
                    s.TotalTickets = TryParseIntFromLine(line);
                }
                else if (line.StartsWith("Average Close Time:", StringComparison.OrdinalIgnoreCase))
                {
                    s.AvgHours = TryParseHoursFromLine(line);
                }
                else if (line.StartsWith("Median Close Time:", StringComparison.OrdinalIgnoreCase))
                {
                    s.MedianHours = TryParseHoursFromLine(line);
                }
                else if (line.StartsWith("Fastest Close Time:", StringComparison.OrdinalIgnoreCase))
                {
                    s.MinHours = TryParseHoursFromLine(line);
                }
                else if (line.StartsWith("Slowest Close Time:", StringComparison.OrdinalIgnoreCase))
                {
                    s.MaxHours = TryParseHoursFromLine(line);
                }
                else if (line.StartsWith("Total Reopened Tickets:", StringComparison.OrdinalIgnoreCase))
                {
                    s.TotalReopened = TryParseIntFromLine(line);
                }
                else if (line.StartsWith("Bad/Missing Dates:", StringComparison.OrdinalIgnoreCase))
                {
                    s.SkippedBadDates = TryParseIntFromLine(line);
                }
                else if (line.StartsWith("Reopened Excluded:", StringComparison.OrdinalIgnoreCase))
                {
                    s.SkippedReopened = TryParseIntFromLine(line);
                }
                else if (line.StartsWith("Incidents Excluded:", StringComparison.OrdinalIgnoreCase))
                {
                    s.SkippedIncidents = TryParseIntFromLine(line);
                }
                else if (line.StartsWith("Duplicate / No Action Excluded:", StringComparison.OrdinalIgnoreCase))
                {
                    s.SkippedDuplicateNoAction = TryParseIntFromLine(line);
                }
            }

            // Basic validation: at least one numeric field must be set
            if (s.TotalTickets == 0 && s.AvgHours == 0 && s.MedianHours == 0 && s.MaxHours == 0 && s.MinHours == 0)
                return null;

            return s;
        }

        private int TryParseIntFromLine(string line)
        {
            int colon = line.IndexOf(':');
            if (colon < 0) return 0;
            string part = line.Substring(colon + 1).Trim();
            int v;
            if (int.TryParse(part, NumberStyles.Integer, CultureInfo.CurrentCulture, out v))
                return v;
            return 0;
        }

        // Parses "X.XX hours" or "Y.YY days" into hours as double.
        private double TryParseHoursFromLine(string line)
        {
            int colon = line.IndexOf(':');
            if (colon < 0) return 0;
            string part = line.Substring(colon + 1).Trim();

            // part looks like "5.50 hours" or "2.00 days"
            var tokens = part.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) return 0;

            double value;
            if (!double.TryParse(tokens[0], NumberStyles.Number, CultureInfo.CurrentCulture, out value))
                return 0;

            if (tokens.Length > 1 && tokens[1].StartsWith("day", StringComparison.OrdinalIgnoreCase))
                return value * 24.0;

            // default hours
            return value;
        }

        private string BuildComparisonText(StatsSummary current, StatsSummary saved)
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("Comparison: Current vs Saved");
            sb.AppendLine("---------------------------");
            sb.AppendFormat("Total Tickets: {0} vs {1}    Diff: {2}", current.TotalTickets, saved.TotalTickets, current.TotalTickets - saved.TotalTickets);
            sb.AppendLine();
            sb.AppendFormat("Average Close Time: {0:F2} h vs {1:F2} h    Diff: {2:F2} h", current.AvgHours, saved.AvgHours, current.AvgHours - saved.AvgHours);
            sb.AppendLine();
            sb.AppendFormat("Median Close Time: {0:F2} h vs {1:F2} h    Diff: {2:F2} h", current.MedianHours, saved.MedianHours, current.MedianHours - saved.MedianHours);
            sb.AppendLine();
            sb.AppendFormat("Fastest Close Time: {0:F2} h vs {1:F2} h    Diff: {2:F2} h", current.MinHours, saved.MinHours, current.MinHours - saved.MinHours);
            sb.AppendLine();
            sb.AppendFormat("Slowest Close Time: {0:F2} h vs {1:F2} h    Diff: {2:F2} h", current.MaxHours, saved.MaxHours, current.MaxHours - saved.MaxHours);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendFormat("Total Reopened: {0} vs {1}    Diff: {2}", current.TotalReopened, saved.TotalReopened, current.TotalReopened - saved.TotalReopened);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Rows Skipped (current vs saved):");
            sb.AppendFormat("Bad/Missing Dates: {0} vs {1}    Diff: {2}", current.SkippedBadDates, saved.SkippedBadDates, current.SkippedBadDates - saved.SkippedBadDates);
            sb.AppendLine();
            sb.AppendFormat("Reopened Excluded: {0} vs {1}    Diff: {2}", current.SkippedReopened, saved.SkippedReopened, current.SkippedReopened - saved.SkippedReopened);
            sb.AppendLine();
            sb.AppendFormat("Incidents Excluded: {0} vs {1}    Diff: {2}", current.SkippedIncidents, saved.SkippedIncidents, current.SkippedIncidents - saved.SkippedIncidents);
            sb.AppendLine();
            sb.AppendFormat("Duplicate / No Action Excluded: {0} vs {1}    Diff: {2}", current.SkippedDuplicateNoAction, saved.SkippedDuplicateNoAction, current.SkippedDuplicateNoAction - saved.SkippedDuplicateNoAction);
            sb.AppendLine();

            return sb.ToString();
        }

        // Add this method to the CaseStats class

        /// <summary>
        /// Attempts to extract an embedded source CSV path from a saved stats file.
        /// Looks for a line like: "# Source CSV: <path>" at the top of the file.
        /// </summary>
        /// <param name="statsFilePath">The path to the saved stats file.</param>
        /// <param name="sourceCsvPath">The extracted CSV path, or null if not found.</param>
        /// <returns>True if a source CSV path was found; otherwise, false.</returns>
        private bool TryExtractSourceCsv(string statsFilePath, out string sourceCsvPath)
        {
            sourceCsvPath = null;
            try
            {
                // Only read the first few lines for a marker
                foreach (var line in File.ReadLines(statsFilePath).Take(10))
                {
                    if (line.StartsWith("# Source CSV:", StringComparison.OrdinalIgnoreCase))
                    {
                        var path = line.Substring("# Source CSV:".Length).Trim();
                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            sourceCsvPath = path;
                            return true;
                        }
                    }
                }
            }
            catch
            {
                // Ignore errors, just return false
            }
            return false;
        }

        // New helper: if a comparison box exists and already contains a comparison,
        // update it by reparsing current & saved stats and rebuilding the comparison.
        private void UpdateComparisonIfPresent()
        {
            var compareBox = FindTextBox("txtCompareResults");
            if (compareBox == null)
                return;

            // only update if there is something currently being shown in the compare box
            if (string.IsNullOrWhiteSpace(compareBox.Text))
                return;

            string current = txtResults.Text;
            var savedBox = FindTextBox("txtSavedResults");
            string saved = savedBox != null ? savedBox.Text : string.Empty;

            // need both current and saved in order to rebuild comparison
            if (string.IsNullOrWhiteSpace(current) || string.IsNullOrWhiteSpace(saved))
                return;

            var curSummary = ParseStats(current);
            var savedSummary = ParseStats(saved);

            if (curSummary == null || savedSummary == null)
                return;

            string compare = BuildComparisonText(curSummary, savedSummary);
            compareBox.Text = compare;
        }

        // Simple container for parsed stats
        private class StatsSummary
        {
            public int TotalTickets { get; set; }
            public double AvgHours { get; set; }
            public double MedianHours { get; set; }
            public double MinHours { get; set; }
            public double MaxHours { get; set; }
            public int TotalReopened { get; set; }
            public int SkippedBadDates { get; set; }
            public int SkippedReopened { get; set; }
            public int SkippedIncidents { get; set; }
            public int SkippedDuplicateNoAction { get; set; }
        }
    }

    public class TicketRecord
    {
        public string CreatedRaw { get; set; }
        public string ResolvedRaw { get; set; }
        public string Assignee { get; set; }
        public string RequestType { get; set; }
        public string ServiceRequestComponent { get; set; }
        public string ServiceRequest { get; set; }

        public string Reopened { get; set; }
    }
}
