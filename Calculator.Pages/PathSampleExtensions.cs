using System;
using System.Collections.Generic;
using System.Linq;
using Calculator.GestureRecognizer;

namespace Calculator.Pages
{
    public static class PathSampleExtensions
    {
        public static IEnumerable<Gesture> ToGesture(this PathSample training)
        {
            if(training.Sample1 != null) yield return new Gesture(training.Sample1.ConvertToStrokes(), training.Character);
            if(training.Sample2 != null) yield return new Gesture(training.Sample2.ConvertToStrokes(), training.Character);
            if(training.Sample3 != null) yield return new Gesture(training.Sample3.ConvertToStrokes(), training.Character);
            if(training.Sample4 != null) yield return new Gesture(training.Sample4.ConvertToStrokes(), training.Character);
            if(training.Sample5 != null) yield return new Gesture(training.Sample5.ConvertToStrokes(), training.Character);
        }

        public static IEnumerable<PathSample> ToPathSamples(this TrainingSet gestures, IEnumerable<string> gestureNamesToLoad)
        {
            if(gestures == null) throw new ArgumentNullException(nameof(gestures));
            if(gestureNamesToLoad == null) throw new ArgumentNullException(nameof(gestureNamesToLoad));

            foreach (var gestureName in gestureNamesToLoad)
            {
                var currentGestures = gestures.Where(g => g.Name == gestureName).ToArray();

                if (currentGestures.Length == 0)
                {
                    yield return new PathSample { Character = gestureName };
                    continue;
                }
                
                yield return ToPathSample(gestureName, currentGestures);
            }
        }

        private static PathSample ToPathSample(string gestureName, IEnumerable<Gesture> gestures)
        {
            var pathSample = new PathSample
            {
                Character = gestureName
            };

            var samples = gestures.Take(5).ToArray();

            if (samples.Length > 0) pathSample.Sample1 = samples[0].Strokes.ConvertToStrokeCollection();
            if (samples.Length > 1) pathSample.Sample2 = samples[1].Strokes.ConvertToStrokeCollection();
            if (samples.Length > 2) pathSample.Sample3 = samples[2].Strokes.ConvertToStrokeCollection();
            if (samples.Length > 3) pathSample.Sample4 = samples[3].Strokes.ConvertToStrokeCollection();
            if (samples.Length > 4) pathSample.Sample5 = samples[4].Strokes.ConvertToStrokeCollection();

            return pathSample;
        }
    }
}