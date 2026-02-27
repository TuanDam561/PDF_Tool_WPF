using Microsoft.Win32;
using Syncfusion.Windows.Shared;
using System.Windows;
using WpfApp1.Functions;
using WpfApp1.Model;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for WatermarkPDF.xaml
    /// </summary>
    public partial class WatermarkPDF : Window
    {
        private string _pdfPath = "";
        private WatermarkOptions _options = new WatermarkOptions();
        public WatermarkPDF()
        {
            InitializeComponent();
            DataContext = _options;
            
            //Mặc định cho cái wattermark nằm dưới 
            _options.DrawOnTop = false;
            _options.ApplyToAllPages = true;

        }
        private void permissionOnly_Checked(object sender, RoutedEventArgs e)
        {
            //Check box cho bên chèn Text
            pageFrom.IsEnabled = false;
            pageTo.IsEnabled = false;

            //Check box cho bên chèn Image  
            pageFrom2.IsEnabled = false;
            pageTo2.IsEnabled = false;
        }
        private void permissionOnly_Unchecked(object sender, RoutedEventArgs e)
        {
            //Check box cho bên chèn Text
            pageFrom.IsEnabled = true;
            pageTo.IsEnabled = true;

            //Check box cho bên chèn Image
            pageFrom2.IsEnabled = true;
            pageTo2.IsEnabled = true;
        }
        //Chọn file PDF
        private async void SelectPdf_Click(object sender, RoutedEventArgs e)
        {
            // 1. Chọn file PDF
            var files = FilePicker.Pick(multiSelect: false);
            if (!files.Any())
                return;

            string selectedPdf = files.First();

            // 2. Thử mở PDF (tự xử lý trường hợp bị khóa)
            bool opened = HandleLockedPdf.TryOpenPdf(
                selectedPdf,
                this,
                out string usablePdfPath,
                out int pageCount);

            if (!opened)
                return;

            _pdfPath = usablePdfPath;

            await PdfViewer.EnsureCoreWebView2Async();
            PdfViewer.Source = new Uri(_pdfPath);

        }
        //Mở ảnh đưa vào watermark
        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Title = "Chọn ảnh làm watermark",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
            };
            if(dlg.ShowDialog() == true)
            {
                _options.ImagePath = dlg.FileName;
            }
        }
        //Thêm watermark vào PDF
        private void AddWatermark_Click(object sender, RoutedEventArgs e)
        {
            string text= inputSign.Text;
            string tabTextFrom=pageFrom.Text;
            string tabTextTo = pageTo.Text;
            string tabImgForm = pageFrom2.Text;
            string tabImgTo = pageTo2.Text;
            bool isCheckedText = ApplyAll.IsChecked == true;
            bool isCheckedImage = ApplyAlll.IsChecked == true;
            int selectionTab = _options.SelectedTabIndex;

            if (string.IsNullOrEmpty(_pdfPath))
            {
                MessageBox.Show("Chưa chọn file PDF!","Cảnh báo",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra có ký tự chữ không
            if (string.IsNullOrWhiteSpace(text)&& selectionTab==0)
            {
                MessageBox.Show("Chưa thêm chữ để đánh dấu chìm", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //if (string.IsNullOrWhiteSpace(tabTextFrom) && string.IsNullOrWhiteSpace(tabTextTo) &&!isCheckedText)
            //{
            //    MessageBox.Show("Thêm số trang cần đánh dấu chìm", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}
            //if (string.IsNullOrWhiteSpace(tabImgForm) && string.IsNullOrWhiteSpace(tabImgTo) && !isCheckedImage)
            //{
            //    MessageBox.Show("Thêm số trang cần đánh dấu chìm", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            if (!_options.ApplyToAllPages)
            {
                if (!_options.PageFrom.HasValue || !_options.PageTo.HasValue)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ từ trang – đến trang");
                    return;
                }

                if (_options.PageFrom <= 0 || _options.PageTo <= 0)
                {
                    MessageBox.Show("Số trang phải lớn hơn 0");
                    return;
                }

                if (_options.PageFrom > _options.PageTo)
                {
                    MessageBox.Show("Trang bắt đầu không được lớn hơn trang kết thúc");
                    return;
                }
            }
            SaveFileDialog saveDlg = new SaveFileDialog
            {
                Title = "PDF đã thêm dấu chìm",
                Filter = "PDF Files|*.pdf",
                FileName = "Watermarked.pdf"
            };
            if(saveDlg.ShowDialog() != true)
            {
                return;
            }
            try
            {
                AddWatermarkService service=new AddWatermarkService();
                service.AddWatermark(
                    _pdfPath,
                    saveDlg.FileName,
                    _options
                );
                MessageBox.Show("Đóng watermark thành công!","Thành công",MessageBoxButton.OK,MessageBoxImage.Information);
                // Load lại PDF mới để xem
                PdfViewer.Source = new Uri(saveDlg.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đóng watermark:\n" + ex.Message);
            }
        }
    }
}
