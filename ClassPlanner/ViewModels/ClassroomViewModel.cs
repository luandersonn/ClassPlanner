using ClassPlanner.Collections;
using ClassPlanner.Data;
using ClassPlanner.Messenger;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClassPlanner.ViewModels;

public partial class ClassroomViewModel : EntityViewModel
{
    public ClassroomViewModel(Classroom classroom)
    {
        ArgumentNullException.ThrowIfNull(classroom);

        Id = classroom.ClassroomId;
        Name = classroom.Name;
        Subjects = new ObservableSortedCollection<SubjectViewModel>(new SubjectViewModelComparer(), classroom.Subjects.Select(s => new SubjectViewModel(s)));

        Subjects.CollectionChanged += (_, _) => UpdatePeriodsCount();
        UpdatePeriodsCount();

        DuplicateCommand = new AsyncRelayCommand(DuplicateAsync, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }

    public override string DisplayName => Name;
    public override long Id { get; set; }

    [ObservableProperty]
    private string _name = null!;

    [ObservableProperty]
    private int _totalPeriodsPerWeek;

    public IAsyncRelayCommand DuplicateCommand { get; }

    public IObservableSortedCollection<SubjectViewModel> Subjects { get; }

    protected override async Task DeleteAsync()
    {
        using IServiceScope scope = ServiceProvider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IMessenger? messenger = scope.ServiceProvider.GetService<IMessenger>();

        Classroom? classroom = context.Classroom.FirstOrDefault(x => x.ClassroomId == Id);

        if (classroom is not null)
        {
            context.Classroom.Remove(classroom);
            await context.SaveChangesAsync();
            messenger?.Send(classroom, MessengerTokens.ClassroomRemoved);
        }
    }

    private async Task DuplicateAsync()
    {
        using IServiceScope scope = ServiceProvider.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IMessenger? messenger = scope.ServiceProvider.GetService<IMessenger>();

        Classroom classroom = new()
        {
            Name = Name!.Trim() + " cópia",
            Subjects = Subjects.Select(s => new Subject
            {
                Name = s.Name,
                PeriodsPerWeek = s.PeriodsPerWeek,
                TeacherId = s.Teacher?.Id,
            }).ToList()
        };


        await dbContext.Classroom.AddAsync(classroom);
        await dbContext.SaveChangesAsync();

        messenger?.Send(classroom, MessengerTokens.ClassroomAdded);
    }

    private void UpdatePeriodsCount()
    {
        TotalPeriodsPerWeek = Subjects.Sum(s => s.PeriodsPerWeek);
    }
}