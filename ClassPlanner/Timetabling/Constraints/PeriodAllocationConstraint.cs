using ClassPlanner.Data;
using Google.OrTools.Sat;
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
}
