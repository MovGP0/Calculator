using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Calculator.GestureRecognizer;
using Reactive.Bindings;
using Serilog;

namespace Calculator.Pages
{
    public sealed class GestureTrainingPageViewModel
    {
        private ILogger Log { get; }

        public ObservableCollection<PathSample> PathSamples { get; } = new ObservableCollection<PathSample>(new List<PathSample>());
        
        public AsyncReactiveCommand SaveCommand { get; }
        
        public AsyncReactiveCommand LoadCommand { get; }

        public ReactiveProperty<string> Directory { get; } 
            = new ReactiveProperty<string>(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ReactivePropertyMode.DistinctUntilChanged);

        public ReactiveProperty<string> FileName { get; } 
            = new ReactiveProperty<string>("training.bin", ReactivePropertyMode.DistinctUntilChanged);

        private FileSystemWatcher Watcher { get; } = new FileSystemWatcher();

        public ReactiveProperty<IEnumerable<string>> PathNamesToLoad { get; } 
            = new ReactiveProperty<IEnumerable<string>>(new List<string>());

        private ReactiveProperty<bool> CanLoad { get; } = new ReactiveProperty<bool>(false);
        private ReactiveProperty<bool> CanSave { get; } = new ReactiveProperty<bool>(false);
        
        private IList<IDisposable> Subscriptions { get; } = new List<IDisposable>();
        
        private void SetupWatcher()
        {
            ResetWatcher();
            Subscriptions.AddRange(SubscribeToWatcher(new SynchronizationContextScheduler(SynchronizationContext.Current)));
        }

        private IEnumerable<IDisposable> SubscribeToWatcher(IScheduler scheduler)
        {
            yield return Observable
                .FromEvent<FileSystemEventHandler, EventArgs>(h => Watcher.Created += h, h => Watcher.Created -= h)
                .ObserveOn(scheduler)
                .Subscribe(_ => IsLoadExecuteable());

            yield return Observable
                .FromEvent<FileSystemEventHandler, EventArgs>(h => Watcher.Deleted += h, h => Watcher.Deleted -= h)
                .ObserveOn(scheduler)
                .Subscribe(_ => IsLoadExecuteable());

            yield return Observable
                .FromEvent<RenamedEventHandler, EventArgs>(h => Watcher.Renamed += h, h => Watcher.Renamed -= h)
                .ObserveOn(scheduler)
                .Subscribe(_ => IsLoadExecuteable());
        }

        private void ResetWatcher()
        {
            Watcher.Path = Directory.Value;
            Watcher.Filter = Path.GetFileName(FileName.Value);
        }
        
        public GestureTrainingPageViewModel()
        {
            Log = Serilog.Log.Logger;

            SetupWatcher();

            SaveCommand = CanSave.ToAsyncReactiveCommand();
            LoadCommand = CanLoad.ToAsyncReactiveCommand();
            
            Subscriptions.Add(SaveCommand.Subscribe(async _ => await SaveAsync(PathSamples)));
            Subscriptions.Add(LoadCommand.Subscribe(async _ => await LoadAsync()));
            Subscriptions.AddRange(SubscribeToFileChanges());
        }

        ~GestureTrainingPageViewModel()
        {
            Dispose(false);
        }

        private IEnumerable<IDisposable> SubscribeToFileChanges()
        {
            yield return Directory.Subscribe(_ =>
            {
                CanLoad.Value = IsLoadExecuteable();
                CanSave.Value = IsSaveExecuteable();
            });
            
            yield return PathNamesToLoad.Subscribe(_ =>
            {
                CanLoad.Value = IsLoadExecuteable();
            });

            yield return FileName.Subscribe(_ =>
            {
                CanLoad.Value = IsLoadExecuteable();
            });

            yield return PathSamples.Subscribe(Observer.Create<PathSample>(_ =>
            {
                CanSave.Value = IsSaveExecuteable();
            }));
        }

        private bool IsLoadExecuteable()
        {
            return PathNamesToLoad.Value.Any() 
                && File.Exists(Path.Combine(Directory.Value, FileName.Value));
        }

        private bool IsSaveExecuteable()
        {
            return PathSamples.Any()
                && System.IO.Directory.Exists(Directory.Value);
        }

        private async Task LoadAsync()
        {
            Log.Information($"Executing {nameof(LoadTrainingSetCommand)}");

            if(PathNamesToLoad.Value == null) throw new InvalidOperationException($"{nameof(PathNamesToLoad)} was not set.");
            
            var gestures = await TrainingSetIo.ReadGestureFromBinaryAsync(FileName.Value, Log);
            
            PathSamples.Clear();

            foreach (var pathSample in gestures.ToPathSamples(PathNamesToLoad.Value))
            {
                PathSamples.Add(pathSample);
            }
        }

        private async Task SaveAsync(IEnumerable<PathSample> pathSamples)
        {
            Log.Information($"Executing {nameof(SaveTrainingSetCommand)}");

            var gestures = pathSamples.SelectMany(sample => sample.ToGesture());
            var trainingSet = new TrainingSet(gestures.ToList());
            await TrainingSetIo.WriteGestureAsBinaryAsync(trainingSet, FileName.Value);
        }

        private bool _isDisposed;
        private void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            Subscriptions.Dispose();
            SaveCommand.Dispose();
            LoadCommand.Dispose();
            Directory.Dispose();
            FileName.Dispose();
            Watcher.Dispose();
            PathNamesToLoad.Dispose();
            CanLoad.Dispose();
            CanSave.Dispose();
            
            _isDisposed = true;
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}