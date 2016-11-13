using System.Collections.Generic;
using System.Linq;
using Calculator.GestureRecognizer;

namespace Calculator.Pages
{
    public static class PathSampleExtensions
    {
        public static IEnumerable<Gesture> ToGesture(this PathSample training)
        {
            if(training.Sample1 != null) yield return new Gesture(training.Sample1.ToList(), training.Character);
            if(training.Sample2 != null) yield return new Gesture(training.Sample2.ToList(), training.Character);
            if(training.Sample3 != null) yield return new Gesture(training.Sample3.ToList(), training.Character);
            if(training.Sample4 != null) yield return new Gesture(training.Sample4.ToList(), training.Character);
            if(training.Sample5 != null) yield return new Gesture(training.Sample5.ToList(), training.Character);
        }
    }
}