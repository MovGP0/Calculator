using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Calculator.Pages
{
    public sealed class GestureTrainingPageViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<PathSample> _trainingSet;
        public ObservableCollection<PathSample> TrainingSet
        {
            get { return _trainingSet; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _trainingSet = value;
                RaisePropertyChanged();
            }
        }

        private IAsyncCommand _saveCommand;
        public IAsyncCommand SaveCommand
        {
            get { return _saveCommand; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _saveCommand = value;
                RaisePropertyChanged();
            }
        }

        private LoadTrainingSetCommand _loadCommand;
        public LoadTrainingSetCommand LoadCommand
        {
            get { return _loadCommand; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _loadCommand = value;
                RaisePropertyChanged();
            }
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged; 

        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion 
    }
}