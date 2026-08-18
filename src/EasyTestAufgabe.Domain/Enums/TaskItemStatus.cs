namespace EasyTestAufgabe.Domain.Enums
{
    /// <summary>
    /// Status einer Aufgabe. Bewusst "TaskItemStatus" statt "TaskStatus" genannt,
    /// um Namenskollisionen mit System.Threading.Tasks.TaskStatus zu vermeiden.
    /// </summary>
    public enum TaskItemStatus
    {
        Open = 0,
        InProgress = 1,
        Done = 2
    }
}
