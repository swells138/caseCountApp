using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;

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
                // embed source CSV path (if any) so we can recall it later
                if (!string.IsNullOrWhiteSpace(mainFilePath))
                    sb.AppendLine("# Source CSV: " + mainFilePath);
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

        // --- Snapshot folder helpers to save/recall results easily ---

        // Standard folder to store snapshots: %LocalAppData%\JiraTicketStats\Snapshots
        private string GetSnapshotsFolder()
        {
            var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(baseDir, "JiraTicketStats", "Snapshots");
        }

        private void EnsureSnapshotsFolderExists()
        {
            var folder = GetSnapshotsFolder();
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        // Determine dominant month/year from CSV by counting resolved (or created) dates.
        // Returns label like "March2026" or null if it cannot be determined.
        private string GetDominantMonthYearLabelFromCsv(string csvPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(csvPath) || !File.Exists(csvPath))
                    return null;

                var records = LoadCsv(csvPath);
                if (records == null || records.Count == 0)
                    return null;

                var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                foreach (var r in records)
                {
                    DateTime dt;
                    // prefer Resolved date; fall back to Created
                    if (TryParseJiraDate(r.ResolvedRaw, out dt) || TryParseJiraDate(r.CreatedRaw, out dt))
                    {
                        // month name + year, e.g. March2026
                        string key = dt.ToString("MMMMyyyy", CultureInfo.CurrentCulture);
                        if (counts.ContainsKey(key))
                            counts[key]++;
                        else
                            counts[key] = 1;
                    }
                }

                if (counts.Count == 0)
                    return null;

                // pick the month-year with the highest count
                var dominant = counts.OrderByDescending(kv => kv.Value).First().Key;
                return dominant; // "March2026"
            }
            catch
            {
                return null;
            }
        }

        // Build a single .stats file content that contains either the current block or the saved block.
        // sourceCsvPathForEmbed will be embedded as "# Source CSV: <path>" if provided.
        private void ExportSingleStatsFile(string filePath, bool includeCurrentBlock, bool includeSavedBlock, string sourceCsvPathForEmbed = null)
        {
            // Gather current UI contents
            string current = txtResults.Text ?? string.Empty;
            var savedBox = FindTextBox("txtSavedResults");
            string saved = savedBox != null ? savedBox.Text : string.Empty;
            var compare = FindRichTextBox("txtCompareResults");
            string compareRtf = compare != null ? compare.Rtf : string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Jira Ticket Stats Export");
            sb.AppendLine("------------------------");
            sb.AppendLine("Export Time: " + DateTime.Now.ToString("u"));
            sb.AppendLine();

            if (!string.IsNullOrWhiteSpace(sourceCsvPathForEmbed))
                sb.AppendLine("# Source CSV: " + sourceCsvPathForEmbed);

            sb.AppendLine();
            sb.AppendLine("Filters:");
            sb.AppendLine(" - Exclude Reopened: " + (chkExcludeReopened.Checked ? "True" : "False"));
            sb.AppendLine(" - Exclude Incidents: " + (chkExcludeIncidents.Checked ? "True" : "False"));
            sb.AppendLine(" - Exclude Duplicate/No-Action: " + (chkExcludeNoActionDuplicate.Checked ? "True" : "False"));
            sb.AppendLine();

            if (includeCurrentBlock)
            {
                sb.AppendLine("=== Current Results ===");
                sb.AppendLine(current);
                sb.AppendLine();
            }

            if (includeSavedBlock)
            {
                sb.AppendLine("=== Saved Results ===");
                sb.AppendLine(saved);
                sb.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(compareRtf))
            {
                sb.AppendLine("=== Comparison ===");
                sb.AppendLine(compareRtf);
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        // Public: Save both current and saved results as individual files labeled by the month they represent.
        // Prompts the user if a target month file already exists: Yes => overwrite, No => skip that file, Cancel => abort overall save.
        public async Task<string[]> SaveSnapshotsByMonthAsync()
        {
            // We'll run dominant-month detection on background threads to avoid blocking UI.
            EnsureSnapshotsFolderExists();

            var savedPaths = new List<string>();

            // Determine current CSV label asynchronously
            string currentCsv = txtFilePath.Text;
            string currentLabel = null;
            if (!string.IsNullOrWhiteSpace(currentCsv) && File.Exists(currentCsv))
            {
                currentLabel = await Task.Run(() => GetDominantMonthYearLabelFromCsv(currentCsv));
            }

            // Determine saved CSV source (try _secondCsvPath, textBox1, embedded marker)
            string savedSource = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(_secondCsvPath) && File.Exists(_secondCsvPath))
                    savedSource = _secondCsvPath;
            }
            catch { }

            if (string.IsNullOrWhiteSpace(savedSource))
            {
                try
                {
                    var candidate = (textBox1 != null ? textBox1.Text : null);
                    if (!string.IsNullOrWhiteSpace(candidate) && File.Exists(candidate))
                    {
                        // if it's a .stats/.txt try to extract embedded CSV
                        if (string.Equals(Path.GetExtension(candidate), ".stats", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(Path.GetExtension(candidate), ".txt", StringComparison.OrdinalIgnoreCase))
                        {
                            string extSource;
                            if (TryExtractSourceCsv(candidate, out extSource) && !string.IsNullOrWhiteSpace(extSource) && File.Exists(extSource))
                                savedSource = extSource;
                            else if (string.Equals(Path.GetExtension(candidate), ".csv", StringComparison.OrdinalIgnoreCase))
                                savedSource = candidate;
                        }
                        else if (string.Equals(Path.GetExtension(candidate), ".csv", StringComparison.OrdinalIgnoreCase))
                        {
                            savedSource = candidate;
                        }
                    }
                }
                catch { }
            }

            // Asynchronously compute saved label
            string savedLabel = null;
            if (!string.IsNullOrWhiteSpace(savedSource) && File.Exists(savedSource))
            {
                savedLabel = await Task.Run(() => GetDominantMonthYearLabelFromCsv(savedSource));
            }
            else
            {
                // also attempt to find embedded "# Source CSV:" within the saved results textbox itself
                var savedBox = FindTextBox("txtSavedResults");
                if (savedBox != null)
                {
                    var lines = savedBox.Text?.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None) ?? new string[0];
                    foreach (var line in lines.Take(10))
                    {
                        if (line.StartsWith("# Source CSV:", StringComparison.OrdinalIgnoreCase))
                        {
                            var candidate = line.Substring("# Source CSV:".Length).Trim();
                            if (!string.IsNullOrWhiteSpace(candidate) && File.Exists(candidate))
                            {
                                savedSource = candidate;
                                savedLabel = await Task.Run(() => GetDominantMonthYearLabelFromCsv(savedSource));
                                break;
                            }
                        }
                    }
                }
            }

            // Build candidate filenames
            string currentFileName = !string.IsNullOrWhiteSpace(currentLabel)
                ? (currentLabel + "Stats.stats")
                : ("Current_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".stats");

            string savedFileName = !string.IsNullOrWhiteSpace(savedLabel)
                ? (savedLabel + "Stats.stats")
                : ("Saved_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".stats");

            // Full paths
            string currentFull = Path.Combine(GetSnapshotsFolder(), currentFileName);
            string savedFull = Path.Combine(GetSnapshotsFolder(), savedFileName);

            // Prompt/overwrite logic for current file
            if (File.Exists(currentFull))
            {
                var res = MessageBox.Show(
                    "A snapshot file already exists for the current month:\n\n" + currentFull + "\n\nOverwrite?",
                    "Overwrite Snapshot?",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (res == System.Windows.Forms.DialogResult.Cancel)
                {
                    // abort entirely
                    return savedPaths.ToArray();
                }
                if (res == System.Windows.Forms.DialogResult.No)
                {
                    // skip saving current file
                    currentFull = null;
                }
                // Yes => overwrite (fall through)
            }

            if (!string.IsNullOrWhiteSpace(currentFull))
            {
                try
                {
                    ExportSingleStatsFile(currentFull, includeCurrentBlock: true, includeSavedBlock: false, sourceCsvPathForEmbed: (File.Exists(currentCsv) ? currentCsv : null));
                    savedPaths.Add(currentFull);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save current snapshot: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Prompt/overwrite logic for saved file
            if (File.Exists(savedFull))
            {
                var res = MessageBox.Show(
                    "A snapshot file already exists for the saved month:\n\n" + savedFull + "\n\nOverwrite?",
                    "Overwrite Snapshot?",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (res == System.Windows.Forms.DialogResult.Cancel)
                {
                    // if user canceled here, we keep whatever was saved so far and return
                    return savedPaths.ToArray();
                }
                if (res == System.Windows.Forms.DialogResult.No)
                {
                    // skip saving saved file
                    savedFull = null;
                }
                // Yes => overwrite
            }

            if (!string.IsNullOrWhiteSpace(savedFull))
            {
                try
                {
                    ExportSingleStatsFile(savedFull, includeCurrentBlock: false, includeSavedBlock: true, sourceCsvPathForEmbed: (File.Exists(savedSource) ? savedSource : null));
                    savedPaths.Add(savedFull);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save saved-results snapshot: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return savedPaths.ToArray();
        }

        // Return snapshot files (full paths), newest first
        private string[] ListSnapshotFiles()
        {
            EnsureSnapshotsFolderExists();
            try
            {
                return Directory.GetFiles(GetSnapshotsFolder(), "*.stats")
                                .OrderByDescending(f => File.GetCreationTimeUtc(f))
                                .ToArray();
            }
            catch
            {
                return new string[0];
            }
        }

        // Async: Load a snapshot file into the "SavedResults" box (or show in dialog if no box).
        // If the snapshot embeds a source CSV and that CSV exists, recalculate stats from that CSV
        // using the current filter checkboxes (same behavior as loading the second CSV file).
        private async Task LoadSnapshotAsSavedAsync(string snapshotFilePath)
        {
            if (string.IsNullOrWhiteSpace(snapshotFilePath) || !File.Exists(snapshotFilePath))
                throw new FileNotFoundException("Snapshot file not found", snapshotFilePath);

            string content = File.ReadAllText(snapshotFilePath, Encoding.UTF8);

            // try to extract embedded source CSV path
            string sourceCsv;
            bool hasSource = TryExtractSourceCsv(snapshotFilePath, out sourceCsv);

            var savedBox = FindTextBox("txtSavedResults");

            if (hasSource && !string.IsNullOrWhiteSpace(sourceCsv) && File.Exists(sourceCsv)
                && string.Equals(Path.GetExtension(sourceCsv), ".csv", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    // Use current filter settings (same as when the second CSV is loaded)
                    bool excludeReopened = chkExcludeReopened.Checked;
                    bool excludeIncidents = chkExcludeIncidents.Checked;
                    bool excludeNoActionDuplicate = chkExcludeNoActionDuplicate.Checked;

                    var recalculated = await BuildStatsTextAsync(sourceCsv, excludeReopened, excludeIncidents, excludeNoActionDuplicate);

                    if (savedBox != null)
                        savedBox.Text = recalculated;
                    else
                        ShowLongTextInDialog("Calculated Stats (from snapshot source CSV)", recalculated);

                    // restore secondary CSV path so UI knows this came from a CSV
                    _secondCsvPath = sourceCsv;
                    textBox1.Text = sourceCsv;
                }
                catch (Exception)
                {
                    // fallback to showing the raw snapshot content if recalculation fails
                    if (savedBox != null)
                        savedBox.Text = content;
                    else
                        ShowLongTextInDialog("Loaded Saved Snapshot", content);

                    _secondCsvPath = null;
                    textBox1.Text = snapshotFilePath;
                }
            }
            else
            {
                // No embedded CSV or CSV not available: load snapshot text directly
                if (savedBox != null)
                    savedBox.Text = content;
                else
                    ShowLongTextInDialog("Loaded Saved Snapshot", content);

                _secondCsvPath = null;
                textBox1.Text = snapshotFilePath;
            }

            // Update comparison panel if present
            try
            {
                UpdateComparisonIfPresent();
            }
            catch
            {
            }
        }

        // Synchronous wrapper kept for backwards compatibility (calls async version and waits).
        // Prefer calling the async variant from UI code.
        private void LoadSnapshotAsSaved(string snapshotFilePath)
        {
            // Fire-and-wait on the async variant (UI callers should use the async API).
            try
            {
                LoadSnapshotAsSavedAsync(snapshotFilePath).GetAwaiter().GetResult();
            }
            catch
            {
            }
        }

        // Open the snapshots folder in Explorer for manual management
        private void OpenSnapshotsFolder()
        {
            try
            {
                EnsureSnapshotsFolderExists();
                var folder = GetSnapshotsFolder();
                var psi = new ProcessStartInfo
                {
                    FileName = folder,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch
            {
                // swallow errors; UI caller may show a MessageBox
            }
        }
    }
}