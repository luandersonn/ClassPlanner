using System;
using System.Collections.Generic;

namespace ClassPlanner.Models;

public class Weekday
{
    public DayOfWeek DayOfWeek { get; set; }
    public List<int> Periods { get; set; } = [];
}

