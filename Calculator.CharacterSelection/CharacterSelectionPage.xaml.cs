using System;
using Serilog;

namespace Calculator.CharacterSelection
{
    public partial class CharacterSelectionPage : IDisposable
    {
        private static ILogger Log => Serilog.Log.ForContext<CharacterSelectionPage>();

        public CharacterSelectionPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }

            Dispatcher.ShutdownStarted += DispatcherOnShutdownStarted;
        }
        
        private void DispatcherOnShutdownStarted(object sender, EventArgs eventArgs)
        {
            Dispose();
        }

        #region IDisposable
        ~CharacterSelectionPage()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private bool _isDisposed;
        public void Dispose(bool disposing)
        {
            if(_isDisposed) return;

            if (DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            _isDisposed = true;
        }
        #endregion
    }
}