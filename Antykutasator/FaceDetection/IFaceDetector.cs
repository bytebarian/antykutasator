using System.Drawing;

namespace Antykutasator.FaceDetection
{
    public interface IFaceDetector
    {
        void Process(Bitmap picture);
    }
}
