//using System.IO;

//namespace WpfApp1.Utils
//{
//    internal static class LibreOfficePath
//    {
//        public static string GetSofficePath()
//        {
//            string[] paths =
//            {
//                @"C:\Program Files\LibreOffice\program\soffice.exe",
//                @"C:\Program Files (x86)\LibreOffice\program\soffice.exe"
//            };

//            foreach (var path in paths)
//            {
//                if (File.Exists(path))
//                    return path;
//            }

//            throw new FileNotFoundException(
//                "Không tìm thấy LibreOffice. Vui lòng cài LibreOffice."
//            );
//        }
//    }
//}


using System.IO;
using System.Windows;

namespace WpfApp1.Utils
{
    internal static class LibreOfficePath
    {
        public static string? GetSofficePath()
        {
            string[] paths =
            {
                @"C:\Program Files\LibreOffice\program\soffice.exe",
                @"C:\Program Files (x86)\LibreOffice\program\soffice.exe"
            };

            foreach (var path in paths)
            {
                if (File.Exists(path))
                    return path;
            }

            MessageBox.Show(
                "Không tìm thấy LibreOffice.\nVui lòng cài LibreOffice để sử dụng chức năng này.",
                "Thiếu LibreOffice",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return null; // ❗ KHÔNG kill app
        }
    }
}
