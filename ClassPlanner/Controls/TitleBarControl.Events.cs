using ClassPlanner.Extensions;
using Microsoft.UI.Xaml;
using System;

namespace ClassPlanner.Controls;


public partial class TitleBarControl
{
    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Window = App.Current.CurrentWindows[XamlRoot];
        Window.Activated += OnWindowActivated;
        Window.ExtendsContentIntoTitleBar = true;
        await TryUpdateRegionsForCustomTitleBarAsync();
        SetWindowTitle(Title);
        SetWindowActiveState(Window.GetIsWindowActive() ? WindowActivationState.CodeActivated : WindowActivationState.Deactivated);
        UpdateTheme();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Window.Activated -= OnWindowActivated;
        }
        catch { }
    }

    private async void OnSizeChanged(object sender, SizeChangedEventArgs e) => await TryUpdateRegionsForCustomTitleBarAsync();

    private void OnActualThemeChanged(FrameworkElement sender, object args) => UpdateTheme();

    private void OnWindowActivated(object sender, WindowActivatedEventArgs args) => SetWindowActiveState(args.WindowActivationState);

    private static async void OnPreferredHeightOption(DependencyObject sender, DependencyPropertyChangedEventArgs _)
    {
        TitleBarControl titleBar = (TitleBarControl)sender;
        await titleBar.TryUpdateRegionsForCustomTitleBarAsync();
    }

    private static void OnTitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        TitleBarControl titleBar = (TitleBarControl)sender;
        titleBar.SetWindowTitle((string)args.NewValue);
    }

    private static async void OnDisplayOptionsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs _)
    {
        if (sender is TitleBarControl titleBar && titleBar.AppIcon is not null)
            await titleBar.TryUpdateRegionsForCustomTitleBarAsync();
    }
}