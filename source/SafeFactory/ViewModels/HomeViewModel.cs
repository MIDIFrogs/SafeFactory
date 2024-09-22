using AsyncImageLoader.Loaders;
using ReactiveUI;
using SafeFactory.Projects;
using SafeFactory.ViewModels.Controls;
using SafeFactory.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using MsBox.Avalonia;
using System.Xml.Linq;
using System.IO;
using Newtonsoft.Json;
using SafeFactory.VideoCapture;
using Avalonia.Media.Imaging;
using SixLabors.ImageSharp.Formats.Png;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Interactivity;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SafeFactory.ViewModels
{
    internal partial class HomeViewModel : ViewModelBase
    {
        private MainViewModel mainViewModel;

        public HomeViewModel() { }

        public HomeViewModel(MainViewModel mvm)
        {
            mainViewModel = mvm;
            LoadRecentProject();
        }

        private void LoadRecentProject()
        {
            if (File.Exists("History.json"))
            {
                string s = File.ReadAllText("History.json");
                RecentProjects = JsonConvert.DeserializeObject<ObservableCollection<RecentProjectInfo>>(s);
            }
            else
            {
                RecentProjects = [];
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
            OpenProject(file[0]);
        }

        public async void CreateProject()
        {
            var file = await new OpenFileDialog()
            {
                Title = "Select a video to open.",
                Filters = [new() { Name = "Video file (MPEG)", Extensions = ["mp4", "avi"] }],
            }.ShowAsync(new Window());
            var projectFile = await new SaveFileDialog()
            {
                DefaultExtension = "json",
                Directory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                InitialFileName = "My Project",
                Filters = [new() { Name = "Project info file (JSON)", Extensions = ["json"] }],
                Title = "Select a location to save the file.",
            }.ShowAsync(new Window());
            string projName = Path.GetFileNameWithoutExtension(projectFile);
            var (preview, duration) = FrameSpliter.AnalyzeVideo(file[0]);
            string previewPath = Path.Combine(new FileInfo(projectFile).Directory.FullName, "preview.png");
            await preview.SaveAsync(File.Create(previewPath), new PngEncoder());
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
                var (preview, duration) = FrameSpliter.AnalyzeVideo(info.VideoPath);
                RecentProjects.Add(recent = new(Path.Combine(info.ProjectPath, "preview.png"), info.Name, DateTime.Now, duration, projectPath));
                SaveRecentProjects();
            }
            mainViewModel.Tabs.Add(new TabInfo(info.Name, new ProjectView() { DataContext = new ProjectViewModel(info) }));
            mainViewModel.SelectedTab = mainViewModel.Tabs.Count - 1;
        }

        void SaveRecentProjects()
        {
            string s = JsonConvert.SerializeObject(RecentProjects, Formatting.Indented);
            File.WriteAllText("History.json", s);
        }
    }
}
