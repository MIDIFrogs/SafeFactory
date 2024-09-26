// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using SkiaSharp;

namespace SafeFactory.Projects
{
    public enum ReportType
    {
        OpenedDoor,
        Helmet,
        StepOver,
    }

    public record ViolationReport(ReportType Type, TimeSpan BeginTimestamp, TimeSpan EndTimestamp)
    {
        private SKBitmap capturedFrame;

        [JsonIgnore]
        public SKBitmap CapturedFrame
        {
            get => capturedFrame;
            set
            {
                capturedFrame = value;
                CapturedBitmap = capturedFrame.ConvertToBitmap();
            }
        }

        [JsonIgnore]
        public Bitmap CapturedBitmap { get; private set; }
    }
}