using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace JiraTicketStats
{
    public partial class CaseStats
    {
        private void Form1_Load(object sender, EventArgs e)
        {
            txtResults.ReadOnly = true;
            txtResults.Multiline = true;
            txtResults.ScrollBars = ScrollBars.Vertical;

            var saved = FindTextBox("txtSavedResults");
            if (saved != null)
            {
                saved.ReadOnly = true;
                saved.Multiline = true;
                saved.ScrollBars = ScrollBars.Vertical;
            }

            var compare = FindRichTextBox("txtCompareResults");
            if (compare != null)
            {
                compare.ReadOnly = true;
                compare.ScrollBars = RichTextBoxScrollBars.Vertical;
            }

            chkExcludeIncidents.Checked = true;
            chkExcludeNoActionDuplicate.Checked = true;
            chkExcludeReopened.Checked = true;

            progressBar1.Visible = false;
            progressBar1.Style = ProgressBarStyle.Blocks;

            _secondCsvPath = null;

            textBox1 = FindTextBox("txtSavedResults2") ?? this.txtSavedResults2 ?? FindTextBox("textBox1");
            if (textBox1 == null)
            {
                textBox1 = new TextBox { Name = "textBox1", Visible = false };
                this.Controls.Add(textBox1);
            }

            textBox1.Text = string.Empty;

            try
            {
                System.Diagnostics.Debug.WriteLine("==== TextBox visual properties at startup ====");
                System.Diagnostics.Debug.WriteLine("txtFilePath: BorderStyle=" + txtFilePath.BorderStyle + ", Multiline=" + txtFilePath.Multiline + ", Font=" + txtFilePath.Font.ToString() + ", Size=" + txtFilePath.Size + ", BackColor=" + txtFilePath.BackColor);

                var tbForDiag = textBox1 ?? txtSavedResults2;
                if (tbForDiag != null)
                {
                    System.Diagnostics.Debug.WriteLine("txtSavedResults2: BorderStyle=" + tbForDiag.BorderStyle + ", Multiline=" + tbForDiag.Multiline + ", Font=" + (tbForDiag.Font != null ? tbForDiag.Font.ToString() : "null") + ", Size=" + tbForDiag.Size + ", BackColor=" + tbForDiag.BackColor);

                    tbForDiag.Font = txtFilePath.Font;
                    tbForDiag.BorderStyle = txtFilePath.BorderStyle;
                    tbForDiag.Multiline = txtFilePath.Multiline;
                    tbForDiag.Size = txtFilePath.Size;
                    tbForDiag.MinimumSize = txtFilePath.MinimumSize;

                    if (textBox1 == null)
                        textBox1 = tbForDiag;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No secondary textbox found for diagnostics.");
                }

               System.Diagnostics.Debug.WriteLine("After normalization:");
                if (textBox1 != null)
                    System.Diagnostics.Debug.WriteLine("txtSavedResults2: BorderStyle=" + textBox1.BorderStyle + ", Multiline=" + textBox1.Multiline + ", Font=" + textBox1.Font.ToString() + ", Size=" + textBox1.Size + ", BackColor=" + textBox1.BackColor);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Diagnostics failed: " + ex.Message);
            }

            try
            {
                txtFilePath.Multiline = false;
                txtFilePath.BorderStyle = BorderStyle.FixedSingle;
                txtFilePath.BackColor = SystemColors.Window;
                txtFilePath.MinimumSize = new System.Drawing.Size(260, 24);
                txtFilePath.Size = new System.Drawing.Size(txtFilePath.Size.Width, Math.Max(24, txtFilePath.MinimumSize.Height));

                if (textBox1 != null)
                {
                    textBox1.Font = txtFilePath.Font;
                    textBox1.Multiline = txtFilePath.Multiline;
                    textBox1.BorderStyle = txtFilePath.BorderStyle;
                    textBox1.BackColor = txtFilePath.BackColor;
                    textBox1.MinimumSize = txtFilePath.MinimumSize;
                    textBox1.Size = txtFilePath.Size;
                }

                if (txtSavedResults2 != null && txtSavedResults2 != textBox1)
                {
                    txtSavedResults2.Font = txtFilePath.Font;
                    txtSavedResults2.Multiline = txtFilePath.Multiline;
                    txtSavedResults2.BorderStyle = txtFilePath.BorderStyle;
                    txtSavedResults2.BackColor = txtFilePath.BackColor;
                    txtSavedResults2.MinimumSize = txtFilePath.MinimumSize;
                    txtSavedResults2.Size = txtFilePath.Size;
                }

                if (btnBrowse != null)
                    btnBrowse.Select();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Visual normalization failed: " + ex.Message);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Jira CSV File";
                ofd.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = ofd.FileName;

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

            if (!IsCsvLike(txtFilePath.Text))
            {
                try
                {
                    string content = File.ReadAllText(txtFilePath.Text);
                    txtResults.Text = content ?? string.Empty;
                    _secondCsvPath = null;
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to read file: " + ex.Message);
                    return;
                }
            }

            bool excludeReopened = chkExcludeReopened.Checked;
            bool excludeIncidents = chkExcludeIncidents.Checked;
            bool excludeNoActionDuplicate = chkExcludeNoActionDuplicate.Checked;

            try
            {
                string resultText;
                try
                {
                    resultText = await BuildStatsTextAsync(txtFilePath.Text, excludeReopened, excludeIncidents, excludeNoActionDuplicate);
                    txtResults.Text = resultText;
                }
                catch (InvalidDataException)
                {
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

                string candidate = (textBox1.Text ?? string.Empty).Trim();

                if (candidate.Length >= 2 && candidate[0] == '"' && candidate[candidate.Length - 1] == '"')
                    candidate = candidate.Substring(1, candidate.Length - 2).Trim();

                string pathToUse = null;

                if (!string.IsNullOrEmpty(candidate) && File.Exists(candidate)
                    && string.Equals(Path.GetExtension(candidate), ".csv", StringComparison.OrdinalIgnoreCase))
                {
                    pathToUse = candidate;
                    _secondCsvPath = candidate;
                    textBox1.Text = candidate;
                }
                else if (!string.IsNullOrWhiteSpace(_secondCsvPath) && File.Exists(_secondCsvPath)
                    && string.Equals(Path.GetExtension(_secondCsvPath), ".csv", StringComparison.OrdinalIgnoreCase))
                {
                    pathToUse = _secondCsvPath;
                    textBox1.Text = _secondCsvPath;
                }
                else
                {
                    _secondCsvPath = null;
                }

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

                                _secondCsvPath = null;
                                textBox1.Text = candidate;
                                UpdateComparisonIfPresent();
                                return;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Failed to load saved stats text: " + ex.Message);
                            }
                        }
                    }
                }

                try
                {
                    UpdateComparisonIfPresent();
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading file: " + ex.Message);
            }
            finally
            {
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtFilePath.Text = string.Empty;
            txtResults.Clear();

            var saved = FindTextBox("txtSavedResults");
            if (saved != null)
                saved.Clear();

            var compare = FindRichTextBox("txtCompareResults");
            if (compare != null)
                compare.Clear();

            _secondCsvPath = null;
            textBox1.Text = string.Empty;

            chkExcludeIncidents.Checked = true;
            chkExcludeNoActionDuplicate.Checked = true;
            chkExcludeReopened.Checked = true;

            try
            {
                progressBar1.Value = 0;
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.MarqueeAnimationSpeed = 0;
                progressBar1.Visible = false;
            }
            catch
            {
            }

            txtFilePath.Focus();
        }

        private void btnSaveStats_Click(object sender, EventArgs e)
        {
            string current = txtResults.Text ?? string.Empty;
            var savedBox = FindTextBox("txtSavedResults");
            string saved = savedBox != null ? savedBox.Text : string.Empty;
            var compare = FindRichTextBox("txtCompareResults");
            string compareRtf = compare != null ? compare.Rtf : string.Empty;

            if (string.IsNullOrWhiteSpace(current) && string.IsNullOrWhiteSpace(saved) && string.IsNullOrWhiteSpace(compareRtf))
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

                    _secondCsvPath = path;
                    textBox1.Text = path;

                    try
                    {
                        UpdateComparisonIfPresent();
                    }
                    catch
                    {
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to calculate stats from CSV: " + ex.Message);
                }
            }
        }

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

            var compare = FindRichTextBox("txtCompareResults");
            if (compare != null)
            {
                ShowCompactComparison(compare, curSummary, savedSummary);
            }
            else
            {
                ShowLongTextInDialog("Stats Comparison", BuildCompactComparisonString(curSummary, savedSummary));
            }
        }

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

        private RichTextBox FindRichTextBox(string name)
        {
            var found = this.Controls.Find(name, true);
            foreach (var c in found)
            {
                var rtb = c as RichTextBox;
                if (rtb != null)
                    return rtb;
            }
            return null;
        }

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

        private void UpdateComparisonIfPresent()
        {
            var compare = FindRichTextBox("txtCompareResults");
            if (compare == null)
                return;

            string current = txtResults.Text;
            var savedBox = FindTextBox("txtSavedResults");
            string saved = savedBox != null ? savedBox.Text : string.Empty;

            if (string.IsNullOrWhiteSpace(current) || string.IsNullOrWhiteSpace(saved))
                return;

            var curSummary = ParseStats(current);
            var savedSummary = ParseStats(saved);

            if (curSummary == null || savedSummary == null)
                return;

            ShowCompactComparison(compare, curSummary, savedSummary);
        }
    }
}