using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Calculator.Pages
{
    public partial class GestureTrainingPage
    {
        #region DependencyProperties
        public static readonly DependencyProperty TrainingSetProperty = DependencyProperty.Register(nameof(TrainingSet), typeof(PathSampleCollection), typeof(GestureTrainingPage), new PropertyMetadata(default(PathSampleCollection)));

        public PathSampleCollection TrainingSet
        {
            get { return (PathSampleCollection) GetValue(TrainingSetProperty); }
            set { SetValue(TrainingSetProperty, value); }
        }

        public ICommand SaveCommand { get; }
        #endregion

        public GestureTrainingPage()
        {
            InitializeComponent();
            TrainingSet = CreateTrainingSet();
            SaveCommand = new SaveTrainingSetCommand(this);
        }

        private static PathSampleCollection CreateTrainingSet()
        {
            var numbers = Enumerable.Range(48, 10).ToChars();
            var smallCharacters = Enumerable.Range(97, 26).ToChars();
            var upperCharacters = Enumerable.Range(65, 26).ToChars();
            var characters = numbers.Concat(smallCharacters).Concat(upperCharacters);
            return characters.ToTrainingSet().ToPathSampleCollection();
        }
    }
}
