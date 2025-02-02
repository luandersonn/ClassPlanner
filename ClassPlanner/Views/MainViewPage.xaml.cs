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
    }

    public MainViewModel ViewModel { get; }

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
                ContentFrame.Navigate(typeof(ClassroomListViewPage), transition);
                break;

            case "Teaches":
                ContentFrame.Navigate(typeof(TeachersListViewPage), transition);
                break;

            case "GenTimetabling":
                ContentFrame.Navigate(typeof(GenerateTimetablingViewPage), transition);
                break;
        }
    }
}
