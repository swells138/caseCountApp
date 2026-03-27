namespace JiraTicketStats
{
    partial class CaseStats
    {
        private System.ComponentModel.IContainer components = null;

        // Controls
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRoot;
        private System.Windows.Forms.FlowLayoutPanel leftPanel;
        private System.Windows.Forms.TableLayoutPanel rightPanel;

        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLoadStats;
        private System.Windows.Forms.CheckBox chkExcludeIncidents;
        private System.Windows.Forms.CheckBox chkExcludeNoActionDuplicate;
        private System.Windows.Forms.CheckBox chkExcludeReopened;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSaveStats;

        // New snapshot UI buttons
        private System.Windows.Forms.Button btnSaveSnapshot;
        private System.Windows.Forms.Button btnLoadSnapshot;
        private System.Windows.Forms.Button btnOpenSnapshots;

        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.TextBox txtSavedResults;
        private System.Windows.Forms.RichTextBox txtCompareResults;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;

        /// <summary> 
        /// Initialize components - simplified, even, responsive layout.
        /// </summary>
        private void InitializeComponent()
        {
            this.label4 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanelRoot = new System.Windows.Forms.TableLayoutPanel();
            this.leftPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblFile = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLoadStats = new System.Windows.Forms.Button();
            this.chkExcludeIncidents = new System.Windows.Forms.CheckBox();
            this.chkExcludeNoActionDuplicate = new System.Windows.Forms.CheckBox();
            this.chkExcludeReopened = new System.Windows.Forms.CheckBox();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSaveStats = new System.Windows.Forms.Button();

            // instantiate new snapshot buttons
            this.btnSaveSnapshot = new System.Windows.Forms.Button();
            this.btnLoadSnapshot = new System.Windows.Forms.Button();
            this.btnOpenSnapshots = new System.Windows.Forms.Button();

            this.rightPanel = new System.Windows.Forms.TableLayoutPanel();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.txtSavedResults = new System.Windows.Forms.TextBox();
            this.txtCompareResults = new System.Windows.Forms.RichTextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.txtSavedResults2 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanelRoot.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(3);
            this.label4.Size = new System.Drawing.Size(1560, 52);
            this.label4.TabIndex = 2;
            this.label4.Text = "Case History Statistic Comparison Tool (Beta)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
           // this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBar1.Location = new System.Drawing.Point(0, 0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1560, 18);
            this.progressBar1.TabIndex = 1;
            // 
            // tableLayoutPanelRoot
            // 
            this.tableLayoutPanelRoot.ColumnCount = 2;
            this.tableLayoutPanelRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 320F));
            this.tableLayoutPanelRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelRoot.Controls.Add(this.leftPanel, 0, 1);
            this.tableLayoutPanelRoot.Controls.Add(this.rightPanel, 1, 1);
            this.tableLayoutPanelRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelRoot.Location = new System.Drawing.Point(0, 18);
            this.tableLayoutPanelRoot.Name = "tableLayoutPanelRoot";
            this.tableLayoutPanelRoot.RowCount = 2;
            this.tableLayoutPanelRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanelRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelRoot.Size = new System.Drawing.Size(1560, 1063);
            this.tableLayoutPanelRoot.TabIndex = 0;
            // 
            // leftPanel
            // 
            this.leftPanel.AutoScroll = true;
            this.leftPanel.BackColor = System.Drawing.Color.Transparent;
            this.leftPanel.Controls.Add(this.lblFile);
            this.leftPanel.Controls.Add(this.txtFilePath);
            this.leftPanel.Controls.Add(this.btnBrowse);
            this.leftPanel.Controls.Add(this.label1);
            this.leftPanel.Controls.Add(this.txtSavedResults2);
            this.leftPanel.Controls.Add(this.btnLoadStats);
            this.leftPanel.Controls.Add(this.chkExcludeIncidents);
            this.leftPanel.Controls.Add(this.chkExcludeNoActionDuplicate);
            this.leftPanel.Controls.Add(this.chkExcludeReopened);
            this.leftPanel.Controls.Add(this.btnCalculate);
            this.leftPanel.Controls.Add(this.btnClear);
            this.leftPanel.Controls.Add(this.btnSaveStats);

            // add new snapshot buttons into the flow (after Export Stats)
            this.leftPanel.Controls.Add(this.btnSaveSnapshot);
            this.leftPanel.Controls.Add(this.btnLoadSnapshot);
            this.leftPanel.Controls.Add(this.btnOpenSnapshots);

            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.leftPanel.Location = new System.Drawing.Point(0, 100);
            this.leftPanel.Margin = new System.Windows.Forms.Padding(0, 30, 0, 30);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(320, 933);
            this.leftPanel.TabIndex = 0;
            this.leftPanel.WrapContents = false;
           // this.leftPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.leftPanel_Paint);
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblFile.Location = new System.Drawing.Point(3, 6);
            this.lblFile.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(205, 25);
            this.lblFile.TabIndex = 0;
            this.lblFile.Text = "Jira Export CSV File";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilePath.Location = new System.Drawing.Point(3, 43);
            this.txtFilePath.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txtFilePath.MinimumSize = new System.Drawing.Size(260, 28);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(284, 32);
            this.txtFilePath.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.AutoSize = true;
            this.btnBrowse.Location = new System.Drawing.Point(3, 87);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnBrowse.MinimumSize = new System.Drawing.Size(90, 30);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(168, 49);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Location = new System.Drawing.Point(3, 148);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Comparison File";
            // 
            // btnLoadStats
            // 
            this.btnLoadStats.AutoSize = true;
            this.btnLoadStats.Location = new System.Drawing.Point(3, 229);
            this.btnLoadStats.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnLoadStats.MinimumSize = new System.Drawing.Size(90, 30);
            this.btnLoadStats.Name = "btnLoadStats";
            this.btnLoadStats.Size = new System.Drawing.Size(168, 49);
            this.btnLoadStats.TabIndex = 5;
            this.btnLoadStats.Text = "Browse...";
            this.btnLoadStats.UseVisualStyleBackColor = true;
            this.btnLoadStats.Click += new System.EventHandler(this.btnLoadStats_Click);
            // 
            // chkExcludeIncidents
            // 
            this.chkExcludeIncidents.AutoSize = true;
            this.chkExcludeIncidents.Location = new System.Drawing.Point(3, 290);
            this.chkExcludeIncidents.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.chkExcludeIncidents.Name = "chkExcludeIncidents";
            this.chkExcludeIncidents.Size = new System.Drawing.Size(213, 29);
            this.chkExcludeIncidents.TabIndex = 6;
            this.chkExcludeIncidents.Text = "Exclude Incidents";
            this.chkExcludeIncidents.UseVisualStyleBackColor = true;
            // 
            // chkExcludeNoActionDuplicate
            // 
            this.chkExcludeNoActionDuplicate.AutoSize = true;
            this.chkExcludeNoActionDuplicate.Location = new System.Drawing.Point(3, 331);
            this.chkExcludeNoActionDuplicate.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.chkExcludeNoActionDuplicate.Name = "chkExcludeNoActionDuplicate";
            this.chkExcludeNoActionDuplicate.Size = new System.Drawing.Size(301, 29);
            this.chkExcludeNoActionDuplicate.TabIndex = 7;
            this.chkExcludeNoActionDuplicate.Text = "Exclude No Action Needed";
            this.chkExcludeNoActionDuplicate.UseVisualStyleBackColor = true;
            // 
            // chkExcludeReopened
            // 
            this.chkExcludeReopened.AutoSize = true;
            this.chkExcludeReopened.Location = new System.Drawing.Point(3, 372);
            this.chkExcludeReopened.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.chkExcludeReopened.Name = "chkExcludeReopened";
            this.chkExcludeReopened.Size = new System.Drawing.Size(237, 29);
            this.chkExcludeReopened.TabIndex = 8;
            this.chkExcludeReopened.Text = "Exclude Re-Opened";
            this.chkExcludeReopened.UseVisualStyleBackColor = true;
            // 
            // btnCalculate
            // 
            this.btnCalculate.AutoSize = true;
            this.btnCalculate.Location = new System.Drawing.Point(3, 413);
            this.btnCalculate.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnCalculate.MinimumSize = new System.Drawing.Size(120, 34);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(213, 50);
            this.btnCalculate.TabIndex = 9;
            this.btnCalculate.Text = "Recalculate Stats";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // btnClear
            // 
            this.btnClear.AutoSize = true;
            this.btnClear.Location = new System.Drawing.Point(3, 475);
            this.btnClear.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnClear.MinimumSize = new System.Drawing.Size(90, 30);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(213, 50);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSaveStats
            // 
            this.btnSaveStats.AutoSize = true;
            this.btnSaveStats.Location = new System.Drawing.Point(3, 537);
            this.btnSaveStats.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnSaveStats.MinimumSize = new System.Drawing.Size(110, 34);
            this.btnSaveStats.Name = "btnSaveStats";
            this.btnSaveStats.Size = new System.Drawing.Size(213, 50);
            this.btnSaveStats.TabIndex = 11;
            this.btnSaveStats.Text = "Export Stats";
            this.btnSaveStats.UseVisualStyleBackColor = true;
            this.btnSaveStats.Click += new System.EventHandler(this.btnSaveStats_Click);
            // 
            // btnSaveSnapshot
            // 
            this.btnSaveSnapshot.AutoSize = true;
            this.btnSaveSnapshot.Location = new System.Drawing.Point(3, 603);
            this.btnSaveSnapshot.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnSaveSnapshot.MinimumSize = new System.Drawing.Size(110, 34);
            this.btnSaveSnapshot.Name = "btnSaveSnapshot";
            this.btnSaveSnapshot.Size = new System.Drawing.Size(213, 50);
            this.btnSaveSnapshot.TabIndex = 12;
            this.btnSaveSnapshot.Text = "Save Snapshot";
            this.btnSaveSnapshot.UseVisualStyleBackColor = true;
            this.btnSaveSnapshot.Click += new System.EventHandler(this.btnSaveSnapshot_Click);
            // 
            // btnLoadSnapshot
            // 
            this.btnLoadSnapshot.AutoSize = true;
            this.btnLoadSnapshot.Location = new System.Drawing.Point(3, 669);
            this.btnLoadSnapshot.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnLoadSnapshot.MinimumSize = new System.Drawing.Size(110, 34);
            this.btnLoadSnapshot.Name = "btnLoadSnapshot";
            this.btnLoadSnapshot.Size = new System.Drawing.Size(213, 50);
            this.btnLoadSnapshot.TabIndex = 13;
            this.btnLoadSnapshot.Text = "Load Snapshot";
            this.btnLoadSnapshot.UseVisualStyleBackColor = true;
            this.btnLoadSnapshot.Click += new System.EventHandler(this.btnLoadSnapshot_Click);
            // 
            // btnOpenSnapshots
            // 
            this.btnOpenSnapshots.AutoSize = true;
            this.btnOpenSnapshots.Location = new System.Drawing.Point(3, 735);
            this.btnOpenSnapshots.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnOpenSnapshots.MinimumSize = new System.Drawing.Size(110, 34);
            this.btnOpenSnapshots.Name = "btnOpenSnapshots";
            this.btnOpenSnapshots.Size = new System.Drawing.Size(213, 50);
            this.btnOpenSnapshots.TabIndex = 14;
            this.btnOpenSnapshots.Text = "Open Snapshots Folder";
            this.btnOpenSnapshots.UseVisualStyleBackColor = true;
            this.btnOpenSnapshots.Click += new System.EventHandler(this.btnOpenSnapshots_Click);
            // 
            // rightPanel
            // 
            this.rightPanel.ColumnCount = 2;
            this.rightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rightPanel.Controls.Add(this.txtResults, 0, 0);
            this.rightPanel.Controls.Add(this.txtSavedResults, 1, 0);
            this.rightPanel.Controls.Add(this.txtCompareResults, 0, 1);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(320, 100);
            this.rightPanel.Margin = new System.Windows.Forms.Padding(0, 30, 0, 30);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Padding = new System.Windows.Forms.Padding(12);
            this.rightPanel.RowCount = 2;
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.rightPanel.Size = new System.Drawing.Size(1240, 933);
            this.rightPanel.TabIndex = 1;
            // 
            // txtResults
            // 
            this.txtResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResults.Location = new System.Drawing.Point(15, 15);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ReadOnly = true;
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResults.Size = new System.Drawing.Size(602, 539);
            this.txtResults.TabIndex = 0;
            // 
            // txtSavedResults
            // 
            this.txtSavedResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSavedResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSavedResults.Location = new System.Drawing.Point(623, 15);
            this.txtSavedResults.Multiline = true;
            this.txtSavedResults.Name = "txtSavedResults";
            this.txtSavedResults.ReadOnly = true;
            this.txtSavedResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSavedResults.Size = new System.Drawing.Size(602, 539);
            this.txtSavedResults.TabIndex = 1;
            // 
            // txtCompareResults
            // 
            this.rightPanel.SetColumnSpan(this.txtCompareResults, 2);
            this.txtCompareResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCompareResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCompareResults.Location = new System.Drawing.Point(15, 560);
            this.txtCompareResults.Name = "txtCompareResults";
            this.txtCompareResults.ReadOnly = true;
            this.txtCompareResults.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtCompareResults.Size = new System.Drawing.Size(1210, 358);
            this.txtCompareResults.TabIndex = 2;
            this.txtCompareResults.Text = "";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(200, 100);
            this.flowLayoutPanel1.TabIndex = 3;
            this.flowLayoutPanel1.Visible = false;
            // 
            // txtSavedResults2
            // 
            this.txtSavedResults2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSavedResults2.Location = new System.Drawing.Point(3, 185);
            this.txtSavedResults2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txtSavedResults2.MinimumSize = new System.Drawing.Size(260, 28);
            this.txtSavedResults2.Name = "txtSavedResults2";
            this.txtSavedResults2.Size = new System.Drawing.Size(284, 32);
            this.txtSavedResults2.TabIndex = 4;
            // 
            // CaseStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1560, 1081);
            this.Controls.Add(this.tableLayoutPanelRoot);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "CaseStats";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Case KPI Calculator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tableLayoutPanelRoot.ResumeLayout(false);
            this.leftPanel.ResumeLayout(false);
            this.leftPanel.PerformLayout();
            this.rightPanel.ResumeLayout(false);
            this.rightPanel.PerformLayout();
            this.ResumeLayout(false);

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

        private System.Windows.Forms.TextBox txtSavedResults2;

      
    }
}
