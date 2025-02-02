using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassPlanner.Timetabling;

public interface ITimetableSolver
{
    Task<TimetableSolverResult> SolverAsync(TimetableInput input, CancellationToken cancellationToken);

    event EventHandler<TimetableFoundEventArgs> OutputFound;
}