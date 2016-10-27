using System;
using DryIoc;

namespace Calculator.DependencyInjection
{
    public static class ResourceLocator
    {
        public static IResolver Get()
        {
            return Resover.Value;
        }

        private static readonly Lazy<IResolver> Resover = new Lazy<IResolver>(Setup);

        private static IResolver Setup()
        {
            return new Container()
                .SetupKeypad()
                .SetupMessageBus();
        }
    }
}
