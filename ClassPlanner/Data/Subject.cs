namespace ClassPlanner.Data;

public class Subject
{
    public long SubjectId { get; set; }
    public string Name { get; set; } = null!;
    public long? ClassroomId { get; set; }
    public Classroom Classroom { get; set; } = null!;
    public long? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
    public int PeriodsPerWeek { get; set; }
}

