// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using System;
using System.Linq;
using System.Threading.Tasks;
using Compunet.YoloV8;
using Compunet.YoloV8.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SafeFactory.VideoCapture
{
    /// <summary>
    /// Represents a service that performs asynchronous frame prediction.
    /// </summary>
    public class FrameProcessor
    {
        private readonly YoloPredictor boxPredictor;
        private readonly YoloConfiguration configuration;
        private readonly YoloPredictor posePredictor;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameProcessor"/> class.
        /// </summary>
        /// <param name="boxPredictor">An instance of the <see cref="YoloPredictor"/> model trained for helmet and human head hitbox predictions.</param>
        /// <param name="posePredictor">An instance of the <see cref="YoloPredictor"/> model trained for the human and robot poses prediction.</param>
        /// <param name="configuration">Shared prediction parameters to customize the prediction.</param>
        public FrameProcessor(YoloPredictor boxPredictor, YoloPredictor posePredictor, YoloConfiguration configuration)
        {
            this.boxPredictor = boxPredictor;
            this.posePredictor = posePredictor;
            this.configuration = configuration;
        }

        /// <summary>
        /// Compares two poses and calculates a normalized average distance between them.
        /// </summary>
        /// <remarks>
        /// Note that this method should be called only on the same poses definitions.
        /// </remarks>
        /// <param name="pose1">The first pose to compare.</param>
        /// <param name="pose2">The second pose to compare.</param>
        /// <returns>A normalized average distance between the keypoints, ranging from 0 to 1.</returns>
        public static float ComparePoses(Pose pose1, Pose pose2)
        {
            var kpPairs = from kp1 in pose1
                          join kp2 in pose2
                          on kp1.Index equals kp2.Index
                          select new
                          {
                              P1 = kp1.Point,
                              P2 = kp2.Point,
                          };

            float sum = 0;
            float max = float.PositiveInfinity;
            float min = float.NegativeInfinity;
            int count = 0;

            foreach (var pair in kpPairs)
            {
                float distance = PointDistance(pair.P1, pair.P2);
                sum += distance;
                count++;

                max = MathF.Max(max, distance);
                min = MathF.Min(min, distance);
            }

            if (count == 0)
            {
                return 0;
            }

            float average = sum / count;

            if (max - min < 0.0001f)
            {
                return 0;
            }

            float normalizedAverage = (average - min) / (max - min);
            return normalizedAverage;

            static float PointDistance(Point p1, Point p2) =>
                (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
        }

        /// <summary>
        /// Performs helmet and head detection asynchronously.
        /// </summary>
        /// <param name="image">An image frame to detect checked objects.</param>
        /// <returns>Results of the detection to validate.</returns>
        public async Task<YoloResult<Detection>> DetectAsync(Image image)
        {
            return await boxPredictor.DetectAsync(image, configuration);
        }

        /// <summary>
        /// Performs robot and humans pose detection asynchronously.
        /// </summary>
        /// <param name="image">An image frame to estimate poses.</param>
        /// <returns>Results of the estimation to validate.</returns>
        public async Task<YoloResult<Pose>> PoseAsync(Image image)
        {
            return await posePredictor.PoseAsync(image, configuration);
        }

        /// <summary>
        /// Performs full frame processing asynchronously.
        /// </summary>
        /// <param name="frame">A frame to process.</param>
        /// <param name="timestamp">Timestamp to save into a frame.</param>
        /// <returns>Processed frame info to validate.</returns>
        public async Task<FrameInfo> ProcessFrameAsync(Image<Rgba32> frame, TimeSpan timestamp, RoomConfig room)
        {
            var boxTask = DetectAsync(frame);
            var poseTask = PoseAsync(frame);
            await Task.WhenAll(boxTask, poseTask);
            return new(frame, timestamp, poseTask.Result, boxTask.Result, DoorBorderTracking.IsDoorOpened(frame, room));
        }
    }
}