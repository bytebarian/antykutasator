using System.Threading.Tasks;
using Antykutasator.FaceDetection;

namespace Antykutasator.Services
{
    public interface IScreenCaptureService
    {
        Task SetLockScreen(FaceRecognitionResult result);
    }
}
