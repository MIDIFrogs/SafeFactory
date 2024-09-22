// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
            var workers = GetWorkerHeads(currentFrame, options).ToList();
            var helmets = GetHelmets(currentFrame, options).ToList();
            for (int i = 0; i < workers.Count; i++)
            {
                if (helmets.Count == 0)
                {
                    return new ValidationResult("A worker without helmet found.", ["helmet"]);
                }

                for (int j = 0; j < helmets.Count; j++)
                {
                    if (helmets[j].Bounds.IntersectsWith(workers[i].Bounds))
                    {
                        workers.RemoveAt(i--);
                        helmets.RemoveAt(j--);
                        break;
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}