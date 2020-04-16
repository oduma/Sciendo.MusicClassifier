using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sciendo.ArtistEnhancer.Logic
{
    public static class Extensions
    {
        public static bool IsAlphaNumeric(this string input)
        {
            return Regex.IsMatch(input, "^[a-zA-Z0-9 ]*$");
        }
    }
}
