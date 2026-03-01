using TaskManager.Domain.Enums;

namespace TaskManager.API.DTOs
{
    public sealed record CreateTaskRequest(
        string _TaskTitle,
        string? _TaskText,
        TaskPriority? _TaskPriority,
        DateTime? _DeadLine);
}
