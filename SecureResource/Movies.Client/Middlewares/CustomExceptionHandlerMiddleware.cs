using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Hosting;
using System.Linq;
using Newtonsoft.Json.Serialization;
using Movies.Client.Exceptions;
using System.IO;

namespace Movies.Client.Middlewares
{
    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }

    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next,
            IWebHostEnvironment env,
            ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            string message = null;
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
            ApiResultStatusCode apiStatusCode = ApiResultStatusCode.ServerError;

            try
            {
                await _next(context);
            }
            //catch (FluentValidation.ValidationException exception)
            //{
            //    httpStatusCode = HttpStatusCode.OK;
            //    apiStatusCode = ApiResultStatusCode.BadRequest;
            //    message = string.Join(" | ", exception.Errors.Select(e => e.ErrorMessage).ToList());
            //    await WriteToResponseAsync();
            //}
            catch (BadRequestException exception)
            {
                _logger.LogWarning(exception, exception.Message);
                message = exception.Message;
                httpStatusCode = HttpStatusCode.BadRequest;
                apiStatusCode = ApiResultStatusCode.BadRequest;
                await WriteToResponseAsync();
            }
            catch (FileNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.ContentType = "text/html;charset=utf-8";
                return;
            }
            catch (AppException exception)
            {
                _logger.LogError(exception, exception.Message);
                httpStatusCode = exception.HttpStatusCode;
                apiStatusCode = exception.ApiStatusCode;
                message = exception.Message;
                //if (_env.IsDevelopment())
                //{
                //    var dic = new Dictionary<string, string>
                //    {
                //        ["Exception"] = exception.Message,
                //        ["StackTrace"] = exception.StackTrace,
                //    };
                //    if (exception.InnerException != null)
                //    {
                //        dic.Add("InnerException.Exception", exception.InnerException.Message);
                //        dic.Add("InnerException.StackTrace", exception.InnerException.StackTrace);
                //    }
                //    if (exception.AdditionalData != null)
                //        dic.Add("AdditionalData", JsonConvert.SerializeObject(exception.AdditionalData));

                //    message = JsonConvert.SerializeObject(dic);
                //}
                //else
                //{
                //    message = exception.Message;
                //}
                await WriteToResponseAsync();
            }
            catch (SecurityTokenExpiredException exception)
            {
                _logger.LogError(exception, exception.Message);
                SetUnAuthorizeResponse(exception);
                await WriteToResponseAsync();
            }
            //catch (UnauthorizedAccessException exception)
            //{
            //    _logger.LogError(exception, exception.Message);
            //    SetUnAuthorizeResponse(exception);
            //    await WriteToResponseAsync();
            //}
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                message = "Internal Server Error";
                //if (_env.IsDevelopment())
                //{
                //    var dic = new Dictionary<string, string>
                //    {
                //        ["Exception"] = exception.Message,
                //        ["StackTrace"] = exception.StackTrace,
                //    };
                //    message = JsonConvert.SerializeObject(dic);
                //}
                await WriteToResponseAsync();
            }

            async Task WriteToResponseAsync()
            {
                if (context.Response.HasStarted)
                    throw new InvalidOperationException("The response has already started, the http status code middleware will not be executed.");

                var result = new ApiResult(false, apiStatusCode, message);
                var json = JsonConvert.SerializeObject(result, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                });

                context.Response.StatusCode = (int)httpStatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }

            void SetUnAuthorizeResponse(Exception exception)
            {
                httpStatusCode = HttpStatusCode.Unauthorized;
                apiStatusCode = ApiResultStatusCode.UnAuthorized;
                message = "UnAuthorized Access";
                //if (_env.IsDevelopment())
                //{
                //    var dic = new Dictionary<string, string>
                //    {
                //        ["Exception"] = exception.Message,
                //        ["StackTrace"] = exception.StackTrace
                //    };
                //    if (exception is SecurityTokenExpiredException tokenException)
                //        dic.Add("Expires", tokenException.Expires.ToString());

                //    message = JsonConvert.SerializeObject(dic);
                //}
            }
        }
    }
}
