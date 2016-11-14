using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Ink;

namespace Calculator.Pages
{
    public sealed class PathSample : INotifyPropertyChanged
    {
        private string _character;
        private StrokeCollection _sample1;
        private StrokeCollection _sample2;
        private StrokeCollection _sample3;
        private StrokeCollection _sample4;
        private StrokeCollection _sample5;

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public string Character
        {
            get { return _character; }
            set
            {
                _character = value;
                RaisePropertyChanged();
            }
        }

        public StrokeCollection Sample1
        {
            get { return _sample1; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _sample1 = value;
                RaisePropertyChanged();
            }
        }

        public StrokeCollection Sample2
        {
            get { return _sample2; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _sample2 = value;
                RaisePropertyChanged();
            }
        }

        public StrokeCollection Sample3
        {
            get { return _sample3; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _sample3 = value;
                RaisePropertyChanged();
            }
        }

        public StrokeCollection Sample4
        {
            get { return _sample4; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _sample4 = value;
                RaisePropertyChanged();
            }
        }

        public StrokeCollection Sample5
        {
            get { return _sample5; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _sample5 = value;
                RaisePropertyChanged();
            }
        }
    }
}
