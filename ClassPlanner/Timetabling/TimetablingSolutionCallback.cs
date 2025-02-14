using ClassPlanner.Data;
using ClassPlanner.Models;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;

namespace ClassPlanner.Timetabling.CP;

public partial class TimetableSolutionCallback(TimetableInput input,
                                               Dictionary<(long subjectId, int periodId), IntVar> variables,
                                               Action<Timetable> solutionFoundCallback) : CpSolverSolutionCallback
{
    public List<Timetable> Timetables { get; } = [];

    public DateTime StartDate { get; set; }

    public override void OnSolutionCallback()
    {
        Timetable timetable = new()
        {
            ObjectiveValue = ObjectiveValue(),
            SolutionTime = DateTime.Now - StartDate,
        };

        int totalPeriods = input.PeriodsPerDay * input.WorkingDaysCount;

        foreach (Classroom classroom in input.Classrooms)
        {
            ClassSchedule classSchedule = new()
            {
                Classroom = classroom,
                PeriodsPerDay = input.PeriodsPerDay
            };

            foreach (Subject subject in classroom.Subjects)
            {
                for (int periodIndex = 0; periodIndex < totalPeriods; periodIndex++)
                {
                    DayOfWeek dayOfWeek = (DayOfWeek)((periodIndex / input.PeriodsPerDay) + 1); // Segunda-feira é 1, Terça-feira é 2, etc.

                    int periodOfDay = periodIndex % input.PeriodsPerDay;

                    (long SubjectId, int PeriodId) key = (subject.SubjectId, periodIndex);

                    if (variables.TryGetValue(key, out IntVar? variable) && Value(variable) == 1)
                    {
                        classSchedule.SubjectSchedules.Add(new SubjectSchedule
                        {
                            Subject = subject,
                            Teacher = subject.Teacher,
                            Day = dayOfWeek,
                            Period = periodOfDay
                        });
                    }
                }
            }

            timetable.ClassSchedules.Add(classSchedule);
        }

        solutionFoundCallback(timetable);

        Timetables.Add(timetable);
    }
}
