using System;
using System.IO;
using System.Text;
using System.Linq;

namespace JiraTicketStats
{
    public partial class CaseStats
    {
        // Exports all visible box content and some context either as a readable text file or as a CSV.
        private void ExportStats(string filePath, bool asCsv)
        {
            // Gather current UI contents
            string current = txtResults.Text ?? string.Empty;
            var savedBox = FindTextBox("txtSavedResults");
            string saved = savedBox != null ? savedBox.Text : string.Empty;
            var compare = FindRichTextBox("txtCompareResults");
            string compareRtf = compare != null ? compare.Rtf : string.Empty;

            string mainFilePath = txtFilePath.Text ?? string.Empty;
            string secondPath = textBox1.Text ?? string.Empty;

            string excludeReopened = chkExcludeReopened.Checked ? "True" : "False";
            string excludeIncidents = chkExcludeIncidents.Checked ? "True" : "False";
            string excludeDuplicates = chkExcludeNoActionDuplicate.Checked ? "True" : "False";

            if (asCsv)
            {
                // Simple CSV with Section,Value. Values are quoted and internal quotes are doubled.
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Section,Value");

                sb.AppendLine("MainFilePath," + EscapeCsv(mainFilePath));
                sb.AppendLine("SecondFilePath," + EscapeCsv(secondPath));
                sb.AppendLine("ExcludeReopened," + EscapeCsv(excludeReopened));
                sb.AppendLine("ExcludeIncidents," + EscapeCsv(excludeIncidents));
                sb.AppendLine("ExcludeDuplicateNoAction," + EscapeCsv(excludeDuplicates));
                sb.AppendLine(); // spacer

                sb.AppendLine("CurrentResults," + EscapeCsv(current));
                sb.AppendLine("SavedResults," + EscapeCsv(saved));
                sb.AppendLine("ComparisonResults," + EscapeCsv(compareRtf));

                File.WriteAllText(filePath, sb.ToString(), System.Text.Encoding.UTF8);
            }
            else
            {
                // Human readable text export with sections
                StringBuilder sb = new StringBuilder();

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
                sb.AppendLine(compareRtf);

                File.WriteAllText(filePath, sb.ToString(), System.Text.Encoding.UTF8);
            }
        }

        // Helper to escape CSV fields 
        private static string EscapeCsv(string value)
        {
            if (value == null)
                value = string.Empty;

            // Double up existing quotes
            string escaped = value.Replace("\"", "\"\"");

            // Wrap in quotes to preserve newlines and commas
            return "\"" + escaped + "\"";
        }

        /// Attempts to extract an embedded source CSV path from a saved stats file.
        /// Looks for a line like: "# Source CSV: <path>" at the top of the file.
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
    }
}