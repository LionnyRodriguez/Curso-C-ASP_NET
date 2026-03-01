using TaskManager.Domain.Enums;

namespace TaskManager.API.DTOs
{
    public sealed record UpdateTaskRequest(
        string? _TaskTitle,
        string? _TaskText,
        TaskPriority? _TaskPriority,
        DateTime? _DeadLine);
}
