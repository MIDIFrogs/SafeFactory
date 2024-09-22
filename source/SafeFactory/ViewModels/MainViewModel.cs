using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using ReactiveUI;
using SafeFactory.Projects;
using SafeFactory.VideoCapture;
using SafeFactory.ViewModels.Controls;
using SafeFactory.Views;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;

namespace SafeFactory.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private WindowState _MainWindowsState;
    private Action<int>? shutdown;

    [ObservableProperty]
    private int selectedTab;

    public MainViewModel()
    {
        
    }

    public MainViewModel(Action<int>? shutdown)
    {
        // HACK:
        if (!File.Exists("Room.json"))
        {
            using var img = SixLabors.ImageSharp.Image.Load<Rgba32>("C:\\Users\\Artyom\\Desktop\\train\\test\\images\\-1_4_mp4-0029_jpg.rf.d9c98aa7a21a54a76ed24e0dae75136d.jpg");
            ProjectManager.Instance.RoomConfig = new RoomConfig()
            {
                TopDoorLine = LineMetric.Compute(new(33, 61), new(236, 24), img),
                BottomDoorLine = LineMetric.Compute(new(55, 503), new(234, 375), img),
                DeadZone = [
                    new(0, 597),
                    new(389, 300),
                    new(640, 430),
                    new(640, 640),
                    new(0, 640)
                ],
                Door = new()
                {
                    UpLeftPoint = new(33, 61),
                    UpRightPoint = new(236, 24),
                    DownLeftPoint = new(55, 503),
                    DownRightPoint = new(234, 375),
                },
            };
            string s = JsonConvert.SerializeObject(ProjectManager.Instance.RoomConfig, Formatting.Indented);
            File.WriteAllText("Room.json", s);
        }

        this.shutdown = shutdown;
        ExitCommand = ReactiveCommand.Create(() => Dispatcher.UIThread.Post(Close));
        Tabs = [new TabInfo("Home", new HomeView() { DataContext = new HomeViewModel(this) })];
        string roomFile = File.ReadAllText("Room.json");
        ProjectManager.Instance.RoomConfig = JsonConvert.DeserializeObject<RoomConfig>(roomFile);
    }


    public ObservableCollection<TabInfo> Tabs { get; }

    public ReactiveCommand<Unit, Unit> ExitCommand { get; }
    
    public void Minimize() => MainWindowsState = WindowState.Minimized;
    public void Close() => shutdown?.Invoke(0);
}
