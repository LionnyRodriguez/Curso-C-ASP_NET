using TaskManager.Domain.Shared;

namespace TaskManager.Domain.Entities.Errors
{
    public static class TaskErrors
    {
        public static Error TaskDueDateBePast
            => new Error("Task.TaskDueDatebePast",
                ErrorType.Validation,
                "The task Deadline cannot be missed.");

        public static Error TaskPriorityIsNotValid
            => new Error(
                "Task.TaskPriorityIsNotValid",
                ErrorType.Validation,
                "Task priority must be valid.");

        public static Error TaskTitleIsNotValid
            => new Error("Task.TaskTitleIsNotValid",
                ErrorType.Validation,
                "The assignment title must not exceed 90 characters.");

        public static Error TaskBeCompletedAndIsCanceled
            => new Error("Task.TaskCannotBeCompletedIfIsCanceled",
                ErrorType.Conflict,
                "Task cannot be Completed and Canceled at the same time.");
    }
}
