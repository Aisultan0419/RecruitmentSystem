using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace RecruitmentSystemAPI.Extensions
{
    public static class ResultExtensions //not really perfect yet, but I will consider it later
    {
        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
            {
                return new NoContentResult();
            }
            return BuildErrorResult(result.Errors); 
        }

        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Value);
            }
            return BuildErrorResult(result.Errors);
        }

        private static IActionResult BuildErrorResult(IReadOnlyList<IError> errors)
        {
            var statusCode = errors
                .Select(error => error.Metadata.TryGetValue("StatusCode", out var v) && v is int code ? code : (int?)null)
                .FirstOrDefault(er => er is not null) ?? StatusCodes.Status400BadRequest;

            return new ObjectResult(new { errors = errors.Select(e => e.Message) })
            {
                StatusCode = statusCode
            };
        }
    }
}
