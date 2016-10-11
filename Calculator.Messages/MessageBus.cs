using MemBus;
using MemBus.Configurators;

namespace Calculator.Messages
{
    public static class MessageBus
    {
        public static IBus Events { get; } = BusSetup.StartWith<Fast>().Construct();
        public static IBus Commands { get; } = BusSetup.StartWith<Conservative>().Construct();
    }
}
