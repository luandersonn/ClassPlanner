using ClassPlanner.Data;
using Google.OrTools.Sat;
using System.Linq;

namespace ClassPlanner.Timetabling.Constraints;

public class ExcessDailyClassesConstraint : IConstraint
{
    public void Register(TimetableInput input, TimetableModel model)
    {
        int totalPeriods = input.PeriodsPerDay * input.WorkingDaysCount;

        foreach (Subject subject in input.Classrooms
                                         .SelectMany(c => c.Subjects)
                                         .Where(c => c.PeriodsPerWeek >= 2))
        {

            var subjectVariables = model.Variables
                                        .Where(v => v.Key.subjectId == subject.SubjectId)
                                        .GroupBy(v => v.Key.periodId / input.PeriodsPerDay) // Agrupar por dia
                                        .ToDictionary(g => g.Key, g => g.Select(v => v.Value).ToList());

            for (int dayIndex = 0; dayIndex < input.WorkingDaysCount; dayIndex++)
            {
                if (subjectVariables.TryGetValue(dayIndex, out var relevantVariables) && relevantVariables.Count != 0)
                {
                    IntVar dailyExcess = model.Model.NewIntVar(0, 10, $"Excess_Daily_{subject.SubjectId}_{dayIndex}");

                    model.Model.Add(LinearExpr.Sum(relevantVariables) <= 2 + dailyExcess);

                    model.Penalties.Add(dailyExcess);
                }
            }
        }
    }
}