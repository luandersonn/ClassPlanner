using System;

namespace ClassPlanner.Timetabling;

public class TimetableFoundEventArgs(Timetable timetable) : EventArgs
{
    public Timetable Timetable { get; } = timetable;
    public bool Cancel { get; set; }
}
