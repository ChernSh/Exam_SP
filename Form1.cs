using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Exam
{
    public partial class Form1 : Form
    {
        private ScannerLogic _scanner;
        public Form1()
        {
            InitializeComponent();
            InitializeScanner();
            SetupUIState(false);
        }
        private void InitializeScanner()
        {
            _scanner = new ScannerLogic();
            _scanner.OnProgressUpdate += Scanner_OnProgressUpdate;
            _scanner.OnFileProcessed += Scanner_OnFileProcessed;
            _scanner.OnScanFinished += Scanner_OnScanFinished;
            _scanner.OnError += Scanner_OnError;
        }
        private void SetupUIState(bool scanningInProgress)
        {
            grpSettings.Enabled = !scanningInProgress;
            btnStart.Enabled = !scanningInProgress;
            btnPause.Enabled = scanningInProgress;
            btnResume.Enabled = scanningInProgress;
            btnStop.Enabled = scanningInProgress;

            if (!scanningInProgress)
            {
                pbOverallProgress.Value = 0;
            }
        }

        private void btnLoadWords_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Текстові файли (*.txt)|*.txt|Усі файли (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        txtForbiddenWords.Text = File.ReadAllText(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка читання файлу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtOutputFolder.Text = fbd.SelectedPath;
                }
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            rtbReport.Clear();
            var forbiddenWordsList = new List<string>();
            if (!string.IsNullOrWhiteSpace(txtForbiddenWords.Text))
            {
                forbiddenWordsList.AddRange(txtForbiddenWords.Text.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                                                             .Select(s => s.Trim()));
            }

            if (!forbiddenWordsList.Any())
            {
                MessageBox.Show("Будь ласка, введіть або завантажте заборонені слова.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtOutputFolder.Text) || !Directory.Exists(txtOutputFolder.Text))
            {
                MessageBox.Show("Будь ласка, оберіть дійсну папку для збереження змінених файлів.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _scanner.ForbiddenWords = new HashSet<string>(forbiddenWordsList, StringComparer.OrdinalIgnoreCase);
            _scanner.OutputFolderPath = txtOutputFolder.Text;

            SetupUIState(true);

            await _scanner.StartScanAsync();

            SetupUIState(false);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            _scanner.PauseScan();
            btnPause.Enabled = false;
            btnResume.Enabled = true;
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            _scanner.ResumeScan();
            btnPause.Enabled = true;
            btnResume.Enabled = false;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _scanner.StopScan();
            SetupUIState(false);
        }
        private void Scanner_OnProgressUpdate(object sender, int progress)
        {
            if (pbOverallProgress.InvokeRequired)
            {
                pbOverallProgress.Invoke(new Action(() => pbOverallProgress.Value = progress));
            }
            else
            {
                pbOverallProgress.Value = progress;
            }
        }

        private void Scanner_OnFileProcessed(object sender, Tuple<FileProcessResult, int, int> args)
        {
            FileProcessResult result = args.Item1;
            int processedCount = args.Item2;
            int totalFiles = args.Item3;

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateFileProcessedUI(result, processedCount, totalFiles)));
            }
            else
            {
                UpdateFileProcessedUI(result, processedCount, totalFiles);
            }
        }

        private void UpdateFileProcessedUI(FileProcessResult result, int processedCount, int totalFiles)
        {
            lblFilesProcessedCount.Text = $"Оброблено файлів: {processedCount} з {totalFiles}";

            string reportEntry = $"Файл: {Path.GetFileName(result.FilePath)} ({(result.FileSize / 1024.0):F2} KB)\n";
            reportEntry += $"  Замін: {result.ReplacementsCount}\n";
            if (result.FoundForbiddenWords.Any())
            {
                reportEntry += "  Знайдені слова:\n";
                foreach (var entry in result.FoundForbiddenWords)
                {
                    reportEntry += $"    - '{entry.Key}': {entry.Value} разів\n";
                }
            }
            if (!result.IsSuccessful)
            {
                reportEntry += $"  Помилка: {result.ErrorMessage}\n";
            }
            reportEntry += "\n";
            rtbReport.AppendText(reportEntry);
        }

        private void Scanner_OnScanFinished(object sender, string report)
        {
            if (rtbReport.InvokeRequired)
            {
                rtbReport.Invoke(new Action(() => rtbReport.Text = report));
            }
            else
            {
                rtbReport.Text = report;
            }
            SetupUIState(false);
        }

        private void Scanner_OnError(object sender, string errorMessage)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => MessageBox.Show(errorMessage, "Помилка сканування", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
            else
            {
                MessageBox.Show(errorMessage, "Помилка сканування", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SingleInstanceApplication.Release();
        }

        private void btnSaveReport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rtbReport.Text))
            {
                MessageBox.Show("Звіт порожній. Немає чого зберігати.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Текстові файли (*.txt)|*.txt|Усі файли (*.*)|*.*";
                sfd.FileName = $"ScanReport_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                sfd.Title = "Зберегти звіт";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(sfd.FileName, rtbReport.Text);
                        MessageBox.Show("Звіт успішно збережено.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка при збереженні звіту: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}