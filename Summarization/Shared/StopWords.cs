using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Summarization.Shared {
    public class StopWords {
        private HashSet<string> stop_words;

        public StopWords(string stop_words_json) {
            this.stop_words = new HashSet<string>();
            JsonElement elements = JsonDocument.Parse(stop_words_json).RootElement;

            for(int i = 0; i < elements.GetArrayLength(); i++) {
                this.stop_words.Add(elements[i].GetProperty("word").GetString());
            }
        }

        public bool IsStopWord(string word) {
            return this.stop_words.Contains(word);
        }
    }
}
