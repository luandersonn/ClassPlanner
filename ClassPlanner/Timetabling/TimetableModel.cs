using ClassPlanner.Data;
using ClassPlanner.Models;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;

namespace ClassPlanner.Timetabling;

public class TimetableModel
{
    public CpModel Model { get; } = new();
    public Dictionary<(long subjectId, DayOfWeek day, long periodId), IntVar> Variables { get; } = [];

    public List<IntVar> Penalties { get; } = [];
    public List<IntVar> Rewards { get; } = [];

    public void InitializeVariables(TimetableInput input)
    {
        foreach (Classroom classroom in input.Classrooms)
        {
            foreach (Subject subject in classroom.Subjects)
            {
                foreach (Weekday day in input.Weekdays)
                {
                    foreach (long period in day.Periods)
                    {
                        (long SubjectId, DayOfWeek DayOfWeek, long PeriodId) key = (subject.SubjectId, day.DayOfWeek, period);
                        Variables[key] = Model.NewBoolVar($"Subject_{subject.SubjectId}_Day_{day.DayOfWeek}_Period_{period}");
                    }
                }
            }
        }
    }
}