using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    public partial class FeedbackWindow : Window
    {
        private const int MAX_IMAGES = 8;
        private List<string> _imagePaths = new List<string>();

        public FeedbackWindow()
        {
            InitializeComponent();
        }

        // ===============================
        // 📷 Chọn nhiều ảnh
        // ===============================
        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var file in dialog.FileNames)
                    AddImage(file);
            }
        }

        // ===============================
        // 🖱️ Drag & Drop nhiều ảnh
        // ===============================
        private void ImageDropBorder_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (var file in files)
                AddImage(file);
        }

        // ===============================
        // ➕ Add image
        // ===============================
        private void AddImage(string filePath)
        {
            if (_imagePaths.Count >= MAX_IMAGES)
            {
                MessageBox.Show($"Chỉ được tối đa {MAX_IMAGES} ảnh");
                return;
            }

            if (!File.Exists(filePath)) return;

            FileInfo file = new FileInfo(filePath);

            if (file.Length > 5 * 1024 * 1024)
                return;

            string ext = file.Extension.ToLower();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                return;

            if (_imagePaths.Contains(filePath))
                return;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(filePath);
            bitmap.EndInit();

            // UI item
            Border item = new Border
            {
                Width = 90,
                Height = 90,
                Margin = new Thickness(6),
                BorderThickness = new Thickness(1),
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                Child = new Grid()
            };

            Image img = new Image
            {
                Source = bitmap,
                Stretch = System.Windows.Media.Stretch.UniformToFill
            };

            Button removeBtn = new Button
            {
                Content = "❌",
                Width = 22,
                Height = 22,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Tag = filePath
            };
            removeBtn.Click += RemoveSingleImage_Click;

            ((Grid)item.Child).Children.Add(img);
            ((Grid)item.Child).Children.Add(removeBtn);

            ImagesPanel.Items.Add(item);
            _imagePaths.Add(filePath);
        }

        // ===============================
        // ❌ Xóa 1 ảnh
        // ===============================
        private void RemoveSingleImage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string path = btn.Tag.ToString();

            for (int i = ImagesPanel.Items.Count - 1; i >= 0; i--)
            {
                Border border = ImagesPanel.Items[i] as Border;
                Button removeBtn = ((Grid)border.Child).Children[1] as Button;

                if (removeBtn.Tag.ToString() == path)
                {
                    ImagesPanel.Items.RemoveAt(i);
                    _imagePaths.Remove(path);
                    break;
                }
            }
        }

        // ===============================
        // 🧹 Xóa tất cả
        // ===============================
        private void ClearImages_Click(object sender, RoutedEventArgs e)
        {
            ImagesPanel.Items.Clear();
            _imagePaths.Clear();
        }
    }
}