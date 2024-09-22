using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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
        private Image<Rgba32> capturedFrame;

        [JsonIgnore]
        public Image<Rgba32> CapturedFrame
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
