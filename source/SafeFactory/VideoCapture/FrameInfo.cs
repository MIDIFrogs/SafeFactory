// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using System;
using Compunet.YoloV8.Data;
using SixLabors.ImageSharp;

namespace SafeFactory.VideoCapture
{
    /// <summary>
    /// Represents an information about the frame to validate.
    /// </summary>
    /// <param name="Frame">A processed image frame instance.</param>
    /// <param name="Timestamp">Timestamp when the frame was made.</param>
    /// <param name="DetectedPoses">All poses detected on the frame.</param>
    /// <param name="DetectedBoxes">All boxes detected on the frame.</param>
    /// <param name="IsDoorOpened">Determines whether the door is opened.</param>
    public record FrameInfo(Image Frame, TimeSpan Timestamp, YoloResult<Pose> DetectedPoses, YoloResult<Detection> DetectedBoxes, bool IsDoorOpened)
    {
    }
}