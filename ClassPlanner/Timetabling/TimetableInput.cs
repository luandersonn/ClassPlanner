using ClassPlanner.Data;
using System;
using System.Collections.Generic;

namespace ClassPlanner.Timetabling;

public class TimetableInput
{
    public TimetableInput(IEnumerable<Classroom> classrooms)
    {
        ArgumentNullException.ThrowIfNull(classrooms);

        Classrooms = [.. classrooms];
    }
    public IList<IConstraint> Constraints { get; } = [];
    public IList<Classroom> Classrooms { get; }
    public required int PeriodsPerDay { get; init; }
    public required int WorkingDaysCount { get; init; }
    public int MaxThreads { get; init; }
}