using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Linq;
using AForge.Video;
using AForge.Video.DirectShow;
using Antykutasator.Helpers;
using Utils.Asynchronous;

namespace Antykutasator.VideoCapture
{
    public class VideoCaptureService : IVideoCaptureService, IDisposable
    {
        private readonly VideoCaptureDeviceCollection _videoDevices;
        private readonly ApplicationConfiguration _applicationConfiguration;
        private readonly IMediator _mediator;
        private VideoCaptureDevice _videoSource;
        private DateTime _lastCaptureTime;
        private IDisposable _videoCapturePipeline;


        public VideoCaptureService(VideoCaptureDeviceCollection captureDeviceCollection,
            ApplicationConfiguration applicationConfiguration,
            IMediator mediator)
        {
            _lastCaptureTime = DateTime.Now;
            // enumerate video devices
            _videoDevices = captureDeviceCollection;
            _applicationConfiguration = applicationConfiguration;
            _mediator = mediator;
        }

        public void Start()
        {
            Initialize();

            // start the video source
            _videoSource.Start();
        }

        private void Initialize()
        {
            if (_videoSource == null)
            {
                // create video source
                _videoSource = _applicationConfiguration.SelectedVideoCaptureDevice == null
                    ? new VideoCaptureDevice(_videoDevices.GetDefaultDeviceName())
                    : new VideoCaptureDevice(_applicationConfiguration.SelectedVideoCaptureDevice.MonikerString);
            }

            if (_videoCapturePipeline == null)
            {
                _videoCapturePipeline = Observable.FromEvent<NewFrameEventHandler, NewFrameEventArgs>(
                        d =>
                        {
                            NewFrameEventHandler nfeHandler = (sender, e) => d(e);
                            return nfeHandler;
                        }, h => _videoSource.NewFrame += h, h => _videoSource.NewFrame -= h)
                    .Select(e => e.Frame)
                    .Subscribe(frame =>
                    {
                        if (DateTime.Now - _lastCaptureTime <
                            TimeSpan.FromSeconds(_applicationConfiguration.FrameCaptureInterval)) return;

                        _mediator.SendMessageAsync(this,
                            frame.Clone(new Rectangle(0, 0, frame.Width, frame.Height), frame.PixelFormat));

                        _lastCaptureTime = DateTime.Now;
                    }, exception => Debug.Write(exception));
            }
        }

        public void Stop()
        {
            _videoSource?.SignalToStop();
        }

        public void Dispose()
        {
            Stop();
            _videoCapturePipeline.Dispose();
        }
    }
}
