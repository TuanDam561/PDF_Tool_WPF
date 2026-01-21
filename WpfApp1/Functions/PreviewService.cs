using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1.Functions
{
    public static class PreviewService
    {
        public static async Task PreviewWithLibreOfficeAsync(
            string inputFile,
            string sofficePath)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "WpfPreview");
            Directory.CreateDirectory(tempDir);

            string expectedPdf = Path.Combine(
                tempDir,
                Path.GetFileNameWithoutExtension(inputFile) + ".pdf"
            );

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = sofficePath,
                    Arguments = $"--headless --convert-to pdf --outdir \"{tempDir}\" \"{inputFile}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                await Task.Run(() =>
                {
                    using Process p = Process.Start(psi)!;
                    p.WaitForExit();
                });

                if (!File.Exists(expectedPdf))
                    throw new Exception("Không tạo được file preview PDF");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = null;
                    var preview = new PreviewFile(expectedPdf);
                    preview.ShowDialog();
                });
            }
            finally
            {
                if (File.Exists(expectedPdf))
                    File.Delete(expectedPdf);
            }
        }
    }
}
