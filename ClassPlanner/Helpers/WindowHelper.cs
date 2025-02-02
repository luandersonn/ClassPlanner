using ClassPlanner.Windowing;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Graphics;
using WinUIEx;

namespace ClassPlanner.Helpers;

public static class WindowHelper
{
    public static void CenterOnWorkArea(this Window window)
    {
        DisplayArea displayArea = DisplayArea.GetFromWindowId(window.AppWindow.Id, DisplayAreaFallback.Primary);

        RectInt32 workArea = displayArea.WorkArea;

        double x = workArea.X + ((workArea.Width - window.AppWindow.Size.Width) / 2d);
        double y = workArea.Y + ((workArea.Height - window.AppWindow.Size.Height) / 2d);

        window.Move((int)Math.Round(x, 1), (int)Math.Round(y, 1));
    }

    public static NavigationWindow NewWindow<T>(object? param = null, Window? parentWindow = null, string? persistenceId = null) where T : Page
    {
        double minWidth = 680;
        double minHeight = 500;


        NavigationWindow newWindow = new()
        {
            MinWidth = minWidth,
            MinHeight = minHeight,
            PersistenceId = persistenceId ?? typeof(T).Name,
        };

        newWindow.Activate();
        try
        {
            if (CanCenterOnWindow(parentWindow))
            {
                SafeCenterWindowOnAnother(parentWindow!, newWindow);
                newWindow.Width = 900;
                newWindow.Height = 550;
            }
        }
        catch
        {
            newWindow.CenterOnWorkArea();
        }
        newWindow.Frame.Navigate(typeof(T), param);

        return newWindow;
    }

    public static void SafeCenterWindowOnAnother(Window parentWindow, Window secondaryWindow)
    {

        int parentWindowWidth = parentWindow.AppWindow.Size.Width;
        int parentWindowHeight = parentWindow.AppWindow.Size.Height;

        int secondaryWindowWidth = secondaryWindow.AppWindow.Size.Width;
        int secondaryWindowHeight = secondaryWindow.AppWindow.Size.Height;

        int secondaryWindowPointX = (int)Math.Round((parentWindowWidth / 2d) - (secondaryWindowWidth / 2d) + parentWindow.AppWindow.Position.X, 1);
        int secondaryWindowPointY = (int)Math.Round((parentWindowHeight / 2d) - (secondaryWindowHeight / 2d) + parentWindow.AppWindow.Position.Y, 1);

        RectInt32 safeArea = parentWindow.GetWorkArea();

        RectInt32 secondaryWindowArea = new(secondaryWindowPointX, secondaryWindowPointY, secondaryWindowWidth, secondaryWindowHeight);

        PointInt32 secondaryWindowPosition = GetPointInsideSafeArea(secondaryWindowArea, safeArea);

        secondaryWindow.AppWindow.Move(secondaryWindowPosition);

    }

    public static RectInt32 GetWorkArea(this Window window)
    {
        DisplayArea displayArea = DisplayArea.GetFromWindowId(window.AppWindow.Id, DisplayAreaFallback.Primary);
        return displayArea.WorkArea;
    }

    private static bool CanCenterOnWindow(Window? parentWindow) => parentWindow != null && WindowManager.Get(parentWindow).WindowState != WindowState.Minimized;

    private static PointInt32 GetPointInsideSafeArea(RectInt32 window, RectInt32 safeArea)
    {
        int pointX = window.X;
        int pointY = window.Y;

        pointX = Math.Max(safeArea.X, Math.Min(pointX, safeArea.X + safeArea.Width - window.Width));
        pointY = Math.Max(safeArea.Y, Math.Min(pointY, safeArea.Y + safeArea.Height - window.Height));

        return new PointInt32(pointX, pointY);
    }
}
