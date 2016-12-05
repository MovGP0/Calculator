using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using Reactive.Bindings;
using Serilog;

namespace Calculator.GestureRecognizer
{
    public sealed class GestureRecognizerViewModel
    {
        private IList<IDisposable> Subscriptions { get; } = new List<IDisposable>();
        private IDisposable StrokesChangedSubsription { get; set; }

        public ReactiveProperty<double> FontSize { get; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<double> Width { get; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<FontFamily> FontFamily { get; } = new ReactiveProperty<FontFamily>(default(FontFamily), ReactivePropertyMode.RaiseLatestValueOnSubscribe);
        public ReactiveProperty<FontStyle> FontStyle { get; } = new ReactiveProperty<FontStyle>(default(FontStyle), ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<FontWeight> FontWeight { get; } = new ReactiveProperty<FontWeight>(default(FontWeight), ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<FontStretch> FontStretch { get; } = new ReactiveProperty<FontStretch>(default(FontStretch), ReactivePropertyMode.DistinctUntilChanged);
        
        
        public ReactiveProperty<double> Height { get; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<double> Baseline { get; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);

        /// <summary>
        /// Height of uppercase letters over Baseline
        /// </summary>
        public ReactiveProperty<double> CapsHeight { get; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);

        /// <summary>
        /// Height of small letters over Baseline
        /// </summary>
        public ReactiveProperty<double> XHeight { get; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);

        public ReactiveProperty<StrokeCollection> Strokes { get; } = new ReactiveProperty<StrokeCollection>(new StrokeCollection(), ReactivePropertyMode.DistinctUntilChanged);

        /// <summary>
        /// Clone of <see cref="Strokes"/> that updates when Stroke changes.
        /// </summary>
        public ReactiveProperty<StrokeCollection> StrokeCollection { get; } = new ReactiveProperty<StrokeCollection>(new StrokeCollection());

        public GestureRecognizerViewModel()
        {
            Subscriptions.Add(FontSize.Subscribe(_ => UpdateValues()));
            Subscriptions.Add(Width.Subscribe(_ => UpdateValues()));
            Subscriptions.Add(FontFamily.Subscribe(_ => UpdateValues()));
            Subscriptions.Add(FontStyle.Subscribe(_ => UpdateValues()));
            Subscriptions.Add(FontWeight.Subscribe(_ => UpdateValues()));
            Subscriptions.Add(FontStretch.Subscribe(_ => UpdateValues()));

            Subscriptions.Add(Strokes.Subscribe(strokeCollection =>
            {
                StrokesChangedSubsription?.Dispose();
                StrokesChangedSubsription = SubscribeToStrokeCollection(strokeCollection);
            }));
            StrokesChangedSubsription = SubscribeToStrokeCollection(Strokes.Value);
        }

        private IDisposable SubscribeToStrokeCollection(StrokeCollection strokeCollection)
        {
            try
            {
                return GetStrokesChanged(strokeCollection)
                    .Subscribe(args => OnStrokesChanged(strokeCollection, args));
            }
            catch(Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        private static IObservable<StrokeCollectionChangedEventArgs> GetStrokesChanged(StrokeCollection strokeCollection)
        {
            return Observable.FromEvent<StrokeCollectionChangedEventHandler, StrokeCollectionChangedEventArgs>(
                    onNextHandler =>
                    {
                        StrokeCollectionChangedEventHandler handler = (sender, args) => onNextHandler(args);
                        return handler;
                    },
                    eh => strokeCollection.StrokesChanged += eh,
                    eh => strokeCollection.StrokesChanged -= eh);
        }

        private void OnStrokesChanged(StrokeCollection sender, StrokeCollectionChangedEventArgs args)
        {
            Log.Information($"{sender.Count} strokes ({args.Added.Count} strokes added; {args.Removed.Count} strokes removed).");
            StrokeCollection.Value = sender;
        }
        
        public void UpdateValues()
        {
            if (FontFamily.Value == null)
            {
                return;
            }
            
            var typeface = new Typeface(FontFamily.Value, FontStyle.Value, FontWeight.Value, FontStretch.Value);
            GlyphTypeface glyphTypeface;
            if (!typeface.TryGetGlyphTypeface(out glyphTypeface))
                throw new InvalidOperationException("No GlyphTypeface found");
            
            var fontSize = FontSize.Value*10;
            var baseline = glyphTypeface.Baseline;

            Baseline.Value = baseline*fontSize;
            CapsHeight.Value = glyphTypeface.CapsHeight*fontSize;
            XHeight.Value = glyphTypeface.XHeight*fontSize;
            Height.Value = glyphTypeface.Height*fontSize;
        }

        #region IDisposable
        ~GestureRecognizerViewModel()
        {
            Dispose(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
        }

        private bool _isDisposed;
        public void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;
            
            foreach (var subscription in Subscriptions)
            {
                subscription?.Dispose();
            }

            StrokesChangedSubsription?.Dispose();
            FontSize?.Dispose();
            Width?.Dispose();
            FontFamily?.Dispose();
            FontStyle?.Dispose();
            FontWeight?.Dispose();
            FontStretch?.Dispose();
            Height?.Dispose();
            Baseline?.Dispose();
            CapsHeight?.Dispose();
            XHeight?.Dispose();
            Strokes?.Dispose();
            StrokeCollection?.Dispose();

            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}