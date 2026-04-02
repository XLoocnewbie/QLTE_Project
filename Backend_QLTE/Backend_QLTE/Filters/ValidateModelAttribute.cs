using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend_QLTE.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        // Custom action filter để validate model tự động trả response đồng nhất.
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                context.Result = new BadRequestObjectResult(new
                {
                    status = false,
                    msg = string.Join("; ", errors),
                    data = (object)null
                });
            }
        }
    }
}
