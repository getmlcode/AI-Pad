using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TextEditor
{
    class DocumentManager
    {
        private string currentFile;
        private RichTextBox textContainer;

        public DocumentManager(RichTextBox textBox)
        {
            textContainer = textBox;
        }

        public void NewDocument()
        {
            currentFile = null;
            textContainer.Document = new FlowDocument();
        }

        public bool OpenDocument()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                currentFile = dlg.FileName;
                using (Stream stream = dlg.OpenFile())
                {
                    TextRange range = new TextRange(
                        textContainer.Document.ContentStart,
                        textContainer.Document.ContentEnd
                        );

                    range.Load(stream, DataFormats.Rtf);
                }
                return true;
            }
            return false;
        }

        public bool CanSaveDocument()
        {
            return !string.IsNullOrEmpty(currentFile);
        }

        public bool SaveDocument()
        {
            if (string.IsNullOrEmpty(currentFile))
            {
                return SaveDocumentAs();
            }
            else
            {
                using (Stream stream =
                new FileStream(currentFile, FileMode.Create))
                {
                    TextRange range = new TextRange(
                    textContainer.Document.ContentStart,
                    textContainer.Document.ContentEnd
                    );
                    range.Save(stream, DataFormats.Rtf);
                }
                return true;
            }
        }

        public bool SaveDocumentAs()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (dlg.ShowDialog() == true)
            {
                currentFile = dlg.FileName;
                return SaveDocument();
            }
            return false;
        }

        public void ApplyToSelection(DependencyProperty property, object value)
        {
            if (value != null)
                textContainer.Selection.ApplyPropertyValue(property, value);
        }
    }
}
