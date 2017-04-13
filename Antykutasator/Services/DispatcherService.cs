using System;
using System.Windows;
using System.Windows.Threading;

namespace Antykutasator.Services
{
    public class DispatcherService : IDispatcherService
    {
        private readonly Dispatcher _dispatcher;

        public DispatcherService()
        {
            _dispatcher = Application.Current.Dispatcher;
            _dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            UnhandledException?.Invoke(sender, e.Exception);
        }

        public void Invoke(Action action)
        {
            try
            {
                _dispatcher.Invoke(action);
            }
            catch (Exception e)
            {
                UnhandledException?.Invoke(this, e);
            }
        }

        public event EventHandler<Exception> UnhandledException;
        public Dispatcher GetDispatcher()
        {
            return _dispatcher;
        }
    }
}
