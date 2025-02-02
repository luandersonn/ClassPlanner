using ClassPlanner.Data;
using ClassPlanner.Models;
using Google.OrTools.Sat;
using System.Linq;

namespace ClassPlanner.Timetabling.Constraints;

public class ExcessDailyClassesConstraint : IConstraint
{
    public void Register(TimetableInput input, TimetableModel model)
    {
        foreach (Classroom classroom in input.Classrooms)
        {
            foreach (Subject subject in classroom.Subjects)
            {
                foreach (Weekday day in input.Weekdays)
                {
                    System.Collections.Generic.IEnumerable<IntVar> relevantVariables = model.Variables
                        .Where(v => v.Key.subjectId == subject.SubjectId && v.Key.day == day.DayOfWeek)
                        .Select(v => v.Value);

                    if (relevantVariables.Any())
                    {
                        // Variável de penalidade para excesso de aulas diárias
                        IntVar dailyExcess = model.Model.NewIntVar(0, 10, $"Excess_Daily_{subject.SubjectId}_{day.DayOfWeek}");
                        model.Model.Add(LinearExpr.Sum(relevantVariables) <= 2 + dailyExcess);

                        model.Penalties.Add(dailyExcess);
                    }
                }
            }
        }
    }
}
