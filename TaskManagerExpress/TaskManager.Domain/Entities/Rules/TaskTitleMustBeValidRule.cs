using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities.Errors;
using TaskManager.Domain.Shared;

namespace TaskManager.Domain.Entities.Rules
{
    //To validate title
    public class TaskTitleMustBeValidRule(string tasktitle) : IBusinessRule
    {
        public Result Check()
        {
            if(tasktitle.Length > 90)
            {
                return Result.WithError(TaskErrors.TaskTitleIsNotValid);
            }

            return Result.Success();
        }
    }
}
