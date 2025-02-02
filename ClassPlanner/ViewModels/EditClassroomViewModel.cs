using ClassPlanner.Data;
using ClassPlanner.Messenger;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace ClassPlanner.ViewModels;

public partial class EditClassroomViewModel : BaseViewModel
{
    private long? _classroomId;
    public EditClassroomViewModel()
    {
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }

    public EditClassroomViewModel(long classroomId) : this() => _classroomId = classroomId;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string? _name;

    public IAsyncRelayCommand LoadDataCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }

    private async Task LoadDataAsync()
    {
        if (_classroomId is not null)
        {
            using IServiceScope scope = ServiceProvider.CreateScope();
            AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Classroom? classroom = await dbContext.Classroom.FindAsync(_classroomId);
            if (classroom is not null)
            {
                Name = classroom.Name;
            }
        }
        else
        {
            Name = "";
        }
    }

    private bool CanSave() => !string.IsNullOrWhiteSpace(Name?.Trim());

    private async Task SaveAsync()
    {
        using IServiceScope scope = ServiceProvider.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IMessenger? messenger = scope.ServiceProvider.GetService<IMessenger>();

        if (_classroomId is null)
        {
            Classroom classroom = new()
            {
                Name = Name!.Trim(),
                Subjects = []
            };

            dbContext.Classroom.Add(classroom);
            await dbContext.SaveChangesAsync();

            messenger?.Send(classroom, MessengerTokens.ClassroomAdded);

            _classroomId = classroom.ClassroomId;
        }
        else
        {
            Classroom? classroom = await dbContext.Classroom.FindAsync(_classroomId);

            if (classroom is not null)
            {
                classroom.Name = Name!.Trim();
                await dbContext.SaveChangesAsync();
                messenger?.Send(classroom, MessengerTokens.ClassroomUpdated);
            }
        }
    }
}