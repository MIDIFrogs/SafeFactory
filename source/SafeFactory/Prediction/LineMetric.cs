// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using CommunityToolkit.Diagnostics;
using SkiaSharp;

namespace SafeFactory.Prediction
{
    public class LineMetric
    {
        public SKPointI Begin { get; set; }

        public SKPointI End { get; set; }

        public SKColor[] KeyColors { get; set; } = [];

        public static IEnumerable<SKPointI> FindKeyPoints(SKPointI x, SKPointI y, int step)
        {
            double dx = y.X - x.X;
            double dy = y.Y - x.Y;
            double angel = Math.Atan2(dy, dx);
            do
            {
                yield return new SKPointI(x.X + step, y.Y + (int)(step * angel));
                x.X += step;
            } while (x.X + step <= y.X);
        }

        public static LineMetric Compute(SKPointI begin, SKPointI end, SKBitmap img)
        {
            List<SKColor> keyColors = [];
            foreach (var point in FindKeyPoints(begin, end, 1))
            {
                keyColors.Add(img.GetPixel(point.X, point.Y));
            }

            return new()
            {
                Begin = begin,
                End = end,
                KeyColors = [.. keyColors],
            };
        }

        public bool Compare(LineMetric other)
        {
            if (Begin != other.Begin || End != other.End)
                return ThrowHelper.ThrowInvalidOperationException<bool>();
            int dc = 0;
            for (int i = 0; i < KeyColors.Length; i++)
            {
                var difference = (KeyColors[i].ToNormalizedVector() - other.KeyColors[i].ToNormalizedVector()).LengthSquared();
                if (difference > 0.25f)
                {
                    dc++;
                }
            }

            return dc < KeyColors.Length / 4;
        }
    }
}