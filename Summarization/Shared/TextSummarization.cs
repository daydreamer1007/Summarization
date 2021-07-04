using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Summarization.Shared {
    public class TextSummarization {
        private StopWords stop_words;
        private BaseWords base_words;
        private DocumentFrequencies document_frequencies;
        private List<string> sentences { get; set; }
        private List<string> prepared_sentences;
        private Dictionary<string, double> tfidf;
        private List<int> ordered_indices;

        public TextSummarization(string stop_words_json, string base_words_json, string document_frequencies_json) {
            this.stop_words = new StopWords(stop_words_json);
            this.base_words = new BaseWords(base_words_json);
            this.document_frequencies = new DocumentFrequencies(document_frequencies_json);
        }

        public void SetNewText(string text) {

        }

        private void SentenceTokenization(string text) {
            
        }

        public string GetSummary(int count) {
            return string.Join("", ordered_indices.Take(count).OrderBy(n => n).Select(n => sentences[n]));
        }
    }
}
