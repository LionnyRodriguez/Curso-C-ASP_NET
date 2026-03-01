using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Shared
{
    public abstract class CheckableObject
    {
        public static Result CheckRules(params IBusinessRule[] rules)
        {
            return Result.Merge(rules.Select(r => r.Check()).ToArray());
        }
    }
}
