using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SafeFactory.Projects
{
    public readonly record struct ProjectInfo(string Name, string ThumbnailImage, string VideoPath, DateTime LastOpen, TimeSpan Duration, [property: JsonIgnore] string ProjectPath)
    {
        internal static ProjectInfo Load(string projectPath)
        {
            if (!File.Exists(projectPath))
            {
                if (Directory.Exists(projectPath))
                {
                    projectPath = Path.Combine(projectPath, "info.json");
                }
                else
                {
                    // TODO: Throw
                    throw new ArgumentOutOfRangeException(nameof(projectPath));
                }
            }
            string text = File.ReadAllText(projectPath);
            return JsonConvert.DeserializeObject<ProjectInfo>(text) with { ProjectPath = projectPath };
        }
    }
}
