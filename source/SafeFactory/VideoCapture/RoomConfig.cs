using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeFactory.VideoCapture
{
    public class RoomConfig
    {
        public LineMetric TopDoorLine { get; set; }

        public LineMetric BottomDoorLine { get; set; }

        public DoorBorder Door { get; set; }

        public Point[] DeadZone { get; set; }
    }
}
