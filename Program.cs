using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exam
{
    internal static class Program
    {
        //для запуску програми в консольному режимі необхідно змінити
        //RUN_AS_CONSOLE_APP на true, змінити тип виводу на Console Application
        //для цього: права клавіша на проект -> Властивості -> Тип виводу -> Console Application
        private const bool RUN_AS_CONSOLE_APP = false;

        [STAThread]
        static void Main(string[] args)
        {
            if (SingleInstanceApplication.IsApplicationAlreadyRunning())
            {
                if (RUN_AS_CONSOLE_APP)
                {
                    Console.WriteLine("Додаток вже запущено.");
                }
                else
                {
                    MessageBox.Show("Додаток вже запущено.", "Помилка запуску", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }

            try
            {
                if (RUN_AS_CONSOLE_APP)
                {
                    Console.WriteLine("Введіть шлях до файлу із забороненими словами:");
                    string forbiddenWordsFilePath = Console.ReadLine();

                    Console.WriteLine("Введіть шлях до папки для збереження оброблених файлів:");
                    string outputFolderPath = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(forbiddenWordsFilePath) || !File.Exists(forbiddenWordsFilePath))
                    {
                        Console.WriteLine($"Помилка: Файл із забороненими словами не знайдено або шлях недійсний: {forbiddenWordsFilePath}");
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(outputFolderPath) || !Directory.Exists(outputFolderPath))
                    {
                        Console.WriteLine($"Помилка: Папка для збереження не знайдена або шлях недійсний: {outputFolderPath}");
                        return;
                    }

                    string[] forbiddenWordsArray = File.ReadAllLines(forbiddenWordsFilePath);
                    HashSet<string> forbiddenWords = new HashSet<string>(
                        forbiddenWordsArray.Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)),
                        StringComparer.OrdinalIgnoreCase
                    );

                    if (!forbiddenWords.Any())
                    {
                        Console.WriteLine("Помилка: Файл із забороненими словами порожній або не містить дійсних слів.");
                        return;
                    }

                    ScannerLogic scanner = new ScannerLogic();
                    scanner.ForbiddenWords = forbiddenWords;
                    scanner.OutputFolderPath = outputFolderPath;

                    scanner.OnProgressUpdate += (sender, progress) =>
                    {
                        Console.Write($"\rПрогрес: {progress}%");
                        if (progress == 100)
                        {
                            Console.WriteLine();
                        }
                    };

                    scanner.OnFileProcessed += (sender, resultTuple) =>
                    {
                        var result = resultTuple.Item1;
                        var processedCount = resultTuple.Item2;
                        var totalFiles = resultTuple.Item3;

                        if (result.ReplacementsCount > 0)
                        {
                            Console.WriteLine($"\nОброблено: {Path.GetFileName(result.FilePath)}, Замін: {result.ReplacementsCount}. Всього оброблено: {processedCount} з {totalFiles}");
                        }
                    };

                    scanner.OnScanFinished += (sender, report) =>
                    {
                        Console.WriteLine("Сканування завершено.");
                        Console.WriteLine("Звіт:");
                        Console.WriteLine(report);
                        Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
                        Console.ReadKey();
                    };

                    scanner.OnError += (sender, errorMessage) => Console.WriteLine($"\nПОМИЛКА: {errorMessage}");

                    scanner.StartScanCommandline();
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
            }
            finally
            {
                SingleInstanceApplication.Release();
            }
        }
    }
}