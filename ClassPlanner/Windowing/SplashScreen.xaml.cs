using ClassPlanner.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using WinUIEx;

namespace ClassPlanner.Windowing;

public sealed partial class SplashScreen : WinUIEx.SplashScreen
{
    public SplashScreen(Type window) : base(window)
    {
        InitializeComponent();
    }

    protected override async Task OnLoading()
    {
        try
        {
            await Task.Delay(200);
            using IServiceScope scope = App.Current.ServiceProvider.CreateScope();
            AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.CreateDatabaseAsync();
        }
        catch (Exception ex)
        {
            infoBar.Title = "Erro";
            infoBar.Message = ex.Message;
            infoBar.IsOpen = true;
            infoBar.Severity = InfoBarSeverity.Error;
            IsAlwaysOnTop = true;
            this.Show();
            await Task.Delay(5_000);
            Environment.Exit(1);
        }
    }
}
