using System.Drawing;

namespace Antykutasator.FaceDetection
{
    public class FaceRecognitionResult
    {
        public string Label { get; set; }
        public Bitmap Image { get; set; }
        public FaceRecognitionResultType Result { get; set; }
    }

    public enum FaceRecognitionResultType
    {
        Unrecognized,
        Recognized
    }
}
