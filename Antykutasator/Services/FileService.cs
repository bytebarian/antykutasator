using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Antykutasator.Services
{
    public class FileService
    {
        public string SaveImage(Bitmap bmp)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var folderPath = Path.GetDirectoryName(dir);
            var fileName = DateTime.Now.Ticks + "_camera_capture.jpeg";
            if (folderPath == null) return null;
            var fullPath = Path.Combine(folderPath, fileName);

            bmp.Save(fullPath, ImageFormat.Jpeg);

            return fullPath;
        }
    }
}
