using System;
using System.Linq;
using System.Threading.Tasks;
using Calculator.GestureRecognizer;
using Serilog;

namespace Calculator.Pages
{
    public sealed class SaveTrainingSetCommand : IAsyncCommand
    {
        private GestureTrainingPageViewModel ViewModel { get; }
        private ILogger Log { get; }
        private const string FileName = "training.bin";

        public SaveTrainingSetCommand(GestureTrainingPageViewModel viewModel, ILogger log)
        {
            ViewModel = viewModel;
            Log = log;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        private event EventHandler CanExecuteChangedHandler;
        public event EventHandler CanExecuteChanged
        {
            add { CanExecuteChangedHandler += value; }
            remove { CanExecuteChangedHandler -= value; }
        }

        public async Task ExecuteAsync(object parameter)
        {
            Log.Information($"Executing {nameof(SaveTrainingSetCommand)}");

            var gestures = ViewModel.PathSamples.SelectMany(sample => sample.ToGesture());
            var trainingSet = new TrainingSet(gestures.ToList());
            await TrainingSetIo.WriteGestureAsBinaryAsync(trainingSet, FileName);
        }
    }
}