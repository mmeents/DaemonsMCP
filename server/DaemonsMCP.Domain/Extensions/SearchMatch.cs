using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.Domain.Extensions {
  public static class SearchMatch {

    public static int LevenshteinDistance(string s1, string s2) {
      var n = s1.Length;
      var m = s2.Length;
      var d = new int[n + 1, m + 1];

      if (n == 0) return m;
      if (m == 0) return n;

      for (int i = 0; i <= n; i++) d[i, 0] = i;
      for (int j = 0; j <= m; j++) d[0, j] = j;

      for (int i = 1; i <= n; i++) {
        for (int j = 1; j <= m; j++) {
          var cost = (s2[j - 1] == s1[i - 1]) ? 0 : 1;
          d[i, j] = Math.Min(
              Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
              d[i - 1, j - 1] + cost);
        }
      }
      return d[n, m];
    }

    public static string Soundex(string word) {
      if (string.IsNullOrWhiteSpace(word)) return "";

      word = word.ToUpper();
      var soundex = new StringBuilder();
      soundex.Append(word[0]);

      var map = new Dictionary<char, char> {
            {'B','1'},{'F','1'},{'P','1'},{'V','1'},
            {'C','2'},{'G','2'},{'J','2'},{'K','2'},
            {'Q','2'},{'S','2'},{'X','2'},{'Z','2'},
            {'D','3'},{'T','3'},
            {'L','4'},
            {'M','5'},{'N','5'},
            {'R','6'}
        };

      for (int i = 1; i < word.Length && soundex.Length < 4; i++) {
        if (map.ContainsKey(word[i])) {
          var code = map[word[i]];
          if (soundex[soundex.Length - 1] != code) {
            soundex.Append(code);
          }
        }
      }

      return soundex.ToString().PadRight(4, '0').Substring(0, 4);
    }

    public static string[] SplitSearchTerm(this string term) {
      // Split on spaces, underscores, hyphens
      var parts = term.Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);

      // Also split camelCase: "FileHandler" -> ["file", "handler"]
      var result = new List<string>();
      foreach (var part in parts) {
        result.AddRange(part.SplitCamelCase());
      }
      return result.ToArray();
    }


    public static List<string> SplitCamelCase(this string input) {
      if (string.IsNullOrEmpty(input))
        return new List<string>();

      var parts = new List<string>();
      var currentWord = new StringBuilder();

      for (int i = 0; i < input.Length; i++) {
        char c = input[i];
        char? next = i + 1 < input.Length ? input[i + 1] : null;
        char? prev = i > 0 ? input[i - 1] : null;

        if (char.IsUpper(c)) {
          if (currentWord.Length == 0) {
            // Start of string
            currentWord.Append(char.ToLower(c));
          } else if (prev.HasValue && char.IsUpper(prev.Value)) {
            // We're in an acronym
            if (next.HasValue && char.IsLower(next.Value)) {
              // This is the last letter of an acronym before a new word
              // e.g., "XMLHttpRequest" -> "XML" is done at 'L'
              if (currentWord.Length > 1) {
                // Remove last char and save the acronym
                var lastChar = currentWord[currentWord.Length - 1];
                currentWord.Length--; // Remove last char
                parts.Add(currentWord.ToString());
                currentWord.Clear();
                currentWord.Append(lastChar); // Start new word with last char
              }
              currentWord.Append(char.ToLower(c));
            } else {
              // Continue the acronym
              currentWord.Append(char.ToLower(c));
            }
          } else {
            // Start of a new word (prev was lowercase or digit)
            if (currentWord.Length > 0) {
              parts.Add(currentWord.ToString());
              currentWord.Clear();
            }
            currentWord.Append(char.ToLower(c));
          }
        } else if (char.IsDigit(c)) {
          if (prev.HasValue && char.IsDigit(prev.Value)) {
            // Continue number sequence
            currentWord.Append(c);
          } else {
            // Start new numeric word
            if (currentWord.Length > 0) {
              parts.Add(currentWord.ToString());
              currentWord.Clear();
            }
            currentWord.Append(c);
          }
        } else if (char.IsLetter(c)) {
          currentWord.Append(char.ToLower(c));
        }
          // Non-alphanumeric characters are treated as word boundaries
          else if (currentWord.Length > 0) {
          parts.Add(currentWord.ToString());
          currentWord.Clear();
        }
      }

      // Add the last word if any
      if (currentWord.Length > 0) {
        parts.Add(currentWord.ToString());
      }

      // Filter out empty strings and single characters (unless it's a meaningful single char)
      return parts.Where(p => p.Length > 0).ToList();
    }


  }
}
