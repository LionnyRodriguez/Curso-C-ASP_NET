using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities.Errors;
using TaskManager.Domain.Shared;

namespace TaskManager.Domain.Entities.Rules
{
    public class TaskCannotBeCompletedAndCanceled(bool CompletedOrCanceled) :IBusinessRule
    {
        public Result Check()
        {
            if(CompletedOrCanceled == true)
            {
                return Result.WithError(TaskErrors.TaskBeCompletedAndIsCanceled);
            }
            return Result.Success();
        }
    }
}
