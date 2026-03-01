using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Shared
{
    public class Error
    {
        public string Code { get; }
        public ErrorType errorType { get; }
        public string Message { get; init; }
        
        public Error(string code, ErrorType errorType, string message)
        {
            Code = code;
            this.errorType = errorType;
            Message = message;
        }
    }
}
