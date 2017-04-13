using Hardcodet.Wpf.TaskbarNotification;

namespace Antykutasator.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IDispatcherService _dispatcherService;
        //private TaskbarIcon toast;

        public NotificationService(IDispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }

        public void SendMessage(string msg)
        {
            _dispatcherService.Invoke(() =>
            {
                //if(toast != null)
                //{
                //    CloseMessage();
                //}
                var toast = new TaskbarIcon();
                toast.ShowBalloonTip("Antykutasator", msg, BalloonIcon.None);
            });           
        }

        public void CloseMessage()
        {
            //if(toast == null) return;
            //_dispatcherService.Invoke(() =>
            //{
            //    toast.HideBalloonTip();
            //    toast.Dispose();
            //});           
        }
    }
}
