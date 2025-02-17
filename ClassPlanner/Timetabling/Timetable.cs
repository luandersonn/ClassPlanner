using ClassPlanner.Models;
using ClassPlanner.Timetabling.Validation;
using System;
using System.Collections.Generic;

namespace ClassPlanner.Timetabling;

public class Timetable
{
    public List<ClassSchedule> ClassSchedules { get; } = [];
    public double ObjectiveValue { get; set; }

    public override string ToString() => $"Timetable {ObjectiveValue}";

    public required TimeSpan SolutionTime { get; set; }

    public string SolutionTimeFormatted => SolutionTime.ToString(@"hh\:mm\:ss");
    public List<TimetableValidationResult> ValidationResults { get; } = [];
}