using System.Linq;
using System.Windows;

namespace Calculator.Pages
{
    public partial class GestureRecognizerTraining
    {
        #region DependencyProperties
        public static readonly DependencyProperty TrainingSetProperty = DependencyProperty.Register(nameof(TrainingSet), typeof(PathSampleCollection), typeof(GestureRecognizerTraining), new PropertyMetadata(default(PathSampleCollection)));

        public PathSampleCollection TrainingSet
        {
            get { return (PathSampleCollection) GetValue(TrainingSetProperty); }
            set { SetValue(TrainingSetProperty, value); }
        }
        #endregion

        public GestureRecognizerTraining()
        {
            InitializeComponent();

            var numbers = Enumerable.Range(48, 10).ToChars();
            var smallCharacters = Enumerable.Range(97, 26).ToChars();
            var upperCharacters = Enumerable.Range(65, 26).ToChars();

            var characters = numbers.Concat(smallCharacters).Concat(upperCharacters);

            TrainingSet = characters.ToTrainingSet().ToPathSampleCollection();
        }
    }
}
