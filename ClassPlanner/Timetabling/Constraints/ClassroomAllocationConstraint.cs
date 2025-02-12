using ClassPlanner.Data;
using Google.OrTools.Sat;
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

            HashSet<long> classroomSubjects = classroom.Subjects
                                                       .Select(s => s.SubjectId)
                                                       .ToHashSet();

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

}
