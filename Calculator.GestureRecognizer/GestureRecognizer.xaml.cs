using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Serilog;

namespace Calculator.GestureRecognizer
{
    [TemplatePart(Name="PART_Canvas", Type=typeof(InkCanvas))]
    [TemplatePart(Name="PART_CapsHeight", Type=typeof(Line))]
    [TemplatePart(Name="PART_XHeight", Type=typeof(Line))]
    [TemplatePart(Name="PART_Baseline", Type=typeof(Line))]
    public partial class GestureRecognizer : IDisposable
    {
        #region DependencyProperties
        public static readonly DependencyProperty IsTrainingProperty = DependencyProperty.Register(
            nameof(IsTraining), typeof(bool), typeof(GestureRecognizer), new PropertyMetadata(false));

        public bool IsTraining
        {
            get { return (bool) GetValue(IsTrainingProperty); }
            set { SetValue(IsTrainingProperty, value); }
        }

        public static readonly DependencyProperty StrokesProperty = DependencyProperty.Register(
            nameof(Strokes), typeof(StrokeCollection), typeof(GestureRecognizer), new PropertyMetadata(default(StrokeCollection)));

        public StrokeCollection Strokes
        {
            get { return (StrokeCollection)GetValue(StrokesProperty); }
            set { SetValue(StrokesProperty, value); }
        }
        #endregion

        #region ReactiveProperties
        private ReadOnlyReactiveProperty<bool> IsTrainingReactiveProperty { get; }
        private ReadOnlyReactiveProperty<StrokeCollection> StrokesReactiveProperty { get; }
        private ReadOnlyReactiveProperty<double> FontSizeReactiveProperty { get; }
        private ReadOnlyReactiveProperty<double> WidthReactiveProperty { get; }
        private ReadOnlyReactiveProperty<FontFamily> FontFamilyReactiveProperty { get; }
        private ReadOnlyReactiveProperty<FontStyle> FontStyleReactiveProperty { get; }
        private ReadOnlyReactiveProperty<FontWeight> FontWeightReactiveProperty { get; }
        private ReadOnlyReactiveProperty<FontStretch> FontStretchReactiveProperty { get; }
        #endregion

        private IList<IDisposable> Subscriptions { get; } = new List<IDisposable>();
        
        private InkCanvas PartCanvas { get; set; }
        private Line CapsHeightLine { get; set; }
        private Line XHeightLine { get; set; }
        private Line BaselineLine { get; set; }

        public GestureRecognizerViewModel ViewModel
        {
            get { return (GestureRecognizerViewModel)DataContext; }
            set { DataContext = value; }
        }
        
        public GestureRecognizer()
        {
            ViewModel = new GestureRecognizerViewModel();

            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }

            InitializeViewModel();
            
            const ReactivePropertyMode distinct = ReactivePropertyMode.DistinctUntilChanged;
            IsTrainingReactiveProperty = this.ToReadOnlyReactiveProperty<bool>(IsTrainingProperty, distinct);
            StrokesReactiveProperty = this.ToReadOnlyReactiveProperty<StrokeCollection>(StrokesProperty, distinct);
            FontSizeReactiveProperty = this.ToReadOnlyReactiveProperty<double>(FontSizeProperty, distinct);
            WidthReactiveProperty = this.ToReadOnlyReactiveProperty<double>(WidthProperty, distinct);
            FontFamilyReactiveProperty = this.ToReadOnlyReactiveProperty<FontFamily>(FontFamilyProperty, distinct);
            FontStyleReactiveProperty = this.ToReadOnlyReactiveProperty<FontStyle>(FontStyleProperty, distinct);
            FontWeightReactiveProperty = this.ToReadOnlyReactiveProperty<FontWeight>(FontWeightProperty, distinct);
            FontStretchReactiveProperty = this.ToReadOnlyReactiveProperty<FontStretch>(FontStretchProperty, distinct);

            var subscriptions = SubscribeToReactiveProperties();
            Subscriptions.AddRange(subscriptions);
            
            Dispatcher.ShutdownStarted += DispatcherOnShutdownStarted;
        }

        private void InitializeViewModel()
        {
            var vm = ViewModel;
            vm.FontSize.Value = FontSize;
            vm.FontFamily.Value = FontFamily;
            vm.FontStyle.Value = FontStyle;
            vm.FontWeight.Value = FontWeight;
            vm.FontStretch.Value = FontStretch;
        }

        private IEnumerable<IDisposable> SubscribeToReactiveProperties()
        {
            yield return StrokesReactiveProperty.Subscribe(value => ViewModel.Strokes.Value = value);
            yield return FontSizeReactiveProperty.Subscribe(value => ViewModel.FontSize.Value = value);
            yield return WidthReactiveProperty.Subscribe(value => ViewModel.Width.Value = value);
            yield return FontFamilyReactiveProperty.Subscribe(value => ViewModel.FontFamily.Value = value);
            yield return FontStyleReactiveProperty.Subscribe(value => ViewModel.FontStyle.Value = value);
            yield return FontWeightReactiveProperty.Subscribe(value => ViewModel.FontWeight.Value = value);
            yield return FontStretchReactiveProperty.Subscribe(value => ViewModel.FontStretch.Value = value);
        }

        #region IDisposable
        private void DispatcherOnShutdownStarted(object sender, EventArgs eventArgs)
        {
            Dispose();
        }

        ~GestureRecognizer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private bool _isDisposed;
        private void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            foreach (var subscription in Subscriptions)
            {
                subscription?.Dispose();
            }

            ViewModel?.Dispose();
            ViewModel = null;

            StrokesReactiveProperty?.Dispose();
            StrokesReactiveProperty?.Dispose();
            FontSizeReactiveProperty?.Dispose();
            WidthReactiveProperty?.Dispose();
            FontFamilyReactiveProperty?.Dispose();
            FontStyleReactiveProperty?.Dispose();
            FontWeightReactiveProperty?.Dispose();
            FontStretchReactiveProperty?.Dispose();
            IsTrainingReactiveProperty?.Dispose();

            _isDisposed = true;
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PartCanvas = GetPartCanvas();
            CapsHeightLine = Template.FindName("PART_CapsHeight", this) as Line;
            XHeightLine = Template.FindName("PART_XHeight", this) as Line;
            BaselineLine = Template.FindName("PART_Baseline", this) as Line;
        }
        
        private InkCanvas GetPartCanvas()
        {
            var partCanvas = Template.FindName("PART_Canvas", this) as InkCanvas;
            if (partCanvas == null)
                throw new InvalidOperationException("Could not find 'PART_Canvas' in control template.");
            
            partCanvas.EditingModeInverted = InkCanvasEditingMode.EraseByStroke;
            partCanvas.EraserShape = new EllipseStylusShape(5, 5);
            
            Subscriptions.Add(GetStrokeCollected(partCanvas).Subscribe(_ => OnStrokeCollected()));
            Subscriptions.Add(GetStrokeBeginStylus(partCanvas).Subscribe(_ => OnBeginStroke()));
            Subscriptions.Add(GetStrokeBeginFinger(partCanvas).Subscribe(_ => OnBeginStroke()));
            Subscriptions.Add(GetStrokeBeginMouse(partCanvas).Subscribe(_ => OnBeginStroke()));
            
            return partCanvas;
        }

        private static IObservable<MouseEventArgs> GetStrokeBeginMouse(IInputElement partCanvas)
        {
            return Observable.FromEvent<MouseButtonEventHandler, MouseEventArgs>(
                h =>
                {
                    MouseButtonEventHandler handler = (sender, args) => h(args);
                    return handler;
                },
                h => partCanvas.PreviewMouseLeftButtonDown += h,
                h => partCanvas.PreviewMouseLeftButtonDown -= h);
        }

        private static IObservable<TouchEventArgs> GetStrokeBeginFinger(UIElement partCanvas)
        {
            return Observable.FromEvent<EventHandler<TouchEventArgs>, TouchEventArgs>(
                handler =>
                {
                    EventHandler<TouchEventArgs> h = (sender, args) => handler(args);
                    return h;
                },
                h => partCanvas.TouchDown += h, 
                h => partCanvas.TouchDown -= h);
        }

        private static IObservable<StylusDownEventArgs> GetStrokeBeginStylus(IInputElement partCanvas)
        {
            return Observable.FromEvent<StylusDownEventHandler, StylusDownEventArgs>(
                handler =>
                {
                    StylusDownEventHandler h = (sender, args) => handler(args);
                    return h;
                },
                h => partCanvas.StylusDown += h, 
                h => partCanvas.StylusDown -= h);
        }

        private static IObservable<InkCanvasStrokeCollectedEventArgs> GetStrokeCollected(InkCanvas partCanvas)
        {
            return Observable.FromEvent<InkCanvasStrokeCollectedEventHandler, InkCanvasStrokeCollectedEventArgs>(
                handler =>
                {
                    InkCanvasStrokeCollectedEventHandler h = (sender, args) => handler(args);
                    return h;
                },
                h => partCanvas.StrokeCollected += h,
                h => partCanvas.StrokeCollected -= h);
        }

        private void OnBeginStroke()
        {
            ResetTimer();
            
            if (_isRecognized)
            {
                PartCanvas.Strokes.Clear();
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

        private void RecognitionTimerOnElapsed()
        {
            NewThreadScheduler.Default.Schedule(() =>
            {
                _isRecognized = true;

                if (!IsTrainingReactiveProperty.Value)
                {
                    Log.Information("Recognizing character");
                    // TODO: start character recognition
                }
                else
                {
                    Log.Information("Skipping character recognition");
                }
            });
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            CapsHeightLine.Measure(size);
            XHeightLine.Measure(size);
            BaselineLine.Measure(size);
            PartCanvas.Measure(size);
            
            return new Size(Width, Height);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = new Rect(new Point(0,0), arrangeBounds);
            CapsHeightLine.Arrange(size);
            XHeightLine.Arrange(size);
            BaselineLine.Arrange(size);
            PartCanvas.Arrange(size);

            return new Size(Width, Height);
        }
    }
}
