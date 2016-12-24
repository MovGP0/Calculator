using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
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
            Log.Logger = resolver.Resolve<ILogger>();

            Log.ForContext<App>().Information("Application starting");
            RegisterGlobalExceptionHandling();
            
            var shellWindow = resolver.Resolve<ShellWindow>();
            shellWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.ForContext<App>().Information("Application exiting");
        }

        private static void RegisterGlobalExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }
        
        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var exception = args.ExceptionObject as Exception;
            var terminatingMessage = args.IsTerminating ? " The application is terminating." : string.Empty;
            var exceptionMessage = exception?.Message ?? "An unmanaged exception occured.";
            var message = string.Concat(exceptionMessage, terminatingMessage);
            Log.ForContext<App>().Error(exception, message);
        }
    }
}
