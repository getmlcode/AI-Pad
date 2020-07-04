using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Data;
using System.Windows.Data;
using TextEditor;
using System.Windows.Input;

namespace TextEditor.TextAnalytics
{
    class DocumentKeywordsExtractor
    {
        // To be used when save option is implemented for 
        // keywords display window

        //private string keywordsFile;

        private RichTextBox textContainer;

        public DocumentKeywordsExtractor(RichTextBox textBox)
        {
            textContainer = textBox;
        }

        public void GetRakeKeywords()
        {
            Dictionary<string, double> keywordsAndScoresDict = new Dictionary<string, double>();

            Rake rakeKeywordsExtractor = new Rake(stopWordsPath: "SmartStopWordsList.txt");

            TextRange textContent = new TextRange(
                textContainer.Document.ContentStart,
                textContainer.Document.ContentEnd
                );

            keywordsAndScoresDict = rakeKeywordsExtractor.extractRakeKeywords(textContent.Text);

            if (keywordsAndScoresDict.Count != 0)
            {
                ShowKeywords(keywordsAndScoresDict);
            }
            else
            {
                MessageBox.Show("Document Must Have More Than 30 Words !");
            }

        }

        private void ShowKeywords( Dictionary<string, double> keywordsAndScoresDict )
        {
            AI_OutputDialog AiOutput = new AI_OutputDialog();
            RichTextBox rtb = AiOutput.AiOutTextBox;

            AiOutput.Title = "KeyPhrase Output";

            foreach (KeyValuePair<string,double> keyPhrase_keyScore in keywordsAndScoresDict)
            {
                string rtbLine = keyPhrase_keyScore.Key + "\t\t\t" + keyPhrase_keyScore.Value.ToString() + "\n";
                AiOutput.AiOutTextBox.AppendText(rtbLine);
            }

            AiOutput.Top = 200;
            AiOutput.Left = 550;

            AiOutput.Show();
        }
    }
}
