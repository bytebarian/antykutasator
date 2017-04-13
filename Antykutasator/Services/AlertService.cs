using System;
using Antykutasator.FaceDetection;
using Utils.Asynchronous;
using Antykutasator.Helpers;

namespace Antykutasator.Services
{
    public class AlertService : IDisposable
    {
        private readonly IMediator _mediator;
        private readonly FileService _fileService;
        private readonly SlackClient _slackClient;
        private readonly ApplicationConfiguration _config;

        private int _faceDetectedInRow;
        private bool _alertSended;

        public AlertService(IMediator mediator,
            FileService fileService,
            SlackClient slackClient,
            ApplicationConfiguration config)
        {
            _mediator = mediator;
            _fileService = fileService;
            _slackClient = slackClient;
            _config = config;
            //_mediator.RegisterAsync<FaceRecognitionResult>(this, Process, null,
            //    FaceRecognitionResultType.Unrecognized);
            _mediator.RegisterAsync<FaceDetectionResult>(this, Process);

        }

        //private void Process(FaceRecognitionResult obj)
        //{
        //    var filePath = _fileService.SaveImage(obj.Image);
        //    _slackClient.SlackSendFile(filePath);
        //}

        private void Process(FaceDetectionResult obj)
        {
            if (!_config.ScreenLocked || _alertSended) return;

            if (obj.Result == FaceDetectionResultType.OneFaceFound)
            {
                _faceDetectedInRow++;
            }
            else
            {
                _faceDetectedInRow = 0;
            }

            if (_faceDetectedInRow < 2)
            {
                return;
            }

            var filePath = _fileService.SaveImage(obj.Image);
            _slackClient.SlackSendFile(filePath);
            _alertSended = true;
        }

        public void Dispose()
        {
            _mediator.UnregisterRecipientAsync(this);
        }
    }
}
