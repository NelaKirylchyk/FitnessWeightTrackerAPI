using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using FitnessWeightTrackerAPI.CustomExceptions;

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
        }
    }
}
