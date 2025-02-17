using ClassPlanner.Data;
using ClassPlanner.Models;
using ClassPlanner.Timetabling.Validation;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassPlanner.Timetabling.Constraints;

public class TeacherAvailabilityConstraint : IConstraint
{
    public void Register(TimetableInput input, TimetableModel model)
    {
        int totalPeriods = input.PeriodsPerDay * input.WorkingDaysCount;

        foreach (Teacher teacher in input.Classrooms
                                         .SelectMany(c => c.Subjects)
                                         .Where(s => s.Teacher is not null)
                                         .Select(s => s.Teacher!)
                                         .Distinct())
        {
            HashSet<long> teacherSubjects = [.. input.Classrooms
                                                 .SelectMany(c => c.Subjects)
                                                 .Where(s => s.TeacherId == teacher.TeacherId)
                                                 .Select(s => s.SubjectId)];

            for (int periodIndex = 0; periodIndex < totalPeriods; periodIndex++)
            {
                var relevantVariables = model.Variables
                                             .Where(v => teacherSubjects.Contains(v.Key.subjectId) && v.Key.periodId == periodIndex)
                                             .Select(v => v.Value)
                                             .ToList();

                if (relevantVariables.Count > 1)
                {
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
            Title = "Nenhum professor pode ministrar duas aulas simultaneamente no mesmo período"
        };

        foreach (Teacher teacher in input.Classrooms
                                         .SelectMany(c => c.Subjects)
                                         .Where(s => s.Teacher is not null)
                                         .Select(s => s.Teacher!)
                                         .Distinct())
        {
            HashSet<long> teacherSubjects = [.. input.Classrooms
                                                     .SelectMany(c => c.Subjects)
                                                     .Where(s => s.TeacherId == teacher.TeacherId)
                                                     .Select(s => s.SubjectId)];

            Dictionary<(DayOfWeek day, long period), List<string>> scheduleByTime = [];

            foreach (ClassSchedule classSchedule in timetable.ClassSchedules)
            {
                foreach (SubjectSchedule subjectSchedule in classSchedule.SubjectSchedules)
                {
                    if (!teacherSubjects.Contains(subjectSchedule.Subject.SubjectId))
                        continue;

                    (DayOfWeek Day, int Period) key = (subjectSchedule.Day, subjectSchedule.Period);

                    if (!scheduleByTime.TryGetValue(key, out List<string>? classesAtTime))
                    {
                        classesAtTime = [];
                        scheduleByTime[key] = classesAtTime;
                    }

                    classesAtTime.Add(classSchedule.Classroom.Name);

                    if (classesAtTime.Count > 1)
                    {
                        validationResult.AddError($"O professor '{teacher.Name}' está alocado em mais de uma turma ao mesmo tempo no dia {key.Day}, período {key.Period}. Turmas: {string.Join(", ", classesAtTime)}.");
                        validationResult.Result = ValidationResultType.Error;
                    }
                }
            }
        }

        return validationResult;
    }
}