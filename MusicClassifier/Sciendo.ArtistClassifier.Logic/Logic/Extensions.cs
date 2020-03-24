using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sciendo.ArtistClassifier.Logic
{
    public static class Extensions
    {
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
