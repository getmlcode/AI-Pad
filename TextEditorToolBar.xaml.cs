using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TextEditor.TextAnalytics;


namespace TextEditor
{
    public partial class TextEditorToolBar : UserControl
    {
        public delegate void getDocumentKeywords();
        public event getDocumentKeywords getDocumentRakeKeywordsEvent;

        public TextEditorToolBar()
        {
            InitializeComponent();
        }

        public bool isSynchronizing { get; private set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            for (double i = 8; i < 48; i += 2)
            {
                fontSize.Items.Add(i);
            }
        }

        public void SynchronizeWith(TextSelection selectedText)
        {
            isSynchronizing = true;

            Synchronize<double>(selectedText, TextBlock.FontSizeProperty, UpdateFontSizeInfo);

            Synchronize<FontWeight>(selectedText, TextBlock.FontWeightProperty, UpdateFontWeightInfo);

            Synchronize<FontStyle>(selectedText, TextBlock.FontStyleProperty, UpdateFontStyleInfo);

            Synchronize<TextDecorationCollection>(
                selectedText,
                TextBlock.TextDecorationsProperty,
                UpdateTextDecorationInfo);

            Synchronize<FontFamily>(selectedText, TextBlock.FontFamilyProperty, UpdateFontFamilyInfo);

            isSynchronizing = false;
        }

        private void Synchronize<T>(
            TextSelection selectedText,
            DependencyProperty property,
            Action<T> actionToPerform
            )
        {
            object propertyValue = selectedText.GetPropertyValue(property);

            if (propertyValue != DependencyProperty.UnsetValue)
            {
                actionToPerform((T)propertyValue);
            }
        }

        private void UpdateFontSizeInfo(double size)
        {
            fontSize.SelectedValue = size;
        }

        private void UpdateFontWeightInfo(FontWeight weight)
        {
            boldButton.IsChecked = (weight == FontWeights.Bold);
        }

        private void UpdateFontStyleInfo(FontStyle style)
        {
            italicButton.IsChecked = (style == FontStyles.Italic);
        }

        private void UpdateTextDecorationInfo(TextDecorationCollection decoration)
        {
            underlineButton.IsChecked = (decoration == TextDecorations.Underline);
        }

        private void UpdateFontFamilyInfo(FontFamily font)
        {
            fonts.SelectedItem = font ;
        }

        private void ExtractRakeKeywords(object sender, RoutedEventArgs e)
        {
            if (getDocumentRakeKeywordsEvent != null)
                getDocumentRakeKeywordsEvent();
        }

    }
}
