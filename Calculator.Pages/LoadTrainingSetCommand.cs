using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Calculator.GestureRecognizer;
using Serilog;

namespace Calculator.Pages
{
    public sealed class LoadTrainingSetCommand : IAsyncCommand, IDisposable
    {
        private ILogger Log { get; }

        private IEnumerable<string> _pathNamesToLoad = new List<string>();
        public IEnumerable<string> PathNamesToLoad {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _pathNamesToLoad = value;
                OnCanExecuteChanged(null, EventArgs.Empty);
            }
            private get { return _pathNamesToLoad; }
        }

        private GestureTrainingPageViewModel ViewModel { get; }
        private const string FileName = "training.bin";
        private string Directory { get; }
        private FileSystemWatcher Watcher { get; set; } = new FileSystemWatcher();

        public LoadTrainingSetCommand(GestureTrainingPageViewModel viewModel, ILogger log)
        {
            Log = log;
            ViewModel = viewModel;
            Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            SetupWatcher();
        }

        #region IDisposable
        ~LoadTrainingSetCommand()
        {
            Dispose(false);
        }

        private bool _isDisposed;
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            Watcher.Created -= OnFileChanged;
            Watcher.Deleted -= OnFileChanged;
            Watcher.Renamed -= OnFileChanged;
            Watcher.Dispose();
            Watcher = null;

            _isDisposed = true;
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        private void SetupWatcher()
        {
            Watcher.Path = Directory;
            Watcher.Filter = Path.GetFileName(FileName);
            Watcher.Created += OnFileChanged;
            Watcher.Deleted += OnFileChanged;
            Watcher.Renamed += OnFileChanged;
        }

        private void OnFileChanged(object sender, EventArgs args)
        {
            OnCanExecuteChanged(sender, args);
        }

        private void OnCanExecuteChanged(object sender, EventArgs args)
        {
            CanExecuteChanged?.Invoke(sender, args);
        }

        public bool CanExecute(object parameter)
        {
            return PathNamesToLoad.Any() 
                && File.Exists(Path.Combine(Directory, FileName));
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public event EventHandler CanExecuteChanged;
        public async Task ExecuteAsync(object parameter)
        {
            Log.Information($"Executing {nameof(LoadTrainingSetCommand)}");

            if(PathNamesToLoad == null) throw new InvalidOperationException($"{nameof(PathNamesToLoad)} was not set.");
            
            var gestures = await TrainingSetIo.ReadGestureFromBinaryAsync(FileName, Log);
            
            ViewModel.TrainingSet.Clear();

            foreach (var pathSample in gestures.ToPathSamples(PathNamesToLoad))
            {
                ViewModel.TrainingSet.Add(pathSample);
            }
        }
    }
}