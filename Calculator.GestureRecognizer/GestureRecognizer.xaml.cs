using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Timer = System.Threading.Timer;

namespace Calculator.GestureRecognizer
{
    [TemplatePart(Name="PART_Canvas", Type=typeof(InkCanvas))]
    [TemplatePart(Name="PART_CapsHeight", Type=typeof(Line))]
    [TemplatePart(Name="PART_XHeight", Type=typeof(Line))]
    [TemplatePart(Name="PART_Baseline", Type=typeof(Line))]
    public partial class GestureRecognizer
    {
        #region DependencyProperties
        public static readonly DependencyProperty BaselineProperty = DependencyProperty.Register(nameof(Baseline), typeof(double), typeof(GestureRecognizer), new PropertyMetadata(default(double)));

        public double Baseline
        {
            get { return (double) GetValue(BaselineProperty); }
            set { SetValue(BaselineProperty, value); }
        }
        
        public static readonly DependencyProperty CapsHeightProperty = DependencyProperty.Register(nameof(CapsHeight), typeof(double), typeof(GestureRecognizer), new PropertyMetadata(default(double)));

        /// <summary>
        /// Height of uppercase letters over Baseline
        /// </summary>
        public double CapsHeight
        {
            get { return (double) GetValue(CapsHeightProperty); }
            set { SetValue(CapsHeightProperty, value); }
        }

        public static readonly DependencyProperty XHeightProperty = DependencyProperty.Register(nameof(XHeight), typeof(double), typeof(GestureRecognizer), new PropertyMetadata(default(double)));

        /// <summary>
        /// Height of small letters over Baseline
        /// </summary>
        public double XHeight
        {
            get { return (double) GetValue(XHeightProperty); }
            set { SetValue(XHeightProperty, value); }
        }

        public static readonly DependencyProperty StrokesProperty = DependencyProperty.Register(nameof(Strokes), typeof(IEnumerable<Stroke>), typeof(GestureRecognizer), new PropertyMetadata(null));

        public IEnumerable<Stroke> Strokes
        {
            get { return (IEnumerable<Stroke>) GetValue(StrokesProperty); }
            set { SetValue(StrokesProperty, value); }
        }

        public static readonly DependencyProperty IsTrainingProperty = DependencyProperty.Register(
            nameof(IsTraining), typeof(bool), typeof(GestureRecognizer), new PropertyMetadata(false));

        public bool IsTraining
        {
            get { return (bool) GetValue(IsTrainingProperty); }
            set { SetValue(IsTrainingProperty, value); }
        }
        #endregion

        public GestureRecognizer()
        {
            InitializeComponent();
            
            Dispatcher.ShutdownStarted += DispatcherOnShutdownStarted;

            //FontSizeProperty.OverrideMetadata(typeof(double), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, (sender, args) => UpdateBaseline()));
            //FontFamilyProperty.OverrideMetadata(typeof(FontFamily), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, (sender, args) => UpdateBaseline()));

            var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            GlyphTypeface glyphTypeface;
            if (!typeface.TryGetGlyphTypeface(out glyphTypeface))
                throw new InvalidOperationException("No GlyphTypeface found");

            var fontSize = FontSize * 10;
            var baseline = glyphTypeface.Baseline;
            Baseline = baseline * fontSize;
            CapsHeight = glyphTypeface.CapsHeight * fontSize;
            XHeight = glyphTypeface.XHeight * fontSize;
            Height = glyphTypeface.Height * fontSize;
        }

        private void DispatcherOnShutdownStarted(object sender, EventArgs eventArgs)
        {
            foreach (var subscription in Subscriptions)
            {
                subscription?.Dispose();
            }
        }

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

            //Subscriptions.Add(SubscribeToRecognitionTimer(scheduler, args => RecognitionTimerOnElapsed(args)));

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
                var strokes = PartCanvas.Strokes.ConvertToStrokes();    
                Strokes = strokes;
                
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
