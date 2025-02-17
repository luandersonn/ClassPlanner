using ClassPlanner.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;

namespace ClassPlanner.Views;

public sealed partial class MainViewPage : Page
{
    public MainViewPage()
    {
        InitializeComponent();

        ViewModel = App.Current.ServiceProvider.GetRequiredService<MainViewModel>();
        GenerateViewModel = App.Current.ServiceProvider.GetRequiredService<GenerateTimetablingViewModel>();
    }

    public MainViewModel ViewModel { get; }
    public GenerateTimetablingViewModel GenerateViewModel { get; }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        NavView.SelectedItem = NavView.MenuItems.FirstOrDefault();
    }

    private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {

    }

    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        NavigateTo(args.SelectedItemContainer?.Tag?.ToString(), args.RecommendedNavigationTransitionInfo);
    }


    private void NavigateTo(string? page, NavigationTransitionInfo transition)
    {
        switch (page)
        {
            case "Classrooms":
                ContentFrame.Navigate(typeof(ClassroomListViewPage), null, transition);
                break;

            case "Teaches":
                ContentFrame.Navigate(typeof(TeachersListViewPage), null, transition);
                break;

            case "GenTimetabling":
                ContentFrame.Navigate(typeof(GenerateTimetablingViewPage), null, transition);
                break;
        }
    }
}
