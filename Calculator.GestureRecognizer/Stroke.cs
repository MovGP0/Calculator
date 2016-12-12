using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Windows;

namespace Calculator.GestureRecognizer
{
    [Serializable]
    public sealed class Stroke : ISerializable
    {
        public IEnumerable<Point> Points { get; }

        public Stroke(IEnumerable<Point> points)
        {
            Points = points;
        }

        #region ISerializable
        private Stroke(SerializationInfo info, StreamingContext context)
        {
            if(info == null) throw new ArgumentNullException(nameof(info));
            
            Points = (Point[])info.GetValue("points", typeof(Point[]));
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            info.AddValue("points", Points.ToArray());
        }

        #endregion
    }
}