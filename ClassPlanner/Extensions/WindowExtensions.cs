using ClassPlanner.Interop;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;
using Windows.UI;
using WinUIEx;

namespace ClassPlanner.Extensions;

public static class WindowsExtensions
{
    public static void CenterOnWorkArea(this Window window)
    {
        DisplayArea displayArea = DisplayArea.GetFromWindowId(window.AppWindow.Id, DisplayAreaFallback.Primary);

        RectInt32 workArea = displayArea.WorkArea;

        double x = workArea.X + ((workArea.Width - window.AppWindow.Size.Width) / 2d);
        double y = workArea.Y + ((workArea.Height - window.AppWindow.Size.Height) / 2d);

        window.Move((int)Math.Round(x, 1), (int)Math.Round(y, 1));
    }

    public static bool GetIsWindowActive(this Window window) => window.GetWindowHandle() == Win32.GetActiveWindow();

    public static void SetTitleBarTheme(this AppWindow appWindow, ElementTheme theme)
    {
        if (appWindow != null && AppWindowTitleBar.IsCustomizationSupported())
        {
            AppWindowTitleBar titleBar = appWindow.TitleBar;

            Color foreground = default, background = default;

            switch (theme)
            {
                case ElementTheme.Default:
                    foreground = (Color)Application.Current.Resources["SystemBaseHighColor"];
                    background = (Color)Application.Current.Resources["SystemAltHighColor"];
                    break;
                case ElementTheme.Light:
                    foreground = Colors.Black;
                    background = Colors.White;
                    break;
                case ElementTheme.Dark:
                    foreground = Colors.White;
                    background = Colors.Black;
                    break;
            }

            titleBar.ForegroundColor = foreground;
            titleBar.BackgroundColor = background;

            titleBar.ButtonForegroundColor = foreground;
            titleBar.ButtonBackgroundColor = Colors.Transparent;

            titleBar.ButtonHoverForegroundColor = foreground;
            byte newAlpha = (byte)(foreground.A * 0.2);
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb(newAlpha, foreground.R, foreground.G, foreground.B);

            titleBar.ButtonPressedForegroundColor = foreground;
            newAlpha = (byte)(foreground.A * 0.4);
            titleBar.ButtonPressedBackgroundColor = Color.FromArgb(newAlpha, foreground.R, foreground.G, foreground.B);

            //titleBar.ButtonInactiveForegroundColor = Colors.Gray;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }
    }
}
