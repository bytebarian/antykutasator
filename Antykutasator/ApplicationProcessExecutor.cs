using System.Threading;
using Antykutasator.Helpers;
using Utils.Asynchronous;

namespace Antykutasator
{
    public class ApplicationProcessExecutor
    {
        private readonly AsyncExecutor<ApplicationStateMessage> _process;
        private readonly ManualResetEvent _stopEvent;

        public ApplicationProcessExecutor(ApplicationProcess process)
        {
            _stopEvent = new ManualResetEvent(false);
            _process = new AsyncExecutor<ApplicationStateMessage>(process, _stopEvent, 1);
        }

        public void Start()
        {
            _process.Start();
            _process.Enqueue(new ApplicationStateMessage(ApplicationStateEvent.Start, null));
        }

        public void Stop()
        {
            _process.Enqueue(new ApplicationStateMessage(ApplicationStateEvent.Stop, null));
            _stopEvent.Set();
            _process.Stop();
        }

        public void Enqueue(ApplicationStateMessage msg)
        {
            _process.Enqueue(msg);
        }
    }
}
