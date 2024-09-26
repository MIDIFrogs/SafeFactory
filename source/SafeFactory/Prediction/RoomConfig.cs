// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using SkiaSharp;

namespace SafeFactory.Prediction
{
    public class RoomConfig
    {
        public LineMetric TopDoorLine { get; set; }

        public LineMetric BottomDoorLine { get; set; }

        public DoorBorder Door { get; set; }

        public SKPointI[] DeadZone { get; set; }
    }
}