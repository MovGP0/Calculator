using System;
using System.Windows;
using Calculator.DependencyInjection;
using DryIoc;
using Serilog;

namespace Calculator
{
    public sealed partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var resolver = ResolverFactory.Get();

            var logger = resolver.Resolve<ILogger>();
            RegisterGlobalExceptionHandling(logger);
            
            var shellWindow = resolver.Resolve<Pages.ShellWindow>();
            shellWindow.Show();
        }
        
        private static void RegisterGlobalExceptionHandling(ILogger log)
        {
            AppDomain.CurrentDomain.UnhandledException += 
                (sender, args) => CurrentDomainOnUnhandledException(args, log);
        }
        
        private static void CurrentDomainOnUnhandledException(UnhandledExceptionEventArgs args, ILogger log)
        {
            var exception = args.ExceptionObject as Exception;
            var terminatingMessage = args.IsTerminating ? " The application is terminating." : string.Empty;
            var exceptionMessage = exception?.Message ?? "An unmanaged exception occured.";
            var message = string.Concat(exceptionMessage, terminatingMessage);
            log.Error(exception, message);
        }
    }
}
