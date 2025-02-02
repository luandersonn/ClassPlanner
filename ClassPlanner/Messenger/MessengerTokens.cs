namespace ClassPlanner.Messenger;

public static class MessengerTokens
{
    public static string SubjectAdded { get; } = "SubjectAdded";
    public static string SubjectRemoved { get; } = "SubjectRemoved";
    public static string SubjectUpdated { get; } = "SubjectUpdated";

    public static string TeacherAdded { get; } = "TeacherAdded";
    public static string TeacherRemoved { get; } = "TeacherRemoved";
    public static string TeacherUpdated { get; } = "TeacherUpdated";

    public static string ClassroomAdded { get; } = "ClassroomAdded";
    public static string ClassroomRemoved { get; } = "ClassroomRemoved";
    public static string ClassroomUpdated { get; } = "ClassroomUpdated";
}

public class ItemAddedMessage<T>
{
    public required T Item { get; init; }
}