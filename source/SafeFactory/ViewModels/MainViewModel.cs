﻿// Copyright 2024 (c) MIDIFrogs (contact https://github.com/MIDIFrogs)
// Distributed under AGPL v.3.0 license. See LICENSE.md file in the project root for more information
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using ReactiveUI;
using SafeFactory.Prediction;
using SafeFactory.Projects;
using SafeFactory.ViewModels.Controls;
using SafeFactory.Views;
using SkiaSharp;
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
        ProjectManager.Instance.LoadRoomConfig();
    }

    public ObservableCollection<TabInfo> Tabs { get; }

    public ReactiveCommand<Unit, Unit> ExitCommand { get; }

    public void Minimize() => MainWindowsState = WindowState.Minimized;

    public void Close() => shutdown?.Invoke(0);
}