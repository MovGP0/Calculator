using System;
using System.Linq;
using System.Threading.Tasks;
using Calculator.GestureRecognizer;

namespace Calculator.Pages
{
    public sealed class SaveTrainingSetCommand : IAsyncCommand
    {
        private GestureTrainingPage Control { get; }

        public SaveTrainingSetCommand(GestureTrainingPage control)
        {
            Control = control;
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
            var gestures = Control.TrainingSet.SelectMany(sample => sample.ToGesture());
            var trainingSet = new TrainingSet(gestures.ToList());
            await TrainingSetIo.WriteGestureAsBinaryAsync(trainingSet, "training.xml");
        }
    }
}