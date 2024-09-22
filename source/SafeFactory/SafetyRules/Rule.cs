// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Compunet.YoloV8.Data;
using SafeFactory.VideoCapture;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;

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

        protected static Pose GetRobotPose(FrameInfo frame, RuleOptions options) => frame.DetectedPoses.Single(x => x.Name.Name == options.RobotTagName);

        protected static IEnumerable<Pose> GetWorkerPoses(FrameInfo frame, RuleOptions options) => frame.DetectedPoses.Where(x => x.Name.Name == options.WorkerTagName);

        protected static IEnumerable<Detection> GetWorkerHeads(FrameInfo frame, RuleOptions options) => frame.DetectedBoxes.Where(x => x.Name.Name == options.WorkerHeadTagName);

        protected static IEnumerable<Detection> GetHelmets(FrameInfo frame, RuleOptions options) => frame.DetectedBoxes.Where(x => x.Name.Name == options.WorkerHelmetTagName);

        public class RuleOptions
        {
            public float MovementThreshold { get; init; }

            public string RobotTagName { get; init; }

            public string WorkerTagName { get; init; }

            public string WorkerHeadTagName { get; init; }

            public string WorkerHelmetTagName { get; init; }

            public Point[] KillZone { get; init; }
        }
    }
}