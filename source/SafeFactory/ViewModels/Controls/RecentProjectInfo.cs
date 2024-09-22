using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;

namespace SafeFactory.ViewModels.Controls
{
    public record RecentProjectInfo(IImage? Preview, string ProjectName, DateTime LastOpen, TimeSpan Duration, string ProjectPath)
    {
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
                        return "Long time ago.";
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
                    return "Recently.";
                result.Append("ago.");
                return result.ToString();
            }
        }
    }
}
