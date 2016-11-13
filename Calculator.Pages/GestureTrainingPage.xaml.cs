using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

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

        public IAsyncCommand SaveCommand { get; }
        public LoadTrainingSetCommand LoadCommand { get; }
        #endregion

        public GestureTrainingPage(
            Func<GestureTrainingPage, SaveTrainingSetCommand> saveTrainingSetCommandFactory, 
            Func<GestureTrainingPage, LoadTrainingSetCommand> loadTrainingSetCommandFactory)
        {
            InitializeComponent();
            
            SaveCommand = saveTrainingSetCommandFactory(this);
            LoadCommand = loadTrainingSetCommandFactory(this);
            
            Loaded += OnLoaded;

            var trainingSet = SetupTrainingSet();
            TrainingSet = trainingSet;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadCommand.PathNamesToLoad = GetCharactersToLoad().Select(c => c.ToString());

            if (LoadCommand.CanExecute(null))
            {
                await LoadCommand.ExecuteAsync(null);
            }
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
            var characters = GetCharactersToLoad();
            return characters.ToTrainingSet();
        }

        private static IEnumerable<char> GetCharactersToLoad()
        {
            var numbers = Enumerable.Range(48, 10).ToChars();
            var smallCharacters = Enumerable.Range(97, 26).ToChars();
            var upperCharacters = Enumerable.Range(65, 26).ToChars();
            var characters = numbers.Concat(smallCharacters).Concat(upperCharacters);
            return characters;
        }
    }
}
