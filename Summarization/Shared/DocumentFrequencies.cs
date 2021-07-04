using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Summarization.Shared {
    public class DocumentFrequencies {
        private Dictionary<string, int> document_frequencies;

        public DocumentFrequencies(string document_frequencies_json) {
            this.document_frequencies = new Dictionary<string, int>();
            JsonElement elements = JsonDocument.Parse(document_frequencies_json).RootElement;

            for(int i = 0; i < elements.GetArrayLength(); i++) {
                string word = elements[i].GetProperty("word").GetString();
                int count = elements[i].GetProperty("count").GetInt32();

                this.document_frequencies.Add(word, count);
            }
        }

        public int GetDocumentFrequency(string word) {
            return this.document_frequencies.ContainsKey(word) ? this.document_frequencies[word] : 0;
        }
    }
}
