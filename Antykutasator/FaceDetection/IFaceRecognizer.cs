using System.Drawing;

namespace Antykutasator.FaceDetection
{
    public interface IFaceRecognizer
    {
        void Process(FaceDetectionResult result);
    }
}
