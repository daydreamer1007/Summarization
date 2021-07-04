using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Summarization.Shared {
    public class BaseWords {
        private Dictionary<string, string> base_words;

        public BaseWords(string base_words_csv) {
            this.base_words = base_words_csv.Split('\n').Where(s => !string.IsNullOrWhiteSpace(s)).Skip(1).Select(s => s.Split(';')).ToDictionary(a => new string(a[0].Where(c => char.IsLetter(c)).ToArray()), a => new string(a[1].Where(c => char.IsLetter(c)).ToArray()));
        }

        public string GetBaseWord(string word) {
            return this.base_words.ContainsKey(word) ? this.base_words[word] : word;
        }
    }
}
