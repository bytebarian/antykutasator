using System;
using Emgu.CV;
using Emgu.CV.Structure;
using Utils.Asynchronous;

namespace Antykutasator.FaceDetection
{
    public class OpenCvFaceReconizer : IFaceRecognizer, IDisposable
    {
        private readonly IMediator _mediator;
        private readonly OpenCvFaceClassifier _classifier;

        public OpenCvFaceReconizer(IMediator mediator)
        {
            _mediator = mediator;
            //_mediator.RegisterAsync<FaceDetectionResult>(this, Process, null, FaceDetectionResultType.OneFaceFound);
            _classifier = new OpenCvFaceClassifier();
        }

        public void Process(FaceDetectionResult result)
        {
            if (!_classifier.IsTrained)
            {
                _classifier.Retrain();
            }

            using (var grayframe = new Image<Gray, byte>(result.Face).Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic))
            {
                if (_classifier.Recognise(grayframe))
                {
                    _mediator.SendMessageAsync(this, new FaceRecognitionResult
                    {
                        Label = _classifier.GetEigenLabel,
                        Image = result.Image,
                        Result = FaceRecognitionResultType.Recognized
                    }, FaceRecognitionResultType.Recognized);
                }
                else
                {
                    _mediator.SendMessageAsync(this,
                        new FaceRecognitionResult
                        {
                            Image = result.Image,
                            Result = FaceRecognitionResultType.Unrecognized
                        }, FaceRecognitionResultType.Unrecognized);
                }
            }
        }

        public void Dispose()
        {
            _mediator.UnregisterRecipientAsync(this);
        }
    }
}
