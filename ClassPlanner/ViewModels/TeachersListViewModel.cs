using ClassPlanner.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassPlanner.ViewModels;

public partial class TeachersListViewModel(IServiceProvider provider) : CollectionListViewModel<TeacherViewModel>(provider)
{
    protected override async Task LoadItemsAsync()
    {
        using IServiceScope scope = Provider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        List<Teacher> results = await context.Teacher.AsNoTracking()
                                                     .OrderBy(t => t.Name)
                                                     .ToListAsync();

        Items.Clear();

        foreach (Teacher teacher in results)
        {
            Items.Add(new TeacherViewModel(teacher));
        }
    }

    public async Task AddTeacherAsync(string name)
    {
        using IServiceScope scope = Provider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Teacher teacher = new()
        {
            Name = name
        };
        context.Teacher.Add(teacher);
        await context.SaveChangesAsync();
        Items.Add(new TeacherViewModel(teacher));
    }

    public async Task DeleteTeacherAsync(long id)
    {
        using IServiceScope scope = Provider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Teacher? teacher = await context.Teacher.FindAsync(id);
        if (teacher is not null)
        {
            context.Teacher.Remove(teacher);
            await context.SaveChangesAsync();
            TeacherViewModel? teacherViewModel = Items.FirstOrDefault(t => t.Id == id);
            if (teacherViewModel is not null)
            {
                Items.Remove(teacherViewModel);
            }
        }
    }
}