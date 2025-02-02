using ClassPlanner.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace ClassPlanner.Dialogs;

public partial class EditClassroomDialog : ContentDialog
{
    public EditClassroomDialog()
    {
        InitializeComponent();
        ViewModel = new EditClassroomViewModel();
        Title = "Adicionar turma";
        Opened += OnOpened;
    }

    public EditClassroomDialog(long classroomId)
    {
        InitializeComponent();
        ViewModel = new EditClassroomViewModel(classroomId);
        Title = "Editar turma";
        Opened += OnOpened;
    }

    public EditClassroomViewModel ViewModel { get; }

    private void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        ViewModel.LoadDataCommand.Execute(null);
    }
}