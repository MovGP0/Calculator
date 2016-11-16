using System;
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

            Log.Information("Application starting");
            RegisterGlobalExceptionHandling();
            
            var shellWindow = resolver.Resolve<Pages.ShellWindow>();
            shellWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Application exiting");
        }

        private static void RegisterGlobalExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Current.DispatcherUnhandledException += ApplicationOnDispatcherUnhandledException;
        }

        private static void ApplicationOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            Log.Error(args.Exception, args.Exception.Message);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var exception = args.ExceptionObject as Exception;
            var terminatingMessage = args.IsTerminating ? " The application is terminating." : string.Empty;
            var exceptionMessage = exception?.Message ?? "An unmanaged exception occured.";
            var message = string.Concat(exceptionMessage, terminatingMessage);
            Log.Error(exception, message);
        }
    }
}
