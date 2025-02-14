using ClassPlanner.Data;
using ClassPlanner.Timetabling.CP;
using Google.OrTools.Sat;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassPlanner.Timetabling;

public class TimetableSolver : ITimetableSolver
{
    public event EventHandler<TimetableFoundEventArgs>? OutputFound;

    public async Task<TimetableSolverResult> SolverAsync(TimetableInput input, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(input);

        // Detectar se é inviável
        foreach (Classroom classroom in input.Classrooms)
        {
            int totalPeriodsPerWeek = classroom.Subjects.Sum(x => x.PeriodsPerWeek);
            int maxPeriodsPerWeek = input.PeriodsPerDay * input.WorkingDaysCount;

            if (totalPeriodsPerWeek > maxPeriodsPerWeek)
            {
                return new TimetableSolverResult
                {
                    Result = CpSolverStatus.Infeasible,
                    Timetables = []
                };
            }
        }

        return await Task.Run(() =>
        {
            // 1. Inicializar o modelo e as variáveis
            TimetableModel model = new();
            model.InitializeVariables(input);

            // 2. Registrar todas as restrições fornecidas no input
            foreach (IConstraint constraint in input.Constraints)
            {
                constraint.Register(input, model);
            }

            // 3. Adicionar objetivo ao modelo
            AddObjective(model);

            // 4. Resolver o modelo
            CpSolver solver = new();
            TimetableSolutionCallback callback = new(input,
                                                     model.Variables,
                                                     output => OutputFound?.Invoke(this, new TimetableFoundEventArgs(output)));

            cancellationToken.Register(() => solver.StopSearch());

            int workers = Math.Clamp(input.MaxThreads, 1, 64);

            string[] parameters =
            [
                $"num_search_workers:{workers}", // Configuração para múltiplos threads
                //"max_time_in_seconds:60" // timeout                
            ];

            solver.StringParameters = string.Join(";", parameters);

            cancellationToken.ThrowIfCancellationRequested();

            callback.StartDate = DateTime.Now;

            CpSolverStatus result = solver.Solve(model.Model, callback);


            return new TimetableSolverResult()
            {
                Result = result,
                Timetables = callback.Timetables
            };
        }, cancellationToken);
    }

    public static void AddObjective(TimetableModel model)
    {
        // Minimizar penalidades e maximizar recompensas
        LinearExpr penalties = LinearExpr.Sum(model.Penalties);
        LinearExpr rewards = LinearExpr.Sum(model.Rewards);

        model.Model.Maximize(rewards - penalties);
    }
}

