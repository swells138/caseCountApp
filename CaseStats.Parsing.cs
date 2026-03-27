using System;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace JiraTicketStats
{
    public partial class CaseStats
    {
        // Parsing a result block produced by this tool into an object.
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

            // at least one numeric field must be set
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

        // Build a compact plain-text comparison
        private string BuildCompactComparisonString(StatsSummary current, StatsSummary saved)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Differences (Current - Saved)");
            sb.AppendLine("----------------------------");
            sb.AppendFormat("Total Tickets: {0}", current.TotalTickets - saved.TotalTickets); sb.AppendLine();
            sb.AppendFormat("Average Close Time (h): {0:F2}", current.AvgHours - saved.AvgHours); sb.AppendLine();
            sb.AppendFormat("Median Close Time (h): {0:F2}", current.MedianHours - saved.MedianHours); sb.AppendLine();
            sb.AppendFormat("Fastest Close Time (h): {0:F2}", current.MinHours - saved.MinHours); sb.AppendLine();
            sb.AppendFormat("Slowest Close Time (h): {0:F2}", current.MaxHours - saved.MaxHours); sb.AppendLine();
            sb.AppendFormat("Total Reopened: {0}", current.TotalReopened - saved.TotalReopened); sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Rows Skipped Differences:");
            sb.AppendFormat("Bad/Missing Dates: {0}", current.SkippedBadDates - saved.SkippedBadDates); sb.AppendLine();
            sb.AppendFormat("Reopened Excluded: {0}", current.SkippedReopened - saved.SkippedReopened); sb.AppendLine();
            sb.AppendFormat("Incidents Excluded: {0}", current.SkippedIncidents - saved.SkippedIncidents); sb.AppendLine();
            sb.AppendFormat("Duplicate / No Action Excluded: {0}", current.SkippedDuplicateNoAction - saved.SkippedDuplicateNoAction); sb.AppendLine();

            return sb.ToString();
        }

        // Render the compact differences into the RichTextBox with color:
        // green -> improvement (better than saved), red -> worse, black -> unchanged.
        private void ShowCompactComparison(RichTextBox rtb, StatsSummary current, StatsSummary saved)
        {
            if (rtb == null) return;

            rtb.SuspendLayout();
            rtb.Clear();

            // Total Tickets: higher is better
            AppendLabelAndColoredValue(rtb, "Total Tickets: ", (current.TotalTickets - saved.TotalTickets), higherIsBetter: true);
            // Time metrics: lower is better
            AppendLabelAndColoredValue(rtb, "Average Close Time (h): ", (current.AvgHours - saved.AvgHours), higherIsBetter: false);
            AppendLabelAndColoredValue(rtb, "Median Close Time (h): ", (current.MedianHours - saved.MedianHours), higherIsBetter: false);
            AppendLabelAndColoredValue(rtb, "Fastest Close Time (h): ", (current.MinHours - saved.MinHours), higherIsBetter: false);
            AppendLabelAndColoredValue(rtb, "Slowest Close Time (h): ", (current.MaxHours - saved.MaxHours), higherIsBetter: false);
            // Total Reopened: lower is better
            AppendLabelAndColoredValue(rtb, "Total Reopened: ", (current.TotalReopened - saved.TotalReopened), higherIsBetter: false);

            rtb.AppendText(Environment.NewLine);
            rtb.AppendText("Rows Skipped Differences:" + Environment.NewLine);

            // Rows skipped: lower is better
            AppendLabelAndColoredValue(rtb, "  Bad/Missing Dates: ", (current.SkippedBadDates - saved.SkippedBadDates), higherIsBetter: false);
            AppendLabelAndColoredValue(rtb, "  Reopened Excluded: ", (current.SkippedReopened - saved.SkippedReopened), higherIsBetter: false);
            AppendLabelAndColoredValue(rtb, "  Incidents Excluded: ", (current.SkippedIncidents - saved.SkippedIncidents), higherIsBetter: false);
            AppendLabelAndColoredValue(rtb, "  Duplicate / No Action Excluded: ", (current.SkippedDuplicateNoAction - saved.SkippedDuplicateNoAction), higherIsBetter: false);

            rtb.ResumeLayout();
        }

        // Helper to append label and a colored numeric value. Works for int and double via overloads.
        private void AppendLabelAndColoredValue(RichTextBox rtb, string label, int diff, bool higherIsBetter)
        {
            rtb.SelectionColor = Color.Black;
            rtb.AppendText(label);
            var start = rtb.TextLength;
            rtb.AppendText((diff >= 0 ? "+" : "") + diff.ToString(CultureInfo.CurrentCulture));
            var len = rtb.TextLength - start;

            rtb.Select(start, len);
            bool improved = (higherIsBetter && diff > 0) || (!higherIsBetter && diff < 0);
            rtb.SelectionColor = improved ? Color.Green : (diff == 0 ? Color.Black : Color.Red);
            rtb.Select(rtb.TextLength, 0);
            rtb.AppendText(Environment.NewLine);
            rtb.SelectionColor = Color.Black;
        }

        private void AppendLabelAndColoredValue(RichTextBox rtb, string label, double diff, bool higherIsBetter)
        {
            rtb.SelectionColor = Color.Black;
            rtb.AppendText(label);
            var start = rtb.TextLength;
            rtb.AppendText((diff >= 0 ? "+" : "") + diff.ToString("F2", CultureInfo.CurrentCulture));
            var len = rtb.TextLength - start;

            rtb.Select(start, len);
            bool improved = (higherIsBetter && diff > 0.0) || (!higherIsBetter && diff < 0.0);
            rtb.SelectionColor = improved ? Color.Green : (Math.Abs(diff) < 1e-9 ? Color.Black : Color.Red);
            rtb.Select(rtb.TextLength, 0);
            rtb.AppendText(Environment.NewLine);
            rtb.SelectionColor = Color.Black;
        }

        // container for parsed stats
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
}