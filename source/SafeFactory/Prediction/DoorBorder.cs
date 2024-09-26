// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using SkiaSharp;

namespace SafeFactory.Prediction
{
    public class DoorBorder
    {
        public SKPointI UpLeft { get; set; }
        public SKPointI UpRight { get; set; }
        public SKPointI DownLeft { get; set; }
        public SKPointI DownRight { get; set; }
    }
}