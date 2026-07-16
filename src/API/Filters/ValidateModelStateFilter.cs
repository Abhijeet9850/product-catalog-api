using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    public class ValidateModelStateFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray());

                context.Result = new BadRequestObjectResult(new
                {
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest,
                    Errors = errors,
                    TraceId = context.HttpContext.TraceIdentifier
                });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}