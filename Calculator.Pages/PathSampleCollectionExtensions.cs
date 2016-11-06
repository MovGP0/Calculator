using System.Collections.Generic;
using System.Linq;
using Calculator.GestureRecognizer;

namespace Calculator.Pages
{
    public static class PathSampleCollectionExtensions
    {
        public static PathSampleCollection ToPathSampleCollection(this IEnumerable<PathSample> pathSamples)
        {
            return new PathSampleCollection(pathSamples);
        }

        public static IEnumerable<Gesture> ToGestures(this PathSampleCollection trainingSet)
        {
            foreach (var training in trainingSet)
            {
                if(training.Sample1 != null) yield return new Gesture(training.Sample1.ToList(), training.Character);
                if(training.Sample2 != null) yield return new Gesture(training.Sample2.ToList(), training.Character);
                if(training.Sample3 != null) yield return new Gesture(training.Sample3.ToList(), training.Character);
                if(training.Sample4 != null) yield return new Gesture(training.Sample4.ToList(), training.Character);
                if(training.Sample5 != null) yield return new Gesture(training.Sample5.ToList(), training.Character);
            }
        }
    }
}