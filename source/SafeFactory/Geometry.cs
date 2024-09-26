// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace SafeFactory
{
    public static class Geometry
    {
        public static SKPoint3 ToNormalizedVector(this SKColor c)
        {
            float r = c.Red / 255f;
            float g = c.Green / 255f;
            float b = c.Blue / 255f;
            return new(r, g, b);
        }

        public static float LengthSquared(this SKPoint3 point) => (point.X * point.X) + (point.Y * point.Y) + (point.Z * point.Z);

        public static Bitmap ConvertToBitmap(this SKBitmap preview)
        {
            using var stream = new MemoryStream();
            SKImage.FromBitmap(preview).Encode().SaveTo(stream);
            stream.Position = 0;
            return new Bitmap(stream);
        }

        public static bool IntersectsWith(this SKRectI rect, SKPointI[] polygon)
        {
            // Check if any edge of the polygon intersects with the rectangle
            for (int i = 0; i < polygon.Length; i++)
            {
                SKPointI p1 = polygon[i];
                SKPointI p2 = polygon[(i + 1) % polygon.Length]; // Wrap around to the first point

                // Check for intersection with rectangle edges
                if (LinesIntersect(p1, p2, new SKPointI(rect.Left, rect.Top), new SKPointI(rect.Right, rect.Top)) ||
                    LinesIntersect(p1, p2, new SKPointI(rect.Right, rect.Top), new SKPointI(rect.Right, rect.Bottom)) ||
                    LinesIntersect(p1, p2, new SKPointI(rect.Right, rect.Bottom), new SKPointI(rect.Left, rect.Bottom)) ||
                    LinesIntersect(p1, p2, new SKPointI(rect.Left, rect.Bottom), new SKPointI(rect.Left, rect.Top)))
                {
                    return true; // An edge intersects
                }
            }

            // Check if the rectangle is completely inside the polygon
            if (IsSKPointIInPolygon(rect.Location, polygon) &&
                IsSKPointIInPolygon(new SKPointI(rect.Right, rect.Top), polygon) &&
                IsSKPointIInPolygon(new SKPointI(rect.Right, rect.Bottom), polygon) &&
                IsSKPointIInPolygon(new SKPointI(rect.Left, rect.Bottom), polygon))
            {
                return true; // SKRectangle is completely inside the polygon
            }

            // Check if any vertex of the polygon is inside the rectangle
            foreach (var point in polygon)
            {
                if (rect.Contains(point))
                {
                    return true; // A vertex of the polygon is inside the rectangle
                }
            }

            return false; // No intersection found
        }

        private static bool LinesIntersect(SKPointI p1, SKPointI p2, SKPointI p3, SKPointI p4)
        {
            int d1 = Direction(p3, p4, p1);
            int d2 = Direction(p3, p4, p2);
            int d3 = Direction(p1, p2, p3);
            int d4 = Direction(p1, p2, p4);

            // General case
            if (d1 != d2 && d3 != d4)
                return true;

            // Special cases
            return (d1 == 0 && OnSegment(p3, p4, p1)) ||
                   (d2 == 0 && OnSegment(p3, p4, p2)) ||
                   (d3 == 0 && OnSegment(p1, p2, p3)) ||
                   (d4 == 0 && OnSegment(p1, p2, p4));
        }

        private static int Direction(SKPointI p1, SKPointI p2, SKPointI p3)
        {
            int val = (p2.Y - p1.Y) * (p3.X - p2.X) - (p2.X - p1.X) * (p3.Y - p2.Y);
            return (val == 0) ? 0 : (val > 0) ? 1 : 2; // collinear, clockwise, counterclockwise
        }

        private static bool OnSegment(SKPointI p1, SKPointI p2, SKPointI p)
        {
            return p.X <= Math.Max(p1.X, p2.X) && p.X >= Math.Min(p1.X, p2.X) &&
                   p.Y <= Math.Max(p1.Y, p2.Y) && p.Y >= Math.Min(p1.Y, p2.Y);
        }

        private static bool IsSKPointIInPolygon(SKPointI point, SKPointI[] polygon)
        {
            int n = polygon.Length;
            bool inside = false;

            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if ((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y) &&
                    (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}