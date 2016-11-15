using System;
using Calculator.GestureRecognizer;
using Calculator.Keypad;
using Calculator.Messages;
using Calculator.Pages;
using DryIoc;
using MemBus;
using MemBus.Configurators;
using Serilog;

namespace Calculator.DependencyInjection
{
    public static class ContainerExtensions
    {
        public static IRegistrator RegisterFactory<T>(this IRegistrator container)
        {
            container.Register<T>();
            container.RegisterDelegate<Func<T>>(r => () => r.Resolve<T>());
            return container;
        }

        public static IContainer SetupKeypad(this IContainer container)
        {
            container.RegisterFactory<TrigDialog>();
            container.RegisterFactory<TrigDialogViewModel>();
            container.RegisterFactory<SetsDialog>();

            container.Register(Made.Of(() => new KeypadViewModel(
                Arg.Of<IEventBus>(), 
                Arg.Of<Func<TrigDialog>>(), 
                Arg.Of<Func<TrigDialogViewModel>>(), 
                Arg.Of<Func<SetsDialog>>())
                ), Reuse.Transient);

            return container;
        }

        public static IContainer SetupMessageBus(this IContainer container)
        {
            container.RegisterDelegate(resoler => BusSetup.StartWith<Conservative>().Construct(), Reuse.Singleton, serviceKey: BusTypes.CommandBus);
            container.RegisterDelegate(resoler => BusSetup.StartWith<Fast>().Construct(), Reuse.Singleton, serviceKey: BusTypes.EventBus);
            container.RegisterDelegate<ICommandBus>(r => new CommandBus(r.Resolve<IBus>(BusTypes.CommandBus)), Reuse.Singleton);
            container.RegisterDelegate<IEventBus>(r => new EventBus(r.Resolve<IBus>(BusTypes.EventBus)), Reuse.Singleton);
            return container;
        }

        public static IContainer SetupPages(this IContainer container)
        {
            container.Register<ShellWindow>();
            SetupGestureTrainingPage(container);
            SetupMainPage(container);
            return container;
        }

        private static void SetupMainPage(IRegistrator container)
        {
            container.RegisterFactory<MainPage>();
            container.Register<NavigateToTrainCommand>(Reuse.Transient);
        }

        private static void SetupGestureTrainingPage(IRegistrator container)
        {
            container.Register<GestureRecognizerViewModel>(Reuse.Transient, setup: Setup.With(allowDisposableTransient: true));
            container.Register<GestureRecognizer.GestureRecognizer>(Reuse.Transient, setup: Setup.With(allowDisposableTransient: true));

            container.RegisterDelegate(r =>
                {
                    var saveTrainingSetCommandFactory = r.Resolve<Func<GestureTrainingPageViewModel, SaveTrainingSetCommand>>();
                    var loadTrainingSetCommandFactory = r.Resolve<Func<GestureTrainingPageViewModel, LoadTrainingSetCommand>>();

                    var viewModel = new GestureTrainingPageViewModel();
                    viewModel.SaveCommand = saveTrainingSetCommandFactory(viewModel);
                    viewModel.LoadCommand = loadTrainingSetCommandFactory(viewModel);
                    return viewModel;
                }, Reuse.Singleton);

            container.RegisterFactory<GestureTrainingPage>();

            container.RegisterDelegate<Func<GestureTrainingPageViewModel, LoadTrainingSetCommand>>(r =>
                    viewModel => new LoadTrainingSetCommand(viewModel, r.Resolve<ILogger>()), Reuse.Singleton);

            container.RegisterDelegate<Func<GestureTrainingPageViewModel, SaveTrainingSetCommand>>(r =>
                    viewModel => new SaveTrainingSetCommand(viewModel, r.Resolve<ILogger>()), Reuse.Singleton);
        }

        public static IContainer SetupLogging(this IContainer container)
        {
            container.RegisterDelegate(_ => SetupLogger(), Reuse.Singleton);
            return container;
        }

        private static ILogger SetupLogger()
        {
            return new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .CreateLogger();
        }
    }
}