using ClassPlanner.Data;
using ClassPlanner.Models;
using ClassPlanner.Timetabling.Validation;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
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

    public TimetableValidationResult Validate(TimetableInput input, Timetable timetable)
    {
        TimetableValidationResult validationResult = new()
        {
            Result = ValidationResultType.Success,
            Title = "É recomendável ter no máximo 2 aulas da mesma disciplina por dia"
        };

        foreach (ClassSchedule classSchedule in timetable.ClassSchedules)
        {
            Dictionary<(DayOfWeek Day, long SubjectId), int> subjectSchedulesByDay = classSchedule.SubjectSchedules
                                                                                                  .GroupBy(s => (s.Day, s.Subject.SubjectId))
                                                                                                  .ToDictionary(g => g.Key, g => g.Count());

            foreach (((DayOfWeek day, long subjectId), int classCount) in subjectSchedulesByDay)
            {
                if (classCount > 2)
                {
                    Subject subject = input.Classrooms.First(c => c.ClassroomId == classSchedule.Classroom.ClassroomId)
                                            .Subjects.First(s => s.SubjectId == subjectId);

                    validationResult.AddError($"A disciplina '{subject.Name}' da turma '{classSchedule.Classroom.Name}' tem {classCount} aulas no mesmo dia ({day}), acima do limite recomendado (2)");
                    validationResult.Result = ValidationResultType.Warning;
                }
            }
        }

        return validationResult;
    }

}