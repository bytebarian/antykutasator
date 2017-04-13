using System.Drawing;

namespace Antykutasator.FaceDetection
{
    public class FaceDetectionResult
    {
        public FaceDetectionResultType Result { get; set; }
        public Bitmap Image { get; set; }
        public Bitmap Face { get; set; }
    }

    public enum FaceDetectionResultType
    {
        FaceNotFound,
        OneFaceFound,
        MultipleFacesFound,
        Error
    }
}
