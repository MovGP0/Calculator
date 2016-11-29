using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using Reactive.Bindings;

namespace Calculator.GestureRecognizer
{
    public sealed class GestureRecognizerViewModel
    {
        private IList<IDisposable> Subscriptions { get; } = new List<IDisposable>();

        public ReactiveProperty<double> FontSize { get; set; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<double> Width { get; set; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<FontFamily> FontFamily { get; set; } = new ReactiveProperty<FontFamily>(default(FontFamily), ReactivePropertyMode.RaiseLatestValueOnSubscribe);
        public ReactiveProperty<FontStyle> FontStyle { get; set; } = new ReactiveProperty<FontStyle>(default(FontStyle), ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<FontWeight> FontWeight { get; set; } = new ReactiveProperty<FontWeight>(default(FontWeight), ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<FontStretch> FontStretch { get; set; } = new ReactiveProperty<FontStretch>(default(FontStretch), ReactivePropertyMode.DistinctUntilChanged);
        
        public GestureRecognizerViewModel()
        {
            Subscriptions.Add(FontSize.Subscribe(_ => UpdateValues()));
            Subscriptions.Add(Width.Subscribe(_ => UpdateValues()));
            Subscriptions.Add(FontFamily.Subscribe(_ => UpdateValues()));
            Subscriptions.Add(FontStyle.Subscribe(_ => UpdateValues()));
            Subscriptions.Add(FontWeight.Subscribe(_ => UpdateValues()));
            Subscriptions.Add(FontStretch.Subscribe(_ => UpdateValues()));
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

        public ReactiveProperty<double> Height { get; set; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<double> Baseline { get; set; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);

        /// <summary>
        /// Height of uppercase letters over Baseline
        /// </summary>
        public ReactiveProperty<double> CapsHeight { get; set; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);

        /// <summary>
        /// Height of small letters over Baseline
        /// </summary>
        public ReactiveProperty<double> XHeight { get; set; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);

        public ReactiveProperty<StrokeCollection> Strokes { get; set; } = new ReactiveProperty<StrokeCollection>(new StrokeCollection(), ReactivePropertyMode.DistinctUntilChanged);

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

            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}