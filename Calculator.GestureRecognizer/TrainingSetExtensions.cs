using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Calculator.GestureRecognizer
{
    public static class TrainingSetExtensions
    {
        public static string Classify(this TrainingSet trainingSet, Gesture candidate)
        {
            var numberOfStrokes = candidate.NumberOfStrokes;
            var gestures = trainingSet.Gestures.FilterByNumberOfStrokes(numberOfStrokes);

            var minDistance = double.MaxValue;
            var gestureClass = string.Empty;
            foreach (var t in gestures.Select(training => MeasureDistance(training, candidate)))
            {
                if (t.Distance >= minDistance) continue;

                minDistance = t.Distance;
                gestureClass = t.Name;
            }

            return gestureClass;
        }

        private static IEnumerable<Gesture> FilterByNumberOfStrokes(this IEnumerable<Gesture> gestures, int numberOfStrokes)
        {
            return gestures.Where(g => g.NumberOfStrokes == numberOfStrokes);
        }

        private struct TemplateDistance
        {
            public TemplateDistance(double distance, string name)
            {
                Distance = distance;
                Name = name;
            }

            public double Distance { get; }
            public string Name { get; }
        }

        private static TemplateDistance MeasureDistance(Gesture candidate, Gesture template)
        {
            var points1 = candidate.Strokes.SelectMany(s => s.Points).ToArray();
            var points2 = template.Strokes.SelectMany(s => s.Points).ToArray();

            var distance = GreedyCloudMatch(points1, points2);
            return new TemplateDistance(distance, template.Name);
        }

        private static double GreedyCloudMatch(IList<Point> points1, IList<Point> points2)
        {
            if (points1.Count != points2.Count)
            {
                var message = $"{nameof(points1)} and {nameof(points2)} must have the same sumber of points";
                throw new ArgumentException(message, nameof(points2));
            }

            var n = points1.Count; 
            const double epsilon = 0.5; // controls the number of greedy search trials [0..1]
            var step = (int)Math.Floor(Math.Pow(n, 1.0 - epsilon));
            var minDistance = double.MaxValue;

            for (var i = 0; i < n; i += step)
            {
                var dist1 = CloudDistance(points1, points2, i);   // match points1 --> points2 starting with index point i
                var dist2 = CloudDistance(points2, points1, i);   // match points2 --> points1 starting with index point i
                minDistance = Math.Min(minDistance, Math.Min(dist1, dist2));
            }

            return minDistance;
        }
        
        private static double CloudDistance(IList<Point> points1, IList<Point> points2, int startIndex)
        {
            var n = points1.Count;       // the two clouds should have the same number of points by now
            var matched = new bool[n]; // matched[i] signals whether point i from the 2nd cloud has been already matched
            Array.Clear(matched, 0, n);   // no points are matched at the beginning

            var sum = 0d;  // computes the sum of distances between matched points (i.e., the distance between the two clouds)
            var i = startIndex;
            do
            {
                var index = -1;
                var minDistance = double.MaxValue;
                for(var j = 0; j < n; j++)
                {
                    if (matched[j]) continue;

                    var dist = Geometry.SqrEuclideanDistance(points1[i], points2[j]);  // use squared Euclidean distance to save some processing time
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        index = j;
                    }
                }

                matched[index] = true; // point index from the 2nd cloud is matched to point i from the 1st cloud
                var weight = 1.0 - ((i - startIndex + n) % n) / (1.0 * n);
                sum += weight * minDistance; // weight each distance with a confidence coefficient that decreases from 1 to 0
                i = (i + 1) % n;
            } while (i != startIndex);
            return sum;
        }
    }
}