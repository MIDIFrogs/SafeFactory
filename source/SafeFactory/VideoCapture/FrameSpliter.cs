using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using OpenCvSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SafeFactory.VideoCapture
{
    public static class FrameSpliter
    {
        public static IEnumerable<Image<Rgba32>> Split(string videoPath, double fps)
        {
            if (videoPath == null || !File.Exists(videoPath))
            {
                throw new FileNotFoundException();
            }

            var capture = new OpenCvSharp.VideoCapture(videoPath);
            var image = new Mat();
            int i = 0;
            double frameStep = capture.Fps / fps;
            double nextFrame = 0;
            while (capture.IsOpened())
            {
                if (i++ > nextFrame)
                {
                    nextFrame += frameStep;
                    if (!capture.Read(image)) break;
                    if (image.Empty()) break;
                    using var stream = image.ToMemoryStream();
                    yield return Image.Load<Rgba32>(stream);
                }
            } 
        }
    }
}

