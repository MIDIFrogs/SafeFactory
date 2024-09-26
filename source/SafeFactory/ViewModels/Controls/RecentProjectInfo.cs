// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using System.Text;

namespace SafeFactory.ViewModels.Controls
{
    public record RecentProjectInfo(string PreviewPath, string ProjectName, DateTime LastOpen, TimeSpan Duration, string ProjectPath)
    {
        [JsonIgnore]
        public Bitmap? Preview { get; } = ExtractPreview(PreviewPath);

        [JsonIgnore]
        public string LastOpenDate
        {
            get
            {
                TimeSpan delta = DateTime.Now - LastOpen;
                StringBuilder result = new();
                if (delta.Days > 0)
                {
                    if (delta.Days > 60)
                    {
                        return "Long time ago";
                    }

                    result.Append(delta.Days).Append("d. ");
                }

                if (delta.Hours > 0)
                {
                    result.Append(delta.Hours).Append("h ");
                }

                if (delta.Minutes > 0)
                {
                    result.Append(delta.Minutes).Append("m ");
                }
                if (result.Length == 0)
                    return "Recently";
                result.Append("ago");
                return result.ToString();
            }
        }

        [JsonIgnore]
        public string DurationText => $"{Duration:hh\\:mm\\:ss}";

        private static Bitmap? ExtractPreview(string previewPath)
        {
            if (!string.IsNullOrEmpty(previewPath) && File.Exists(previewPath))
            {
                return new Bitmap(previewPath);
            }

            return null;
        }
    }
}