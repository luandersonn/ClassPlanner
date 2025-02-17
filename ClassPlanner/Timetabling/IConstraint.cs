using ClassPlanner.Timetabling.Validation;

namespace ClassPlanner.Timetabling;

public interface IConstraint
{
    void Register(TimetableInput input, TimetableModel model);
    TimetableValidationResult Validate(TimetableInput input, Timetable timetable);
}