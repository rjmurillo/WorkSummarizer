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

        public static string Sanitize(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return null;
            }
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
            var tokens = Tokenize(input).ToArray();
            var tagger = new OpenNLP.Tools.PosTagger.EnglishMaximumEntropyPosTagger("Resources/EnglishPOS.nbin", "Resources/tagdict");
            var tags = tagger.Tag(tokens);
            return tokens.Zip(tags, (s, s1) => { return new Tuple<string, string>(s, s1); });
        }

        public IDictionary<string, int> GetWords(string inputText)
        {
            var tokens = Tokenize(inputText);

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

            return wordLookup;
        }
    }
}