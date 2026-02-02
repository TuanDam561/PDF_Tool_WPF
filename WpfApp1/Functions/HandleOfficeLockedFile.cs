using DocumentFormat.OpenXml.Packaging;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace WpfApp1.Functions
{
    internal static class HandleOfficeLockedFile
    {
        /// <summary>
        /// Check Word / Excel có bị khóa password hay không (NHẸ – NHANH)
        /// Trả về true nếu KHÔNG bị khóa
        /// Trả về false nếu CÓ password
        /// </summary>
        public static bool TryOpenOfficeFile(
            string inputFile,
            Window owner)
        {
            var ext = Path.GetExtension(inputFile).ToLower();

            try
            {
                if (ext == ".docx")
                {
                    using var doc = WordprocessingDocument.Open(inputFile, false);
                    return true; // mở được → không khóa
                }

                if (ext == ".xlsx")
                {
                    using var doc = SpreadsheetDocument.Open(inputFile, false);
                    return true;
                }

                // các file khác coi như OK
                return true;
            }
            catch
            {
                //MessageBox.Show(
                //    $"File \"{Path.GetFileName(inputFile)}\" đang bị khóa mật khẩu vui lòng mở khóa trước khi đưa vào!.",
                //    "File bị khóa",
                //    MessageBoxButton.OK,
                //    MessageBoxImage.Warning);

                var result = MessageBox.Show(
                    $"File \"{Path.GetFileName(inputFile)}\" đang bị khóa mật khẩu.\n\n" +
                    "Bạn có muốn xem hướng dẫn cách xóa mật khẩu Word / Excel không?",
                    "File bị khóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://tuandam561.github.io/Tutoria_Remove_EncryptFile/",
                        UseShellExecute = true
                    });
                }

                return false;
            }
        }
    }
}

//using System.Diagnostics;
//using System.IO;
//using System.Windows;
//using WpfApp1.Utils;

//namespace WpfApp1.Functions
//{
//    internal static class HandleOfficeLockedFile
//    {
//        public static bool TryOpenOfficeFile(
//            string inputFile,
//            Window owner,
//            out string usableFilePath)
//        {
//            usableFilePath = inputFile;
//            var ext = Path.GetExtension(inputFile).ToLower();

//            try
//            {
//                // thử mở nhanh bằng OpenXML
//                if (ext == ".docx")
//                {
//                    using var _ = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(inputFile, false);
//                    return true;
//                }

//                if (ext == ".xlsx")
//                {
//                    using var _ = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Open(inputFile, false);
//                    return true;
//                }

//                return true;
//            }
//            catch
//            {
//                // 👉 BỊ KHÓA
//                var dlg = new PasswordDialog
//                {
//                    Owner = owner,
//                    PdfFileName = Path.GetFileName(inputFile)
//                };

//                if (dlg.ShowDialog() != true)
//                    return false;

//                return UnlockByLibreOffice(inputFile, owner, out usableFilePath);
//            }
//        }

//        /// <summary>
//        /// Gọi LibreOffice CLI để mở file encrypt và xuất file mới (không password)
//        /// </summary>
//        private static bool UnlockByLibreOffice(
//    string inputFile,
//    Window owner,
//    out string outputFile)
//        {
//            outputFile = string.Empty;

//            var sofficePath = LibreOfficePath.GetSofficePath();
//            if (sofficePath == null)
//            {
//                MessageBox.Show(owner, "Không tìm thấy LibreOffice.");
//                return false;
//            }

//            var dir = Path.GetDirectoryName(inputFile)!;
//            var ext = Path.GetExtension(inputFile);
//            var nameNoExt = Path.GetFileNameWithoutExtension(inputFile);

//            // 👉 gợi ý tên file sau khi save
//            var suggestedOutput = Path.Combine(
//                dir,
//                $"{nameNoExt}_unlocked{ext}"
//            );

//            MessageBox.Show(
//                owner,
//                "LibreOffice sẽ được mở.\n\n" +
//                "👉 Hãy nhập mật khẩu\n" +
//                "👉 Sau đó chọn File → Save As\n" +
//                $"👉 Lưu với tên: {Path.GetFileName(suggestedOutput)}\n\n" +
//                "Sau khi lưu xong thì đóng LibreOffice.",
//                "Mở file bị khóa",
//                MessageBoxButton.OK,
//                MessageBoxImage.Information
//            );

//            var psi = new ProcessStartInfo
//            {
//                FileName = sofficePath,
//                Arguments = $"\"{inputFile}\"",   // ❗ CHỈ MỞ FILE
//                UseShellExecute = true,
//                CreateNoWindow = false,
//                WindowStyle = ProcessWindowStyle.Normal
//            };

//            try
//            {
//                var proc = Process.Start(psi);
//                proc!.WaitForExit(); // chờ user đóng LibreOffice

//                if (!File.Exists(suggestedOutput))
//                {
//                    MessageBox.Show(
//                        owner,
//                        "Không tìm thấy file đã lưu.\n\n" +
//                        "Có thể bạn:\n" +
//                        "- Nhập sai mật khẩu\n" +
//                        "- Chưa Save As\n" +
//                        "- Hoặc lưu với tên khác",
//                        "Chưa unlock được",
//                        MessageBoxButton.OK,
//                        MessageBoxImage.Warning
//                    );
//                    return false;
//                }

//                outputFile = suggestedOutput;
//                return true;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(
//                    owner,
//                    "LibreOffice lỗi:\n" + ex.Message,
//                    "Lỗi",
//                    MessageBoxButton.OK,
//                    MessageBoxImage.Error
//                );
//                return false;
//            }
//        }
//    }
//}