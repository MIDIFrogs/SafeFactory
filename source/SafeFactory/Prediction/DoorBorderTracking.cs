// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using SkiaSharp;

namespace SafeFactory.Prediction
{
    public static class DoorBorderTracking
    {
        public static List<SKPointI> FindKeySKPointIs(SKPointI x, SKPointI y, int e)
        {
            List<SKPointI> keypoints = [];
            double dx = y.X - x.X;
            double dy = y.Y - x.Y;
            double angel = Math.Atan2(dy, dx);
            do
            {
                keypoints.Add(new SKPointI(x.X + e, y.Y + (int)(e * angel)));
            } while (x.X + e <= y.X);

            return keypoints;
        }

        public static bool IsDoorOpened(SKBitmap img, RoomConfig config)
        {
            LineMetric topMetric = LineMetric.Compute(config.Door.UpLeft, config.Door.UpRight, img);
            if (!topMetric.Compare(config.TopDoorLine))
                return false;
            LineMetric bottomMetric = LineMetric.Compute(config.Door.DownLeft, config.Door.DownRight, img);
            return bottomMetric.Compare(config.BottomDoorLine);
        }
    }
}