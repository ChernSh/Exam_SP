using System;
using System.Collections.Generic;
namespace Exam
{
    public class FileProcessResult
    {
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public int ReplacementsCount { get; set; }
        public Dictionary<string, int> FoundForbiddenWords { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public FileProcessResult(string filePath)
        {
            FilePath = filePath;
            IsSuccessful = true;
        }
    }
}
