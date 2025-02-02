using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;

namespace ClassPlanner.Windowing;

public sealed partial class NavigationWindow : WindowEx
{
    public NavigationWindow()
    {
        InitializeComponent();

        PersistenceId = "MainWindowPersistenceId";
    }

    public Frame Frame => frame;

    private void OnFrameLoaded(object sender, RoutedEventArgs e) => App.Current.CurrentWindows.TryAdd(Content.XamlRoot, this);
}