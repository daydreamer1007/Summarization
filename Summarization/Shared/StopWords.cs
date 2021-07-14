using System.Collections.Generic;
using System.Linq;

namespace Summarization.Shared {
    public class StopWords {
        private HashSet<string> stop_words;

        public StopWords(string stop_words_csv) {
            this.stop_words = stop_words_csv.Split('\n').Skip(1).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => new string(s.Where(c => char.IsLetter(c)).ToArray())).ToHashSet<string>();
        }

        public bool IsStopWord(string word) {
            return this.stop_words.Contains(word);
        }
    }
}
