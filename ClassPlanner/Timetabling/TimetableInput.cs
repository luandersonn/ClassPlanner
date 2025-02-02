using ClassPlanner.Data;
using ClassPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassPlanner.Timetabling;

public class TimetableInput
{
    public TimetableInput(IEnumerable<Classroom> classrooms, IEnumerable<Weekday> weekdays, int periodsPerDay)
    {
        ArgumentNullException.ThrowIfNull(classrooms);

        ArgumentNullException.ThrowIfNull(weekdays);

        Classrooms = classrooms.ToList();
        Weekdays = weekdays.ToList();
        PeriodsPerDay = periodsPerDay;
    }
    public IList<IConstraint> Constraints { get; } = [];
    public IList<Classroom> Classrooms { get; }
    public IList<Weekday> Weekdays { get; }
    public int PeriodsPerDay { get; }
}