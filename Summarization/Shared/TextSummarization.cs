using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Summarization.Shared {
    public class TextSummarization {
        private StopWords stop_words;
        private BaseWords base_words;
        public DocumentFrequencies document_frequencies;
        public List<string> sentences;
        public List<string> prepared_sentences;
        public List<List<string>> words;
        public Dictionary<string, int> words_count;
        public Dictionary<string, double> tf_values;
        public Dictionary<string, double> idf_values;
        public Dictionary<string, double> tfidf_values;
        public List<double> tfidf_scores;
        public List<List<double>> similarities;
        public List<double> textrank_scores;
        public List<double> final_scores;
        public List<int> ordered_indices;
        public List<List<int>> references = new List<List<int>>() {
            new List<int>(){ 1, 3, 5, 10, 11, 13, 15, 17, 18 },
            new List<int>(){ 1, 2, 3, 6, 8, 11, 13, 14, 15, 16, 17, 18 },
            new List<int>(){ 1,2, 6, 7, 15, 17, 20 },
            new List<int>(){ 1, 3, 4, 9, 11, 14, 16, 17, 18 },
            new List<int>(){ 1, 3, 5, 6, 8, 11, 15 },
            new List<int>(){ 1, 2, 3, 4, 5, 6, 8, 9, 11, 12, 15, 18 },
            new List<int>(){ 1, 2, 3, 4, 6, 7, 10, 12, 14 },
            new List<int>(){ 1, 3, 6, 7, 8 },
            new List<int>(){ 1, 2, 4, 5, 7, 8, 10, 12, 13, 14, 17, 19, 20 },
            new List<int>(){ 1, 3, 4, 8, 12, 13, 14, 15, 16 },
            new List<int>(){ 1, 2, 5, 6, 7, 15, 16 },
            new List<int>(){ 1, 2, 3, 4, 7, 8, 17 },
            new List<int>(){ 1, 2, 6, 12, 14 },
            new List<int>(){ 1, 2, 3, 4, 5, 6, 8, 9, 12, 14, 17, 18, 19, 20, 22 },
            new List<int>(){ 1, 2, 6, 7, 9, 12 },
            new List<int>(){ 1, 2, 3, 6, 8, 9, 12, 13, 14, 15 },
            new List<int>(){ 1, 2, 4, 7, 9, 10, 11, 12, 13 },
            new List<int>(){ 1, 2, 4, 5, 7, 8, 10, 11, 13 },
            new List<int>(){ 1, 2, 3, 5, 7, 9, 10, 12, 14, 15 },
            new List<int>(){ 1, 2, 6, 7, 8, 9, 11 }
        };

        public TextSummarization(string stop_words_csv, string base_words_csv, string document_frequencies_csv) {
            this.stop_words = new StopWords(stop_words_csv);
            this.base_words = new BaseWords(base_words_csv);
            this.document_frequencies = new DocumentFrequencies(document_frequencies_csv);
        }

        public void Summarize(string text) {
            this.SentenceTokenization(text);
            this.CaseFolding();
            this.Cleaning();
            this.WordTokenization();
            this.StopWordsRemoval();
            this.Stemming();
            this.StopWordsRemoval();
            this.GetWordsCount();
            this.CalculateTFValues();
            this.CalculateIDFValues();
            this.CalculateTFIDFValues();
            this.CalculateTFIDFScores();
            this.GenerateSimilaritiesMatrix();
            this.NormalizeSimilaritiesMatrix();
            this.CalculateTextRankScores();
            this.CalculateFinalScores();
            this.OrderIndices();
        }

        private void SentenceTokenization(string text) {
            List<char> delimiter = text.Where(c => ".!?".Contains(c)).ToList();
            this.sentences = Regex.Replace(text, "[\n]", " ").Split(new char[] { '.', '!', '?' }).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToList();

            for(int i = 0; i < this.sentences.Count; i++) {
                this.sentences[i] += (i < delimiter.Count ? "" + delimiter[i] : "");
            }
        }

        private void CaseFolding() {
            this.prepared_sentences = this.sentences.Select(s => s.ToLower()).ToList();
        }

        private void Cleaning() {
            this.prepared_sentences = this.prepared_sentences.Select(s => Regex.Replace(Regex.Replace(s, "[^a-z]", " "), "[ ]{2,}", " ").Trim()).ToList();
        }

        private void WordTokenization() {
            this.words = this.prepared_sentences.Select(s => s.Split(' ').Where(st => !string.IsNullOrWhiteSpace(st)).ToList()).ToList();
        }

        private void StopWordsRemoval() {
            this.words = this.words.Select(l => l.Where(s => !this.stop_words.IsStopWord(s)).ToList()).ToList();
        }

        private void Stemming() {
            this.words = this.words.Select(l => l.Select(s => this.base_words.GetBaseWord(s)).ToList()).ToList();
        }

        private void GetWordsCount() {
            this.words_count = new Dictionary<string, int>();

            foreach(List<string> list in this.words) {
                foreach(string s in list) {
                    if(!this.words_count.ContainsKey(s)) {
                        this.words_count.Add(s, 1);
                    }
                    else {
                        this.words_count[s] += 1;
                    }
                }
            }
        }

        private void CalculateTFValues() {
            this.tf_values = this.words_count.ToDictionary(d => d.Key, d => Math.Log10(1 + d.Value));
        }

        private void CalculateIDFValues() {
            this.idf_values = new Dictionary<string, double>();

            foreach(string s in this.words_count.Keys) {
                this.idf_values.Add(s, 1 + this.document_frequencies.GetDocumentFrequency(s));
            }

            double max_df = this.idf_values.Max(d => d.Value);
            this.idf_values = this.idf_values.ToDictionary(d => d.Key, d => Math.Log10(max_df / d.Value));
        }

        private void CalculateTFIDFValues() {
            this.tfidf_values = this.tf_values.ToDictionary(d => d.Key, d => d.Value * this.idf_values[d.Key]);
        }

        private void CalculateTFIDFScores() {
            this.tfidf_scores = this.words.Select(l => l.Sum(k => this.tfidf_values[k])).ToList();
        }

        private void GenerateSimilaritiesMatrix() {
            this.similarities = new List<List<double>>();

            for(int i= 0; i < this.sentences.Count; i++) {
                List<double> list = new List<double>();

                for(int j = 0; j < this.sentences.Count; j++) {
                    if(i == j) {
                        list.Add(0.0);
                    }
                    else {
                        double weight = 0.0;
                        double denom = 0.0;

                        foreach(string s in this.words_count.Keys) {
                            weight += Math.Min(this.words[i].Count(st => st == s), this.words[j].Count(st => st == s));
                        }

                        double len_i = Convert.ToDouble(this.words[i].Count);
                        double len_j = Convert.ToDouble(this.words[j].Count);
                        denom = Math.Log10(Math.Max(len_i, 1.0)) + Math.Log10(Math.Max(len_j, 1.0));

                        list.Add(weight / (denom > 0 ? denom : 1));
                    }
                }

                this.similarities.Add(list);
            }
        }

        private void NormalizeSimilaritiesMatrix() {
            for(int i = 0; i < this.sentences.Count; i++) {
                double sum_at_col_i = Enumerable.Range(0, this.sentences.Count).Sum(j => this.similarities[j][i]);

                for(int j = 0; j < this.sentences.Count; j++) {
                    this.similarities[j][i] = this.similarities[j][i] / (sum_at_col_i > 0 ? sum_at_col_i : 1);
                }
            }
        }

        private void CalculateTextRankScores() {
            double d = 0.85;
            this.textrank_scores = new List<double>();
            bool stable = false;

            for(int i = 0; i < this.sentences.Count; i++) {
                this.textrank_scores.Add(1.0 / Convert.ToDouble(this.sentences.Count));
            }

            List<double> old_textrank_scores = this.textrank_scores.ToList();

            while(!stable) {
                for(int i = 0; i < this.sentences.Count; i++) {
                    double sum1 = 0.0;
                    
                    for(int j = 0; j < this.sentences.Count; j++) {
                        if(i != j) {
                            double sum2 = Enumerable.Range(0, this.sentences.Count).Where(k => k != j).Sum(k => this.similarities[j][k]);
                            sum1 += this.similarities[j][i] * this.textrank_scores[j] / (sum2 > 0 ? sum2 : 1);
                        }
                    }

                    this.textrank_scores[i] = (1 - d) + d * sum1;
                }

                for(int i = 0; i < this.sentences.Count; i++) {
                    if(this.textrank_scores[i] != old_textrank_scores[i]) {
                        stable = false;
                        break;
                    }
                    else {
                        stable = true;
                    }
                }

                old_textrank_scores = this.textrank_scores.ToList();
            }
        }

        private void CalculateFinalScores() {
            this.final_scores =  Enumerable.Range(0, this.sentences.Count).Select(i => this.tfidf_scores[i] * this.textrank_scores[i]).ToList();
        }

        private void OrderIndices() {
            this.ordered_indices = Enumerable.Range(0, this.sentences.Count).OrderByDescending(i => this.final_scores[i]).ToList();
        }

        public string GetSummary(int compression) {
            int count = Convert.ToInt32(Math.Floor(Convert.ToDouble(this.sentences.Count) * Convert.ToDouble(100 - compression) / 100.0));

            return this.sentences == null ? "" : string.Join("\n\n", ordered_indices.Take(count).OrderBy(n => n).Select(n => sentences[n]));
        }

        public void Clear() {
            this.sentences = null;
            this.prepared_sentences = null;
            this.words = null;
            this.words_count = null;
            this.tf_values = null;
            this.idf_values = null;
            this.tfidf_values = null;
            this.tfidf_scores = null;
            this.similarities = null;
            this.textrank_scores = null;
            this.final_scores = null;
            this.ordered_indices = null;
        }
    }
}
