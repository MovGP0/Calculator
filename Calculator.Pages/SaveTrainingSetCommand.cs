using System;
using System.Linq;
using System.Windows.Input;
using Calculator.GestureRecognizer;

namespace Calculator.Pages
{
    public sealed class SaveTrainingSetCommand : ICommand
    {
        private GestureTrainingFrame Control { get; }

        public SaveTrainingSetCommand(GestureTrainingFrame control)
        {
            Control = control;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var gestures = Control.TrainingSet.ToGestures();
            var trainingSet = new TrainingSet(gestures.ToList());
            TrainingSetIo.WriteGestureAsBinaryAsync(trainingSet, "training.xml");
        }
        
        public event EventHandler CanExecuteChanged;
    }
}