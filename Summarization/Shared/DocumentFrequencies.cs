using System;
using System.Collections.Generic;
using System.Linq;

namespace Summarization.Shared {
    public class DocumentFrequencies {
        private Dictionary<string, int> document_frequencies;

        public DocumentFrequencies(string document_frequencies_csv) {
            this.document_frequencies = document_frequencies_csv.Split('\n').Skip(1).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Split(';')).ToDictionary(a => new string(a[0].Where(c => char.IsLetter(c)).ToArray()), a => Convert.ToInt32(a[1]));
        }

        public int GetDocumentFrequency(string word) {
            return this.document_frequencies.ContainsKey(word) ? this.document_frequencies[word] : 0;
        }
    }
}
