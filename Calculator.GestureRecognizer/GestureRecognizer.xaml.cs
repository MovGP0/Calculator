using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Shapes;
using Serilog;
using Timer = System.Threading.Timer;

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

        public GestureRecognizerViewModel ViewModel
        {
            get { return (GestureRecognizerViewModel)DataContext; }
            set { DataContext = value; }
        }
        
        public GestureRecognizer()
        {
            ViewModel = new GestureRecognizerViewModel(Log.Logger);
            InitializeComponent();
            InitializeViewModel();

            var subscriptions = SubscribeToDependencyProperties();
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

        private IEnumerable<IDisposable> SubscribeToDependencyProperties()
        {
            yield return this.Observe(StrokesProperty).Subscribe(_ => ViewModel.Strokes.Value = Strokes);
            yield return this.Observe(FontSizeProperty).Subscribe(_ => ViewModel.FontSize.Value = FontSize);
            yield return this.Observe(WidthProperty).Subscribe(_ => ViewModel.Width.Value = Width);
            yield return this.Observe(FontFamilyProperty).Subscribe(_ => ViewModel.FontFamily.Value = FontFamily);
            yield return this.Observe(FontStyleProperty).Subscribe(_ => ViewModel.FontStyle.Value = FontStyle);
            yield return this.Observe(FontWeightProperty).Subscribe(_ => ViewModel.FontWeight.Value = FontWeight);
            yield return this.Observe(FontFamilyProperty).Subscribe(_ => ViewModel.FontFamily.Value = FontFamily);
            yield return this.Observe(FontStretchProperty).Subscribe(_ => ViewModel.FontStretch.Value = FontStretch);
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

            _isDisposed = true;
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        private InkCanvas PartCanvas { get; set; }
        private Line CapsHeightLine { get; set; }
        private Line XHeightLine { get; set; }
        private Line BaselineLine { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PartCanvas = GetPartCanvas();
            CapsHeightLine = Template.FindName("PART_CapsHeight", this) as Line;
            XHeightLine = Template.FindName("PART_XHeight", this) as Line;
            BaselineLine = Template.FindName("PART_Baseline", this) as Line;
        }
        
        private IList<IDisposable> Subscriptions { get; } = new List<IDisposable>();

        private InkCanvas GetPartCanvas()
        {
            var partCanvas = Template.FindName("PART_Canvas", this) as InkCanvas;
            if (partCanvas == null)
                throw new InvalidOperationException("Could not find 'PART_Canvas' in control template.");
            
            partCanvas.EditingModeInverted = InkCanvasEditingMode.EraseByStroke;
            partCanvas.EraserShape = new EllipseStylusShape(5, 5);
            
            var scheduler = new SynchronizationContextScheduler(SynchronizationContext.Current);
            var strokeCollected = Observable.FromEvent<InkCanvasStrokeCollectedEventHandler, InkCanvasStrokeCollectedEventArgs>(
                h =>
                {
                    InkCanvasStrokeCollectedEventHandler handler = (sender, args) => h(args);
                    return handler;
                },
                h => partCanvas.StrokeCollected += h,
                h => partCanvas.StrokeCollected -= h);
            Subscriptions.Add(strokeCollected.SubscribeOn(scheduler).Subscribe(_ => OnStrokeCollected(scheduler)));

            var strokeBeginStylus = Observable.FromEvent<StylusDownEventHandler, StylusDownEventArgs>(
                h =>
                {
                    StylusDownEventHandler handler = (sender, args) => h(args);
                    return handler;
                },
                h => partCanvas.StylusDown += h, 
                h => partCanvas.StylusDown -= h);
            Subscriptions.Add(strokeBeginStylus.SubscribeOn(scheduler).Subscribe(_ => OnBeginStroke(scheduler)));

            var strokeBeginFinger = Observable.FromEventPattern<TouchEventArgs>(
                h => partCanvas.TouchDown += h, 
                h => partCanvas.TouchDown -= h);
            Subscriptions.Add(strokeBeginFinger.SubscribeOn(scheduler).Subscribe(_ => OnBeginStroke(scheduler)));

            var strokeBeginMouse = Observable.FromEvent<MouseButtonEventHandler, MouseEventArgs>(
                h =>
                {
                    MouseButtonEventHandler handler = (sender, args) => h(args);
                    return handler;
                },
                h => partCanvas.PreviewMouseLeftButtonDown += h,
                h => partCanvas.PreviewMouseLeftButtonDown -= h);
            Subscriptions.Add(strokeBeginMouse.SubscribeOn(scheduler).Subscribe(args => OnBeginStroke(scheduler)));
            
            return partCanvas;
        }
        
        private void OnBeginStroke(IScheduler scheduler)
        {
            ResetTimer(scheduler);
            
            if (_isRecognized)
            {
                PartCanvas.Strokes.Clear();
                _isRecognized = false;
            }
        }

        private Timer _timer;
        private bool _isRecognized;

        public void OnStrokeCollected(IScheduler scheduler)
        {
            _isRecognized = false;
            ResetTimer(scheduler);
        }

        private void ResetTimer(IScheduler scheduler)
        {
            _timer?.Dispose();
            _timer = new Timer(
                _ => RecognitionTimerOnElapsed(scheduler), 
                null, 
                TimeSpan.FromSeconds(0.5d),
                TimeSpan.FromMilliseconds(-1));
        }

        private void RecognitionTimerOnElapsed(IScheduler scheduler)
        {
            scheduler.Schedule(() =>
            {
                _isRecognized = true;
                
                if (!IsTraining)
                {
                    // TODO: start character recognition
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
