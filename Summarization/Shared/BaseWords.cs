using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Summarization.Shared {
    public class BaseWords {
        private Dictionary<string, string> base_words;

        public BaseWords(string base_words_json) {
            this.base_words = new Dictionary<string, string>();
            JsonElement elements = JsonDocument.Parse(base_words_json).RootElement;

            for(int i = 0; i < elements.GetArrayLength(); i++) {
                string word = elements.GetProperty("word").GetString();
                string base_word = elements.GetProperty("base_word").GetString();

                this.base_words.Add(word, base_word);
            }
        }

        public string GetBaseWord(string word) {
            return this.base_words.ContainsKey(word) ? this.base_words[word] : word;
        }
    }
}
