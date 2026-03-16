using TaskManager.Domain.Enums;

namespace TaskManager.API.DTOs
{
    public sealed record TaskRequestParametersForGet(
        TaskPriority? _priority,
        bool? _isCompleted,
        bool? _isCanceled,
        DateTime? _deadLine);
}
