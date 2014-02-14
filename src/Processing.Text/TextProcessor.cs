using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Processing.Text
{
    public class TextProcessor
    {
        private readonly IEnumerable<string> m_stopWords = GetStopWords();
        private static readonly Regex s_sanitizeRegex = new Regex(@"[^0-9a-zA-Z\s\u00E9-\u00F8]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex s_normalizeWhitespaceRegex = new Regex(@"\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex HtmlToTextRegex = new Regex("<[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string Sanitize(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            text = HtmlToPlainText(text);
            text = s_sanitizeRegex.Replace(text, " ");
            text = text.Trim();
            text = s_normalizeWhitespaceRegex.Replace(text, " ");
            return text.ToUpperInvariant();
        }

        public IEnumerable<string> Tokenize(string input)
        {
            var sanitizedInput = Sanitize(input);

            var tokenizer = new OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer("Resources/EnglishTok.nbin");

            var tokenized = tokenizer.Tokenize(sanitizedInput);

            var output = tokenized.Where(token => !m_stopWords.Any(x => x.Equals(token, StringComparison.OrdinalIgnoreCase))).ToList();

            return output;
        }

        public IEnumerable<Tuple<string, string>> Tag(string input)
        {
            var tokens = Tokenize(input).Distinct().ToArray();
            var tagger = new OpenNLP.Tools.PosTagger.EnglishMaximumEntropyPosTagger("Resources/EnglishPOS.nbin", "Resources/tagdict");
            var tags = tagger.Tag(tokens);
            return tokens.Zip(tags, (s, s1) => new Tuple<string, string>(s, s1));
        }

        public IEnumerable<IGrouping<string, Tuple<string, string>>> TagGroups(string input)
        {
            return Tag(input).GroupBy(x => x.Item2);
        }

        public IDictionary<string, int> GetWords(string input)
        {
            var tokens = Tokenize(input);

            var wordLookup = new Dictionary<string, int>();

            foreach (var token in tokens)
            {
                int value;
                if (wordLookup.TryGetValue(token, out value))
                {
                    wordLookup[token] = ++value;
                }
                else
                {
                    wordLookup[token] = 1;
                }
            }

            return wordLookup.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        public IDictionary<string, int> GetNouns(string input)
        {
            var tagGroups = TagGroups(input);
            var wordCountLookup = GetWords(input);

            var foo = tagGroups.Where(x => x.Key == "NN" || x.Key == "NNP" || x.Key == "NNS").SelectMany(x => x.Select(y => y.Item1)).Distinct();

            var nounLookup = new Dictionary<string, int>();
            foreach (var key in foo)
            {
                int value;
                if (wordCountLookup.TryGetValue(key, out value))
                {
                    nounLookup.Add(key, value);
                }
            }

            return nounLookup.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        public IEnumerable<string> GetImportanEvents(IEnumerable<string> eventText, IDictionary<string, int> nouns)
        {
            var outputSentences = new HashSet<string>();
            foreach (var noun in nouns.Keys.Take(nouns.Keys.Count / 10))
            {
                var nounKey = noun;

                var foundSentences = eventText.Where(x => x.IndexOf(nounKey, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                foundSentences.ForEach(y => outputSentences.Add(y));
            }

            return outputSentences.ToList();
        }

        public IEnumerable<string> GetSentences(string input)
        {
            var sentenceDetect = new OpenNLP.Tools.SentenceDetect.EnglishMaximumEntropySentenceDetector("Resources/EnglishSD.nbin");
            return sentenceDetect.SentenceDetect(input);
        }

        private static string HtmlToPlainText(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return String.Empty;
            }

            return HtmlToTextRegex.Replace(html, "");
        }

        private static IEnumerable<string> GetStopWords()
        {
            var stopWords = new List<string>();
            using (var sr = new StreamReader("Resources/en-US_Stopwords.txt"))
            {
                string word;
                while ((word = sr.ReadLine()) != null)
                {
                    stopWords.Add(word);
                }
            }

            return stopWords;
        }
    }
}