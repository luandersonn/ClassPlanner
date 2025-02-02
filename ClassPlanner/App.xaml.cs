using ClassPlanner.Data;
using ClassPlanner.Timetabling;
using ClassPlanner.ViewModels;
using ClassPlanner.Windowing;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Concurrent;
using System.IO;
using WinUIEx;

namespace ClassPlanner;

public partial class App : Application
{
    private const string databaseFileName = "classplanner.db";
    public App()
    {
        InitializeComponent();

        ServiceProvider = CreateServiceProvider();

        CurrentWindows = new();
    }

    public new static App Current => (App)Application.Current;

    public IServiceProvider ServiceProvider { get; }
    public NavigationWindow MainWindow { get; private set; } = null!;
    public ConcurrentDictionary<XamlRoot, NavigationWindow> CurrentWindows { get; }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Get the activation args
        Microsoft.Windows.AppLifecycle.AppActivationArguments appArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();

        // Get or register the main instance
        Microsoft.Windows.AppLifecycle.AppInstance mainInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("main");

        // If the main instance isn't this current instance
        if (!mainInstance.IsCurrent)
        {
            // Redirect activation to that instance
            await mainInstance.RedirectActivationToAsync(appArgs);

            // And exit our instance and stop
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return;
        }

        Windowing.SplashScreen splash = new(typeof(NavigationWindow));

        mainInstance.Activated += OnAppActivated;

        splash.Completed += (s, e) =>
        {
            if (e is not null)
            {
                MainWindow = (NavigationWindow)e;
                MainWindow.Frame.Navigate(typeof(Views.MainViewPage));
            }
        };
    }

    private void OnAppActivated(object? sender, Microsoft.Windows.AppLifecycle.AppActivationArguments e)
    {
        IntPtr currentWindow = HwndExtensions.GetActiveWindow();

        HwndExtensions.ShowWindow(currentWindow);
    }

    private static ServiceProvider CreateServiceProvider()
    {
        ServiceCollection services = new();

        string databaseFilePath = Path.Join(Windows.Storage.ApplicationData.Current.LocalFolder.Path, databaseFileName);

        services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={databaseFilePath}"));


        services.AddSingleton<MainViewModel>()
                .AddSingleton<TeachersListViewModel>()
                .AddSingleton<ClassroomListViewModel>()
                .AddSingleton<GenerateTimetablingViewModel>()

                .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
                .AddTransient<ITimetableSolver, TimetableSolver>()
                .AddSingleton<DefaultClassroomDataGenerator>()
            ;

        return services.BuildServiceProvider(true);
    }
}
