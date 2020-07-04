using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TextEditor
{
    public class PrintManager
    {
        public static readonly int DPI = 96;
        private readonly RichTextBox textBox;

        public PrintManager(RichTextBox _textBox)
        {
            textBox = _textBox;
        }

        public void PrintPreview()
        {
            PrintPreviewDialog dlg = new PrintPreviewDialog(this);
            dlg.ShowDialog();
        }

        public bool Print()
        {
            PrintDialog dlg = new PrintDialog();
            if (dlg.ShowDialog() == true)
            {
                PrintQueue printQueue = dlg.PrintQueue;

                DocumentPaginator paginator = GetPaginator(
                    printQueue.UserPrintTicket.PageMediaSize.Width.Value,
                    printQueue.UserPrintTicket.PageMediaSize.Height.Value
                    );

                dlg.PrintDocument(paginator, "TextEditor Printing");

                return true;
            }
            return false;
        }

        public DocumentPaginator GetPaginator(double pageWidth, double pageHeight)
        {
            TextRange originalRange = new TextRange(
                textBox.Document.ContentStart,
                textBox.Document.ContentEnd
                );

            MemoryStream memoryStream = new MemoryStream();

            originalRange.Save(memoryStream, DataFormats.Xaml);

            FlowDocument clonedDocument = new FlowDocument();

            TextRange copyRange = new TextRange(clonedDocument.ContentStart, clonedDocument.ContentEnd);

            copyRange.Load(memoryStream, DataFormats.Xaml);

            DocumentPaginator paginator = ((IDocumentPaginatorSource)clonedDocument).DocumentPaginator;

            Size pageSize = new Size(pageWidth, pageHeight);
            Size pageMargin = new Size(DPI, DPI);

            return new PrintingPaginator(
                paginator,
                pageSize,
                pageMargin
                );
        }
    }
}
