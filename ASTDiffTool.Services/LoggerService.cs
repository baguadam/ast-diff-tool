using ASTDiffTool.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services
{
    public class LoggerService : ILoggerService
    {
        public void Log(string filePath, string output, string error)
        {
            try
            {
                using var logWriter = new StreamWriter(filePath, append: true);

                logWriter.WriteLine("========================================");
                logWriter.WriteLine($"Log Timestamp: {DateTime.Now}");
                logWriter.WriteLine("========================================");

                if (!string.IsNullOrEmpty(output))
                {
                    logWriter.WriteLine("Standard Output:");
                    logWriter.WriteLine(output);
                }

                if (!string.IsNullOrEmpty(error))
                {
                    logWriter.WriteLine("Standard Error:");
                    logWriter.WriteLine(error);
                }

                logWriter.WriteLine(); // blank line for readability
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to write log: {ex.Message}");
            }
        }
    }
}
