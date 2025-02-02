using ClassPlanner.Data;
using ClassPlanner.Models;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;

namespace ClassPlanner.Timetabling.CP;

public partial class TimetableSolutionCallback(Dictionary<(long subjectId, DayOfWeek day, long periodId), IntVar> variables,
                                               List<Classroom> classrooms,
                                               List<Weekday> weekdays,
                                               int periodsPerDay,
                                               Action<Timetable> solutionFoundCallback) : CpSolverSolutionCallback
{
    public List<Timetable> Timetables { get; } = [];

    public override void OnSolutionCallback()
    {
        Timetable timetable = new()
        {
            ObjectiveValue = ObjectiveValue()
        };

        foreach (Classroom classroom in classrooms)
        {
            ClassSchedule classSchedule = new()
            {
                Classroom = classroom,
                PeriodsPerDay = periodsPerDay
            };

            foreach (Subject subject in classroom.Subjects)
            {
                foreach (Weekday day in weekdays)
                {
                    foreach (int period in day.Periods)
                    {
                        (long SubjectId, DayOfWeek DayOfWeek, long PeriodId) key = (subject.SubjectId, day.DayOfWeek, period);

                        if (variables.TryGetValue(key, out IntVar? variable) && Value(variable) == 1)
                        {
                            classSchedule.SubjectSchedules.Add(new SubjectSchedule
                            {
                                Subject = subject,
                                Teacher = subject.Teacher,
                                Day = day.DayOfWeek,
                                Period = period
                            });
                        }
                    }
                }
            }

            timetable.ClassSchedules.Add(classSchedule);
        }

        solutionFoundCallback(timetable);

        Timetables.Add(timetable);
    }
}
