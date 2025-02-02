using ClassPlanner.Data;
using ClassPlanner.Models;
using System.Linq;

namespace ClassPlanner.Timetabling.Constraints;

public class ConsecutiveClassesRewardConstraint : IConstraint
{
    public void Register(TimetableInput input, TimetableModel model)
    {
        foreach (Classroom classroom in input.Classrooms)
        {
            foreach (Subject subject in classroom.Subjects)
            {
                foreach (Weekday day in input.Weekdays)
                {
                    var periodsInDay = model.Variables
                        .Where(v => v.Key.subjectId == subject.SubjectId && v.Key.day == day.DayOfWeek)
                        .OrderBy(v => day.Periods.First(p => p == v.Key.periodId))
                        .ToList();

                    for (int i = 0; i < periodsInDay.Count - 1; i++)
                    {
                        var first = periodsInDay[i].Value;
                        var second = periodsInDay[i + 1].Value;

                        // Criar variável de recompensa para aulas consecutivas
                        var consecutiveReward = model.Model.NewBoolVar($"Consecutive_Reward_{subject.SubjectId}_{day.DayOfWeek}_{i}");
                        model.Model.Add(first + second == 2).OnlyEnforceIf(consecutiveReward);

                        model.Rewards.Add(consecutiveReward);
                    }
                }
            }
        }
    }
}
