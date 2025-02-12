using ClassPlanner.Data;
using Google.OrTools.Sat;
using System.Collections.Generic;

namespace ClassPlanner.Timetabling;

public class TimetableModel
{
    public CpModel Model { get; } = new();
    public Dictionary<(long subjectId, int periodId), IntVar> Variables { get; } = [];

    public List<IntVar> Penalties { get; } = [];
    public List<IntVar> Rewards { get; } = [];

    public void InitializeVariables(TimetableInput input)
    {
        long totalPeriods = input.PeriodsPerDay * input.WorkingDaysCount;

        foreach (Classroom classroom in input.Classrooms)
        {
            foreach (Subject subject in classroom.Subjects)
            {
                for (int p = 0; p < totalPeriods; p++)
                {
                    (long SubjectId, int PeriodId) key = (subject.SubjectId, p);
                    Variables[key] = Model.NewBoolVar($"Subject_{subject.SubjectId}_Period_{p}");
                }
            }
        }
    }
}