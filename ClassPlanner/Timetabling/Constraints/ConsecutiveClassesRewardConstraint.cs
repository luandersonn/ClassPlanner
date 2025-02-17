using ClassPlanner.Data;
using ClassPlanner.Models;
using ClassPlanner.Timetabling.Validation;
using Google.OrTools.Sat;
using System.Collections.Generic;
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

    public TimetableValidationResult Validate(TimetableInput input, Timetable timetable)
    {
        TimetableValidationResult validationResult = new()
        {
            Result = ValidationResultType.Success,
            Title = "É recomendável alocar aulas em pares de duas consecutivamente"
        };

        foreach (ClassSchedule classSchedule in timetable.ClassSchedules)
        {
            foreach (Subject subject in input.Classrooms
                                             .First(c => c.ClassroomId == classSchedule.Classroom.ClassroomId)
                                             .Subjects)
            {
                List<SubjectSchedule> subjectSchedules = [.. classSchedule.SubjectSchedules
                                                                          .Where(s => s.Subject.SubjectId == subject.SubjectId)
                                                                          .OrderBy(s => s.Day)
                                                                          .ThenBy(s => s.Period)];

                int totalClasses = subjectSchedules.Count;

                if (totalClasses <= 1)
                    continue;

                int expectedPairs = totalClasses / 2;
                int consecutivePairs = 0;
                int remainingClasses = totalClasses % 2;

                for (int i = 0; i < subjectSchedules.Count - 1; i++)
                {
                    SubjectSchedule first = subjectSchedules[i];
                    SubjectSchedule second = subjectSchedules[i + 1];

                    if (first.Subject.SubjectId == second.Subject.SubjectId &&
                        first.Day == second.Day &&
                        first.Period + 1 == second.Period)
                    {
                        consecutivePairs++;
                        i++;
                    }
                }

                if (consecutivePairs != expectedPairs || remainingClasses > 1)
                {
                    validationResult.AddError($"A disciplina '{subject.Name}' da turma '{classSchedule.Classroom.Name}' podia ter {expectedPairs} pares consecutivos, mas apenas {consecutivePairs} foram alocadas");
                    validationResult.Result = ValidationResultType.Warning;
                }
            }
        }

        return validationResult;
    }

}
