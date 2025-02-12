using ClassPlanner.Data;
using Google.OrTools.Sat;
using System.Linq;

namespace ClassPlanner.Timetabling.Constraints;

public class ConsecutiveClassesRewardConstraint : IConstraint
{
    public void Register(TimetableInput input, TimetableModel model)
    {
        int totalPeriods = input.PeriodsPerDay * input.WorkingDaysCount;

        foreach (Subject subject in input.Classrooms
                                         .SelectMany(c => c.Subjects)
                                         .Where(s => s.PeriodsPerWeek >= 2))
        {
            var periodsInWeek = model.Variables
                                     .Where(v => v.Key.subjectId == subject.SubjectId)
                                     .OrderBy(v => v.Key.periodId)
                                     .ToList();

            for (int i = 0; i < periodsInWeek.Count - 1; i++)
            {
                IntVar first = periodsInWeek[i].Value;
                IntVar second = periodsInWeek[i + 1].Value;

                // Verifica se os períodos i e i+1 pertencem ao mesmo dia
                // Isso é feito dividindo o ID do período pelo número de períodos por dia (inteiro)
                // Como os IDs dos períodos são sequenciais, essa divisão retorna o "índice do dia"
                // Se o índice do dia for o mesmo para ambos os períodos, significa que eles estão no mesmo dia
                bool sameDay = (periodsInWeek[i].Key.periodId / input.PeriodsPerDay) ==
                               (periodsInWeek[i + 1].Key.periodId / input.PeriodsPerDay);

                if (sameDay)
                {
                    var consecutiveReward = model.Model.NewBoolVar(
                        $"Consecutive_Reward_{subject.SubjectId}_{periodsInWeek[i].Key.periodId}");

                    model.Model.Add(first + second == 2).OnlyEnforceIf(consecutiveReward);

                    model.Rewards.Add(consecutiveReward);
                }
            }
        }
    }
}
