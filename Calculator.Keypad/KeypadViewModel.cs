using System.Reactive.Subjects;
using System.Windows.Input;
using Calculator.Controls;
using Calculator.Messages;
using Calculator.Messages.Events;
using FunctionalSharp.OptionTypes;
using FunctionalSharp.PatternMatching;
using MaterialDesignThemes.Wpf;
using Command = Calculator.Controls.Command;

namespace Calculator.Keypad
{
    public sealed class KeypadViewModel
    {
        private ISubject<Event, Event> ReplaySubject { get; }
        public ICommand KeyUpCommand => new Command<KeyEventArgs>(OnKeyUp);
        public ICommand OpenTrigDialogCommand => new Command(OpenTrigDialog);

        public KeypadViewModel()
        {
            ReplaySubject = new ReplaySubject<Event>();
            MessageBus.Events.Publish(ReplaySubject);
        }
        
        private void OnKeyUp(KeyEventArgs args)
        {
            if(args == null) return;

            MapKeyToEvent(args.Key)
                .Match(value => ReplaySubject.OnNext(value), () => {});
        }

        private static IOption<Event> MapKeyToEvent(Key key)
        {
            return key.Match()
                .With(Key.Left, new CursorLeftEvent().ToOption<Event>())
                .With(Key.Right, new CursorRightEvent().ToOption<Event>())
                .With(Key.Up, new CursorUpEvent().ToOption<Event>())
                .With(Key.Down, new CursorDownEvent().ToOption<Event>())
                .Else(new None<Event>());
        }
        
        private async void OpenTrigDialog(object o)
        {
            // TODO: use DI here
            var view = new TrigDialog
            {
                DataContext = new TrigDialogViewModel()
            };
            
            var result = (string)await DialogHost.Show(view, "RootDialog");

            var @event = MapTrigDialogResultToEvent(result);
            ReplaySubject.OnNext(@event);
        }

        private static Event MapTrigDialogResultToEvent(string result)
        {
            return result.Match()
                .With<Event>("sin", new SinEvent())
                .With("cos", new CosEvent())
                .With("tan", new TanEvent())
                .With("asin", new AsinEvent())
                .With("acos", new AcosEvent())
                .With("atan", new AtanEvent())
                .With("csc", new CscEvent())
                .With("sec", new SecEvent())
                .With("cot", new CotEvent())
                .With("acsc", new AcscEvent())
                .With("asec", new AsecEvent())
                .With("acot", new AcotEvent())
                .ElseException();
        }
    }
}