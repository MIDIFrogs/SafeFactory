// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using CommunityToolkit.Diagnostics;

namespace SafeFactory.Prediction
{
    /// <summary>
    /// Represents a context to store last frames and video streaming context.
    /// </summary>
    public class FrameContext
    {
        private readonly LinkedList<FrameInfo> lastFrames = [];
        private int maxLastFrames;

        /// <summary>
        /// Gets or sets the maximal count of frames cached by the context.
        /// </summary>
        public int MaxLastFrames
        {
            get => maxLastFrames;
            set
            {
                if (value < 1)
                    ThrowHelper.ThrowArgumentOutOfRangeException(nameof(value), "Value cannot be less than 1.");
                while (lastFrames.Count > value)
                {
                    lastFrames.RemoveFirst();
                }

                maxLastFrames = value;
            }
        }

        /// <summary>
        /// Gets the beginning time of the video streaming.
        /// </summary>
        public DateTime StreamingStartTimestamp { get; init; }

        /// <summary>
        /// Gets a value indicating whether the captured video is streamed.
        /// </summary>
        public bool StreamMode { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameContext"/> class.
        /// </summary>
        /// <param name="maxLastFrames">Maximal frames to store.</param>
        /// <param name="stream">Tests if </param>
        public FrameContext(int maxLastFrames = 3, bool stream = false)
        {
            StreamingStartTimestamp = DateTime.Now;
            MaxLastFrames = maxLastFrames;
            StreamMode = stream;
        }

        /// <summary>
        /// Adds a new frame into the context.
        /// </summary>
        /// <param name="frame">An instance of the <see cref="FrameInfo"/> to add to the context.</param>
        public void PushFrame(FrameInfo frame)
        {
            lastFrames.AddLast(frame);
            if (lastFrames.Count > MaxLastFrames)
            {
                lastFrames.RemoveFirst();
            }
        }

        /// <summary>
        /// Gets the N-th last frame <paramref name="fromEnd">from the end</paramref>.
        /// </summary>
        /// <param name="fromEnd">Numer of the frame from the end to retrieve. Number begins with 1.</param>
        /// <returns>Info about requested frame or <see langword="null"/> if the frame is inaccessible.</returns>
        public FrameInfo? GetLastFrame(int fromEnd = 1)
        {
            var node = lastFrames.Last;
            while (fromEnd > 1)
            {
                node = node?.Previous;
                fromEnd--;
            }

            return node?.Value;
        }
    }
}