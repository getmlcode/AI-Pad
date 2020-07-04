﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;

namespace TextEditor.TextAnalytics
{
    class Rake
    {
        // Implementation taken from : https://github.com/benmcevoy/Rake/blob/master/Rake/Rake.cs

        private readonly string _stopWordsPattern;
        private readonly int _minCharLength;
        private readonly int _maxWordsLength;
        private readonly double _minKeywordFrequency;

        public Rake(
            string stopWordsPath = null,
            int minCharLength = 1,
            int maxWordsLength = 5,
            double minKeywordFrequency = 1
            )
        {
            _minCharLength = minCharLength;
            _maxWordsLength = maxWordsLength;
            _minKeywordFrequency = minKeywordFrequency;
            _stopWordsPattern = BuildStopWordRegEx(stopWordsPath);
        }

        public Dictionary<string, double> extractRakeKeywords(string textEditorData)
        {
            Dictionary<string, double> keywordsDict = new Dictionary<string, double>();

            if (textEditorData.Length > 30)
            {
                var sentenceList = SplitSentences(textEditorData);

                var phraseList = GenerateCandidateKeywords(
                    sentenceList,
                    _stopWordsPattern,
                    _minCharLength,
                    _maxWordsLength
                    );

                var wordScores = CalculateWordScores(phraseList);

                keywordsDict = GenerateCandidateKeywordScores(
                    phraseList,
                    wordScores,
                    _minKeywordFrequency
                    );

                return keywordsDict
                    .OrderByDescending(pair => pair.Value)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            else
            {
                return keywordsDict;
            }
        }

        private static IList<string> SplitSentences(string text)
        {
            string regExp = @"[\[\]\n.!?,;:\t\-\""\(\)\\\'\u2019\u2013]";

            var sentenceDelimiters = new Regex(regExp, RegexOptions.Compiled);
            var sentences = sentenceDelimiters.Split(text);

            return sentences;
        }

        private static IList<string> GenerateCandidateKeywords(
            IEnumerable<string> sentenceList,
            string stopWordsPattern,
            int minCharLength,
            int maxWordsLength
            )
        {
            var phraseList = new List<string>();

            foreach (var s in sentenceList)
            {
                var tmp = Regex.Replace(s.Trim().ToLowerInvariant(), stopWordsPattern, "|");
                var phrases = tmp.Split('|');

                foreach (var phrase in phrases)
                {
                    var p = phrase.Trim();

                    if (!string.IsNullOrWhiteSpace(p) && IsAcceptable(p, minCharLength, maxWordsLength))
                        phraseList.Add(p);
                }
            }

            return phraseList;
        }

        private Dictionary<string, double> GenerateCandidateKeywordScores(
            IList<string> phraseList,
            Dictionary<string, double> wordScores,
            double minKeywordFrequency)
        {
            var keywordCandidates = new Dictionary<string, double>();

            foreach (var phrase in phraseList)
            {
                if (minKeywordFrequency > 1)
                {
                    if (phraseList.Count(s => s.Equals(phrase)) < minKeywordFrequency)
                        continue;
                }

                if (!keywordCandidates.ContainsKey(phrase)) keywordCandidates[phrase] = 0;

                var words = SeparateWords(phrase, 0);
                var candidateScore = words.Sum(word => wordScores[word]);

                keywordCandidates[phrase] = candidateScore;
            }

            return keywordCandidates;
        }

        private Dictionary<string, double> CalculateWordScores(IEnumerable<string> phraseList)
        {
            var wordFrequency = new Dictionary<string, double>();
            var wordDegree = new Dictionary<string, double>();

            foreach (var phrase in phraseList)
            {
                var words = SeparateWords(phrase, 0);
                var wordsLength = words.Count;

                var wordListDegree = wordsLength - 1;
                // if word_list_degree > 3: word_list_degree = 3 #exp.

                foreach (var word in words)
                {
                    if (!wordFrequency.ContainsKey(word)) wordFrequency[word] = 0;

                    wordFrequency[word] = wordFrequency[word] + 1;

                    if (!wordDegree.ContainsKey(word)) wordDegree[word] = 0;
                    // orig.
                    // word_degree[word] += 1/(word_list_length*1.0) #exp.
                    wordDegree[word] = wordDegree[word] + wordListDegree;
                }
            }
            foreach (var item in wordFrequency)
            {
                wordDegree[item.Key] = wordDegree[item.Key] + wordFrequency[item.Key];
            }

            // Calculate Word scores = deg(w)/frew(w)
            var wordScore = new Dictionary<string, double>();
            foreach (var item in wordFrequency)
            {
                if (!wordScore.ContainsKey(item.Key)) wordScore[item.Key] = 0;

                wordScore[item.Key] = wordDegree[item.Key] / (wordFrequency[item.Key] * 1.0); // orig.
                                                                                              // word_score[item] = word_frequency[item]/(word_degree[item] * 1.0) #exp.
            }

            return wordScore;
        }

        /// <summary>
        ///  Utility function to return a list of all words that are have a length greater than a specified number of characters.
        /// </summary>
        /// <param name="phrase">The text that must be split in to words.</param>
        /// <param name="minWordReturnSize">The minimum no of characters a word must have to be included.</param>
        /// <returns></returns>
        private IList<string> SeparateWords(string phrase, int minWordReturnSize)
        {
            var splitter = new Regex(@"[^a-zA-Z0-9_\+\-/]", RegexOptions.Compiled);
            var words = new List<string>();

            foreach (var singleWord in splitter.Split(phrase))
            {
                var currentWord = singleWord.Trim().ToLowerInvariant();
                // leave numbers in phrase, but don't count as words, since they tend to invalidate scores of their phrases

                if (!string.IsNullOrWhiteSpace(currentWord) && currentWord.Length > minWordReturnSize &&
                    !IsNumber(currentWord))
                {
                    words.Add(currentWord);
                }
            }

            return words;
        }

        private static string BuildStopWordRegEx(string stopWordsPath)
        {
            var stopWordList = LoadStopWords(stopWordsPath);
            var stopWordRegexList = new List<string>();

            foreach (var word in stopWordList)
            {
                var wordRegex = $@"\b{word}\b";
                stopWordRegexList.Add(wordRegex);
            }

            var stopWordPattern = string.Join("|", stopWordRegexList).ToLowerInvariant();

            return $"({stopWordPattern})";
        }

        private static IList<string> LoadStopWords(string stopWordsPath)
        {
            var stopWords = new List<string>();

            try
            {
                foreach (var line in string.IsNullOrWhiteSpace(stopWordsPath) ? ReadAllLines()
                    : File.ReadAllLines(stopWordsPath))
                {
                    if (line.Trim().StartsWith("#")) continue;

                    stopWords.AddRange(line.Split(' '));
                    return stopWords;
                }
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return stopWords;
        }

        private static IEnumerable<string> ReadAllLines()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Rake.SmartStoplist.txt";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        private static bool IsNumber(string word)
        {
            float tmp;
            return float.TryParse(word, out tmp);
        }

        private static bool IsAcceptable(string phrase, int minCharLength, int maxWordsLength)
        {
            if (phrase.Length < minCharLength) return false;

            var words = phrase.Split(' ');

            if (words.Length > maxWordsLength) return false;

            var digits = 0;
            var alpha = 0;

            for (var i = 0; i < phrase.Length; i++)
            {
                if (char.IsDigit(phrase[i])) digits++;
                if (char.IsLetter(phrase[i])) alpha++;
            }

            // a phrase must have at least one alpha character
            if (alpha == 0) return false;

            // a phrase must have more alpha than digits characters
            if (digits > alpha) return false;

            return true;
        }
    }
}
