using System;
using System.Collections.Generic;
using System.Windows.Ink;
using Calculator.GestureRecognizer;
using Reactive.Bindings;
using Serilog;

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
        
        public ReactiveProperty<string> Recognized { get; }
            = new ReactiveProperty<string>(string.Empty);

        private IList<IDisposable> Subscriptions { get; } = new List<IDisposable>();
        
        public PathSampleViewModel()
        {
            Subscriptions.Add(Sample1.Subscribe(_ => Log.Verbose("Sample1 changed"), ex => Log.Error(ex, ex.Message)));
            Subscriptions.Add(Sample2.Subscribe(_ => Log.Verbose("Sample2 changed"), ex => Log.Error(ex, ex.Message)));
            Subscriptions.Add(Sample3.Subscribe(_ => Log.Verbose("Sample3 changed"), ex => Log.Error(ex, ex.Message)));
            Subscriptions.Add(Sample4.Subscribe(_ => Log.Verbose("Sample4 changed"), ex => Log.Error(ex, ex.Message)));
            Subscriptions.Add(Sample5.Subscribe(_ => Log.Verbose("Sample5 changed"), ex => Log.Error(ex, ex.Message)));

            Subscriptions.Add(Recognized.Subscribe(value =>
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        Log.Information($"Character '{value}' recognized.");
                        return;
                    }
                    Log.Verbose("No character recognized.");
                }, 
                ex => Log.Error(ex, ex.Message)));
        }

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
            
            Subscriptions?.Dispose();

            _isDisposed = true;
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}
