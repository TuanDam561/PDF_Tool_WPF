using Microsoft.Win32;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Model;
using PdfItem = WpfApp1.Model.PdfItem;

namespace WpfApp1
{
    public partial class MergePdfWindow : Window
    {
        private ObservableCollection<PdfItem> pdfItems
            = new ObservableCollection<PdfItem>();

        public MergePdfWindow()
        {
            InitializeComponent();
            PdfListBox.ItemsSource = pdfItems;
        }

        // =========================
        // THÊM FILE
        // =========================
        private void AddPdf_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var file in dialog.FileNames)
                {
                    if (!pdfItems.Any(x => x.FullPath == file))
                    {
                        pdfItems.Add(new PdfItem
                        {
                            FileName = Path.GetFileName(file),
                            FullPath = file
                        });
                    }
                }
            }
        }

        // =========================
        // KÉO THẢ FILE
        // =========================
        private void PdfListBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        private void PdfListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in files)
                {
                    if (Path.GetExtension(file).ToLower() == ".pdf"
                        && !pdfItems.Any(x => x.FullPath == file))
                    {
                        pdfItems.Add(new PdfItem
                        {
                            FileName = Path.GetFileName(file),
                            FullPath = file
                        });
                    }
                }
            }
        }

        // =========================
        // XÓA FILE
        // =========================
        private void RemovePdf_Click(object sender, RoutedEventArgs e)
        {
            if (PdfListBox.SelectedItem is PdfItem item)
            {
                pdfItems.Remove(item);
            }
        }

        // =========================
        // SẮP XẾP ⬆ ⬇
        // =========================
        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            int index = PdfListBox.SelectedIndex;

            if (index > 0)
            {
                var item = pdfItems[index];
                pdfItems.RemoveAt(index);
                pdfItems.Insert(index - 1, item);
                PdfListBox.SelectedIndex = index - 1;
            }
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            int index = PdfListBox.SelectedIndex;

            if (index >= 0 && index < pdfItems.Count - 1)
            {
                var item = pdfItems[index];
                pdfItems.RemoveAt(index);
                pdfItems.Insert(index + 1, item);
                PdfListBox.SelectedIndex = index + 1;
            }
        }

        // =========================
        // MERGE PDF
        // =========================
        private void MergePdf_Click(object sender, RoutedEventArgs e)
        {
            if (pdfItems.Count < 2)
            {
                MessageBox.Show("Cần ít nhất 2 file PDF để nối.");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "PDF file (*.pdf)|*.pdf",
                FileName = "merged.pdf"
            };

            if (saveDialog.ShowDialog() != true)
                return;

            try
            {
                PdfDocument outputDocument = new PdfDocument();

                foreach (var item in pdfItems)
                {
                    PdfDocument inputDocument =
                        PdfReader.Open(item.FullPath, PdfDocumentOpenMode.Import);

                    for (int i = 0; i < inputDocument.PageCount; i++)
                    {
                        outputDocument.AddPage(inputDocument.Pages[i]);
                    }
                }

                outputDocument.Save(saveDialog.FileName);

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
        }
    }
}
