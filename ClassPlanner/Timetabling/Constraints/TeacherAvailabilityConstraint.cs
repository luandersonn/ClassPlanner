using ClassPlanner.Data;
using Google.OrTools.Sat;
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
            HashSet<long> teacherSubjects = input.Classrooms
                                                 .SelectMany(c => c.Subjects)
                                                 .Where(s => s.TeacherId == teacher.TeacherId)
                                                 .Select(s => s.SubjectId)
                                                 .ToHashSet();

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

}