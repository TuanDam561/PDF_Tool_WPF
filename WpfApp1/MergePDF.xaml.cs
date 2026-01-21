using Microsoft.Win32;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Functions;
using WpfApp1.Model;
using PdfItem = WpfApp1.Model.PdfItem;

namespace WpfApp1
{
    public partial class MergePdfWindow: Window
    {
        private readonly ObservableCollection<PdfItem> _pdfItems
            = new ObservableCollection<PdfItem>();

        public MergePdfWindow()
        {
            InitializeComponent();
            PdfListBox.ItemsSource = _pdfItems;
        }

        // =========================
        // THÊM FILE
        // =========================
        private void AddPdf_Click(object sender, RoutedEventArgs e)
        {
            var files = FilePicker.Pick(true);
            FilePicker.AddToCollection(files, _pdfItems);
            PreviewButton.IsEnabled = _pdfItems.Count >= 2;

        }

        // =========================
        // KÉO THẢ FILE
        // =========================
        private void PdfListBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;

            e.Handled = true;
        }

        private void PdfListBox_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (var file in files.Where(f => Path.GetExtension(f).ToLower() == ".pdf"))
            {
                if (_pdfItems.Any(x => x.FullPath == file))
                    continue;

                _pdfItems.Add(new PdfItem
                {
                    FileName = Path.GetFileName(file),
                    FullPath = file
                });
            }
        }

        // =========================
        // XÓA FILE
        // =========================
        private void RemovePdf_Click(object sender, RoutedEventArgs e)
        {
            if (PdfListBox.SelectedItem is PdfItem item)
                _pdfItems.Remove(item);
        }

        // =========================
        // SẮP XẾP ⬆ ⬇
        // =========================
        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            int index = PdfListBox.SelectedIndex;
            if (index <= 0) return;

            (_pdfItems[index - 1], _pdfItems[index]) =
                (_pdfItems[index], _pdfItems[index - 1]);

            PdfListBox.SelectedIndex = index - 1;
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            int index = PdfListBox.SelectedIndex;
            if (index < 0 || index >= _pdfItems.Count - 1) return;

            (_pdfItems[index + 1], _pdfItems[index]) =
                (_pdfItems[index], _pdfItems[index + 1]);

            PdfListBox.SelectedIndex = index + 1;
        }

        // =========================
        // PREVIEW FILE ĐANG CHỌN
        // =========================
        private async void PreviewPdf_Click(object sender, RoutedEventArgs e)
        {
            if (_pdfItems.Count < 2)
            {
                MessageBox.Show("Cần ít nhất 2 file PDF để xem trước bản ghép.");
                return;
            }

            string tempFile = Path.Combine(
                Path.GetTempPath(),
                $"preview_merge_{Guid.NewGuid():N}.pdf"
            );

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                await Task.Run(() =>
                {
                    using PdfDocument output = new PdfDocument();

                    foreach (var item in _pdfItems)
                    {
                        using PdfDocument input =
                            PdfReader.Open(item.FullPath, PdfDocumentOpenMode.Import);

                        for (int i = 0; i < input.PageCount; i++)
                            output.AddPage(input.Pages[i]);
                    }

                    output.Save(tempFile);
                });

                Mouse.OverrideCursor = null;

                var preview = new PreviewFile(tempFile)
                {
                    Cursor = Cursors.Arrow
                };
                preview.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể preview:\n" + ex.Message);
            }
            finally
            {
                Mouse.OverrideCursor = null;

                // dọn file tạm
                if (File.Exists(tempFile))
                {
                    try { File.Delete(tempFile); }
                    catch { /* ignore */ }
                }
            }
        }


        // =========================
        // MERGE PDF (ASYNC)
        // =========================
        private async void MergePdf_Click(object sender, RoutedEventArgs e)
        {
            if (_pdfItems.Count < 2)
            {
                MessageBox.Show("Cần ít nhất 2 file PDF để nối.");
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Filter = "PDF file (*.pdf)|*.pdf",
                FileName = "merged.pdf"
            };

            if (saveDialog.ShowDialog() != true)
                return;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                await MergePdfAsync(saveDialog.FileName);
                MessageBox.Show("Nối PDF thành công 🎉");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể nối PDF:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private Task MergePdfAsync(string outputPath)
        {
            return Task.Run(() =>
            {
                using PdfDocument output = new PdfDocument();

                foreach (var item in _pdfItems)
                {
                    using PdfDocument input =
                        PdfReader.Open(item.FullPath, PdfDocumentOpenMode.Import);

                    for (int i = 0; i < input.PageCount; i++)
                        output.AddPage(input.Pages[i]);
                }

                output.Save(outputPath);
            });
        }
    }
}
