using TaskManager.Domain.Common;
using TaskManager.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.API.Extensions
{
    public static class ControllerExtensions
    {
        public static ActionResult FromResult<T>(this ControllerBase controller, Result<T> result)
        {
            return FromResult(controller, result.ToResult());       
        }

        public static ActionResult FromResult(this ControllerBase controller, Result result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot convert into an error action result a success domain result.");

            string message = string.Empty;
            foreach (var error in result.Errors)
            {
                message += $"{error.Code}: {error.Message}";
            }

            var errorType = result.Errors.First().errorType;

            return controller.Problem(
                statusCode: GetStatus(errorType),
                title: GetTitle(errorType),
                detail: message);
        }

        private static int GetStatus(ErrorType _errorType)
            => _errorType switch
            {
                ErrorType.Failure => StatusCodes.Status500InternalServerError,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => throw new NotImplementedException(),
            };

        private static string GetTitle(ErrorType _errorType)
            => _errorType switch
            {
                ErrorType.Failure => "InternalServerError",
                ErrorType.Validation => "BadRequest",
                ErrorType.NotFound => "NotFound",
                ErrorType.Conflict => "Conflict",
                ErrorType.Unauthorized => "Unauthorized",
                ErrorType.Forbidden => "Forbidden",
                _ => throw new NotImplementedException(),
            };
    }
}
