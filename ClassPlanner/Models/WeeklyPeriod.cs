using ClassPlanner.Data;

namespace ClassPlanner.Models;

public class WeeklyPeriod
{
    public int PeriodIndex { get; set; }
    public Subject? Monday { get; set; }
    public Subject? Tuesday { get; set; }
    public Subject? Wednesday { get; set; }
    public Subject? Thursday { get; set; }
    public Subject? Friday { get; set; }
}