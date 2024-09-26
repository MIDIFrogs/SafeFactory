// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using Avalonia.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SafeFactory.Projects;
using SafeFactory.VideoCapture;
using SafeFactory.ViewModels.Controls;
using SafeFactory.Views;
using System.Collections.ObjectModel;

namespace SafeFactory.ViewModels
{
    internal partial class HomeViewModel : ViewModelBase
    {
        private MainViewModel mainViewModel;

        public HomeViewModel()
        { }

        public HomeViewModel(MainViewModel mvm)
        {
            mainViewModel = mvm;
            LoadRecentProject();
        }

        private void LoadRecentProject()
        {
            RecentProjects = [];
            if (File.Exists("History.json"))
            {
                string s = File.ReadAllText("History.json");
                JArray arr = JArray.Parse(s);
                foreach (var item in arr)
                {
                    try
                    {
                        RecentProjects.Add(item.ToObject<RecentProjectInfo>());
                    }
                    catch
                    {
                    }
                }
            }
        }

        public ObservableCollection<RecentProjectInfo> RecentProjects { get; set; }

        public async void SelectProject()
        {
            var file = await new OpenFileDialog()
            {
                Filters = [new() { Name = "Project info file (JSON)", Extensions = ["json"] }],
                Title = "Select a file to open.",
            }.ShowAsync(new Window());
            if (file == null || file.Length == 0)
                return;
            OpenProject(file[0]);
        }

        public async void CreateProject()
        {
            var file = await new OpenFileDialog()
            {
                Title = "Select a video to open.",
                Filters = [new() { Name = "Video file (MPEG)", Extensions = ["mp4", "avi"] }],
            }.ShowAsync(new Window());
            if (file == null || file.Length == 0) return;
            var projectFile = await new SaveFileDialog()
            {
                DefaultExtension = "json",
                Directory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                InitialFileName = "My Project",
                Filters = [new() { Name = "Project info file (JSON)", Extensions = ["json"] }],
                Title = "Select a location to save the file.",
            }.ShowAsync(new Window());
            if (projectFile == null) return;
            string projName = Path.GetFileNameWithoutExtension(projectFile);
            var (preview, duration) = FrameSplitter.AnalyzeVideo(file[0]);
            string previewPath = Path.Combine(new FileInfo(projectFile).Directory.FullName, "preview.png");
            using (var imgSave = File.Create(previewPath))
            {
                preview?.Encode(imgSave, SkiaSharp.SKEncodedImageFormat.Png, 100);
            }

            var info = new ProjectInfo(projName, previewPath, file[0], DateTime.Now, duration, projectFile);
            File.WriteAllText(projectFile, JsonConvert.SerializeObject(info));
            RecentProjects.Add(new(previewPath, info.Name, DateTime.Now, duration, projectFile));
            SaveRecentProjects();
            OpenProject(projectFile);
        }

        internal void OpenProject(string projectPath)
        {
            if (ProjectManager.Instance.Any(x => x.Info.ProjectPath == projectPath))
            {
                return;
            }

            ProjectInfo info = ProjectInfo.Load(projectPath);
            var recent = RecentProjects.FirstOrDefault(x => x.ProjectPath == projectPath);
            if (recent == null)
            {
                var (preview, duration) = FrameSplitter.AnalyzeVideo(info.VideoPath);
                RecentProjects.Add(recent = new(Path.Combine(info.ProjectPath, "preview.png"), info.Name, DateTime.Now, duration, projectPath));
                SaveRecentProjects();
            }

            mainViewModel.Tabs.Add(new TabInfo(info.Name, new ProjectView() { DataContext = new ProjectViewModel(info) }));
            mainViewModel.SelectedTab = mainViewModel.Tabs.Count - 1;
        }

        private void SaveRecentProjects()
        {
            string s = JsonConvert.SerializeObject(RecentProjects, Formatting.Indented);
            File.WriteAllText("History.json", s);
        }
    }
}