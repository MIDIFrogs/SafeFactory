using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SafeFactory.ViewModels;
using SafeFactory.ViewModels.Controls;

namespace SafeFactory.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RecentProjectsList.SelectedItem == null)
            return;
        (DataContext as HomeViewModel).OpenProject(((RecentProjectInfo)RecentProjectsList.SelectedItem).ProjectPath);
    }

    private void Binding(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }

    private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        (DataContext as HomeViewModel).SelectProject();
    }

    private void Button_Click_2(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        (DataContext as HomeViewModel).CreateProject();
    }
}