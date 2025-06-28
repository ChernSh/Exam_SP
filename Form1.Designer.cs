namespace Exam
{
    partial class Form1
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
            System.Windows.Forms.Label label3;
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.btnBrowseOutput = new System.Windows.Forms.Button();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLoadWords = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtForbiddenWords = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnResume = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.grpProgress = new System.Windows.Forms.GroupBox();
            this.pbOverallProgress = new System.Windows.Forms.ProgressBar();
            this.grpResults = new System.Windows.Forms.GroupBox();
            this.rtbReport = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSaveReport = new System.Windows.Forms.Button();
            this.lblFilesProcessedCount = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.grpSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpProgress.SuspendLayout();
            this.grpResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(8, 26);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(136, 16);
            label3.TabIndex = 0;
            label3.Text = "Загальний прогрес:";
            // 
            // grpSettings
            // 
            this.grpSettings.Controls.Add(this.btnBrowseOutput);
            this.grpSettings.Controls.Add(this.txtOutputFolder);
            this.grpSettings.Controls.Add(this.label2);
            this.grpSettings.Controls.Add(this.btnLoadWords);
            this.grpSettings.Controls.Add(this.label1);
            this.grpSettings.Controls.Add(this.txtForbiddenWords);
            this.grpSettings.Location = new System.Drawing.Point(12, 12);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Size = new System.Drawing.Size(395, 148);
            this.grpSettings.TabIndex = 0;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "Settings";
            // 
            // btnBrowseOutput
            // 
            this.btnBrowseOutput.Location = new System.Drawing.Point(196, 102);
            this.btnBrowseOutput.Name = "btnBrowseOutput";
            this.btnBrowseOutput.Size = new System.Drawing.Size(185, 24);
            this.btnBrowseOutput.TabIndex = 4;
            this.btnBrowseOutput.Text = "Огляд...";
            this.btnBrowseOutput.UseVisualStyleBackColor = true;
            this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(15, 103);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(164, 22);
            this.txtOutputFolder.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(270, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Папка для збереження змінених файлів:";
            // 
            // btnLoadWords
            // 
            this.btnLoadWords.Location = new System.Drawing.Point(195, 48);
            this.btnLoadWords.Name = "btnLoadWords";
            this.btnLoadWords.Size = new System.Drawing.Size(186, 28);
            this.btnLoadWords.TabIndex = 1;
            this.btnLoadWords.Text = "Завантажити з файлу...";
            this.btnLoadWords.UseVisualStyleBackColor = true;
            this.btnLoadWords.Click += new System.EventHandler(this.btnLoadWords_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(298, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Заборонені слова (через кому або з файлу):";
            // 
            // txtForbiddenWords
            // 
            this.txtForbiddenWords.Location = new System.Drawing.Point(15, 48);
            this.txtForbiddenWords.Multiline = true;
            this.txtForbiddenWords.Name = "txtForbiddenWords";
            this.txtForbiddenWords.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtForbiddenWords.Size = new System.Drawing.Size(164, 28);
            this.txtForbiddenWords.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Controls.Add(this.btnPause);
            this.groupBox1.Controls.Add(this.btnResume);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Location = new System.Drawing.Point(12, 177);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(395, 157);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.DarkMagenta;
            this.btnStop.Location = new System.Drawing.Point(200, 91);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(187, 44);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Зупинити";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPause
            // 
            this.btnPause.BackColor = System.Drawing.Color.Crimson;
            this.btnPause.Location = new System.Drawing.Point(200, 31);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(187, 44);
            this.btnPause.TabIndex = 2;
            this.btnPause.Text = "Пауза";
            this.btnPause.UseVisualStyleBackColor = false;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnResume
            // 
            this.btnResume.BackColor = System.Drawing.Color.Turquoise;
            this.btnResume.Location = new System.Drawing.Point(7, 91);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(187, 44);
            this.btnResume.TabIndex = 1;
            this.btnResume.Text = "Відновити";
            this.btnResume.UseVisualStyleBackColor = false;
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.YellowGreen;
            this.btnStart.Location = new System.Drawing.Point(7, 31);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(187, 44);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Старт";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // grpProgress
            // 
            this.grpProgress.Controls.Add(this.lblFilesProcessedCount);
            this.grpProgress.Controls.Add(this.pbOverallProgress);
            this.grpProgress.Controls.Add(label3);
            this.grpProgress.Location = new System.Drawing.Point(12, 354);
            this.grpProgress.Name = "grpProgress";
            this.grpProgress.Size = new System.Drawing.Size(395, 111);
            this.grpProgress.TabIndex = 2;
            this.grpProgress.TabStop = false;
            this.grpProgress.Text = "Progress";
            // 
            // pbOverallProgress
            // 
            this.pbOverallProgress.Location = new System.Drawing.Point(11, 49);
            this.pbOverallProgress.Name = "pbOverallProgress";
            this.pbOverallProgress.Size = new System.Drawing.Size(378, 23);
            this.pbOverallProgress.TabIndex = 1;
            // 
            // grpResults
            // 
            this.grpResults.Controls.Add(this.btnSaveReport);
            this.grpResults.Controls.Add(this.rtbReport);
            this.grpResults.Controls.Add(this.label4);
            this.grpResults.Location = new System.Drawing.Point(426, 13);
            this.grpResults.Name = "grpResults";
            this.grpResults.Size = new System.Drawing.Size(295, 452);
            this.grpResults.TabIndex = 3;
            this.grpResults.TabStop = false;
            this.grpResults.Text = "Results";
            // 
            // rtbReport
            // 
            this.rtbReport.Location = new System.Drawing.Point(10, 47);
            this.rtbReport.Name = "rtbReport";
            this.rtbReport.ReadOnly = true;
            this.rtbReport.Size = new System.Drawing.Size(279, 353);
            this.rtbReport.TabIndex = 1;
            this.rtbReport.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Звіт:";
            // 
            // btnSaveReport
            // 
            this.btnSaveReport.Location = new System.Drawing.Point(64, 412);
            this.btnSaveReport.Name = "btnSaveReport";
            this.btnSaveReport.Size = new System.Drawing.Size(163, 23);
            this.btnSaveReport.TabIndex = 2;
            this.btnSaveReport.Text = "Зберегти звіт..";
            this.btnSaveReport.UseVisualStyleBackColor = true;
            this.btnSaveReport.Click += new System.EventHandler(this.btnSaveReport_Click);
            // 
            // lblFilesProcessedCount
            // 
            this.lblFilesProcessedCount.AutoSize = true;
            this.lblFilesProcessedCount.Location = new System.Drawing.Point(10, 83);
            this.lblFilesProcessedCount.Name = "lblFilesProcessedCount";
            this.lblFilesProcessedCount.Size = new System.Drawing.Size(143, 16);
            this.lblFilesProcessedCount.TabIndex = 2;
            this.lblFilesProcessedCount.Text = "Оброблено файлів: 0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 477);
            this.Controls.Add(this.grpResults);
            this.Controls.Add(this.grpProgress);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpSettings);
            this.Name = "Form1";
            this.Text = "CheckFilesApp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.grpProgress.ResumeLayout(false);
            this.grpProgress.PerformLayout();
            this.grpResults.ResumeLayout(false);
            this.grpResults.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSettings;
        private System.Windows.Forms.TextBox txtForbiddenWords;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLoadWords;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowseOutput;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.GroupBox grpProgress;
        private System.Windows.Forms.ProgressBar pbOverallProgress;
        private System.Windows.Forms.GroupBox grpResults;
        private System.Windows.Forms.RichTextBox rtbReport;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSaveReport;
        private System.Windows.Forms.Label lblFilesProcessedCount;
    }
}

