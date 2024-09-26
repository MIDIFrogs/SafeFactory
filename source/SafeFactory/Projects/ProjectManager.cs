using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using SafeFactory.SafetyRules;
using SafeFactory.Prediction;

namespace SafeFactory.Projects
{
    public class ProjectManager : ObservableCollection<Project>
    {
        private static Lazy<ProjectManager> _projectManager = new();

        public static ProjectManager Instance => _projectManager.Value;

        public RoomConfig RoomConfig { get; set; }

        public async Task<Project> CreateProjectAsync(string name, string path, string videoPath, IProgress<(int, int)> progress)
        {
            var info = new ProjectInfo(name, string.Empty, videoPath, DateTime.Now, TimeSpan.Zero, path);
            var result = new Project(info);
            await result.ProcessAsync(progress);
            return result;
        }

        public async Task<Project> LoadProjectAsync(ProjectInfo info, IProgress<(int, int)> progressTracker)
        {
            var result = new Project(info);
            await result.RestoreAsync(progressTracker);
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

        public void LoadRoomConfig()
        {
            if (File.Exists("Room.json"))
            {
                string t = File.ReadAllText("Room.json");
                RoomConfig = JsonConvert.DeserializeObject<RoomConfig>(t);
                RoomConfig.Door = new()
                {
                    UpLeft = new(33, 61),
                    UpRight = new(236, 24),
                    DownLeft = new(55, 503),
                    DownRight = new(234, 375),
                };
            }
        }
    }
}
