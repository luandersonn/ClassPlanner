﻿using ClassPlanner.Data;
using ClassPlanner.Messenger;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassPlanner.ViewModels;

public partial class ClassroomListViewModel : CollectionListViewModel<ClassroomViewModel>
{
    private readonly IMessenger? _messenger;

    public ClassroomListViewModel(IServiceProvider provider) : base(provider)
    {
        _messenger = provider.GetService<IMessenger>();

        _messenger?.Register<Classroom, string>(this, MessengerTokens.ClassroomAdded, OnClassroomAdded);
        _messenger?.Register<Classroom, string>(this, MessengerTokens.ClassroomUpdated, OnClassroomUpdated);
        _messenger?.Register<Classroom, string>(this, MessengerTokens.ClassroomRemoved, OnClassroomRemoved);

        _messenger?.Register<Subject, string>(this, MessengerTokens.SubjectAdded, OnSubjectAdded);
        _messenger?.Register<Subject, string>(this, MessengerTokens.SubjectUpdated, OnSubjectUpdated);
        _messenger?.Register<Subject, string>(this, MessengerTokens.SubjectRemoved, OnSubjectRemoved);
    }

    protected override async Task LoadItemsAsync()
    {
        using IServiceScope scope = Provider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        List<Classroom> results = await context.Classroom.AsNoTracking()
                                                         .Include(c => c.Subjects)
                                                         .ThenInclude(c => c.Teacher)
                                                         .OrderBy(t => t.Name)
                                                         .ToListAsync();

        Items.Clear();

        foreach (Classroom classroom in results)
        {
            Items.Add(new ClassroomViewModel(classroom));
        }
    }


    private void OnClassroomAdded(object _, Classroom classroom)
    {
        Items.Add(new ClassroomViewModel(classroom));
    }
    private void OnClassroomUpdated(object _, Classroom classroom)
    {
        ClassroomViewModel? classroomViewModel = Items.FirstOrDefault(c => c.Id == classroom.ClassroomId);
        if (classroomViewModel is not null)
        {
            classroomViewModel.Name = classroom.Name;
        }
    }
    private void OnClassroomRemoved(object _, Classroom classroom)
    {
        ClassroomViewModel? viewModel = Items.FirstOrDefault(x => x.Id == classroom.ClassroomId);

        if (viewModel is not null)
        {
            Items.Remove(viewModel);
        }
    }


    private async void OnSubjectAdded(object _, Subject subject)
    {
        ClassroomViewModel? classroomView = Items.FirstOrDefault(c => c.Id == subject.ClassroomId);
        if (classroomView is null) return;

        using IServiceScope scope = Provider.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        subject = await dbContext.Subject
                                 .Include(s => s.Teacher)
                                 .FirstAsync(s => s.SubjectId == subject.SubjectId);

        classroomView.Subjects.Add(new SubjectViewModel(subject));
    }
    private async void OnSubjectUpdated(object _, Subject subject)
    {
        ClassroomViewModel? classroomView = Items.FirstOrDefault(c => c.Id == subject.ClassroomId);
        if (classroomView is null) return;
        SubjectViewModel? subjectView = classroomView.Subjects.FirstOrDefault(s => s.Id == subject.SubjectId);
        if (subjectView is null) return;

        int index = classroomView.Subjects.IndexOf(subjectView);

        classroomView.Subjects.Remove(subjectView);

        using IServiceScope scope = Provider.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        subject = await dbContext.Subject
                                 .Include(s => s.Teacher)
                                 .FirstAsync(s => s.SubjectId == subject.SubjectId);

        subjectView = new SubjectViewModel(subject);
        classroomView.Subjects.Insert(index, subjectView);
    }
    private void OnSubjectRemoved(object _, Subject subject)
    {
        ClassroomViewModel? classroomView = Items.FirstOrDefault(c => c.Id == subject.ClassroomId);
        if (classroomView is null) return;
        SubjectViewModel? subjectView = classroomView.Subjects.FirstOrDefault(s => s.Id == subject.SubjectId);
        if (subjectView is null) return;
        classroomView.Subjects.Remove(subjectView);
    }
}