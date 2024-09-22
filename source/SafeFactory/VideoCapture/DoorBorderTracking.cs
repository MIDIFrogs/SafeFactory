using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SafeFactory.VideoCapture
{
    public static class DoorBorderTracking
    {
        public static List<Point> FindKeyPoints(Point x, Point y, int e)
        {
            List<Point> keypoints = [];
            double dx = y.X - x.X;
            double dy = y.Y - x.Y;
            double angel = Math.Atan2(dy, dx);
            do
            {
                keypoints.Add(new Point(x.X + e, y.Y + (int)(e * angel)));
            } while (x.X + e <= y.X);

            return keypoints;
        }

        public static bool IsDoorOpened(Image img, DoorBorder doorBorder)
        {

        }
    }
}
