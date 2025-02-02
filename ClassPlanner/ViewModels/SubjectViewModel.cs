using ClassPlanner.Data;
using ClassPlanner.Messenger;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClassPlanner.ViewModels;

public partial class SubjectViewModel : EntityViewModel
{
    public override string DisplayName => Name;
    public override long Id { get; set; }

    [ObservableProperty]
    private string _name = null!;

    [ObservableProperty]
    private int _periodsPerWeek;

    public TeacherViewModel? Teacher { get; set; }

    public SubjectViewModel()
    {
        Name = "";
        PeriodsPerWeek = 1;
    }

    public SubjectViewModel(Subject subject)
    {
        ArgumentNullException.ThrowIfNull(subject);
        Id = subject.SubjectId;
        Name = subject.Name;

        PeriodsPerWeek = subject.PeriodsPerWeek;

        if (subject.Teacher is not null)
        {
            Teacher = new TeacherViewModel(subject.Teacher);
        }
    }

    protected override async Task DeleteAsync()
    {
        using IServiceScope scope = ServiceProvider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IMessenger? messenger = scope.ServiceProvider.GetService<IMessenger>();

        Subject? subject = context.Subject.FirstOrDefault(x => x.SubjectId == Id);

        if (subject is not null)
        {
            context.Subject.Remove(subject);
            await context.SaveChangesAsync();
            messenger?.Send(subject, MessengerTokens.SubjectRemoved);
        }
    }
}
