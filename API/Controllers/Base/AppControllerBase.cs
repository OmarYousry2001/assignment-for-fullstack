using BL.GenericResponse;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace API.Base
{
    [ApiController]
    public class AppControllerBase : ControllerBase
    {
        protected string? UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        protected Guid GuidUserId =>
       Guid.TryParse(UserId, out var guid) ? guid : Guid.NewGuid();

        protected string? UserEmail => User.FindFirst(ClaimTypes.Email)?.Value;

        #region Actions

        public ObjectResult NewResult<T>(Response<T> response)
        {
            var statusCode = GetStatusCode(response.Type);
            response.StatusCode = (HttpStatusCode)statusCode;

            return new ObjectResult(response)
            {
                StatusCode = statusCode
            };
        }

        private int GetStatusCode(ResponseType type)
        {
            return type switch
            {
                ResponseType.Success => StatusCodes.Status200OK,
                ResponseType.Created => StatusCodes.Status201Created,
                ResponseType.Accepted => StatusCodes.Status202Accepted,
                ResponseType.NotFound => StatusCodes.Status404NotFound,
                ResponseType.Unauthorized => StatusCodes.Status401Unauthorized,
                ResponseType.BadRequest => StatusCodes.Status400BadRequest,
                ResponseType.Unprocessable => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status400BadRequest
            };
        }

        #endregion



    }
}
