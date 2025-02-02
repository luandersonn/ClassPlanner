using ClassPlanner.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace ClassPlanner.Views;

public sealed partial class TeachersListViewPage : Page
{
    public TeachersListViewPage()
    {
        InitializeComponent();

        ViewModel = App.Current.ServiceProvider.GetRequiredService<TeachersListViewModel>();
    }

    public TeachersListViewModel ViewModel { get; }


    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel.RefreshListCommand.Execute(null);
    }

    private async void AddTeacherButtonClick(object sender, RoutedEventArgs e)
    {
        TextBox textbox = new()
        {
            PlaceholderText = "Digite o nome do professor",
            Text = "",
            MinWidth = 360,
            TextWrapping = TextWrapping.Wrap
        };

        ContentDialog dialog = new()
        {
            Title = "Adicionar professor",
            Content = textbox,
            PrimaryButtonText = "Adicionar",
            DefaultButton = ContentDialogButton.Primary,
            CloseButtonText = "Cancelar",
            XamlRoot = XamlRoot,
            RequestedTheme = ActualTheme
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            if (!string.IsNullOrWhiteSpace(textbox.Text))
            {
                string name = textbox.Text.Trim();

                await ViewModel.AddTeacherAsync(name);
            }
        }
    }

    private async void DeleteTeacherButtonClick(object sender, RoutedEventArgs e)
    {
        FrameworkElement element = (FrameworkElement)sender;
        TeacherViewModel teacher = (TeacherViewModel)element.DataContext;

        ContentDialog dialog = new()
        {
            Title = "Tem certeza disso?",
            Content = $"Você está prestes a deletar \"{teacher.Name}\"",
            PrimaryButtonText = "Deletar",
            CloseButtonText = "Cancelar",
            XamlRoot = XamlRoot,
            RequestedTheme = ActualTheme
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
        {
            await ViewModel.DeleteTeacherAsync(teacher.Id);
        }
    }
}