using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Calculator.GestureRecognizer
{
    [Serializable]
    public sealed class Gesture : ISerializable
    {
        public IEnumerable<Stroke> Strokes { get; }
        public int NumberOfStrokes => Strokes.Count();
        public string Name { get; }
        private const int SamplingResolution = 32;
        
        public Gesture(IEnumerable<Stroke> strokes, string gestureName = "")
        {
            var strokeArray = strokes.ToArray();

            Name = gestureName;
            Strokes = strokeArray
                .Scale()
                .TranslateTo(strokeArray.Controid())
                .ToArray()
                .Resample(SamplingResolution);
        }

        #region ISerializable
        private Gesture(SerializationInfo info, StreamingContext context)
        {
            if(info == null) throw new ArgumentNullException(nameof(info));

            Name = info.GetString("name");
            Strokes = (Stroke[])info.GetValue("strokes", typeof(Stroke[]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if(info == null) throw new ArgumentNullException(nameof(info));

            info.AddValue("name", Name);
            info.AddValue("strokes", Strokes.ToArray());
        }
        #endregion
    }
}