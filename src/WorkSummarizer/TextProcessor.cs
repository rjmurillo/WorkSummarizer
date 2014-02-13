using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WorkSummarizer
{
    public class TextProcessor
    {
        private static readonly Regex s_sanitizeRegex = new Regex(@"[^0-9a-zA-Z\s\u00E9-\u00F8]");
        private static readonly Regex s_normalizeWhitespaceRegex = new Regex(@"\s+");
        private static readonly Regex HtmlToTextRegex = new Regex("<[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static string HtmlToPlainText(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return String.Empty;
            }

            return HtmlToTextRegex.Replace(html, "");
        }

        public static string Sanitize(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return null;
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

            var path = "Resources/EnglishTok.nbin";
            var tokenizer = new OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer(path);

            return tokenizer.Tokenize(sanitizedInput);
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
    }
}