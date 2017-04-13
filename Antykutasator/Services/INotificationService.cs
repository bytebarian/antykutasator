namespace Antykutasator.Services
{
    public interface INotificationService
    {
        void SendMessage(string msg);

        void CloseMessage();
    }
}
