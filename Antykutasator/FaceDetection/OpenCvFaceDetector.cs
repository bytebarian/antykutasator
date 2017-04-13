using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Utils.Asynchronous;

namespace Antykutasator.FaceDetection
{
    public class OpenCvFaceDetector : IFaceDetector, IDisposable
    {
        private readonly CascadeClassifier _frontFaceCascadeClassifier;
        private readonly CascadeClassifier _profileCascadeClassifier;
        private readonly IMediator _mediator;

        public OpenCvFaceDetector(IMediator mediator)
        {
            _frontFaceCascadeClassifier = new CascadeClassifier("Resources/haarcascades/haarcascade_frontalface_alt_tree.xml");
            _profileCascadeClassifier = new CascadeClassifier("Resources/haarcascades/haarcascade_profileface.xml");
            _mediator = mediator;
            _mediator.RegisterAsync<Bitmap>(this, Process);
        }

        public void Process(Bitmap picture)
        {
            using (var grayframe = new Image<Gray, byte>(picture))
            {
                var result = new FaceDetectionResult
                {
                    Result = FaceDetectionResultType.FaceNotFound,
                };

                var frontfaces = _frontFaceCascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, Size.Empty);
                if (frontfaces.Length > 0)
                {
                    result.Result = FaceDetectionResultType.OneFaceFound;
                    result.Image = picture;
                    result.Face = picture.Clone(frontfaces[0], picture.PixelFormat);
                    Debug.WriteLine("front face found");
                    _mediator.SendMessageAsync(this, result, result.Result);
                    return;
                }

                var leftProfile = _profileCascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, Size.Empty);
                if (leftProfile.Length > 0)
                {
                    result.Result = FaceDetectionResultType.OneFaceFound;
                    result.Image = picture;
                    result.Face = picture.Clone(leftProfile[0], picture.PixelFormat);
                    Debug.WriteLine("left profile found");
                    _mediator.SendMessageAsync(this, result, result.Result);
                    return;
                }
                picture.RotateFlip(RotateFlipType.Rotate180FlipY);
                var rotatedFrame = new Image<Gray, byte>(picture);
                var rightProfile = _profileCascadeClassifier.DetectMultiScale(rotatedFrame, 1.1, 10, Size.Empty);
                rotatedFrame.Dispose();
                if (rightProfile.Length > 0)
                {
                    result.Result = FaceDetectionResultType.OneFaceFound;
                    result.Image = picture;
                    result.Face = picture.Clone(rightProfile[0], picture.PixelFormat);
                }

                Debug.WriteLine(result.Result == FaceDetectionResultType.FaceNotFound ? "face not found" : "right profile found");
                _mediator.SendMessageAsync(this, result, result.Result);
            }
        }

        public void Dispose()
        {
            _frontFaceCascadeClassifier.Dispose();
            _profileCascadeClassifier.Dispose();
            _mediator.UnregisterRecipientAsync(this);
        }
    }
}
