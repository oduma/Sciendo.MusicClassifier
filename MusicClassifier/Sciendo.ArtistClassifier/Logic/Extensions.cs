using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sciendo.ArtistClassifier.Logic
{
    public static class Extensions
    {
        public static string JoinTo(this string[] input, int position, char simpleWordsSplitter)
        {
            StringBuilder accumulator = new StringBuilder("");
            for (int i = 0; i < position; i++)
                accumulator.Append(input[i] + simpleWordsSplitter);
            return accumulator.ToString().Trim();
        }

        public static string JoinFrom(this string[] input, int position, char simpleWordsSplitter)
        {
            StringBuilder accumulator = new StringBuilder("");
            for (int i = position +1; i <input.Length; i++)
                accumulator.Append(input[i] + simpleWordsSplitter);
            return accumulator.ToString().Trim();
        }

        public static bool AnyMatch(this IEnumerable<string> regularExpressions, string input)
        {
            foreach(var regEx in regularExpressions)
            {
                if (Regex.IsMatch(input, regEx))
                    return true;
            }
            return false;
        }
        public static string ReplaceAll(this string input, IEnumerable<string> regexFind, string withString)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            string result = input;
            foreach (var regexFindPart in regexFind)
            {
                var possibleMatches = Regex.Matches(input, regexFindPart);
                if (possibleMatches.Count > 0)
                {
                    foreach (var possibleMatch in possibleMatches)
                    {
                        result = result.Replace(possibleMatch.ToString(), withString);
                    }
                }
            }

            return result;
        }

    }
}
