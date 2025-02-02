using ClassPlanner.Timetabling.CP;
using Google.OrTools.Sat;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassPlanner.Timetabling;

public class TimetableSolver : ITimetableSolver
{
    public event EventHandler<TimetableFoundEventArgs>? OutputFound;

    public async Task<TimetableSolverResult> SolverAsync(TimetableInput input, CancellationToken cancellationToken)
    {
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
            TimetableSolutionCallback callback = new(model.Variables,
                                                     [.. input.Classrooms],
                                                     [.. input.Weekdays],
                                                     input.PeriodsPerDay,
                                                     output => OutputFound?.Invoke(this, new TimetableFoundEventArgs(output)));

            cancellationToken.Register(() => solver.StopSearch());

            string[] parameters =
            [
                "num_search_workers:2", // Configuração para múltiplos threads
                //"max_time_in_seconds:60" // timeout
            ];

            solver.StringParameters = string.Join(";", parameters);

            cancellationToken.ThrowIfCancellationRequested();

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

        model.Model.Minimize(penalties - rewards);
    }
}

