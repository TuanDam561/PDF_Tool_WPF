using Microsoft.Win32;
using System.IO;
using WpfApp1.Model;

namespace WpfApp1.Functions
{
    internal class FilePicker
    {
        /// <summary>
        /// Mở dialog chọn file PDF
        /// </summary>
        /// <param name="multiSelect">Cho phép chọn nhiều file</param>
        /// <returns>Danh sách full path file PDF</returns>
        public static List<string> Pick(bool multiSelect = true)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Multiselect = multiSelect
            };

            return dialog.ShowDialog() == true
                ? dialog.FileNames.ToList()
                : new List<string>();
        }

        public static List<string> PickWord(bool multiSelect = true)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Word Files (*.doc;*.docx)|*.doc;*.docx",
                Multiselect = multiSelect
            };

            return dialog.ShowDialog() == true
                ? dialog.FileNames.ToList()
                : new List<string>();
        }

        public static List<string> PickExcel(bool multiSelect = true)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xls;*.xlsx)|*.xls;*.xlsx",
                Multiselect = multiSelect
            };

            return dialog.ShowDialog() == true
                ? dialog.FileNames.ToList()
                : new List<string>();
        }


        /// <summary>
        /// Thêm file PDF vào collection, tự tránh trùng
        /// </summary>
        public static void AddToCollection(
            IEnumerable<string> files,
            IList<PdfItem> target)
        {
            foreach (var file in files)
            {
                if (!target.Any(x => x.FullPath == file))
                {
                    target.Add(new PdfItem
                    {
                        FileName = Path.GetFileName(file),
                        FullPath = file
                    });
                }
            }
        }
    }
}
