using System;
using System.Windows.Ink;
using Reactive.Bindings;

namespace Calculator.Pages
{
    public sealed class PathSampleViewModel
    {
        public ReactiveProperty<string> Character { get; } 
            = new ReactiveProperty<string>(string.Empty);

        public ReactiveProperty<StrokeCollection> Sample1 { get; } 
            = new ReactiveProperty<StrokeCollection>(new StrokeCollection());

        public ReactiveProperty<StrokeCollection> Sample2 { get; } 
            = new ReactiveProperty<StrokeCollection>(new StrokeCollection());

        public ReactiveProperty<StrokeCollection> Sample3 { get; } 
            = new ReactiveProperty<StrokeCollection>(new StrokeCollection());

        public ReactiveProperty<StrokeCollection> Sample4 { get; } 
            = new ReactiveProperty<StrokeCollection>(new StrokeCollection());

        public ReactiveProperty<StrokeCollection> Sample5 { get; } 
            = new ReactiveProperty<StrokeCollection>(new StrokeCollection());

        ~PathSampleViewModel()
        {
            Dispose(false);
        }

        private bool _isDisposed;
        private void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            Character?.Dispose();
            Sample1?.Dispose();
            Sample2?.Dispose();
            Sample3?.Dispose();
            Sample4?.Dispose();
            Sample5?.Dispose();

            _isDisposed = true;
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}
