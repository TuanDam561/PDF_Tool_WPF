using WpfApp1;
using System.Windows;


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
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}