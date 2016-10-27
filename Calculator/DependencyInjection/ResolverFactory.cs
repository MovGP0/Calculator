using System;
using DryIoc;

namespace Calculator.DependencyInjection
{
    public static class ResolverFactory
    {
        public static IResolver Get()
        {
            return Resover.Value;
        }

        private static readonly Lazy<IResolver> Resover = new Lazy<IResolver>(Setup);

        private static IResolver Setup()
        {
            return new Container()
                .SetupLogging()
                .SetupKeypad()
                .SetupMessageBus()
                .SetupPages();
        }
    }
}
