using Antykutasator.VideoCapture;

namespace Antykutasator
{
    public class ApplicationCommands
    {
        private readonly IVideoCaptureService _videoCaptureService;

        public ApplicationCommands(IVideoCaptureService videoCaptureService)
        {
            _videoCaptureService = videoCaptureService;
        }

        public void StartFaceDetection()
        {
            _videoCaptureService.Start();
        }

        public void StopFaceDetection()
        {
            _videoCaptureService.Stop();
        }

        public void ChangeVideoCaptureDevice()
        {
            _videoCaptureService.Stop();
            _videoCaptureService.Start();
        }
    }
}
