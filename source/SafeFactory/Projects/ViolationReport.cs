using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [JsonIgnore]
        public Image<Rgba32> CapturedFrame { get; set; }
    }
}
