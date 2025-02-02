using ClassPlanner.Data;
using ClassPlanner.Models;
using Google.OrTools.Sat;
using System.Linq;

namespace ClassPlanner.Timetabling.Constraints;

public class ClassroomAllocationConstraint : IConstraint
{
    public void Register(TimetableInput input, TimetableModel model)
    {
        foreach (Classroom classroom in input.Classrooms)
        {
            foreach (Weekday day in input.Weekdays)
            {
                foreach (long period in day.Periods)
                {
                    var relevantVariables = model.Variables
                                                 .Where(v => classroom.Subjects.Any(s => s.SubjectId == v.Key.subjectId))
                                                 .Where(v => v.Key.day == day.DayOfWeek && v.Key.periodId == period)
                                                 .Select(v => v.Value);

                    if (relevantVariables.Any())
                    {
                        model.Model.Add(LinearExpr.Sum(relevantVariables) <= 1);
                    }
                }
            }
        }
    }
}
