// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using System.ComponentModel.DataAnnotations;
using SafeFactory.Prediction;

namespace SafeFactory.SafetyRules
{
    /// <summary>
    /// This rule says workers should stay door closed while robot is working.
    /// </summary>
    internal class CloseDoorRule : Rule
    {
        /// <inheritdoc/>
        public override ValidationResult? CheckFrame(FrameContext context, RuleOptions options)
        {
            var lastFrame = context.GetLastFrame();
            if (lastFrame == null)
                return ValidationResult.Success;
            bool botMoving = IsRobotMoving(context, options);
            if (botMoving && lastFrame.IsDoorOpened)
            {
                return new ValidationResult("You shouldn't open the door if robot is online.", ["openedDoor"]);
            }
            return ValidationResult.Success;
        }
    }
}