﻿using BL.GenericResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace API.MiddleWare
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger;
        public ErrorHandlerMiddleware(RequestDelegate next , Serilog.ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new Response<string>() { Succeeded = false, Message = error?.Message };
                //Log.Error(error, error.Message, context.Request, "");
                _logger.Error(error, error.Message, context.Request, "");
                //TODO:: cover all validation errors
                switch (error)
                {
                    case UnauthorizedAccessException e:
                        // custom application error
                        responseModel.Message = error.Message;
                        responseModel.StatusCode = HttpStatusCode.Unauthorized;
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;

                    case ValidationException e:
                        // custom validation error
                        responseModel.Message = error.Message;
                        responseModel.StatusCode = HttpStatusCode.UnprocessableEntity;
                        response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        responseModel.Message = error.Message;
                        responseModel.StatusCode = HttpStatusCode.NotFound;
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    case DbUpdateException e:
                        // can't update error
                        responseModel.Message = e.Message;
                        responseModel.StatusCode = HttpStatusCode.BadRequest;
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case Exception e:
                        if (e.GetType().ToString()=="ApiException")
                        {
                            responseModel.Message+= e.Message;
                            responseModel.Message += e.InnerException == null ? "" : "\n" + e.InnerException.Message;
                            responseModel.StatusCode = HttpStatusCode.BadRequest;
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                        }
                        responseModel.Message = e.Message;
                        responseModel.Message += e.InnerException == null ? "" : "\n" + e.InnerException.Message;

                        responseModel.StatusCode = HttpStatusCode.InternalServerError;
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;

                    default:
                        // unhandled error
                        responseModel.Message = error.Message;
                        responseModel.StatusCode = HttpStatusCode.InternalServerError;
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
                var result = JsonSerializer.Serialize(responseModel);

                await response.WriteAsync(result);
            }
        }
    }

}
