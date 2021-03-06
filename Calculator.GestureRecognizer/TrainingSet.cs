﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Calculator.GestureRecognizer
{
    [Serializable]
    public sealed class TrainingSet : ISerializable, IEnumerable<Gesture>
    {
        public static TrainingSet Empty => new TrainingSet(new List<Gesture>());

        public TrainingSet(IList<Gesture> gestures)
        {
            Gestures = new ReadOnlyCollection<Gesture>(gestures);
        }
        
        public IEnumerable<Gesture> Gestures { get; }

        #region ISerializable
        private TrainingSet(SerializationInfo info, StreamingContext context)
        {
            if(info == null) throw new ArgumentNullException(nameof(info));
            
            var gestures = (Gesture[])info.GetValue("gestures", typeof(Gesture[]));
            Gestures = new ReadOnlyCollection<Gesture>(gestures);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if(info == null) throw new ArgumentNullException(nameof(info));
            
            info.AddValue("gestures", Gestures.ToArray());
        }
        #endregion

        public IEnumerator<Gesture> GetEnumerator()
        {
            return Gestures.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}