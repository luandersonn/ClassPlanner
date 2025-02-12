using ClassPlanner.Dialogs;
using ClassPlanner.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace ClassPlanner.Views;

public sealed partial class ClassroomListViewPage : Page
{
    public ClassroomListViewPage()
    {
        InitializeComponent();

        ViewModel = App.Current.ServiceProvider.GetRequiredService<ClassroomListViewModel>();
    }

    public ClassroomListViewModel ViewModel { get; }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel.RefreshListCommand.Execute(null);
    }

    private async void OnAddClassroomClick(object sender, RoutedEventArgs e)
    {
        await new EditClassroomDialog()
        {
            XamlRoot = XamlRoot
        }.ShowAsync();
    }

    private async void OnAddSubjectClick(object sender, RoutedEventArgs e)
    {
        FrameworkElement element = (FrameworkElement)sender;
        ClassroomViewModel? classroom = element.DataContext as ClassroomViewModel;

        await new EditSubjectDialog(classroom)
        {
            XamlRoot = XamlRoot
        }.ShowAsync();
    }

    private async void OnEditSubjectClick(object sender, RoutedEventArgs e)
    {
        FrameworkElement element = (FrameworkElement)sender;
        SubjectViewModel subject = (SubjectViewModel)element.DataContext;

        await new EditSubjectDialog(subject)
        {
            XamlRoot = XamlRoot
        }.ShowAsync();
    }

    private async void OnEditClassroomClick(object sender, RoutedEventArgs e)
    {
        FrameworkElement element = (FrameworkElement)sender;
        ClassroomViewModel classroom = (ClassroomViewModel)element.DataContext;

        await new EditClassroomDialog(classroom.Id)
        {
            XamlRoot = XamlRoot
        }.ShowAsync();
    }

    private async void OnDeleteItemClick(object sender, RoutedEventArgs e)
    {
        FrameworkElement element = (FrameworkElement)sender;
        EntityViewModel? entity = (EntityViewModel?)element.DataContext;

        if (entity is null) return;

        await new DeleteItemDialog(entity)
        {
            XamlRoot = XamlRoot
        }.ShowAsync();
    }

    private void SemanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
    {
        e.DestinationItem.Item = e.SourceItem.Item;
    }

    private void Button_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        e.Handled = true;
    }
}