using Google.OrTools.Sat;
using System.Collections.Generic;

namespace ClassPlanner.Timetabling;

public class TimetableSolverResult
{
    public required CpSolverStatus Result { get; init; }
    public ICollection<Timetable> Timetables { get; init; } = [];
}