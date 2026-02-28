using System.IO;
using System.Windows;
using System.Windows.Input;
using WpfApp1;


namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MergePdf_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MergePdfWindow window = new MergePdfWindow();

            this.Hide();                 // Ẩn cửa sổ chính
            window.Owner = this;         // Gán chủ
            window.ShowDialog();         // Mở dạng modal

            this.Show();                 // Hiện lại khi cửa sổ kia đóng
        }


        private void DocToPdf_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ConvertPDF window=new ConvertPDF();
            this.Hide();
            window.Owner = this;
            window.ShowDialog();
            this.Show();
        }

        private void SplitPdf_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SplitPDF window = new SplitPDF();
            this.Hide();
            window.Owner = this;
            window.ShowDialog();
            this.Show();
        }
        private void EncryptPdf_Click(object sender,System.Windows.Input.MouseButtonEventArgs e)
        {
            EncryptPdfWindow window = new EncryptPdfWindow();
            this.Hide();
            window.Owner = this;
            window.ShowDialog();
            this.Show();
        }
        private void ComparePdf_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ComparePdfWindow window = new ComparePdfWindow();
            this.Hide();
            window.Owner = this;
            window.ShowDialog();
            this.Show();
        }
        private void ExcelToPdf_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ExcelToPDF window = new ExcelToPDF();
            this.Hide();
            window.Owner = this;
            window.ShowDialog();
            this.Show();
        }

        private void EditPdf_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditPDF window = new EditPDF();
            this.Hide();
            window.Owner = this;
            window.ShowDialog();
            this.Show();
        }

        private void AddwatermarkPdf_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WatermarkPDF window = new WatermarkPDF();
            this.Hide();
            window.Owner = this;
            window.ShowDialog();
            this.Show();
        }
        private void ClearBG(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           MessageBox.Show("Chức năng này đang được phát triển, vui lòng quay lại sau!",
               "Thông báo",
               MessageBoxButton.OK,
               MessageBoxImage.Exclamation
               );
        }
        private void ClearUnlockedFiles(object sender, RoutedEventArgs e)
        {
            string unlockedDir = Path.Combine(
                AppContext.BaseDirectory,
                "UnlockedFiles"
            );

            if (!Directory.Exists(unlockedDir))
            {
                MessageBox.Show("Không tồn tại thư mục UnlockedFiles");
                return;
            }

            int count = 0;

            foreach (var file in Directory.GetFiles(unlockedDir))
            {
                try
                {
                    File.Delete(file);
                    count++;
                }
                catch { }
            }

            foreach (var dir in Directory.GetDirectories(unlockedDir))
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch { }
            }

            MessageBox.Show(
                $"Đã xóa {count} file tạm",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}