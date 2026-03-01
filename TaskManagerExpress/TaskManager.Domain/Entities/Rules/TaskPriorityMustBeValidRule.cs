using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities.Errors;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Shared;

namespace TaskManager.Domain.Entities.Rules
{
    public class TaskPriorityMustBeValidRule(TaskPriority _priority) : IBusinessRule
    {
        public Result Check()
        {
            if(_priority!=TaskPriority.Low
                &&  _priority!=TaskPriority.Medium
                &&  _priority!=TaskPriority.High
                && _priority != TaskPriority.Critical)
            {
                return Result.WithError(TaskErrors.TaskPriorityIsNotValid);
            }
            return Result.Success();
        }
    }
}
