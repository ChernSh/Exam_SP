using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Exam
{
    public class ScannerLogic
    {
        public HashSet<string> ForbiddenWords { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public string OutputFolderPath { get; set; }
        public string ReplacementString { get; set; } = "*******";

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _pauseTokenSource;
        private List<string> _allDrives;
        private List<string> _foundFiles;
        private int _totalFilesToProcess = 0;
        private int _processedFilesCount = 0;
        private readonly object _lockObject = new object();
        private List<FileProcessResult> _processingResults;
        private Dictionary<string, int> _overallForbiddenWordCounts;

        public event EventHandler<int> OnProgressUpdate;
        public event EventHandler<Tuple<FileProcessResult, int, int>> OnFileProcessed;
        public event EventHandler<string> OnScanFinished;
        public event EventHandler<string> OnError;

        public ScannerLogic()
        {
            _allDrives = DriveInfo.GetDrives()
                                 .Where(d => d.IsReady && d.DriveType != DriveType.CDRom)
                                 .Select(d => d.RootDirectory.FullName)
                                 .ToList();
            _foundFiles = new List<string>();
        }

        public async Task StartScanAsync()
        {
            if (ForbiddenWords == null || ForbiddenWords.Count == 0)
            {
                OnError?.Invoke(this, "Будь ласка, введіть або завантажте заборонені слова.");
                return;
            }
            if (string.IsNullOrWhiteSpace(OutputFolderPath) || !Directory.Exists(OutputFolderPath))
            {
                OnError?.Invoke(this, "Будь ласка, вкажіть дійсну папку для збереження.");
                return;
            }
            _cancellationTokenSource = new CancellationTokenSource();
            _pauseTokenSource = new CancellationTokenSource();
            _processingResults = new List<FileProcessResult>();
            _overallForbiddenWordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            _totalFilesToProcess = 0;
            _processedFilesCount = 0;
            OnProgressUpdate?.Invoke(this, 0);

            try
            {
                _foundFiles.Clear();
                await Task.Run(() => CollectFilesSynchronously(_foundFiles, _cancellationTokenSource.Token));

                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    OnScanFinished?.Invoke(this, "Сканування зупинено користувачем.");
                    return;
                }

                _totalFilesToProcess = _foundFiles.Count;
                if (_totalFilesToProcess == 0)
                {
                    OnScanFinished?.Invoke(this, "Файлів для обробки не знайдено.");
                    return;
                }

                await ProcessFilesAsync(_cancellationTokenSource.Token, _pauseTokenSource.Token);

                GenerateReport();
            }
            catch (OperationCanceledException)
            {
                OnScanFinished?.Invoke(this, "Сканування зупинено користувачем.");
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Сталася несподівана помилка: {ex.Message}");
                OnScanFinished?.Invoke(this, "Сканування завершено з помилками.");
            }
        }

        public void StartScanCommandline()
        {
            if (ForbiddenWords == null || ForbiddenWords.Count == 0)
            {
                Console.WriteLine("Помилка: Заборонені слова не вказані.");
                return;
            }
            if (string.IsNullOrWhiteSpace(OutputFolderPath) || !Directory.Exists(OutputFolderPath))
            {
                Console.WriteLine("Помилка: Недійсна папка для збереження.");
                return;
            }
            _cancellationTokenSource = new CancellationTokenSource();
            _pauseTokenSource = new CancellationTokenSource();
            _processingResults = new List<FileProcessResult>();
            _overallForbiddenWordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            _totalFilesToProcess = 0;
            _processedFilesCount = 0;
            try
            {
                _foundFiles.Clear();
                CollectFilesSynchronously(_foundFiles, _cancellationTokenSource.Token);

                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Console.WriteLine("Сканування зупинено.");
                    return;
                }

                _totalFilesToProcess = _foundFiles.Count;
                if (_totalFilesToProcess == 0)
                {
                    Console.WriteLine("Файлів для обробки не знайдено.");
                    return;
                }
                ProcessFilesSync(_cancellationTokenSource.Token, _pauseTokenSource.Token);

                GenerateReport();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Сканування зупинено.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Сталася несподівана помилка: {ex.Message}");
            }
        }

        private void CollectFilesSynchronously(List<string> files, CancellationToken cancellationToken)
        {
            foreach (string drivePath in _allDrives)
            {
                if (cancellationToken.IsCancellationRequested) return;

                try
                {
                    foreach (string file in Directory.EnumerateFiles(drivePath, "*.*", SearchOption.AllDirectories))
                    {
                        if (cancellationToken.IsCancellationRequested) return;
                        if (CanReadFile(file))
                        {
                            lock (_lockObject)
                            {
                                files.Add(file);
                            }
                        }
                    }
                }
                catch (UnauthorizedAccessException) { }
                catch (PathTooLongException) { }
                catch (Exception ex) { OnError?.Invoke(this, $"Помилка збору файлів на диску {drivePath}: {ex.Message}"); }
            }
        }

        private bool CanReadFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return false;
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return true;
                }
            }
            catch (UnauthorizedAccessException) { return false; }
            catch (IOException) { return false; }
            catch (Exception) { return false; }
        }

        private async Task ProcessFilesAsync(CancellationToken cancellationToken, CancellationToken pauseToken)
        {
            using (SemaphoreSlim semaphore = new SemaphoreSlim(Environment.ProcessorCount))
            {
                List<Task> processingTasks = new List<Task>();

                foreach (string filePath in _foundFiles)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    await semaphore.WaitAsync(cancellationToken);
                    processingTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            await HandlePauseResume(pauseToken, cancellationToken);
                            if (cancellationToken.IsCancellationRequested) return;

                            ProcessFile(filePath);
                        }
                        finally
                        {
                            semaphore.Release();
                            lock (_lockObject)
                            {
                                _processedFilesCount++;
                                int percentage = (int)((double)_processedFilesCount / _totalFilesToProcess * 100);
                                OnProgressUpdate?.Invoke(this, percentage);
                            }
                        }
                    }, cancellationToken));
                }

                await Task.WhenAll(processingTasks);
            }
        }

        private void ProcessFilesSync(CancellationToken cancellationToken, CancellationToken pauseToken)
        {
            Parallel.ForEach(_foundFiles, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount, CancellationToken = cancellationToken }, (filePath) =>
            {
                try
                {
                    HandlePauseResumeSync(pauseToken, cancellationToken);
                    if (cancellationToken.IsCancellationRequested) return;

                    ProcessFile(filePath);
                }
                finally
                {
                    lock (_lockObject)
                    {
                        _processedFilesCount++;
                        int percentage = (int)((double)_processedFilesCount / _totalFilesToProcess * 100);
                        OnProgressUpdate?.Invoke(this, percentage);
                    }
                }
            });
        }

        private async Task HandlePauseResume(CancellationToken pauseToken, CancellationToken cancellationToken)
        {
            if (pauseToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(Timeout.Infinite, pauseToken);
                }
                catch (OperationCanceledException)
                {
                }
            }
            cancellationToken.ThrowIfCancellationRequested();
        }

        private void HandlePauseResumeSync(CancellationToken pauseToken, CancellationToken cancellationToken)
        {
            while (pauseToken.IsCancellationRequested)
            {
                Thread.Sleep(100);
                cancellationToken.ThrowIfCancellationRequested();
            }
            cancellationToken.ThrowIfCancellationRequested();
        }

        private void ProcessFile(string filePath)
        {
            if (_cancellationTokenSource.Token.IsCancellationRequested) return;

            string fileContent;
            try
            {
                fileContent = File.ReadAllText(filePath);
            }
            catch (IOException ex)
            {
                lock (_lockObject)
                {
                    _processingResults.Add(new FileProcessResult(filePath) { IsSuccessful = false, ErrorMessage = $"Помилка читання файлу: {ex.Message}" });
                }
                OnError?.Invoke(this, $"Не вдалося прочитати файл {filePath}: {ex.Message}");
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                lock (_lockObject)
                {
                    _processingResults.Add(new FileProcessResult(filePath) { IsSuccessful = false, ErrorMessage = $"Відмовлено в доступі до файлу: {ex.Message}" });
                }
                OnError?.Invoke(this, $"Відмовлено в доступі до файлу {filePath}: {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                lock (_lockObject)
                {
                    _processingResults.Add(new FileProcessResult(filePath) { IsSuccessful = false, ErrorMessage = $"Невідома помилка при читанні файлу: {ex.Message}" });
                }
                OnError?.Invoke(this, $"Невідома помилка при читанні файлу {filePath}: {ex.Message}");
                return;
            }

            int replacements = 0;
            Dictionary<string, int> currentFileForbiddenWordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            string newFileContent = fileContent;

            var sortedForbiddenWords = ForbiddenWords.OrderByDescending(w => w.Length);

            foreach (string word in sortedForbiddenWords)
            {
                int currentWordReplacements = 0;
                int lastIndex = -1;
                string tempContentForCounting = newFileContent;
                while ((lastIndex = tempContentForCounting.IndexOf(word, lastIndex + 1, StringComparison.OrdinalIgnoreCase)) != -1)
                {
                    currentWordReplacements++;
                }

                if (currentWordReplacements > 0)
                {
                    newFileContent = Regex.Replace(newFileContent, @"\b" + Regex.Escape(word) + @"\b", ReplacementString, RegexOptions.IgnoreCase);
                    replacements += currentWordReplacements;
                    if (currentFileForbiddenWordCounts.ContainsKey(word))
                    {
                        currentFileForbiddenWordCounts[word] += currentWordReplacements;
                    }
                    else
                    {
                        currentFileForbiddenWordCounts.Add(word, currentWordReplacements);
                    }
                }
            }

            if (replacements > 0)
            {
                string fileName = Path.GetFileName(filePath);
                string newFilePath = Path.Combine(OutputFolderPath, fileName);
                int counter = 1;
                string originalNewFilePath = newFilePath;
                while (File.Exists(newFilePath))
                {
                    string nameOnly = Path.GetFileNameWithoutExtension(originalNewFilePath);
                    string extension = Path.GetExtension(originalNewFilePath);
                    newFilePath = Path.Combine(OutputFolderPath, $"{nameOnly}_{counter}{extension}");
                    counter++;
                }

                try
                {
                    File.WriteAllText(newFilePath, newFileContent);
                    lock (_lockObject)
                    {
                        var result = new FileProcessResult(filePath)
                        {
                            FileSize = new FileInfo(filePath).Length,
                            ReplacementsCount = replacements,
                            FoundForbiddenWords = currentFileForbiddenWordCounts
                        };
                        _processingResults.Add(result);
                        foreach (var entry in currentFileForbiddenWordCounts)
                        {
                            if (_overallForbiddenWordCounts.ContainsKey(entry.Key))
                            {
                                _overallForbiddenWordCounts[entry.Key] += entry.Value;
                            }
                            else
                            {
                                _overallForbiddenWordCounts.Add(entry.Key, entry.Value);
                            }
                        }
                        OnFileProcessed?.Invoke(this, Tuple.Create(result, _processedFilesCount, _totalFilesToProcess));
                    }
                }
                catch (IOException ex)
                {
                    lock (_lockObject)
                    {
                        _processingResults.Add(new FileProcessResult(filePath) { IsSuccessful = false, ErrorMessage = $"Помилка запису файлу: {ex.Message}" });
                    }
                    OnError?.Invoke(this, $"Не вдалося записати змінений файл {newFilePath}: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    lock (_lockObject)
                    {
                        _processingResults.Add(new FileProcessResult(filePath) { IsSuccessful = false, ErrorMessage = $"Відмовлено в доступі при записі файлу: {ex.Message}" });
                    }
                    OnError?.Invoke(this, $"Відмовлено в доступі при записі файлу {newFilePath}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    lock (_lockObject)
                    {
                        _processingResults.Add(new FileProcessResult(filePath) { IsSuccessful = false, ErrorMessage = $"Невідома помилка при записі файлу: {ex.Message}" });
                    }
                    OnError?.Invoke(this, $"Невідома помилка при записі файлу {newFilePath}: {ex.Message}");
                }
            }
        }

        public void PauseScan()
        {
            _pauseTokenSource?.Cancel();
        }

        public void ResumeScan()
        {
            _pauseTokenSource = new CancellationTokenSource();
        }

        public void StopScan()
        {
            _cancellationTokenSource?.Cancel();
        }

        private void GenerateReport()
        {
            string report = "=== Звіт про сканування ===\n\n";

            report += $"Загальна кількість оброблених файлів: {_processingResults.Count}\n";
            report += $"Загальна кількість знайдених заборонених слів: {_overallForbiddenWordCounts.Sum(x => x.Value)}\n\n";

            report += "--- Деталі по файлах з замінами ---\n";
            if (_processingResults.Any())
            {
                foreach (var result in _processingResults.OrderByDescending(r => r.ReplacementsCount))
                {
                    report += $"Файл: {result.FilePath}\n";
                    report += $"  Розмір: {result.FileSize} байт\n";
                    report += $"  Кількість замін: {result.ReplacementsCount}\n";
                    if (result.FoundForbiddenWords.Any())
                    {
                        report += "  Знайдені слова та їх кількість:\n";
                        foreach (var entry in result.FoundForbiddenWords.OrderByDescending(e => e.Value))
                        {
                            report += $"    - '{entry.Key}': {entry.Value} разів\n";
                        }
                    }
                    if (!result.IsSuccessful)
                    {
                        report += $"  Помилка: {result.ErrorMessage}\n";
                    }
                    report += "\n";
                }
            }
            else
            {
                report += "Не знайдено файлів, що містять заборонені слова.\n\n";
            }
            report += "\n--- Топ-10 найпопулярніших заборонених слів ---\n";
            if (_overallForbiddenWordCounts.Any())
            {
                var top10 = _overallForbiddenWordCounts.OrderByDescending(kv => kv.Value)
                                                      .Take(10);
                int rank = 1;
                foreach (var entry in top10)
                {
                    report += $"{rank}. '{entry.Key}': {entry.Value} разів\n";
                    rank++;
                }
            }
            else
            {
                report += "Заборонені слова не знайдено.\n";
            }

            OnScanFinished?.Invoke(this, report);
        }
    }
}