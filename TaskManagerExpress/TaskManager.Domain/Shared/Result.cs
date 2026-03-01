using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Shared
{
    public class Result
    {
        private List<Error> errors = [];

        public IReadOnlyCollection<Error> Errors => errors;

        public bool IsFailure => errors.Count > 0;

        public bool IsSuccess => errors.Count == 0;

        internal Result() { }

        public static Result Success()
        {
            var result = new Result();
            return result;
        }
        public static Result<T> Success<T>(T value)
        {
            var result = new Result<T>();
            result.Value = value;
            return result;
        }

        public static Result WithErrors(IEnumerable<Error> errors)
        {
            var result = new Result();
            result.errors.AddRange(errors);
            return result;
        }

        public static Result WithError(Error error)
        {
            var result = new Result();
            result.errors.Add(error);
            return result;
        }
        public static Result Merge(params Result[] results)
        {
            var mergedResult = Success();
            foreach(var result in results)
            {
                mergedResult.errors.AddRange(result.Errors);    
            }
            return mergedResult;
        }
    }

    public class Result<T>
    {
        private List<Error> errors = [];

        public IReadOnlyCollection<Error> Errors => errors;

        public bool IsFailure => errors.Count > 0;

        public bool IsSuccess => errors.Count == 0;

        public T? Value { get; internal set; } = default;

        public static Result<T> WithErrors(IEnumerable<Error> errors)
        {
            var result = new Result<T>();
            result.errors.AddRange(errors);
            return result;
        }
        public static Result<T> WithError(Error error)
        {
            var result = new Result<T>();
            result.errors.Add(error);
            return result;
        }

        public static implicit operator Result<T>(Result result)
        {
            return Result.WithErrors(result.Errors);
        }

        public Result ToResult()
        {
            return Result.WithErrors(errors);
        }
    }
}
