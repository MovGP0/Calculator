using System;
using System.Collections.Generic;
using System.Linq;
using Calculator.GestureRecognizer;

namespace Calculator.Pages
{
    public static class PathSampleExtensions
    {
        public static IEnumerable<Gesture> ToGesture(this PathSampleViewModel training)
        {
            if(training.Sample1.Value != null && training.Sample1.Value.Any()) yield return new Gesture(training.Sample1.Value.ConvertToStrokes(), training.Character.Value);
            if(training.Sample2.Value != null && training.Sample2.Value.Any()) yield return new Gesture(training.Sample2.Value.ConvertToStrokes(), training.Character.Value);
            if(training.Sample3.Value != null && training.Sample3.Value.Any()) yield return new Gesture(training.Sample3.Value.ConvertToStrokes(), training.Character.Value);
            if(training.Sample4.Value != null && training.Sample4.Value.Any()) yield return new Gesture(training.Sample4.Value.ConvertToStrokes(), training.Character.Value);
            if(training.Sample5.Value != null && training.Sample5.Value.Any()) yield return new Gesture(training.Sample5.Value.ConvertToStrokes(), training.Character.Value);
        }

        public static IEnumerable<PathSampleViewModel> ToPathSamples(this TrainingSet gestures, IEnumerable<string> gestureNamesToLoad)
        {
            if(gestures == null) throw new ArgumentNullException(nameof(gestures));
            if(gestureNamesToLoad == null) throw new ArgumentNullException(nameof(gestureNamesToLoad));

            foreach (var gestureName in gestureNamesToLoad)
            {
                var currentGestures = gestures.Where(g => g.Name == gestureName).ToArray();

                if (currentGestures.Length == 0)
                {
                    var model = new PathSampleViewModel();
                    model.Character.Value = gestureName;
                    yield return model;
                    continue;
                }
                
                yield return ToPathSample(gestureName, currentGestures);
            }
        }

        private static PathSampleViewModel ToPathSample(string gestureName, IEnumerable<Gesture> gestures)
        {
            var pathSample = new PathSampleViewModel();
            pathSample.Character.Value = gestureName;

            var samples = gestures.Take(5).ToArray();

            if (samples.Length > 0) pathSample.Sample1.Value = samples[0].Strokes.ConvertToStrokeCollection();
            if (samples.Length > 1) pathSample.Sample2.Value = samples[1].Strokes.ConvertToStrokeCollection();
            if (samples.Length > 2) pathSample.Sample3.Value = samples[2].Strokes.ConvertToStrokeCollection();
            if (samples.Length > 3) pathSample.Sample4.Value = samples[3].Strokes.ConvertToStrokeCollection();
            if (samples.Length > 4) pathSample.Sample5.Value = samples[4].Strokes.ConvertToStrokeCollection();

            return pathSample;
        }
    }
}