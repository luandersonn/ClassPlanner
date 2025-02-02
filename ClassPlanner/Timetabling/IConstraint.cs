namespace ClassPlanner.Timetabling;

public interface IConstraint
{
    void Register(TimetableInput input, TimetableModel model);
}
