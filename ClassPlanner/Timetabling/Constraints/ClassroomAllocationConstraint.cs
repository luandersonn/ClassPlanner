using ClassPlanner.Data;
using ClassPlanner.Models;
using ClassPlanner.Timetabling.Validation;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassPlanner.Timetabling.Constraints;

public class ClassroomAllocationConstraint : IConstraint
{
    public void Register(TimetableInput input, TimetableModel model)
    {
        int totalPeriods = input.PeriodsPerDay * input.WorkingDaysCount;

        foreach (Classroom classroom in input.Classrooms) // Turma t
        {
            HashSet<long> classroomSubjects = [.. classroom.Subjects.Select(s => s.SubjectId)];

            for (int periodIndex = 0; periodIndex < totalPeriods; periodIndex++)
            {
                var relevantVariables = model.Variables
                                             .Where(v => classroomSubjects.Contains(v.Key.subjectId) && v.Key.periodId == periodIndex)
                                             .Select(v => v.Value)
                                             .ToList();

                if (relevantVariables.Count > 0)
                {
                    // Somatório de variáveis de disciplinas d alocadas na turma t no período p deve ser menor ou igual a 1
                    model.Model.Add(LinearExpr.Sum(relevantVariables) <= 1);
                }
            }
        }
    }

    public TimetableValidationResult Validate(TimetableInput input, Timetable timetable)
    {
        TimetableValidationResult validationResult = new()
        {
            Result = ValidationResultType.Success,
            Title = "Duas disciplinas de uma mesma turma não devem ser alocadas para o mesmo período"
        };

        foreach (ClassSchedule classSchedule in timetable.ClassSchedules)
        {
            HashSet<long> classroomSubjects = [.. input.Classrooms
                                                       .First(c => c.ClassroomId == classSchedule.Classroom.ClassroomId).Subjects
                                                       .Select(s => s.SubjectId)];

            Dictionary<(DayOfWeek day, long period), List<string>> scheduleByTime = [];

            foreach (SubjectSchedule subjectSchedule in classSchedule.SubjectSchedules)
            {
                if (!classroomSubjects.Contains(subjectSchedule.Subject.SubjectId))
                    continue;

                (DayOfWeek Day, int Period) key = (subjectSchedule.Day, subjectSchedule.Period);

                if (!scheduleByTime.TryGetValue(key, out List<string>? subjectsAtTime))
                {
                    subjectsAtTime = [];
                    scheduleByTime[key] = subjectsAtTime;
                }

                subjectsAtTime.Add(subjectSchedule.Subject.Name);

                if (subjectsAtTime.Count > 1)
                {
                    validationResult.AddError($"A turma '{classSchedule.Classroom.Name}' tem múltiplas disciplinas ({string.Join(", ", subjectsAtTime)}) alocadas no dia {key.Day} e período {key.Period}");
                    validationResult.Result = ValidationResultType.Error;
                }
            }
        }

        return validationResult;
    }

}
