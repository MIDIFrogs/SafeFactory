using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using OpenCvSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SafeFactory.VideoCapture
{
    public static class FrameSpliter
    {
        public static (Image<Rgba32>? Preview, TimeSpan Duration) AnalyzeVideo(string videoPath)
        {
            if (videoPath == null || !File.Exists(videoPath))
            {
                return ThrowHelper.ThrowInvalidOperationException<(Image<Rgba32> Preview, TimeSpan Duration)>();
            }

            var capture = new OpenCvSharp.VideoCapture(videoPath);
            Image<Rgba32>? preview = null;
            TimeSpan duration = TimeSpan.FromSeconds(capture.FrameCount / capture.Fps);
            if (capture.IsOpened())
            {
                var image = new Mat();
                if (!capture.Read(image)) return (preview, duration);
                if (image.Empty()) return (preview, duration);
                using var stream = image.ToMemoryStream();
                stream.Position = 0;
                preview = Image.Load<Rgba32>(stream);
            }

            return (preview, duration);
        }

        public static IReadOnlyCollection<Image<Rgba32>> Split(string videoPath, double fps)
        {
            if (videoPath == null || !File.Exists(videoPath))
            {
                return ThrowHelper.ThrowInvalidOperationException<IReadOnlyCollection<Image<Rgba32>>>();
            }

            var capture = new OpenCvSharp.VideoCapture(videoPath);
            return new Wrapper(SplitInternal(capture, fps), (int)(capture.FrameCount / (capture.Fps / fps)));
        }

        private static IEnumerable<Image<Rgba32>> SplitInternal(OpenCvSharp.VideoCapture video, double fps)
        {
            var image = new Mat();
            int i = 0;
            double frameStep = video.Fps / fps;
            double nextFrame = 0;
            while (video.IsOpened())
            {
                if (i++ > nextFrame)
                {
                    nextFrame += frameStep;
                    if (!video.Read(image)) break;
                    if (image.Empty()) break;
                    using var stream = image.ToMemoryStream();
                    stream.Position = 0;
                    yield return Image.Load<Rgba32>(stream);
                }
            }
        }

        private class Wrapper : IReadOnlyCollection<Image<Rgba32>>
        {
            private IEnumerable<Image<Rgba32>> images;

            public Wrapper(IEnumerable<Image<Rgba32>> source, int count)
            {
                images = source;
                Count = count;
            }

            public int Count { get; }

            public IEnumerator<Image<Rgba32>> GetEnumerator()
            {
                return images.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return images.GetEnumerator();
            }
        }
    }
}

