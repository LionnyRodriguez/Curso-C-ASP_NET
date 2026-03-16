using TaskManager.Domain.Enums;

namespace TaskManager.API.DTOs
{
    public sealed record TaskResponse(
        Guid _Id,
        string _TaskTitle,
        string _TaskText,
        TaskPriority? _TaskPriority,
        bool? _IsCompleted,
        bool? _IsCanceled,
        DateTime? _DeadLine
        );
    
}
