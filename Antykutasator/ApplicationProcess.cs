using System;
using System.Diagnostics;
using Antykutasator.Helpers;
using Antykutasator.Services;
using Utils.Asynchronous;

namespace Antykutasator
{
    public class ApplicationProcess : IAsyncExecuteReady<ApplicationStateMessage>
    {
        private readonly ApplicationStateMachine _stateMachine;

        public ApplicationProcess(IDispatcherService dispatcherService,
            ApplicationStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            dispatcherService.UnhandledException += DispatcherService_UnhandledException;
        }

        public void Execute(ApplicationStateMessage item)
        {
            _stateMachine.Fire(item.Event);
        }

        public void HandleException(Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }

        private void DispatcherService_UnhandledException(object sender, Exception e)
        {
            HandleException(e);
        }
    }
}
