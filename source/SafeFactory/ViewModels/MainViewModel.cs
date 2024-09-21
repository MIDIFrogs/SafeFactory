using SafeFactory.ViewModels.Controls;
using SafeFactory.Views;
using System.Collections.ObjectModel;

namespace SafeFactory.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public ObservableCollection<TabInfo> Tabs { get; } = [new TabInfo("Home", new HomeView())];
}
