using System.Collections.Generic;

namespace ClassPlanner.Data;

public class Classroom
{
    public long ClassroomId { get; set; }
    public string Name { get; set; } = null!;
    public List<Subject> Subjects { get; set; } = [];
}

