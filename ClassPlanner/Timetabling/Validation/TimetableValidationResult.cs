using System.Collections.Generic;

namespace ClassPlanner.Timetabling.Validation;

public class TimetableValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public List<string> Errors { get; } = [];

    public void AddError(string errorMessage)
    {
        Errors.Add(errorMessage);
    }
}