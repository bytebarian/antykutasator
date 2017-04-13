using System.Collections.Generic;
using System.Linq;
using AForge.Video.DirectShow;

namespace Antykutasator.VideoCapture
{
    public class VideoCaptureDeviceCollection
    {
        private readonly FilterInfoCollection _videoDevices;

        public VideoCaptureDeviceCollection()
        {
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }

        public IEnumerable<FilterInfo> GetAllDevices() => _videoDevices.Cast<FilterInfo>().ToList();

        public string GetDefaultDeviceName()
            => _videoDevices != null && _videoDevices.Count > 0 ? _videoDevices[0].MonikerString : null;
    }
}
