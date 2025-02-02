using ClassPlanner.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace ClassPlanner.Dialogs;

public sealed partial class EditSubjectDialog : ContentDialog
{
    public EditSubjectDialog(ClassroomViewModel classroom) : this()
    {
        ViewModel = new EditSubjectViewModel
        {
            Classroom = classroom
        };
        Title = "Nova disciplina";
    }

    public EditSubjectDialog(SubjectViewModel subject) : this()
    {
        ViewModel = new EditSubjectViewModel(subject.Id);
        Title = "Editar disciplina";
    }

    private EditSubjectDialog()
    {
        InitializeComponent();

        Opened += OnOpened;

        ViewModel = null!;
    }

    public EditSubjectViewModel ViewModel { get; }

    private void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        ViewModel.LoadDataCommand.Execute(null);
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}