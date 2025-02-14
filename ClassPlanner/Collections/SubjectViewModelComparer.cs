using ClassPlanner.ViewModels;
using System.Collections.Generic;

namespace ClassPlanner.Collections;

public class SubjectViewModelComparer : IComparer<SubjectViewModel>
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Converter em expressão condicional", Justification = "<Pendente>")]
    public int Compare(SubjectViewModel? x, SubjectViewModel? y)
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

        return x.Name.CompareTo(y.Name);
    }
}
