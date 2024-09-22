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

namespace SafeFactory.ViewModels
{
    internal class HomeViewModel
    {
        // HACK
        private static RamCachedWebImageLoader loader = new RamCachedWebImageLoader();

        private MainViewModel mainViewModel;

        public HomeViewModel()
        {
            
        }

        public HomeViewModel(MainViewModel mvm)
        {
            mainViewModel = mvm;
        }

        // Unit это типа void
        public ReactiveCommand<Unit, Unit> ChooseProject { get; set; }

        public ObservableCollection<RecentProjectInfo> RecentProjects { get; set; } = [
            // HACK for preview.
            new(loader.ProvideImageAsync("https://i.pinimg.com/736x/c0/78/83/c078830e530f6a89799824e51b2f24a4.jpg").Result, "Sample", DateTime.Now, new(1, 2, 3), "./info.json"),
            new(loader.ProvideImageAsync("https://i.pinimg.com/736x/c0/78/83/c078830e530f6a89799824e51b2f24a4.jpg").Result, "Sample 2", new(2020, 10, 15), new(4, 3, 2), "./info.json")
            ];

        internal void OpenProject(string projectPath)
        {
            if (ProjectManager.Instance.Any(x => x.Info.ProjectPath == projectPath))
            {
                return;
            }
            ProjectInfo info = ProjectInfo.Load(projectPath);
            mainViewModel.Tabs.Add(new TabInfo(info.Name, new ProjectView() { DataContext = new ProjectViewModel(info) }));
            mainViewModel.SelectedTab = mainViewModel.Tabs.Count - 1;
        }
    }
}
