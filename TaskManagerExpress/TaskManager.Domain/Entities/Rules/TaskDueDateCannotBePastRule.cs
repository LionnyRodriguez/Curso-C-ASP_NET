using TaskManager.Domain.Entities.Errors;
using TaskManager.Domain.Shared;

namespace TaskManager.Domain.Entities.Rules
{
    
    public class TaskDueDateCannotBePastRule(DateTime testDeadLine) : IBusinessRule
    {
        public Result Check()
        {
            if (testDeadLine < DateTime.Today)
            {
                return Result.WithError(TaskErrors.TaskDueDateBePast);
            }
            return Result.Success();
        }
    }
}
