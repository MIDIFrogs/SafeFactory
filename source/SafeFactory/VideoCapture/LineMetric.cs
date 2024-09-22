using Avalonia.Controls;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeFactory.VideoCapture
{
    public class LineMetric
    {
        public Point Begin { get; set; }

        public Point End { get; set; }

        public Rgba32[] KeyColors { get; set; } = [];

        public static IEnumerable<Point> FindKeyPoints(Point x, Point y, int step)
        {
            double dx = y.X - x.X;
            double dy = y.Y - x.Y;
            double angel = Math.Atan2(dy, dx);
            do
            {
                yield return new Point(x.X + step, y.Y + (int)(step * angel));
            } while (x.X + step <= y.X);
        }

        public static LineMetric Compute(Point begin, Point end, Image<Rgba32> img)
        {
            List<Rgba32> keyColors = [];
            foreach (var point in FindKeyPoints(begin, end, 1))
            {
                keyColors.Add(img[point.X, point.Y]);
            }
            return new()
            {
                Begin = begin,
                End = end,
                KeyColors = [.. keyColors]
            };
        }

        public bool Compare(LineMetric other)
        {
            if (Begin != other.Begin) 
                // Throw
        }
    }
}
