using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using LibVLCSharp.Avalonia;
using SafeFactory.Projects;
using SafeFactory.VideoCapture;
using SafeFactory.Views;

namespace SafeFactory.ViewModels
{
    public partial class ProjectViewModel : ViewModelBase, IDisposable
    {
        private readonly Project project;
        private readonly VideoPlayer player;

        private Task processingTask;
        private ProgressBar error1, error2, error3;

        public ProjectViewModel(ProjectInfo info)
        {
            project = new(info);
            player = new();
            ProjectManager.Instance.Add(project);
        }

        public async void Play(ProjectView view)
        {
            var progress = new Progress<(int, int)>();
            progress.ProgressChanged += Progress_ProgressChanged;
            processingTask = project.RestoreAsync(progress);
            view.Get<VideoView>("VideoViewer").MediaPlayer = player.Player;
            error1 = view.Get<ProgressBar>("DoorBar");
            error2 = view.Get<ProgressBar>("HelmetBar");
            error3 = view.Get<ProgressBar>("StepOverBar");
            player.Play(new(project.Info.VideoPath));
            await processingTask;
            // TODO: on task completion.
        }

        private void Progress_ProgressChanged(object? sender, (int, int) e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                Debug.WriteLine($"Progress {e.Item1}/{e.Item2}");
                error1.Maximum = error2.Maximum = error3.Maximum = e.Item2;
                error1.Value = error2.Value = error3.Value = e.Item1;
            });
        }

        public void Dispose()
        {
            player.Dispose();
        }
    }
}
