using System;
using Calculator.Keypad;
using Calculator.Messages;
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
            container.Register<T>(Reuse.Transient);
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
            container.Register<Pages.MainWindow>();
            return container;
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