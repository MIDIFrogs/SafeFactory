using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using SafeFactory.ViewModels.Controls;
using SafeFactory.Views;
using System;
using System.Collections.ObjectModel;
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
        this.shutdown = shutdown;
        ExitCommand = ReactiveCommand.Create(() => Dispatcher.UIThread.Post(Close));
        Tabs = [new TabInfo("Home", new HomeView() { DataContext = new HomeViewModel(this) })];
    }


    public ObservableCollection<TabInfo> Tabs { get; }

    public ReactiveCommand<Unit, Unit> ExitCommand { get; }
    
    public void Minimize() => MainWindowsState = WindowState.Minimized;
    public void Close() => shutdown?.Invoke(0);
}
