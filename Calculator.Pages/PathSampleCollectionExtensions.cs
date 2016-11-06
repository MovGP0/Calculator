using System.Collections.Generic;

namespace Calculator.Pages
{
    public static class PathSampleCollectionExtensions
    {
        public static PathSampleCollection ToPathSampleCollection(this IEnumerable<PathSample> pathSamples)
        {
            return new PathSampleCollection(pathSamples);
        }
    }
}