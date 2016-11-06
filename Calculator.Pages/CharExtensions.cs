using System.Collections.Generic;
using System.Linq;

namespace Calculator.Pages
{
    public static class CharExtensions
    {
        public static IEnumerable<char> ToChars(this IEnumerable<int> list)
        {
            return list.Select(i => (char) i);
        }
    }
}