using ClassPlanner.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassPlanner.Timetabling;

public class TimetableInput
{
    public TimetableInput(IEnumerable<Classroom> classrooms, int periodsPerDay, int workingDaysCount)
    {
        ArgumentNullException.ThrowIfNull(classrooms);

        Classrooms = classrooms.ToList();
        PeriodsPerDay = periodsPerDay;
        WorkingDaysCount = workingDaysCount;
    }
    public IList<IConstraint> Constraints { get; } = [];
    public IList<Classroom> Classrooms { get; }
    public int PeriodsPerDay { get; }
    public int WorkingDaysCount { get; }
}