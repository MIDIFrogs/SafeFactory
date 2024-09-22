using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafeFactory.Projects;
using SafeFactory.VideoCapture;

namespace SafeFactory.ViewModels
{
    public partial class ProjectViewModel : ViewModelBase, IDisposable
    {
        private readonly Project project;
        private readonly VideoPlayer player;

        public ProjectViewModel(ProjectInfo info)
        {
            project = new(info);
            player = new();
            ProjectManager.Instance.Add(project);
            player.Play(new(project.Info.VideoPath));
        }

        public void Dispose()
        {
            player.Dispose();
        }
    }
}
