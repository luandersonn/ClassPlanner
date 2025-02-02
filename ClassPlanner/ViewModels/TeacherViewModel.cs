using ClassPlanner.Data;
using ClassPlanner.Messenger;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClassPlanner.ViewModels;

public partial class TeacherViewModel : EntityViewModel
{
    public override string DisplayName => Name;
    public override long Id { get; set; }

    [ObservableProperty]
    private string _name = null!;

    public TeacherViewModel()
    {
        Name = "";
    }

    public TeacherViewModel(Teacher teacher)
    {
        ArgumentNullException.ThrowIfNull(teacher);

        Id = teacher.TeacherId;
        Name = teacher.Name;
    }

    protected override async Task DeleteAsync()
    {
        using IServiceScope scope = App.Current.ServiceProvider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IMessenger? messenger = scope.ServiceProvider.GetService<IMessenger>();

        Teacher? teacher = context.Teacher.FirstOrDefault(x => x.TeacherId == Id);

        if (teacher is not null)
        {
            context.Teacher.Remove(teacher);
            await context.SaveChangesAsync();
            messenger?.Send(teacher, MessengerTokens.TeacherRemoved);
        }
    }
}