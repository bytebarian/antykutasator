using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using Antykutasator.FaceDetection;
using Antykutasator.Helpers;
using Microsoft.Win32;
using Utils.Asynchronous;

namespace Antykutasator.Services
{
    public class LockingService : ILockingService, IDisposable
    {
        private readonly IMouseService _mouseService;
        private readonly IKeyboardService _keyboardService;
        private readonly INotificationService _notificationService;
        private readonly IDispatcherService _dispatcherService;
        private readonly ApplicationConfiguration _applicationConfiguration;
        private readonly IMediator _mediator;
        private int _faceNotDetectedInRow;

        public LockingService(IMediator mediator,
            IMouseService mouseService,
            IKeyboardService keyboardService,
            INotificationService notificationService,
            IDispatcherService dispatcherService,
            ApplicationConfiguration applicationConfiguration)
        {
            _mediator = mediator;
            _mediator.RegisterAsync<FaceDetectionResult>(this, FaceDetectionResultHandler);
            _mouseService = mouseService;
            _keyboardService = keyboardService;
            _notificationService = notificationService;
            _dispatcherService = dispatcherService;

            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            _applicationConfiguration = applicationConfiguration;
        }

        public void LockWorkstation()
        {
            if (DateTime.Now - _mouseService.LastDeviceActivityTime < _applicationConfiguration.CommandDelayInterval ||
                DateTime.Now - _keyboardService.LastDeviceActivityTime < _applicationConfiguration.CommandDelayInterval) return;

            _notificationService.CloseMessage();

            NativeMethodsHelper.LockWorkStation();
            _mediator.SendMessageAsync(this, ApplicationStateEvent.LockScreen);
        }

        private void FaceDetectionResultHandler(FaceDetectionResult args)
        {
            if (_applicationConfiguration.ScreenLocked) return;
            if (args.Result == FaceDetectionResultType.FaceNotFound)
            {
                _faceNotDetectedInRow++;
            }
            else
            {
                _faceNotDetectedInRow = 0;
            }

            if (_faceNotDetectedInRow < _applicationConfiguration.FaceNotDetectedLimit)
            {
                return;
            }
            if (DateTime.Now - _mouseService.LastDeviceActivityTime < _applicationConfiguration.DeviceInactivityLimit ||
                DateTime.Now - _keyboardService.LastDeviceActivityTime < _applicationConfiguration.DeviceInactivityLimit) return;

            _faceNotDetectedInRow = 0;

            NewThreadScheduler.Default.Schedule(_applicationConfiguration.CommandDelayInterval,
                () => _dispatcherService.Invoke(LockWorkstation));

            _notificationService.SendMessage("Screen will be locked within 5 seconds. Press any key or move mouse");
            
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                _applicationConfiguration.ScreenLocked = true;
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                _applicationConfiguration.ScreenLocked = false;
            }
        }

        public void Dispose()
        {
            _mediator.UnregisterRecipientAsync(this);
        }
    }
}
