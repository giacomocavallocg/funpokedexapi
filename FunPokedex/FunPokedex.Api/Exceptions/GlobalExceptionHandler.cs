
using FunPokedex.Api.Models;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FunPokedex.Api.Exceptions
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            ErrorDto errorDetails = ErrorDto.InternalServerError(context.Exception.Message);

            context.Result = new JsonResult(errorDetails)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}
