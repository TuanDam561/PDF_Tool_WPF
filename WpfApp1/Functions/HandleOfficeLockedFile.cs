using System;
using System.IO;
using System.Windows;
using DocumentFormat.OpenXml.Packaging;

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
                MessageBox.Show(
                    $"File \"{Path.GetFileName(inputFile)}\" đang bị khóa mật khẩu vui lòng mở khóa trước khi đưa vào!.",
                    "File bị khóa",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }
        }
    }
}
