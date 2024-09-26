// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using SkiaSharp;
using YoloDotNet;
using YoloDotNet.Models;

namespace SafeFactory.Prediction
{
    /// <summary>
    /// Represents a service that performs asynchronous frame prediction.
    /// </summary>
    public class FrameProcessor : IDisposable
    {
        private readonly Yolo boxPredictor;
        private readonly Yolo posePredictor;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameProcessor"/> class.
        /// </summary>
        /// <param name="boxPredictor">An instance of the <see cref="YoloPredictor"/> model trained for helmet and human head hitbox predictions.</param>
        /// <param name="posePredictor">An instance of the <see cref="YoloPredictor"/> model trained for the human and robot poses prediction.</param>
        public FrameProcessor(Yolo boxPredictor, Yolo posePredictor)
        {
            this.boxPredictor = boxPredictor;
            this.posePredictor = posePredictor;
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
        public static float ComparePoses(PoseEstimation pose1, PoseEstimation pose2)
        {
            float sum = 0;
            float max = float.PositiveInfinity;
            float min = float.NegativeInfinity;
            int count = 0;

            foreach (var (first, second) in pose1.KeyPoints.Zip(pose2.KeyPoints))
            {
                float distance = PointDistance(first, second);
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

            static float PointDistance(KeyPoint p1, KeyPoint p2) =>
                (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
        }

        /// <summary>
        /// Performs helmet and head detection asynchronously.
        /// </summary>
        /// <param name="image">An image frame to detect checked objects.</param>
        /// <returns>Results of the detection to validate.</returns>
        public async Task<List<ObjectDetection>> DetectAsync(SKBitmap image)
        {
            return await Task.Run(() => boxPredictor.RunObjectDetection(SKImage.FromBitmap(image)));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            boxPredictor.Dispose();
            posePredictor.Dispose();
        }

        /// <summary>
        /// Performs robot and humans pose detection asynchronously.
        /// </summary>
        /// <param name="image">An image frame to estimate poses.</param>
        /// <returns>Results of the estimation to validate.</returns>
        public async Task<List<PoseEstimation>> PoseAsync(SKBitmap image)
        {
            return await Task.Run(() => posePredictor.RunPoseEstimation(SKImage.FromBitmap(image)));
        }

        /// <summary>
        /// Performs full frame processing asynchronously.
        /// </summary>
        /// <param name="frame">A frame to process.</param>
        /// <param name="timestamp">Timestamp to save into a frame.</param>
        /// <returns>Processed frame info to validate.</returns>
        public async Task<FrameInfo> ProcessFrameAsync(SKBitmap frame, TimeSpan timestamp, RoomConfig room)
        {
            var boxTask = DetectAsync(frame);
            var poseTask = PoseAsync(frame);
            await Task.WhenAll(boxTask, poseTask);
            return new(frame, timestamp, poseTask.Result, boxTask.Result, DoorBorderTracking.IsDoorOpened(frame, room));
        }
    }
}