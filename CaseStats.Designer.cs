namespace JiraTicketStats
{
    partial class CaseStats
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblFile = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.chkExcludeIncidents = new System.Windows.Forms.CheckBox();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.chkExcludeNoActionDuplicate = new System.Windows.Forms.CheckBox();
            this.chkExcludeReopened = new System.Windows.Forms.CheckBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txtSavedResults = new System.Windows.Forms.TextBox();
            this.txtSavedResults2 = new System.Windows.Forms.TextBox();
            this.txtCompareResults = new System.Windows.Forms.TextBox();
            this.btnSaveStats = new System.Windows.Forms.Button();
            this.btnLoadStats = new System.Windows.Forms.Button();
            this.btnCompare = new System.Windows.Forms.Button();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblFile.Location = new System.Drawing.Point(12, 44);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(205, 25);
            this.lblFile.TabIndex = 0;
            this.lblFile.Text = "Jira Export CSV File";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(17, 84);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(175, 31);
            this.txtFilePath.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnBrowse.Location = new System.Drawing.Point(211, 78);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(119, 43);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // chkExcludeIncidents
            // 
            this.chkExcludeIncidents.AutoSize = true;
            this.chkExcludeIncidents.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.chkExcludeIncidents.Location = new System.Drawing.Point(22, 140);
            this.chkExcludeIncidents.Name = "chkExcludeIncidents";
            this.chkExcludeIncidents.Size = new System.Drawing.Size(213, 29);
            this.chkExcludeIncidents.TabIndex = 3;
            this.chkExcludeIncidents.Text = "Exclude Incidents";
            this.chkExcludeIncidents.UseVisualStyleBackColor = true;
            // 
            // txtResults
            // 
            this.txtResults.Location = new System.Drawing.Point(377, 80);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.Size = new System.Drawing.Size(426, 515);
            this.txtResults.TabIndex = 6;
            // 
            // chkExcludeNoActionDuplicate
            // 
            this.chkExcludeNoActionDuplicate.AutoSize = true;
            this.chkExcludeNoActionDuplicate.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.chkExcludeNoActionDuplicate.Location = new System.Drawing.Point(22, 196);
            this.chkExcludeNoActionDuplicate.Name = "chkExcludeNoActionDuplicate";
            this.chkExcludeNoActionDuplicate.Size = new System.Drawing.Size(301, 29);
            this.chkExcludeNoActionDuplicate.TabIndex = 7;
            this.chkExcludeNoActionDuplicate.Text = "Exclude No Action Needed";
            this.chkExcludeNoActionDuplicate.UseVisualStyleBackColor = true;
            // 
            // chkExcludeReopened
            // 
            this.chkExcludeReopened.AutoSize = true;
            this.chkExcludeReopened.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.chkExcludeReopened.Location = new System.Drawing.Point(22, 250);
            this.chkExcludeReopened.Name = "chkExcludeReopened";
            this.chkExcludeReopened.Size = new System.Drawing.Size(237, 29);
            this.chkExcludeReopened.TabIndex = 8;
            this.chkExcludeReopened.Text = "Exclude Re-Opened";
            this.chkExcludeReopened.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnClear.Location = new System.Drawing.Point(46, 436);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(221, 45);
            this.btnClear.TabIndex = 9;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(403, 44);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(796, 19);
            this.progressBar1.TabIndex = 10;
            // 
            // txtSavedResults
            // 
            this.txtSavedResults.Location = new System.Drawing.Point(826, 80);
            this.txtSavedResults.Multiline = true;
            this.txtSavedResults.Name = "txtSavedResults";
            this.txtSavedResults.Size = new System.Drawing.Size(429, 515);
            this.txtSavedResults.TabIndex = 11;
            // 
            // txtSavedResults2
            // 
            this.txtSavedResults2.Location = new System.Drawing.Point(17, 614);
            this.txtSavedResults2.Multiline = true;
            this.txtSavedResults2.Name = "txtSavedResults2";
            this.txtSavedResults2.Size = new System.Drawing.Size(159, 38);
            this.txtSavedResults2.TabIndex = 16;
            // 
            // txtCompareResults
            // 
            this.txtCompareResults.Location = new System.Drawing.Point(377, 614);
            this.txtCompareResults.Multiline = true;
            this.txtCompareResults.Name = "txtCompareResults";
            this.txtCompareResults.Size = new System.Drawing.Size(878, 315);
            this.txtCompareResults.TabIndex = 12;
            // 
            // btnSaveStats
            // 
            this.btnSaveStats.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnSaveStats.Location = new System.Drawing.Point(46, 379);
            this.btnSaveStats.Name = "btnSaveStats";
            this.btnSaveStats.Size = new System.Drawing.Size(221, 45);
            this.btnSaveStats.TabIndex = 13;
            this.btnSaveStats.Text = "Export Stats";
            this.btnSaveStats.UseVisualStyleBackColor = true;
            this.btnSaveStats.Click += new System.EventHandler(this.btnSaveStats_Click);
            // 
            // btnLoadStats
            // 
            this.btnLoadStats.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnLoadStats.Location = new System.Drawing.Point(191, 607);
            this.btnLoadStats.Name = "btnLoadStats";
            this.btnLoadStats.Size = new System.Drawing.Size(132, 45);
            this.btnLoadStats.TabIndex = 14;
            this.btnLoadStats.Text = "Browse";
            this.btnLoadStats.UseVisualStyleBackColor = true;
            this.btnLoadStats.Click += new System.EventHandler(this.btnLoadStats_Click);
            // 
            // btnCompare
            // 
            this.btnCompare.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnCompare.Location = new System.Drawing.Point(76, 675);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(206, 45);
            this.btnCompare.TabIndex = 15;
            this.btnCompare.Text = "Compare Results";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // btnCalculate
            // 
            this.btnCalculate.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnCalculate.Location = new System.Drawing.Point(46, 322);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(221, 46);
            this.btnCalculate.TabIndex = 5;
            this.btnCalculate.Text = "Recalculate Stats";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // CaseStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1377, 1054);
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.btnLoadStats);
            this.Controls.Add(this.btnSaveStats);
            this.Controls.Add(this.txtCompareResults);
            this.Controls.Add(this.txtSavedResults2);
            this.Controls.Add(this.txtSavedResults);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.chkExcludeReopened);
            this.Controls.Add(this.chkExcludeNoActionDuplicate);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.chkExcludeIncidents);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.lblFile);
            this.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Name = "CaseStats";
            this.Text = "Case KPI Calculator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.CheckBox chkExcludeIncidents;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.CheckBox chkExcludeNoActionDuplicate;
        private System.Windows.Forms.CheckBox chkExcludeReopened;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox txtSavedResults;
        private System.Windows.Forms.TextBox txtSavedResults2;
        private System.Windows.Forms.TextBox txtCompareResults;
        private System.Windows.Forms.Button btnSaveStats;
        private System.Windows.Forms.Button btnLoadStats;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.Button btnCalculate;
    }
}

