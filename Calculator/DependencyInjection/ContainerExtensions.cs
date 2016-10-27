using Calculator.Keypad;
using Calculator.Messages;
using DryIoc;
using MemBus;
using MemBus.Configurators;

namespace Calculator.DependencyInjection
{
    public static class ContainerExtensions
    {
        public static IContainer SetupKeypad(this IContainer container)
        {
            container.RegisterDelegate(r => new KeypadViewModel(r.Resolve<IEventBus>()), Reuse.Transient);
            return container;
        }

        public static IContainer SetupMessageBus(this IContainer resolver)
        {
            resolver.RegisterDelegate(resoler => BusSetup.StartWith<Conservative>().Construct(), Reuse.Singleton, serviceKey: BusTypes.CommandBus);
            resolver.RegisterDelegate(resoler => BusSetup.StartWith<Fast>().Construct(), Reuse.Singleton, serviceKey: BusTypes.EventBus);
            resolver.RegisterDelegate<ICommandBus>(r => new CommandBus(r.Resolve<IBus>(BusTypes.CommandBus)), Reuse.Singleton);
            resolver.RegisterDelegate<IEventBus>(r => new EventBus(r.Resolve<IBus>(BusTypes.EventBus)), Reuse.Singleton);
            return resolver;
        }

        public static IContainer SetupPages(this IContainer resolver)
        {
            resolver.Register<Pages.MainWindow>();
            return resolver;
        }
    }
}