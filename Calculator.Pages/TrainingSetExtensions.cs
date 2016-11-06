using System.Collections.Generic;
using System.Linq;

namespace Calculator.Pages
{
    internal static class TrainingSetExtensions
    {
        public static IEnumerable<PathSample> ToTrainingSet(this IEnumerable<char> chars)
        {
            return chars.Select(ToPathSample);
        }

        private static PathSample ToPathSample(this char character)
        {
            return new PathSample
            {
                Character = character.ToString()
            };
        }
    }
}