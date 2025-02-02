using ClassPlanner.Models;
using System.Collections.Generic;

namespace ClassPlanner.Timetabling;

public class Timetable
{
    public List<ClassSchedule> ClassSchedules { get; } = [];
    public double ObjectiveValue { get; set; }

    public override string ToString() => $"Timetable {ObjectiveValue}";
}