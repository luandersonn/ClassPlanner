using ClassPlanner.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace ClassPlanner.Dialogs;

public partial class DeleteItemDialog : ContentDialog
{
    public DeleteItemDialog(EntityViewModel entityViewModel)
    {
        InitializeComponent();
        EntityViewModel = entityViewModel;
    }

    public EntityViewModel EntityViewModel { get; }
}
