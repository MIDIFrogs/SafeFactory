using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SafeFactory.ViewModels;

namespace SafeFactory.Views;

public partial class ProjectView : UserControl
{
    public ProjectView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        if (!Design.IsDesignMode)
        {
            (DataContext as ProjectViewModel)?.Play(this);
        }

        base.OnLoaded(e);
    }
}