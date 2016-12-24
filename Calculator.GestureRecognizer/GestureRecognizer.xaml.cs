using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Calculator.Common;
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
        private static ILogger Log { get; } = Serilog.Log.ForContext<GestureRecognizer>();

        #region DependencyProperties
        public static readonly DependencyProperty IsTrainingProperty = DependencyProperty.Register(
            nameof(IsTraining), typeof(bool), typeof(GestureRecognizer), new PropertyMetadata(false));

        public bool IsTraining
        {
            get { return (bool) GetValue(IsTrainingProperty); }
            set { SetValue(IsTrainingProperty, value); }
        }

        public static readonly DependencyProperty StrokesCollectionProperty = DependencyProperty.Register(
            nameof(StrokesCollection), typeof(StrokeCollection), typeof(GestureRecognizer), new PropertyMetadata(default(StrokeCollection)));

        public StrokeCollection StrokesCollection
        {
            get { return (StrokeCollection)GetValue(StrokesCollectionProperty); }
            set { SetValue(StrokesCollectionProperty, value); }
        }

        public static readonly DependencyProperty TrainingSetProperty = DependencyProperty.Register(
            nameof(TrainingSet), typeof(TrainingSet), typeof(GestureRecognizer), new PropertyMetadata(TrainingSet.Empty));

        public TrainingSet TrainingSet
        {
            get { return (TrainingSet) GetValue(TrainingSetProperty); }
            set { SetValue(TrainingSetProperty, value); }
        }

        public static readonly DependencyProperty RecognizedProperty = DependencyProperty.Register(
            nameof(Recognized), typeof(string), typeof(GestureRecognizer), new PropertyMetadata(default(string)));

        public string Recognized
        {
            get { return (string) GetValue(RecognizedProperty); }
            set { SetValue(RecognizedProperty, value); }
        }
        
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(GestureRecognizer), new PropertyMetadata(default(string)));
        
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
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
        private ReadOnlyReactiveProperty<TrainingSet> TrainingSetReactiveProperty { get; }
        private ReactiveProperty<string> RecognizedReactiveProperty { get; }
        private ReactiveProperty<string> TextReactiveProperty { get; }
        #endregion

        private CompositeDisposable Subscriptions { get; } = new CompositeDisposable();
        
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

            const ReactivePropertyMode distinct = ReactivePropertyMode.DistinctUntilChanged;
            IsTrainingReactiveProperty = this.ToReadOnlyReactiveProperty<bool>(IsTrainingProperty, distinct);
            StrokesReactiveProperty = this.ToReadOnlyReactiveProperty<StrokeCollection>(StrokesCollectionProperty, distinct);
            FontSizeReactiveProperty = this.ToReadOnlyReactiveProperty<double>(FontSizeProperty, distinct);
            WidthReactiveProperty = this.ToReadOnlyReactiveProperty<double>(WidthProperty, distinct);
            FontFamilyReactiveProperty = this.ToReadOnlyReactiveProperty<FontFamily>(FontFamilyProperty, distinct);
            FontStyleReactiveProperty = this.ToReadOnlyReactiveProperty<FontStyle>(FontStyleProperty, distinct);
            FontWeightReactiveProperty = this.ToReadOnlyReactiveProperty<FontWeight>(FontWeightProperty, distinct);
            FontStretchReactiveProperty = this.ToReadOnlyReactiveProperty<FontStretch>(FontStretchProperty, distinct);
            TrainingSetReactiveProperty = this.ToReadOnlyReactiveProperty<TrainingSet>(TrainingSetProperty, distinct);
            RecognizedReactiveProperty = this.ToReactiveProperty<string>(RecognizedProperty);
            TextReactiveProperty = this.ToReactiveProperty<string>(TextProperty);

            Subscriptions.AddRange(SubscribeToViewModel());

            var subscriptions = SubscribeToReactiveProperties();
            Subscriptions.AddRange(subscriptions);

            InitializeViewModel();

            Dispatcher.ShutdownStarted += DispatcherOnShutdownStarted;
        }

        private IEnumerable<IDisposable> SubscribeToViewModel()
        {
            yield return ViewModel.RecognizedCharacter.Subscribe(
                value => RecognizedReactiveProperty.Value = value, 
                ex => Log.Error(ex, ex.Message));
        }

        private void InitializeViewModel()
        {
            var vm = ViewModel;
            vm.FontSize.Value = FontSizeReactiveProperty.Value;
            vm.FontFamily.Value = FontFamilyReactiveProperty.Value;
            vm.FontStyle.Value = FontStyleReactiveProperty.Value;
            vm.FontWeight.Value = FontWeightReactiveProperty.Value;
            vm.FontStretch.Value = FontStretchReactiveProperty.Value;
            vm.Text.Value = TextReactiveProperty.Value;
        }

        private IEnumerable<IDisposable> SubscribeToReactiveProperties()
        {
            yield return StrokesReactiveProperty.Subscribe(value => ViewModel.Strokes.Value = value, ex => Log.Error(ex, ex.Message));
            yield return FontSizeReactiveProperty.Subscribe(value => ViewModel.FontSize.Value = value, ex => Log.Error(ex, ex.Message));
            yield return WidthReactiveProperty.Subscribe(value => ViewModel.Width.Value = value, ex => Log.Error(ex, ex.Message));
            yield return FontFamilyReactiveProperty.Subscribe(value => ViewModel.FontFamily.Value = value, ex => Log.Error(ex, ex.Message));
            yield return FontStyleReactiveProperty.Subscribe(value => ViewModel.FontStyle.Value = value, ex => Log.Error(ex, ex.Message));
            yield return FontWeightReactiveProperty.Subscribe(value => ViewModel.FontWeight.Value = value, ex => Log.Error(ex, ex.Message));
            yield return FontStretchReactiveProperty.Subscribe(value => ViewModel.FontStretch.Value = value, ex => Log.Error(ex, ex.Message));
            yield return TrainingSetReactiveProperty.Subscribe(value => ViewModel.TrainingSet.Value = value, ex => Log.Error(ex, ex.Message));
            yield return IsTrainingReactiveProperty.Subscribe(value => ViewModel.IsTraining.Value = value, ex => Log.Error(ex, ex.Message));
            yield return TextReactiveProperty.Subscribe(value => ViewModel.Text.Value = value, ex => Log.Error(ex, ex.Message));
        }

        private void DispatcherOnShutdownStarted(object sender, EventArgs eventArgs)
        {
            Dispose();
        }

        #region IDisposable
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

            Subscriptions.Dispose();

            if (DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }

            StrokesReactiveProperty.Dispose();
            StrokesReactiveProperty.Dispose();
            FontSizeReactiveProperty.Dispose();
            WidthReactiveProperty.Dispose();
            FontFamilyReactiveProperty.Dispose();
            FontStyleReactiveProperty.Dispose();
            FontWeightReactiveProperty.Dispose();
            FontStretchReactiveProperty.Dispose();
            IsTrainingReactiveProperty.Dispose();

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            _isDisposed = true;
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
            
            Subscriptions.Add(GetStrokeCollected(partCanvas).Subscribe(_ => ViewModel.OnStrokeCollected(), ex => Log.Error(ex, ex.Message)));
            Subscriptions.Add(GetStrokeBeginStylus(partCanvas).Subscribe(_ => ViewModel.OnBeginStroke(), ex => Log.Error(ex, ex.Message)));
            Subscriptions.Add(GetStrokeBeginFinger(partCanvas).Subscribe(_ => ViewModel.OnBeginStroke(), ex => Log.Error(ex, ex.Message)));
            Subscriptions.Add(GetStrokeBeginMouse(partCanvas).Subscribe(_ => ViewModel.OnBeginStroke(), ex => Log.Error(ex, ex.Message)));
            
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
