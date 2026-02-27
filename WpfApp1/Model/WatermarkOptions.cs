using PdfSharp.Drawing;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp1.Model
{
    public class WatermarkOptions : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ================= TEXT WATERMARK =================

        private bool _enabled = true;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _text="";
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _fontName = "Arial";
        public string FontName
        {
            get => _fontName;
            set
            {
                if (_fontName != value)
                {
                    _fontName = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _fontSize = 36;
        public double FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _textOpacity = 80;
        public int TextOpacity
        {
            get => _textOpacity;
            set
            {
                if (_textOpacity != value)
                {
                    _textOpacity = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _textRotation = -45;
        public double TextRotation
        {
            get => _textRotation;
            set
            {
                if (_textRotation != value)
                {
                    _textRotation = value;
                    OnPropertyChanged();
                }
            }
        }

        private XColor _textColor = XColors.Red;
        public XColor TextColor
        {
            get => _textColor;
            set
            {
                if (_textColor != value)
                {
                    _textColor = value;
                    OnPropertyChanged();
                }
            }
        }

        // ================= IMAGE WATERMARK =================

        private bool _enableImage;
        public bool EnableImage
        {
            get => _enableImage;
            set
            {
                if (_enableImage != value)
                {
                    _enableImage = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _imageScale = 0.5;
        public double ImageScale
        {
            get => _imageScale;
            set
            {
                if (_imageScale != value)
                {
                    _imageScale = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _imageOpacity = 80;
        public int ImageOpacity
        {
            get => _imageOpacity;
            set
            {
                if (_imageOpacity != value)
                {
                    _imageOpacity = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _imageRotation;
        public double ImageRotation
        {
            get => _imageRotation;
            set
            {
                if (_imageRotation != value)
                {
                    _imageRotation = value;
                    OnPropertyChanged();
                }
            }
        }

        // ================= COMMON =================

        private bool _drawOnTop;
        public bool DrawOnTop
        {
            get => _drawOnTop;
            set
            {
                if (_drawOnTop != value)
                {
                    _drawOnTop = value;
                    OnPropertyChanged();
                }
            }
        }
        // Cho phép nhập trang hay không
        public bool IsPageRangeEnabled => !ApplyToAllPages;
        private bool _applyToAllPages = true;
        //public bool ApplyToAllPages
        //{
        //    get => _applyToAllPages;
        //    set
        //    {
        //        if (_applyToAllPages != value)
        //        {
        //            _applyToAllPages = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}
        public bool ApplyToAllPages
        {
            get => _applyToAllPages;
            set
            {
                if (_applyToAllPages != value)
                {
                    _applyToAllPages = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsPageRangeEnabled));
                }
            }
        }
        // ================= PAGE RANGE =================

        private int? _pageFrom;
        public int? PageFrom
        {
            get => _pageFrom;
            set
            {
                if (_pageFrom != value)
                {
                    _pageFrom = value;
                    OnPropertyChanged();
                }
            }
        }

        private int? _pageTo;
        public int? PageTo
        {
            get => _pageTo;
            set
            {
                if (_pageTo != value)
                {
                    _pageTo = value;
                    OnPropertyChanged();
                }
            }
        }
        // ================= SELECTION TAB =================
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}