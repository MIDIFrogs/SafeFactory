// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using SafeFactory.Prediction;
using SkiaSharp;
using System.ComponentModel.DataAnnotations;
using YoloDotNet.Models;

namespace SafeFactory.SafetyRules
{
    public abstract class Rule
    {
        public abstract ValidationResult? CheckFrame(FrameContext context, RuleOptions options);

        protected bool IsRobotMoving(FrameContext context, RuleOptions options, int moveOffset = 0)
        {
            var previous = context.GetLastFrame(2 + moveOffset);
            if (previous == null)
                return false;
            var current = context.GetLastFrame(moveOffset)!;
            return FrameProcessor.ComparePoses(GetRobotPose(previous, options), GetRobotPose(current, options)) > options.MovementThreshold;
        }

        protected static PoseEstimation GetRobotPose(FrameInfo frame, RuleOptions options) => frame.DetectedPoses.Single(x => x.Label.Name == options.RobotTagName);

        protected static IEnumerable<PoseEstimation> GetWorkerPoses(FrameInfo frame, RuleOptions options) => frame.DetectedPoses.Where(x => x.Label.Name == options.WorkerTagName);

        protected static IEnumerable<ObjectDetection> GetWorkerHeads(FrameInfo frame, RuleOptions options) => frame.DetectedBoxes.Where(x => x.Label.Name == options.WorkerHeadTagName);

        protected static IEnumerable<ObjectDetection> GetHelmets(FrameInfo frame, RuleOptions options) => frame.DetectedBoxes.Where(x => x.Label.Name == options.WorkerHelmetTagName);

        public class RuleOptions
        {
            public float MovementThreshold { get; init; }

            public string RobotTagName { get; init; }

            public string WorkerTagName { get; init; }

            public string WorkerHeadTagName { get; init; }

            public string WorkerHelmetTagName { get; init; }

            public SKPointI[] KillZone { get; init; }
        }
    }
}