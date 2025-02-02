using ClassPlanner.Collections;
using ClassPlanner.Data;
using ClassPlanner.Models;
using ClassPlanner.Timetabling;
using ClassPlanner.Timetabling.Constraints;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.OrTools.Sat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassPlanner.ViewModels;

public partial class GenerateTimetablingViewModel : BaseViewModel
{
    public GenerateTimetablingViewModel()
    {
        GenerateTimetablingCommand = new AsyncRelayCommand(GenerateTimetablingAsync, CanGenerateTimetabling, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);

        Timetables = new ObservableSortedCollection<Timetable>(new TimetableMinObjectiveValueComparer());

        _periodPerDay = 1;

        _excessDailyClassesConstraintIsEnabled =
            _earlyAllocationPreferenceConstraintIsEnabled =
            _consecutiveClassesRewardConstraintIsEnabled = true;
    }

    [ObservableProperty]
    private int _periodPerDay;

    [ObservableProperty]
    private bool _excessDailyClassesConstraintIsEnabled;

    [ObservableProperty]
    private bool _earlyAllocationPreferenceConstraintIsEnabled;

    [ObservableProperty]
    private bool _consecutiveClassesRewardConstraintIsEnabled;

    [ObservableProperty]
    private CpSolverStatus _result;

    public IObservableSortedCollection<Timetable> Timetables { get; }

    public IAsyncRelayCommand GenerateTimetablingCommand { get; }


    private bool CanGenerateTimetabling() => true;

    private async Task GenerateTimetablingAsync(CancellationToken cancellationToken)
    {
        try
        {
            Timetables.Clear();

            using IServiceScope scope = ServiceProvider.CreateScope();

            AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            List<Classroom> classrooms = await dbContext.Classroom
                                                        .Include(c => c.Subjects)
                                                        .ThenInclude(s => s.Teacher)
                                                        .AsNoTracking()
                                                        .ToListAsync(cancellationToken);

            DayOfWeek[] days = [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday];

            List<Weekday> weekdays = new(5);
            int count = 0;

            foreach (DayOfWeek day in days)
            {
                Weekday weekday = new()
                {
                    DayOfWeek = day
                };
                foreach (int _ in Enumerable.Range(0, PeriodPerDay))
                {
                    weekday.Periods.Add(count++);
                }
                weekdays.Add(weekday);
            }

            TimetableInput input = new(classrooms, weekdays, PeriodPerDay);

            input.Constraints.Add(new PeriodAllocationConstraint());
            input.Constraints.Add(new TeacherAvailabilityConstraint());
            input.Constraints.Add(new ClassroomAllocationConstraint());

            if (ExcessDailyClassesConstraintIsEnabled)
                input.Constraints.Add(new ExcessDailyClassesConstraint());
            if (EarlyAllocationPreferenceConstraintIsEnabled)
                input.Constraints.Add(new EarlyAllocationPreferenceConstraint());
            if (ConsecutiveClassesRewardConstraintIsEnabled)
                input.Constraints.Add(new ConsecutiveClassesRewardConstraint());

            ITimetableSolver solver = scope.ServiceProvider.GetRequiredService<ITimetableSolver>();

            solver.OutputFound += (sender, output) => DispatcherQueue.TryEnqueue(() => Timetables.AddItem(output.Timetable));

            TimetableSolverResult timetableSolverResult = await solver.SolverAsync(input, cancellationToken);

            Result = timetableSolverResult.Result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }
}
