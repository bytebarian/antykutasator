using System;
using System.Configuration;
using AForge.Video.DirectShow;

namespace Antykutasator.Helpers
{
    public class ApplicationConfiguration
    {
        private readonly int _frameCaptureInterval;
        private readonly int _faceNotDetectedLimit;
        private readonly TimeSpan _deviceInactivityLimit;
        private readonly TimeSpan _commandDelayInterval;
        private readonly string _slackToken;
        private readonly string _channelId;

        public FilterInfo SelectedVideoCaptureDevice { get; set; }

        public bool ScreenLocked { get; set; }

        public int FrameCaptureInterval
        {
            get
            {
                return _frameCaptureInterval;
            }
        }

        public int FaceNotDetectedLimit
        {
            get
            {
                return _faceNotDetectedLimit;
            }
        }

        public TimeSpan DeviceInactivityLimit
        {
            get
            {
                return _deviceInactivityLimit;
            }
        }

        public TimeSpan CommandDelayInterval
        {
            get
            {
                return _commandDelayInterval;
            }
        }

        public string SlackToken
        {
            get
            {
                return _slackToken;
            }
        }

        public string ChannelId
        {
            get
            {
                return _channelId;
            }
        }

        public ApplicationConfiguration()
        {
            _frameCaptureInterval = int.Parse(ConfigurationManager.AppSettings.Get("frameCaptureInterval"));
            _faceNotDetectedLimit = int.Parse(ConfigurationManager.AppSettings.Get("faceNotDetectedLimit"));
            _deviceInactivityLimit = TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings.Get("deviceInactivityLimit")));
            _commandDelayInterval = TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings.Get("commandDelayInterval")));
            _slackToken = ConfigurationManager.AppSettings.Get("slackToken");
            _channelId = ConfigurationManager.AppSettings.Get("chanelId");
        }
    }
}
