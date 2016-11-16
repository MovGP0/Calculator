using System.Collections.Generic;
using System.Linq;

namespace Calculator.Pages
{
    internal static class TrainingSetExtensions
    {
        public static IEnumerable<PathSampleViewModel> ToTrainingSet(this IEnumerable<char> chars)
        {
            return chars.Select(ToPathSample);
        }

        private static PathSampleViewModel ToPathSample(this char character)
        {
            var model = new PathSampleViewModel();
            model.Character.Value = character.ToString();
            return model;
        }
    }
}