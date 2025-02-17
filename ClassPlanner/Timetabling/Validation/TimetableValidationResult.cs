using System.Collections.Generic;

namespace ClassPlanner.Timetabling.Validation;

public class TimetableValidationResult
{
    public ValidationResultType Result { get; set; }
    public required string Title { get; set; }
    public List<string> Errors { get; } = [];
    public void AddError(string errorMessage)
    {
        Errors.Add(errorMessage);
    }
}