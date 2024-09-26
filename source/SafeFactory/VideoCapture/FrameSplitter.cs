// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using CommunityToolkit.Diagnostics;
using OpenCvSharp;
using SkiaSharp;
using System.Collections;

namespace SafeFactory.VideoCapture
{
    public static class FrameSplitter
    {
        public static (SKBitmap? Preview, TimeSpan Duration) AnalyzeVideo(string videoPath)
        {
            if (videoPath == null || !File.Exists(videoPath))
            {
                return ThrowHelper.ThrowInvalidOperationException<(SKBitmap Preview, TimeSpan Duration)>();
            }

            var capture = new OpenCvSharp.VideoCapture(videoPath);
            SKBitmap? preview = null;
            TimeSpan duration = TimeSpan.FromSeconds(capture.FrameCount / capture.Fps);
            if (capture.IsOpened())
            {
                var image = new Mat();
                if (!capture.Read(image)) return (preview, duration);
                if (image.Empty()) return (preview, duration);
                using var stream = image.ToMemoryStream();
                stream.Position = 0;
                preview = SKBitmap.Decode(stream);
            }

            return (preview, duration);
        }

        public static IReadOnlyCollection<SKBitmap> Split(string videoPath, double fps)
        {
            if (videoPath == null || !File.Exists(videoPath))
            {
                return ThrowHelper.ThrowInvalidOperationException<IReadOnlyCollection<SKBitmap>>();
            }

            var capture = new OpenCvSharp.VideoCapture(videoPath);
            return new Wrapper(SplitInternal(capture, fps), (int)(capture.FrameCount / (capture.Fps / fps)));
        }

        private static IEnumerable<SKBitmap> SplitInternal(OpenCvSharp.VideoCapture video, double fps)
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
                    yield return SKBitmap.Decode(stream);
                }
            }
        }

        private class Wrapper : IReadOnlyCollection<SKBitmap>
        {
            private IEnumerable<SKBitmap> images;

            public Wrapper(IEnumerable<SKBitmap> source, int count)
            {
                images = source;
                Count = count;
            }

            public int Count { get; }

            public IEnumerator<SKBitmap> GetEnumerator()
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