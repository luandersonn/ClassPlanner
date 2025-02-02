using ClassPlanner.Data;
using System;

namespace ClassPlanner.Models;

public class SubjectSchedule
{
    public Subject Subject { get; set; } = null!;
    public Teacher? Teacher { get; set; }
    public DayOfWeek Day { get; set; }
    public int Period { get; set; }
}
