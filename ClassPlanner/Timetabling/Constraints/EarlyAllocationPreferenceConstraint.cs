using ClassPlanner.Data;
using System.Linq;

namespace ClassPlanner.Timetabling.Constraints;

public class EarlyAllocationPreferenceConstraint : IConstraint
{
    public void Register(TimetableInput input, TimetableModel model)
    {
        foreach (Classroom classroom in input.Classrooms)
        {
            // Calcular o total de períodos disponíveis na semana
            int totalAvailablePeriods = input.Weekdays.Sum(day => day.Periods.Count);
            int totalRequiredPeriods = classroom.Subjects.Sum(subject => subject.PeriodsPerWeek);

            // Aplicar restrição apenas se houver mais espaços disponíveis do que o necessário
            if (totalRequiredPeriods >= totalAvailablePeriods)
                continue;

            foreach (var day in input.Weekdays)
            {
                foreach (var period in day.Periods)
                {
                    // Variáveis relevantes para a turma, no dia e no período atual
                    var relevantVariables = model.Variables
                        .Where(v => classroom.Subjects.Any(s => s.SubjectId == v.Key.subjectId))
                        .Where(v => v.Key.day == day.DayOfWeek && v.Key.periodId == period)
                        .Select(v => v.Value)
                        .ToList();

                    if (!relevantVariables.Any())
                        continue;

                    // Criar uma variável booleana que representa a penalidade para horários mais tarde
                    var timePenalty = model.Model.NewBoolVar($"TimePenalty_{classroom.ClassroomId}_{day.DayOfWeek}_{period}");

                    // Adicionar implicações para ativar a penalidade quando uma aula for alocada nesse horário
                    foreach (var variable in relevantVariables)
                    {
                        model.Model.Add(variable == 1).OnlyEnforceIf(timePenalty);
                    }

                    // Calcular pesos baseados no DayOfWeek (segunda-feira = 1, sexta-feira = 5, etc.)
                    int dayWeight = (((int)day.DayOfWeek + 6) % 7) + 1; // Reordena o DayOfWeek para começar na segunda-feira
                    int periodWeight = day.Periods.IndexOf(period) + 1; // Períodos mais tarde no dia têm peso maior

                    // Criar uma variável para representar o peso constante multiplicado
                    var weightVar = model.Model.NewConstant((dayWeight * 10) + periodWeight);

                    // Criar uma variável inteira para a penalidade ponderada
                    var weightedPenalty = model.Model.NewIntVar(0, 1000, $"WeightedPenalty_{classroom.ClassroomId}_{day.DayOfWeek}_{period}");
                    model.Model.AddMultiplicationEquality(weightedPenalty, [timePenalty, weightVar]);

                    // Adicionar a penalidade ponderada ao conjunto de penalidades
                    model.Penalties.Add(weightedPenalty);
                }
            }
        }
    }
}