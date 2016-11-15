using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Serilog;

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

        private ILogger Log { get; }

        public GestureTrainingPage(GestureTrainingPageViewModel viewModel, ILogger log)
        {
            Log = log;
            InitializeComponent();
            ApplyTrainingSet(viewModel.PathSamples);
            ViewModel = viewModel;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = ViewModel;
            viewModel.PathNamesToLoad.Value = GetCharactersToLoad().Select(c => c.ToString());

            Log.Information("Loading strokes from file...");
            if (viewModel.LoadCommand.CanExecute())
            {
                viewModel.LoadCommand.Execute(null);
            }
        }

        private static void ApplyTrainingSet(ICollection<PathSample> pathSamples)
        {
            pathSamples.Clear();
            foreach (var pathSample in CreateTrainingSet())
            {
                pathSamples.Add(pathSample);
            }
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
