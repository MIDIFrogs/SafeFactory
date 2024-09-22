using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeFactory.Projects
{
    public class ProjectManager : ObservableCollection<Project>
    {
        private static Lazy<ProjectManager> _projectManager = new();

        public static ProjectManager Instance => _projectManager.Value;

        public async Task<Project> CreateProjectAsync(string name, string path, string videoPath)
        {
            var info = new ProjectInfo(name, string.Empty, videoPath, DateTime.Now, TimeSpan.Zero, path);
            var result = new Project(info);
            await result.ProcessAsync();
            return result;
        }

        public async Task<Project> LoadProjectAsync(ProjectInfo info)
        {
            var result = new Project(info);
            await result.RestoreAsync();
            return result;
        }

        /*
        * + Video
        * - Projects
        * |- P1
        * |- P2
        *  |- info.json
        *  |- Reports
        *   |- Report{X}.json
        *   |- Report{X}_Frames.gif
        */
        public IEnumerable<ProjectInfo> EnumerateProjects(string workingDirectory)
        {
            foreach (var directory in Directory.EnumerateDirectories(workingDirectory))
            {
                string infoPath = Path.Combine(directory, "info.json");
                if (File.Exists(infoPath))
                {
                    yield return ProjectInfo.Load(infoPath);
                }
            }
        }
    }
}
