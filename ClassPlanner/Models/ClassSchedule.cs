using ClassPlanner.Data;
using System.Collections.Generic;

namespace ClassPlanner.Models;

public class ClassSchedule
{
    public Classroom Classroom { get; set; } = null!;
    public List<SubjectSchedule> SubjectSchedules { get; set; } = [];
    public int PeriodsPerDay { get; set; }
}
