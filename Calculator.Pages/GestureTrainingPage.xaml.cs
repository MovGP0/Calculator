using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Calculator.Pages
{
    public partial class GestureTrainingPage
    {
        #region DependencyProperties
        public static readonly DependencyProperty TrainingSetProperty = DependencyProperty.Register(nameof(TrainingSet), typeof(ObservableCollection<PathSample>), typeof(GestureTrainingPage), new PropertyMetadata(null));

        public ObservableCollection<PathSample> TrainingSet
        {
            get { return (ObservableCollection<PathSample>) GetValue(TrainingSetProperty); }
            set { SetValue(TrainingSetProperty, value); }
        }

        public ICommand SaveCommand { get; }
        #endregion

        public GestureTrainingPage()
        {
            InitializeComponent();
            
            SaveCommand = new SaveTrainingSetCommand(this);

            var trainingSet = SetupTrainingSet();
            TrainingSet = trainingSet;
        }

        private static ObservableCollection<PathSample> SetupTrainingSet()
        {
            var trainingSet = new ObservableCollection<PathSample>();
            foreach (var pathSample in CreateTrainingSet())
            {
                trainingSet.Add(pathSample);
            }
            return trainingSet;
        }

        private static IEnumerable<PathSample> CreateTrainingSet()
        {
            var numbers = Enumerable.Range(48, 10).ToChars();
            var smallCharacters = Enumerable.Range(97, 26).ToChars();
            var upperCharacters = Enumerable.Range(65, 26).ToChars();
            var characters = numbers.Concat(smallCharacters).Concat(upperCharacters);
            return characters.ToTrainingSet();
        }
    }
}
