using ClassPlanner.Timetabling;
using System.Collections.Generic;

namespace ClassPlanner.Collections;

public class TimetableMinObjectiveValueComparer : IComparer<Timetable>
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Converter em expressão condicional", Justification = "<Pendente>")]
    public int Compare(Timetable? x, Timetable? y)
    {
        if (x is null && y is null)
        {
            return 0;
        }

        if (x is null)
        {
            return 1;
        }

        if (y is null)
        {
            return -1;
        }

        return -x.ObjectiveValue.CompareTo(y.ObjectiveValue);
    }
}
