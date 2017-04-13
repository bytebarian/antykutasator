using Antykutasator.Helpers;
using Appccelerate.StateMachine;

namespace Antykutasator
{
    public class ApplicationStateMachine
    {
        private readonly PassiveStateMachine<ApplicationState, ApplicationStateEvent> _fsm =
            new PassiveStateMachine<ApplicationState, ApplicationStateEvent>();

        public ApplicationStateMachine(ApplicationCommands commands)
        {
            // in uninitialized state
            _fsm.In(ApplicationState.Uninitialized)
                .On(ApplicationStateEvent.ChangeVideoDevice)
                .Goto(ApplicationState.Uninitialized);
            _fsm.In(ApplicationState.Uninitialized)
                .On(ApplicationStateEvent.Start)
                .Goto(ApplicationState.Running)
                .Execute(commands.StartFaceDetection);
            _fsm.In(ApplicationState.Uninitialized)
                .On(ApplicationStateEvent.Stop)
                .Goto(ApplicationState.Uninitialized);
            // in running state
            _fsm.In(ApplicationState.Running)
                .On(ApplicationStateEvent.ChangeVideoDevice)
                .Goto(ApplicationState.Running)
                .Execute(commands.ChangeVideoCaptureDevice);
            _fsm.In(ApplicationState.Running)
                .On(ApplicationStateEvent.Stop)
                .Goto(ApplicationState.Terminated)
                .Execute(commands.StopFaceDetection);
            _fsm.In(ApplicationState.Running)
                .On(ApplicationStateEvent.Start)
                .Goto(ApplicationState.Running);
            // in terminated state
            _fsm.In(ApplicationState.Terminated)
                .On(ApplicationStateEvent.ChangeVideoDevice)
                .Goto(ApplicationState.Terminated);
            _fsm.In(ApplicationState.Terminated)
                .On(ApplicationStateEvent.Stop)
                .Goto(ApplicationState.Terminated);
            _fsm.In(ApplicationState.Terminated)
                .On(ApplicationStateEvent.Start)
                .Goto(ApplicationState.Running)
                .Execute(commands.StartFaceDetection);

            _fsm.Initialize(ApplicationState.Uninitialized);
            _fsm.Start();
        }

        public void Fire(ApplicationStateEvent appEvent)
        {
            _fsm.Fire(appEvent);
        }
    }
}
