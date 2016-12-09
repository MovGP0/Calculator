using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
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

        #region Input
        public ReactiveProperty<double> FontSize { get; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<double> Width { get; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<FontFamily> FontFamily { get; } = new ReactiveProperty<FontFamily>(default(FontFamily), ReactivePropertyMode.RaiseLatestValueOnSubscribe);
        public ReactiveProperty<FontStyle> FontStyle { get; } = new ReactiveProperty<FontStyle>(default(FontStyle), ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<FontWeight> FontWeight { get; } = new ReactiveProperty<FontWeight>(default(FontWeight), ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<FontStretch> FontStretch { get; } = new ReactiveProperty<FontStretch>(default(FontStretch), ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<bool> IsTraining { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<StrokeCollection> Strokes { get; } = new ReactiveProperty<StrokeCollection>(new StrokeCollection(), ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<TrainingSet> TrainingSet { get; } = new ReactiveProperty<TrainingSet>(Calculator.GestureRecognizer.TrainingSet.Empty);
        #endregion

        #region Output
        public ReactiveProperty<double> Height { get; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);
        public ReactiveProperty<double> Baseline { get; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);
        
        /// <summary>Height of uppercase letters over Baseline</summary>
        public ReactiveProperty<double> CapsHeight { get; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);

        /// <summary>Height of small letters over Baseline</summary>
        public ReactiveProperty<double> XHeight { get; } = new ReactiveProperty<double>(0d, ReactivePropertyMode.DistinctUntilChanged);

        public ReactiveProperty<string> RecognizedCharacter { get; } = new ReactiveProperty<string>(string.Empty);
        #endregion

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

        private static void OnStrokesChanged(StrokeCollection sender, StrokeCollectionChangedEventArgs args)
        {
            Log.Information($"{sender.Count} strokes ({args.Added.Count} strokes added; {args.Removed.Count} strokes removed).");
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

            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
        #endregion
        
        public void OnBeginStroke()
        {
            ResetTimer();
            
            if (_isRecognized)
            {
                Strokes.Value.Clear();
                _isRecognized = false;
            }
        }

        private Timer _timer;
        private bool _isRecognized;

        public void OnStrokeCollected()
        {
            _isRecognized = false;
            ResetTimer();
        }

        private void ResetTimer()
        {
            _timer?.Dispose();
            _timer = new Timer(_ => RecognitionTimerOnElapsed(), null, TimeSpan.FromSeconds(0.5d), TimeSpan.FromMilliseconds(-1));
        }

        private static readonly Random Random = new Random();
        private static string RandomString(int length)
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return Enumerable.Repeat(characters, length)
                .Select(chars => chars[Random.Next(chars.Length)])
                .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c), sb => sb.ToString());
        }

        private void RecognitionTimerOnElapsed()
        {
            NewThreadScheduler.Default.Schedule(() =>
            {
                _isRecognized = true;

                if (!IsTraining.Value && TrainingSet.Value != null)
                {
                    Log.Information("Recognizing character");
                    var gesture = new Gesture(Strokes.Value.ConvertToStrokes(), string.Empty);
                    RecognizedCharacter.Value = TrainingSet.Value.Classify(gesture);
                }
                else
                {
                    Log.Information("Skipping character recognition");
                    RecognizedCharacter.Value = RandomString(1);
                }
            });
        }
    }
}