using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Calculator.Pages
{
    public partial class GestureTrainingPage
    {
        private GestureTrainingPageViewModel ViewModel
        {
            get { return (GestureTrainingPageViewModel) DataContext; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                DataContext = value;
            }
        }

        public GestureTrainingPage(GestureTrainingPageViewModel viewModel)
        {
            InitializeComponent();
            var trainingSet = SetupTrainingSet();
            viewModel.TrainingSet = trainingSet;
            ViewModel = viewModel;

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var loadCommand = ViewModel.LoadCommand;
            loadCommand.PathNamesToLoad = GetCharactersToLoad().Select(c => c.ToString());

            if (loadCommand.CanExecute(null))
            {
                await loadCommand.ExecuteAsync(null);
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
