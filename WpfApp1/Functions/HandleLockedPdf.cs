using PdfSharp.Pdf.IO;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WpfApp1.Utils;

namespace WpfApp1.Functions
{
    internal static class HandleLockedPdf
    {
        /// <summary>
        /// Thử mở PDF bằng PdfSharp.
        /// Nếu bị khóa → hỏi mật khẩu → unlock bằng QPDF.
        /// Trả về đường dẫn PDF hợp lệ để sử dụng tiếp.
        /// </summary>
        public static bool TryOpenPdf(
            string inputPdf,
            Window owner,
            out string usablePdfPath,
            out int pageCount)
        {
            usablePdfPath = inputPdf;
            pageCount = 0;

            try
            {
                using var doc = PdfReader.Open(
                    inputPdf,
                    PdfDocumentOpenMode.Import);

                pageCount = doc.PageCount;
                return true;
            }
            catch (PdfReaderException)
            {
                return HandleEncryptedPdf(
                    inputPdf,
                    owner,
                    out usablePdfPath,
                    out pageCount);
            }
        }

        // =========================
        // PDF bị khóa
        // =========================
        private static bool HandleEncryptedPdf(
            string pdfPath,
            Window owner,
            out string unlockedPdfPath,
            out int pageCount)
        {
            unlockedPdfPath = string.Empty;
            pageCount = 0;

            var dlg = new PasswordDialog
            {
                Owner = owner
            };

            if (dlg.ShowDialog() != true)
                return false;

            if (!QPdfUnlock(pdfPath, dlg.Password, out unlockedPdfPath))
            {
                MessageBox.Show(
                    "Mật khẩu không đúng hoặc không thể mở PDF.",
                    "PDF bị khóa",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            using var doc = PdfReader.Open(
                unlockedPdfPath,
                PdfDocumentOpenMode.Import);

            pageCount = doc.PageCount;
            return true;
        }

        // =========================
        // Unlock bằng QPDF
        // =========================
        private static bool QPdfUnlock(
            string inputPdf,
            string password,
            out string outputPdf)
        {
            outputPdf = Path.Combine(
                Path.GetTempPath(),
                $"unlock_{Guid.NewGuid()}.pdf");

            var psi = new ProcessStartInfo
            {
                FileName = QPDFPath.Get(),
                Arguments =
                    $"--password=\"{password}\" --decrypt " +
                    $"\"{inputPdf}\" \"{outputPdf}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var p = Process.Start(psi);
            p?.WaitForExit();

            return p?.ExitCode == 0 && File.Exists(outputPdf);
        }
    }
}
