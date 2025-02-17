using ClassPlanner.Data;
using ClassPlanner.Models;
using ClassPlanner.Timetabling.Validation;
using Google.OrTools.Sat;
using System.Collections.Generic;
using System.Linq;

namespace ClassPlanner.Timetabling.Constraints;

public class PeriodAllocationConstraint : IConstraint
{
    public void Register(TimetableInput input, TimetableModel model)
    {
        foreach (Classroom classroom in input.Classrooms)
        {
            foreach (Subject subject in classroom.Subjects)
            {
                var relevantVariables = model.Variables
                                             .Where(v => v.Key.subjectId == subject.SubjectId)
                                             .Select(v => v.Value);

                model.Model.Add(LinearExpr.Sum(relevantVariables) == subject.PeriodsPerWeek);
            }
        }
    }

    public TimetableValidationResult Validate(TimetableInput input, Timetable timetable)
    {
        TimetableValidationResult validationResult = new()
        {
            Result = ValidationResultType.Success,
            Title = "Cada disciplina deve ter todas as suas horas semanais completamente alocadas no horário da turma"
        };

        foreach (ClassSchedule classSchedule in timetable.ClassSchedules)
        {
            var classroomSubjects = input.Classrooms
                                         .First(c => c.ClassroomId == classSchedule.Classroom.ClassroomId)
                                         .Subjects;

            Dictionary<long, int> subjectPeriodsAllocated = classSchedule.SubjectSchedules
                                                                         .GroupBy(s => s.Subject.SubjectId)
                                                                         .ToDictionary(g => g.Key, g => g.Count());

            foreach (Subject subject in classroomSubjects)
            {
                int expectedPeriods = subject.PeriodsPerWeek;
                int allocatedPeriods = subjectPeriodsAllocated.TryGetValue(subject.SubjectId, out int count) ? count : 0;

                if (allocatedPeriods != expectedPeriods)
                {
                    validationResult.AddError($"A disciplina '{subject.Name}' da turma '{classSchedule.Classroom.Name}' deveria ter {expectedPeriods} períodos, mas foi alocada em {allocatedPeriods}");
                    validationResult.Result = ValidationResultType.Error;
                }
            }
        }

        return validationResult;
    }

}
