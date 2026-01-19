using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using WpfApp1.Model;
using WpfApp1.Utils;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for ExcelToPDF.xaml
    /// </summary>
    public partial class ExcelToPDF : Window
    {
        private List<WordItem> _wordFiles = new();
        private Point _dragStart;
        string sofficePath = LibreOfficePath.GetSofficePath();
        public ExcelToPDF()
        {
            InitializeComponent();
        }

        // ➕ Thêm file Word
        private void AddWord_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Excel Files (*.xls;*.xlsx)|*.xls;*.xlsx",
                Multiselect = true
            };

            if (dlg.ShowDialog() == true)
            {
                foreach (var file in dlg.FileNames)
                {
                    _wordFiles.Add(new WordItem
                    {
                        FileName = Path.GetFileName(file),
                        FullPath = file
                    });
                }

                RefreshList();
            }
        }

        // ❌ Xóa file
        private void RemoveWord_Click(object sender, RoutedEventArgs e)
        {
            if (WordListBox.SelectedItem is WordItem item)
            {
                _wordFiles.Remove(item);
                RefreshList();
            }
        }


        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            if (_wordFiles.Count == 0)
            {
                MessageBox.Show("Chưa có file Word nào!");
                return;
            }

            SaveFileDialog folderDialog = new SaveFileDialog
            {
                Title = "Chọn thư mục xuất PDF",
                Filter = "Folder|*.folder",
                FileName = "select",
                OverwritePrompt = false,
                CheckPathExists = true
            };

            if (folderDialog.ShowDialog() != true)
                return;

            string outputFolder = Path.GetDirectoryName(folderDialog.FileName)!;

            // UI: bật process
            ConvertButton.IsEnabled = false;
            ConvertProgress.Visibility = Visibility.Visible;
            StatusText.Visibility = Visibility.Visible;
            ConvertProgress.Value = 0;

            Thread convertThread = new Thread(() =>
            {
                try
                {
                    string libreOfficePath = sofficePath;
                    int total = _wordFiles.Count;
                    int current = 0;

                    foreach (var item in _wordFiles)
                    {
                        current++;

                        Dispatcher.Invoke(() =>
                        {
                            StatusText.Text = $"Đang chuyển ({current}/{total}): {item.FileName}";
                            ConvertProgress.Value = (double)current / total * 100;
                        });

                        ConvertWordToPdf(
                            libreOfficePath,
                            item.FullPath,
                            outputFolder
                        );
                    }

                    Dispatcher.Invoke(() =>
                    {
                        StatusText.Text = "Hoàn tất chuyển đổi!";
                        ConvertProgress.Value = 100;
                        MessageBox.Show("Chuyển đổi Word → PDF thành công!");
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
                finally
                {
                    Dispatcher.Invoke(() =>
                    {
                        ConvertButton.IsEnabled = true;
                        ConvertProgress.Visibility = Visibility.Collapsed;
                        StatusText.Visibility = Visibility.Collapsed;
                    });
                }
            });

            convertThread.Start();
        }



        private void ConvertWordToPdf(string libreOfficePath, string inputFile, string outputFolder)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = libreOfficePath,
                Arguments = $"--headless --convert-to pdf --outdir \"{outputFolder}\" \"{inputFile}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using Process process = new Process();
            process.StartInfo = psi;
            process.Start();

            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception($"Lỗi convert Excel:\n{inputFile}\n{error}");
        }

        // 🧲 Drag để sắp xếp thứ tự
        private void ListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _dragStart = e.GetPosition(null);

            if (WordListBox.SelectedItem != null)
            {
                DragDrop.DoDragDrop(
                    WordListBox,
                    WordListBox.SelectedItem,
                    DragDropEffects.Move
                );
            }
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(WordItem)))
                return;

            WordItem dropped = (WordItem)e.Data.GetData(typeof(WordItem))!;
            WordItem? target =
                ((FrameworkElement)e.OriginalSource).DataContext as WordItem;

            if (target == null || dropped == target)
                return;

            int oldIndex = _wordFiles.IndexOf(dropped);
            int newIndex = _wordFiles.IndexOf(target);

            _wordFiles.RemoveAt(oldIndex);
            _wordFiles.Insert(newIndex, dropped);

            RefreshList();
        }

        private void RefreshList()
        {
            WordListBox.ItemsSource = null;
            WordListBox.ItemsSource = _wordFiles;
        }

    }
}
