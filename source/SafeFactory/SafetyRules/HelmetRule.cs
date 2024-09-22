// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using System.ComponentModel.DataAnnotations;
using SafeFactory.VideoCapture;

namespace SafeFactory.SafetyRules
{
    /// <summary>
    /// This rule restricts workers to wear helmets while they're in working area.
    /// </summary>
    internal class HelmetRule : Rule
    {
        /// <inheritdoc/>
        public override ValidationResult? CheckFrame(FrameContext context, RuleOptions options)
        {
            var currentFrame = context.GetLastFrame();
            if (currentFrame == null)
                return ValidationResult.Success;
            var workers = GetWorkerHeads(currentFrame, options);
            var helmets = GetHelmets(currentFrame, options);
            throw new System.NotImplementedException();
        }
    }
}