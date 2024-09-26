// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SafeFactory.Prediction;

namespace SafeFactory.SafetyRules
{
    /// <summary>
    /// This rule restricts workers to not step inside robot area while it's moving.
    /// </summary>
    internal class StepOverRule : Rule
    {
        /// <inheritdoc/>
        public override ValidationResult? CheckFrame(FrameContext context, RuleOptions options)
        {
            var lastFrame = context.GetLastFrame();
            if (lastFrame == null)
                return ValidationResult.Success;
            var workers = GetWorkerPoses(lastFrame, options);
            bool robotMoves = IsRobotMoving(context, options);
            if (!robotMoves)
                return ValidationResult.Success;
            if (workers.Any(x => x.BoundingBox.IntersectsWith(options.KillZone)))
            {
                return new ValidationResult("Workers shouldn't enter area while robot is in use.", ["stepOver"]);
            }

            return ValidationResult.Success;
        }
    }
}