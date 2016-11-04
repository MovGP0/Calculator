using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Calculator.GestureRecognizer
{
    [Serializable]
    public sealed class TrainingSet : ISerializable
    {
        public TrainingSet(IList<Gesture> gestures)
        {
            Gestures = new ReadOnlyCollection<Gesture>(gestures);
        }

        public IEnumerable<Gesture> Gestures { get; }

        #region ISerializable
        public TrainingSet(SerializationInfo info, StreamingContext context)
        {
            if(info == null) throw new ArgumentNullException(nameof(info));
            
            var gestures = (Gesture[])info.GetValue("points", typeof(Gesture[]));
            Gestures = new ReadOnlyCollection<Gesture>(gestures);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if(info == null) throw new ArgumentNullException(nameof(info));
            
            info.AddValue("gestures", Gestures.ToArray());
        }
        #endregion
    }
}