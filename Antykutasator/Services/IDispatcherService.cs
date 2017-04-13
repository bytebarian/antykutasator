using System;
using System.Windows.Threading;

namespace Antykutasator.Services
{
    public interface IDispatcherService
    {
        void Invoke(Action action);
        Dispatcher GetDispatcher();
        event EventHandler<Exception> UnhandledException;
    }
}
