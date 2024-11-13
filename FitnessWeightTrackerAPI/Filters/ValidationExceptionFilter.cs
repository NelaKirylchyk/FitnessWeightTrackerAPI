using FitnessWeightTrackerAPI.CustomExceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FitnessWeightTrackerAPI.Filters
{
    public class ValidationExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is CustomValidationException validationException)
            {
                var errors = validationException.ValidationResults.Select(vr => vr.ErrorMessage).ToList();
                var result = new
                {
                    Message = validationException.Message,
                    Errors = errors
                };
                context.Result = new BadRequestObjectResult(result);
                context.ExceptionHandled = true;
            }
            else
            {
                // Handle other types of exceptions here
                context.Result = new ObjectResult("An unexpected error occurred.")
                {
                    StatusCode = 500
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
