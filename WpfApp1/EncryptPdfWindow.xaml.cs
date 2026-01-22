using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using WpfApp1.Functions;
using WpfApp1.Utils;


namespace WpfApp1
{
    public partial class EncryptPdfWindow : Window
    {
        private List<string> _selectedFiles = new();

        public EncryptPdfWindow()
        {
            InitializeComponent();
        }

        // =============================
        // UI LOGIC
        // =============================
        private void PermissionOnly_Checked(object sender, RoutedEventArgs e)
        {
            OpenPasswordBox.Password = "";
            OpenPasswordBox.IsEnabled = false;
        }

        private void PermissionOnly_Unchecked(object sender, RoutedEventArgs e)
        {
            OpenPasswordBox.IsEnabled = true;
        }

        // =============================
        // CHỌN FILE PDF
        // =============================
        private void SelectPdf_Click(object sender, RoutedEventArgs e)
        {
            _selectedFiles = FilePicker.Pick(MultiFileCheckBox.IsChecked == true);

            if (_selectedFiles.Count == 0)
            {
                PdfInfoTextBox.Text = "";
                return;
            }

            PdfInfoTextBox.Text = _selectedFiles.Count == 1
                ? Path.GetFileName(_selectedFiles[0])
                : $"Đã chọn {_selectedFiles.Count} file PDF";
        }


        // =============================
        // CLICK MÃ HÓA
        // =============================
        private void EncryptPdf_Click(object sender, RoutedEventArgs e)
        {
            if (!_selectedFiles.Any())
            {
                MessageBox.Show("Vui lòng chọn file PDF");
                return;
            }

            bool permissionOnly = PermissionOnlyCheckBox.IsChecked == true;

            if (!permissionOnly && string.IsNullOrWhiteSpace(OpenPasswordBox.Password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu mở file");
                return;
            }

            string qpdfPath = QPDFPath.Get();
            //MessageBox.Show(qpdfPath + "\nTồn tại: " + File.Exists(qpdfPath));
            if (!File.Exists(qpdfPath))
            {
                MessageBox.Show("Không tìm thấy qpdf.exe");
                return;
            }

            try
            {
                if (_selectedFiles.Count == 1)
                    EncryptSingleFile(qpdfPath, _selectedFiles[0]);
                else
                    EncryptMultipleFiles(qpdfPath);

                MessageBox.Show("Mã hóa PDF thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi");
            }
        }

        // =============================
        // MÃ HÓA 1 FILE
        // =============================
        private void EncryptSingleFile(string qpdfPath, string inputFile)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = Path.GetFileNameWithoutExtension(inputFile) + "_encrypted.pdf"
            };

            if (saveDialog.ShowDialog() != true)
                return;

            EncryptByQpdf(
                qpdfPath,
                inputFile,
                saveDialog.FileName
            );
        }

        // =============================
        // MÃ HÓA NHIỀU FILE
        // =============================
        private void EncryptMultipleFiles(string qpdfPath)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Title = "Chọn thư mục lưu (chọn tên bất kỳ)",
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = "chon_thu_muc_luu.pdf"
            };

            if (saveDialog.ShowDialog() != true)
                return;

            string outputDir = Path.GetDirectoryName(saveDialog.FileName)!;

            foreach (var file in _selectedFiles)
            {
                string outputFile = Path.Combine(
                    outputDir,
                    Path.GetFileNameWithoutExtension(file) + "_encrypted.pdf");

                EncryptByQpdf(qpdfPath, file, outputFile);
            }
        }

        // =============================
        // CORE QPDF
        // =============================
        private void EncryptByQpdf(string qpdfPath, string inputFile, string outputFile)
        {
            bool permissionOnly = PermissionOnlyCheckBox.IsChecked == true;

            string userPassword = permissionOnly ? "" : OpenPasswordBox.Password;
            string ownerPassword = Guid.NewGuid().ToString("N");

            bool allowPrint = AllowPrintCheckBox.IsChecked == true;
            bool allowCopy = AllowCopyCheckBox.IsChecked == true;
            bool allowEdit = AllowEditCheckBox.IsChecked == true;

            var args = new StringBuilder();

            if (permissionOnly)
                args.Append("--allow-weak-crypto ");

            args.Append($"--encrypt \"{userPassword}\" \"{ownerPassword}\" 256 ");
            args.Append($"--print={(allowPrint ? "full" : "none")} ");
            args.Append($"--extract={(allowCopy ? "y" : "n")} ");
            args.Append($"--modify={(allowEdit ? "all" : "none")} ");
            args.Append("--annotate=n ");
            args.Append("-- ");
            args.Append($"\"{inputFile}\" \"{outputFile}\"");

            var psi = new ProcessStartInfo
            {
                FileName = qpdfPath,
                Arguments = args.ToString(),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            using var process = Process.Start(psi)!;
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception(process.StandardError.ReadToEnd());

            if (!File.Exists(outputFile))
                throw new Exception("qpdf không tạo được file PDF.");
        }




    }
}
