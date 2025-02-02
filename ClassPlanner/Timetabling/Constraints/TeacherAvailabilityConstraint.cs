using Google.OrTools.Sat;
using System.Linq;

namespace ClassPlanner.Timetabling.Constraints;

public class TeacherAvailabilityConstraint : IConstraint
{
    public void Register(TimetableInput input, TimetableModel model)
    {
        foreach (var teacher in input.Classrooms
                                     .SelectMany(c => c.Subjects)
                                     .Where(s => s.Teacher != null)
                                     .Select(s => s.Teacher!)
                                     .Distinct())
        {
            foreach (var day in input.Weekdays)
            {
                foreach (var period in day.Periods)
                {
                    // Obter todas as variáveis relacionadas ao professor para o dia e período atuais
                    var relevantVariables = model.Variables
                        .Where(v => input.Classrooms
                            .Any(c => c.Subjects
                                .Any(s => s.SubjectId == v.Key.subjectId && s.TeacherId == teacher.TeacherId)))
                        .Where(v => v.Key.day == day.DayOfWeek && v.Key.periodId == period)
                        .Select(v => v.Value)
                        .ToList();

                    // Adicionar a restrição: um professor só pode ter no máximo uma aula por vez
                    if (relevantVariables.Count > 1)
                    {
                        model.Model.Add(LinearExpr.Sum(relevantVariables) <= 1);
                    }
                }
            }
        }
    }
}