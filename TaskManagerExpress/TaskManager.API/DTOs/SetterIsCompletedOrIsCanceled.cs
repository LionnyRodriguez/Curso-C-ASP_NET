namespace TaskManager.API.DTOs
{
    public sealed record SetterIsCompletedOrIsCanceled(bool? _isCompleted, bool? _isCanceled);
}
