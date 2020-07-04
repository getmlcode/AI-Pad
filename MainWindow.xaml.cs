using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using TextEditor.TextAnalytics;


namespace TextEditor
{
    public partial class MainWindow : Window
    {
        private DocumentManager documentManager;
        private readonly PrintManager printManager;
        private DocumentKeywordsExtractor KeyWordsExtractor;

        public MainWindow()
        {
            InitializeComponent();

            documentManager = new DocumentManager(richTextBox);
            printManager = new PrintManager(richTextBox);
            KeyWordsExtractor = new DocumentKeywordsExtractor(richTextBox);

            toolbar.getDocumentRakeKeywordsEvent += ExtractRakeKeywords;

        }

        private void NewDocument(object sender, ExecutedRoutedEventArgs e)
        {
            documentManager.NewDocument();
            status.Text = "New Document";
        }

        private void OpenDocument(object sender, ExecutedRoutedEventArgs e)
        {
            if (documentManager.OpenDocument())
                status.Text = "Document loaded";
        }

        private void SaveDocument_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = documentManager.CanSaveDocument();
        }

        private void SaveDocument(object sender, ExecutedRoutedEventArgs e)
        {
            if (documentManager.SaveDocument())
                status.Text = "Document saved";
        }

        private void SaveDocumentAs(object sender,ExecutedRoutedEventArgs e)
        {
            if (documentManager.SaveDocumentAs())
                status.Text = "Document saved";
        }

        private void ApplicationClose(object sender,ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void TextEditorToolBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (toolbar.isSynchronizing) return;

            ComboBox source = (ComboBox)e.OriginalSource;

            if (source == null) return;

            switch (source.Name)
            {
                case "fonts":
                    documentManager.ApplyToSelection(TextBlock.FontFamilyProperty, source.SelectedItem);
                    break;
                case "fontSize":
                    documentManager.ApplyToSelection(TextBlock.FontSizeProperty, source.SelectedItem);
                    break;
            }

            richTextBox.Focus();
        }

        private void text_SelectionChanged(object sender, RoutedEventArgs e)
        {
            toolbar.SynchronizeWith(richTextBox.Selection);
        }

        private void PrintPreview(object sender, ExecutedRoutedEventArgs e)
        {
            printManager.PrintPreview();
        }

        private void PrintDocument(object sender, ExecutedRoutedEventArgs e)
        {
            //PrintDialog dlg = new PrintDialog();
            //if (dlg.ShowDialog() == true)
            //{
            //    dlg.PrintDocument( 
            //        ( (IDocumentPaginatorSource)richTextBox.Document ).DocumentPaginator,
            //        "Text Editor Printing"
            //        );
            //}

            if (printManager.Print())
            {
                status.Text = "Document sent to printer";
            }
        }

        private void ExtractRakeKeywords()
        {
            KeyWordsExtractor.GetRakeKeywords();
        }
    }
}
