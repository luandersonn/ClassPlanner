using ClassPlanner.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassPlanner.ViewModels;

public partial class SubjectListViewModel(IServiceProvider provider) : CollectionListViewModel<SubjectViewModel>(provider)
{
    protected override async Task LoadItemsAsync()
    {
        using IServiceScope scope = Provider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        List<Subject> results = await context.Subject.AsNoTracking()
                                                     .Include(s => s.Teacher)
                                                     .OrderBy(t => t.Name)
                                                     .ToListAsync();

        Items.Clear();

        foreach (Subject subject in results)
        {
            Items.Add(new SubjectViewModel(subject));
        }
    }

    public async Task AddSubjectAsync(string name)
    {
        using IServiceScope scope = Provider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Subject subject = new()
        {
            Name = name
        };
        context.Subject.Add(subject);
        await context.SaveChangesAsync();
        Items.Add(new SubjectViewModel(subject));
    }

    public async Task DeleteSubjectAsync(long id)
    {
        using IServiceScope scope = Provider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Subject? subject = await context.Subject.FindAsync(id);
        if (subject is not null)
        {
            context.Subject.Remove(subject);
            await context.SaveChangesAsync();
            SubjectViewModel? subjectViewModel = Items.FirstOrDefault(t => t.Id == id);
            if (subjectViewModel is not null)
            {
                Items.Remove(subjectViewModel);
            }
        }
    }
}