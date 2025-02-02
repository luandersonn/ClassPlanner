using ClassPlanner.Data;
using ClassPlanner.Messenger;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ClassPlanner.ViewModels;

public partial class EditSubjectViewModel : BaseViewModel
{
    private long? _subjectId;
    private readonly TeacherViewModel _noneTeacher = new()
    {
        Id = -1,
        Name = "[Nenhum]",
    };
    public EditSubjectViewModel()
    {
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);

        Teacher = _noneTeacher;

        Teachers = [];
        Classrooms = [];

        Name = "";
        PeriodsPerWeek = 1;
        _subjectId = null;
    }

    public EditSubjectViewModel(long subjectId) : this() => _subjectId = subjectId;


    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string? _name;


    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private int _periodsPerWeek;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private TeacherViewModel? _teacher;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private ClassroomViewModel? _classroom;

    public ObservableCollection<TeacherViewModel> Teachers { get; set; }
    public ObservableCollection<ClassroomViewModel> Classrooms { get; set; }


    public IAsyncRelayCommand LoadDataCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }

    private async Task LoadDataAsync()
    {
        using IServiceScope scope = ServiceProvider.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();


        if (_subjectId is not null)
        {
            Subject? subject = await dbContext.Subject.AsNoTracking()
                                                      .Include(s => s.Classroom)
                                                      .Include(s => s.Teacher)
                                                      .FirstOrDefaultAsync(s => s.SubjectId == _subjectId);

            if (subject is not null)
            {
                Name = subject.Name;
                PeriodsPerWeek = subject.PeriodsPerWeek;
                Classroom = new ClassroomViewModel(subject.Classroom);
                Teacher = subject.Teacher is null ? _noneTeacher : new TeacherViewModel(subject.Teacher);
            }

        }
        List<Teacher> teachers = await dbContext.Teacher
                                      .AsNoTracking()
                                      .OrderBy(t => t.Name)
                                      .ToListAsync();

        List<Classroom> classrooms = await dbContext.Classroom
                                                  .AsNoTracking()
                                                  .OrderBy(c => c.Name)
                                                  .ToListAsync();
        Teachers.Clear();

        Teachers.Add(_noneTeacher);

        foreach (Teacher teacher in teachers)
        {
            Teachers.Add(new TeacherViewModel(teacher));
        }

        OnPropertyChanged(nameof(Teacher));

        Classrooms.Clear();

        foreach (Classroom classroom in classrooms)
        {
            Classrooms.Add(new ClassroomViewModel(classroom));
        }

        OnPropertyChanged(nameof(Classroom));
    }

    private bool CanSave() => !string.IsNullOrWhiteSpace(Name) && PeriodsPerWeek > 0 && Classroom is not null;

    private async Task SaveAsync()
    {
        using IServiceScope scope = ServiceProvider.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IMessenger? messenger = scope.ServiceProvider.GetService<IMessenger>();

        if (_subjectId is null)
        {
            Subject subject = new()
            {
                Name = Name!,
                ClassroomId = Classroom!.Id,
                PeriodsPerWeek = PeriodsPerWeek,
                TeacherId = Teacher?.Id == -1 ? null : Teacher?.Id
            };

            await dbContext.Subject.AddAsync(subject);
            await dbContext.SaveChangesAsync();

            _subjectId = subject.SubjectId;

            messenger?.Send(subject, MessengerTokens.SubjectAdded);
        }
        else
        {
            Subject? subject = await dbContext.Subject.FirstOrDefaultAsync(s => s.SubjectId == _subjectId);

            if (subject is not null)
            {
                subject.Name = Name!;
                subject.ClassroomId = Classroom!.Id;
                subject.PeriodsPerWeek = PeriodsPerWeek;
                subject.TeacherId = Teacher?.Id == -1 ? null : Teacher?.Id;
                await dbContext.SaveChangesAsync();
                messenger?.Send(subject, MessengerTokens.SubjectUpdated);
            }
        }
    }
}