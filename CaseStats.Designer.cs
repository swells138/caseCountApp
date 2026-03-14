namespace JiraTicketStats
{
    partial class CaseStats
    {
        private System.ComponentModel.IContainer components = null;

        // Controls
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRoot;
        private System.Windows.Forms.TableLayoutPanel leftPanel;
        private System.Windows.Forms.TableLayoutPanel rightPanel;

        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSavedResults2;
        private System.Windows.Forms.Button btnLoadStats;
        private System.Windows.Forms.CheckBox chkExcludeIncidents;
        private System.Windows.Forms.CheckBox chkExcludeNoActionDuplicate;
        private System.Windows.Forms.CheckBox chkExcludeReopened;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSaveStats;

        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.TextBox txtSavedResults;
        private System.Windows.Forms.RichTextBox txtCompareResults;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;

        /// <summary> 
        /// Initialize components - simplified, even, responsive layout.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.label4 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();

            this.tableLayoutPanelRoot = new System.Windows.Forms.TableLayoutPanel();
            this.leftPanel = new System.Windows.Forms.TableLayoutPanel();
            this.rightPanel = new System.Windows.Forms.TableLayoutPanel();

            this.lblFile = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSavedResults2 = new System.Windows.Forms.TextBox();
            this.btnLoadStats = new System.Windows.Forms.Button();
            this.chkExcludeIncidents = new System.Windows.Forms.CheckBox();
            this.chkExcludeNoActionDuplicate = new System.Windows.Forms.CheckBox();
            this.chkExcludeReopened = new System.Windows.Forms.CheckBox();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSaveStats = new System.Windows.Forms.Button();

            this.txtResults = new System.Windows.Forms.TextBox();
            this.txtSavedResults = new System.Windows.Forms.TextBox();
            this.txtCompareResults = new System.Windows.Forms.RichTextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();

            this.SuspendLayout();

            // title (top)
            this.label4.Text = "Case History Statistic Comparison Tool (Beta)";
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label4.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label4.Height = 24;
            this.label4.Name = "label4";

            // progress bar (below title)
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBar1.Height = 18;
            this.progressBar1.Name = "progressBar1";

            // tableLayoutPanelRoot: 2 columns (left fixed, right fills)
            this.tableLayoutPanelRoot.ColumnCount = 2;
            this.tableLayoutPanelRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 260F));
            this.tableLayoutPanelRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelRoot.RowCount = 1;
            this.tableLayoutPanelRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelRoot.Padding = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelRoot.Name = "tableLayoutPanelRoot";

            // leftPanel: vertical stack of controls
            this.leftPanel.ColumnCount = 1;
            this.leftPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.leftPanel.RowCount = 12;
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPanel.Padding = new System.Windows.Forms.Padding(8);
            // rows (absolute for predictable spacing)
            this.leftPanel.RowStyles.Clear();
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F)); // lblFile
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F)); // txtFilePath
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F)); // btnBrowse
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F)); // label1
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F)); // txtSavedResults2
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F)); // btnLoadStats
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F)); // chkIncidents
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F)); // chkNoAction
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F)); // chkReopened
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F)); // btnCalculate
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F)); // btnClear
            this.leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F)); // btnSaveStats

            // left controls - labels / inputs
            this.lblFile.AutoSize = true;
            this.lblFile.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblFile.Text = "Jira Export CSV File";
            this.lblFile.Name = "lblFile";
            this.leftPanel.Controls.Add(this.lblFile, 0, 0);

            this.txtFilePath.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtFilePath.Name = "txtFilePath";
            this.leftPanel.Controls.Add(this.txtFilePath, 0, 1);

            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            this.leftPanel.Controls.Add(this.btnBrowse, 0, 2);

            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Text = "Comparison File";
            this.label1.Name = "label1";
            this.leftPanel.Controls.Add(this.label1, 0, 3);

            this.txtSavedResults2.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtSavedResults2.Name = "txtSavedResults2";
            this.leftPanel.Controls.Add(this.txtSavedResults2, 0, 4);

            this.btnLoadStats.Text = "Browse...";
            this.btnLoadStats.Name = "btnLoadStats";
            this.btnLoadStats.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnLoadStats.Click += new System.EventHandler(this.btnLoadStats_Click);
            this.leftPanel.Controls.Add(this.btnLoadStats, 0, 5);

            this.chkExcludeIncidents.Text = "Exclude Incidents";
            this.chkExcludeIncidents.AutoSize = true;
            this.chkExcludeIncidents.Name = "chkExcludeIncidents";
            this.leftPanel.Controls.Add(this.chkExcludeIncidents, 0, 6);

            this.chkExcludeNoActionDuplicate.Text = "Exclude No Action Needed";
            this.chkExcludeNoActionDuplicate.AutoSize = true;
            this.chkExcludeNoActionDuplicate.Name = "chkExcludeNoActionDuplicate";
            this.leftPanel.Controls.Add(this.chkExcludeNoActionDuplicate, 0, 7);

            this.chkExcludeReopened.Text = "Exclude Re-Opened";
            this.chkExcludeReopened.AutoSize = true;
            this.chkExcludeReopened.Name = "chkExcludeReopened";
            this.leftPanel.Controls.Add(this.chkExcludeReopened, 0, 8);

            this.btnCalculate.Text = "Recalculate Stats";
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            this.leftPanel.Controls.Add(this.btnCalculate, 0, 9);

            this.btnClear.Text = "Clear";
            this.btnClear.Name = "btnClear";
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            this.leftPanel.Controls.Add(this.btnClear, 0, 10);

            this.btnSaveStats.Text = "Export Stats";
            this.btnSaveStats.Name = "btnSaveStats";
            this.btnSaveStats.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnSaveStats.Click += new System.EventHandler(this.btnSaveStats_Click);
            this.leftPanel.Controls.Add(this.btnSaveStats, 0, 11);

            // rightPanel: two rows. Row0: 2 equal columns for stat boxes. Row1: compare box spanning both columns.
            this.rightPanel.ColumnCount = 2;
            this.rightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rightPanel.RowCount = 2;
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Padding = new System.Windows.Forms.Padding(8);
            this.rightPanel.Name = "rightPanel";

            // txtResults (left stat box)
            this.txtResults.Multiline = true;
            this.txtResults.ReadOnly = true;
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResults.Name = "txtResults";

            // txtSavedResults (right stat box)
            this.txtSavedResults.Multiline = true;
            this.txtSavedResults.ReadOnly = true;
            this.txtSavedResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSavedResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSavedResults.Name = "txtSavedResults";

            // txtCompareResults (bottom)
            this.txtCompareResults.ReadOnly = true;
            this.txtCompareResults.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtCompareResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCompareResults.Name = "txtCompareResults";

            // add stat boxes to rightPanel
            this.rightPanel.Controls.Add(this.txtResults, 0, 0);
            this.rightPanel.Controls.Add(this.txtSavedResults, 1, 0);
            // add compare box spanning both columns
            this.rightPanel.Controls.Add(this.txtCompareResults, 0, 1);
            this.rightPanel.SetColumnSpan(this.txtCompareResults, 2);

            // assemble root: leftPanel in col0, rightPanel in col1
            this.tableLayoutPanelRoot.Controls.Add(this.leftPanel, 0, 0);
            this.tableLayoutPanelRoot.Controls.Add(this.rightPanel, 1, 0);

            // flowLayoutPanel1 (unused placeholder)
            this.flowLayoutPanel1.Visible = false;
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";

            // Form properties
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Case KPI Calculator";
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.Name = "CaseStats";

            // add controls to form (top-down)
            this.Controls.Add(this.tableLayoutPanelRoot);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.flowLayoutPanel1);

            // wire up form load to existing initializer in code
            this.Load += new System.EventHandler(this.Form1_Load);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        /// <summary>
        /// Clean up resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }
    }
}
