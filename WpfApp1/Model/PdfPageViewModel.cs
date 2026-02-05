//using System.Windows.Media;

//namespace WpfApp1
//{
//    public class PdfPageViewModel
//    {
//        public int PageIndex { get; set; }
//        public int PageNumber => PageIndex + 1;

//        public ImageSource PageImage { get; set; } = null!;
//        public int Rotation { get; set; } = 0; // 0, 90, 180, 270
//        public bool IsSelected { get; set; }

//        public double OverlayHeight { get; set; }
//    }
//}

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace WpfApp1.Model
{
    public class PdfPageViewModel : INotifyPropertyChanged
    {
        // 👉 Trang gốc trong file PDF (1-based, dùng khi Save)
        public int OriginalPageNumber { get; set; }

        private BitmapSource? _pageImage;
        public BitmapSource? PageImage
        {
            get => _pageImage;
            set
            {
                _pageImage = value;
                OnPropertyChanged();
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        private int _rotation;
        public int Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                OnPropertyChanged();
            }
        }

        public int OverlayHeight { get; set; }

        public int PageNumber => OriginalPageNumber;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

