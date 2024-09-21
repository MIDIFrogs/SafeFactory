// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using System;
using SixLabors.ImageSharp;

namespace SafeFactory
{
    public static class Geometry
    {
        public static bool IntersectsWith(this Rectangle rect, Point[] polygon)
        {
            // Check if any edge of the polygon intersects with the rectangle
            for (int i = 0; i < polygon.Length; i++)
            {
                Point p1 = polygon[i];
                Point p2 = polygon[(i + 1) % polygon.Length]; // Wrap around to the first point

                // Check for intersection with rectangle edges
                if (LinesIntersect(p1, p2, new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top)) ||
                    LinesIntersect(p1, p2, new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Bottom)) ||
                    LinesIntersect(p1, p2, new Point(rect.Right, rect.Bottom), new Point(rect.Left, rect.Bottom)) ||
                    LinesIntersect(p1, p2, new Point(rect.Left, rect.Bottom), new Point(rect.Left, rect.Top)))
                {
                    return true; // An edge intersects
                }
            }

            // Check if the rectangle is completely inside the polygon
            if (IsPointInPolygon(rect.Location, polygon) &&
                IsPointInPolygon(new Point(rect.Right, rect.Top), polygon) &&
                IsPointInPolygon(new Point(rect.Right, rect.Bottom), polygon) &&
                IsPointInPolygon(new Point(rect.Left, rect.Bottom), polygon))
            {
                return true; // Rectangle is completely inside the polygon
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

        private static bool LinesIntersect(Point p1, Point p2, Point p3, Point p4)
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

        private static int Direction(Point p1, Point p2, Point p3)
        {
            int val = (p2.Y - p1.Y) * (p3.X - p2.X) - (p2.X - p1.X) * (p3.Y - p2.Y);
            return (val == 0) ? 0 : (val > 0) ? 1 : 2; // collinear, clockwise, counterclockwise
        }

        private static bool OnSegment(Point p1, Point p2, Point p)
        {
            return p.X <= Math.Max(p1.X, p2.X) && p.X >= Math.Min(p1.X, p2.X) &&
                   p.Y <= Math.Max(p1.Y, p2.Y) && p.Y >= Math.Min(p1.Y, p2.Y);
        }

        private static bool IsPointInPolygon(Point point, Point[] polygon)
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