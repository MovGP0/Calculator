using System;
using Reactive.Bindings;
using System.Reactive.Disposables;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace Calculator.CharacterSelection
{
    public sealed class CharacterSelectionPageViewModel : IDisposable
    {
        private ILogger Log => Serilog.Log.ForContext<CharacterSelectionPageViewModel>();
        public IEnumerable<Encoding> Codepages => CodepageHelper.Codepages;
        public ReactiveProperty<Encoding> CurrentCodepage { get; } = new ReactiveProperty<Encoding>(Encoding.Default);
        public ReactiveCollection<string> Characters { get; } = new ReactiveCollection<string>();
        
        private CompositeDisposable Subscriptions { get; } = new CompositeDisposable();

        public CharacterSelectionPageViewModel()
        {
            Subscriptions.Add(CurrentCodepage.Subscribe(UpdateCharacters, ex => Log.Error(ex, ex.Message)));
        }

        private void UpdateCharacters(Encoding codepage)
        {
            Characters.Clear();
            var characters = CodepageHelper.GetCharactersInCodepage(codepage).Select(c => c.ToString());
            Characters.AddRangeOnScheduler(characters);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~CharacterSelectionPageViewModel()
        {
            Dispose(false);
        }

        private bool _isDisposed;

        public void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            Characters.Dispose();
            CurrentCodepage.Dispose();
            Subscriptions.Dispose();

            _isDisposed = true;
            if (!disposing)
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}