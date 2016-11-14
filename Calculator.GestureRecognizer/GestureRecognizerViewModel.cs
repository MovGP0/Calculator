using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Ink;

namespace Calculator.GestureRecognizer
{
    public sealed class GestureRecognizerViewModel : INotifyPropertyChanged
    {
        private StrokeCollection _strokes = new StrokeCollection();
        private double _xHeight;
        private double _capsHeight;
        private double _baseline;
        private double _width;
        
        public double Baseline
        {
            get { return _baseline; }
            set
            {
                _baseline = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Height of uppercase letters over Baseline
        /// </summary>
        public double CapsHeight
        {
            get { return _capsHeight; }
            set
            {
                _capsHeight = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Height of small letters over Baseline
        /// </summary>
        public double XHeight
        {
            get
            {
                return _xHeight;
            }
            set
            {
                _xHeight = value;
                RaisePropertyChanged();
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged();
            }
        }

        public StrokeCollection Strokes
        {
            get { return _strokes; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _strokes = value;
                RaisePropertyChanged();
            }
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged; 

        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion 
    }
}