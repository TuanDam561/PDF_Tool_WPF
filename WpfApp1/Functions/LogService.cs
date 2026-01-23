using System.IO;
using System.Text;

namespace WpfApp1.Functions
{
    internal static class LogService
    {
        private static readonly string LogDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        public static void LogException(Exception ex, string source)
        {
            try
            {
                if (!Directory.Exists(LogDir))
                    Directory.CreateDirectory(LogDir);

                var filePath = Path.Combine(
                    LogDir,
                    $"log_{DateTime.Now:yyyy-MM-dd}.txt");

                var sb = new StringBuilder();
                sb.AppendLine("================================");
                sb.AppendLine($"Time   : {DateTime.Now:HH:mm:ss}");
                sb.AppendLine($"Source : {source}");
                sb.AppendLine($"Type   : {ex.GetType().FullName}");
                sb.AppendLine($"Message: {ex.Message}");
                sb.AppendLine("Stack  :");
                sb.AppendLine(ex.StackTrace);
                sb.AppendLine();

                File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
            }
            catch
            {
                // ❗ tuyệt đối không throw trong logger
            }
        }
    }
}
