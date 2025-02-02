using ClassPlanner.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Windows.Input;

namespace ClassPlanner.Extensions;

public static class ContentDialogExtensions
{
    private static readonly TwoWayDictionary<ICommand, ContentDialog> commandDialogMap = [];

    public static readonly DependencyProperty PrimaryCommandProperty =
        DependencyProperty.RegisterAttached(
            "PrimaryCommand",
            typeof(IAsyncRelayCommand),
            typeof(ContentDialogExtensions),
            new PropertyMetadata(null, OnPrimaryCommandChanged));

    public static IAsyncRelayCommand GetPrimaryCommand(ContentDialog obj) =>
        (IAsyncRelayCommand)obj.GetValue(PrimaryCommandProperty);

    public static void SetPrimaryCommand(ContentDialog obj, IAsyncRelayCommand value) =>
        obj.SetValue(PrimaryCommandProperty, value);

    private static void OnPrimaryCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ContentDialog dialog) return;

        UnregisterEvents(dialog, e.OldValue as IAsyncRelayCommand);
        RegisterEvents(dialog, e.NewValue as IAsyncRelayCommand);
    }

    private static void RegisterEvents(ContentDialog dialog, IAsyncRelayCommand? command)
    {
        if (command is null) return;

        command.CanExecuteChanged += OnCanExecuteChanged;
        dialog.PrimaryButtonClick += OnPrimaryButtonClick;
        dialog.Unloaded += OnDialogUnloaded;

        dialog.IsPrimaryButtonEnabled = command.CanExecute(null);
        commandDialogMap[command] = dialog;
    }

    private static void UnregisterEvents(ContentDialog dialog, IAsyncRelayCommand? command)
    {
        if (command is null) return;

        command.CanExecuteChanged -= OnCanExecuteChanged;
        dialog.PrimaryButtonClick -= OnPrimaryButtonClick;
        dialog.Unloaded -= OnDialogUnloaded;

        commandDialogMap.Remove(command);
    }

    private static void OnDialogUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is ContentDialog dialog)
        {
            commandDialogMap.Remove(dialog);
        }
    }

    private static void OnCanExecuteChanged(object? sender, EventArgs e)
    {
        if (sender is IAsyncRelayCommand command && GetDialogFromCommand(command) is ContentDialog dialog)
        {
            dialog.IsPrimaryButtonEnabled = command.CanExecute(null);
        }
    }

    private static async void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (GetPrimaryCommand(sender) is IAsyncRelayCommand command && command.CanExecute(null))
        {
            var deferral = args.GetDeferral();
            sender.IsEnabled = false;
            try
            {
                await command.ExecuteAsync(null);
            }
            catch
            {
                args.Cancel = true;
            }
            finally
            {
                sender.IsEnabled = true;
                deferral.Complete();
            }
        }
    }

    private static ContentDialog? GetDialogFromCommand(IAsyncRelayCommand command)
    {
        commandDialogMap.TryGetValue(command, out ContentDialog? dialog);
        return dialog;
    }
}