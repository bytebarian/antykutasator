using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Antykutasator.FaceDetection;
using Utils.Asynchronous;

namespace Antykutasator.Services
{
    public class ScreenCaptureService : IScreenCaptureService
    {
        private readonly IMediator _mediator;

        public ScreenCaptureService(IMediator mediator)
        {
            _mediator = mediator;
            _mediator.RegisterAsync<FaceRecognitionResult>(this, async (_) => await SetLockScreen(_), null,
                FaceRecognitionResultType.Unrecognized);
        }

        public async Task SetLockScreen(FaceRecognitionResult result)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var folderPath = Path.GetDirectoryName(dir);
            var fileName = DateTime.Now.Ticks + "_camera_capture.jpeg";
            if (folderPath == null) return;
            var fullPath = Path.Combine(folderPath, fileName);

            result.Image.Save(fullPath, ImageFormat.Jpeg);
        }
    }
}
