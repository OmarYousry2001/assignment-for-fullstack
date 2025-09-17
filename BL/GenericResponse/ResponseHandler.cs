
using Resources;

namespace BL.GenericResponse

{
    public class ResponseHandler
    {
        public ResponseHandler()
        {
        }
        public Response<T> Deleted<T>(string Message = null)
        {
            return new Response<T>()
            {
                //StatusCode = System.Net.HttpStatusCode.OK,
                Type = ResponseType.Success,
                Succeeded = true,
                Message = Message == null ? NotifiAndAlertsResources.DeletedSuccessfully : Message
            };
        }
        public Response<T> Success<T>(T entity , object Meta = null)
        {
            return new Response<T>()
            {
                Data = entity,
                //StatusCode = System.Net.HttpStatusCode.OK,
                Type= ResponseType.Success, 
                Succeeded = true,
                Message = NotifiAndAlertsResources.Success,
                Meta = Meta
            };
        }
        public Response<T> Unauthorized<T>(string Message = null)
        {
            return new Response<T>()
            {
                //StatusCode = System.Net.HttpStatusCode.Unauthorized,
                Type = ResponseType.Unauthorized,
                Succeeded = true,
                Message = Message == null ? NotifiAndAlertsResources.UnauthorizedAccess : Message
            };
        }
        public Response<T> BadRequest<T>(string Message = null)
        {
            return new Response<T>()
            {
                //StatusCode = System.Net.HttpStatusCode.BadRequest,
                Type = ResponseType.BadRequest,
                Succeeded = false,
                Message = Message == null ? NotifiAndAlertsResources.BadRequest : Message
            };
        }

        public Response<T> UnprocessableEntity<T>(string Message = null)
        {
            return new Response<T>()
            {
                //StatusCode = System.Net.HttpStatusCode.UnprocessableEntity,
                Type = ResponseType.Unprocessable,
                Succeeded = false,
                Message = Message == null ? NotifiAndAlertsResources.UnprocessableEntity : Message
            };
        }


        public Response<T> NotFound<T>(string message = null)
        {
            return new Response<T>()
            {
                //StatusCode = System.Net.HttpStatusCode.NotFound,
                Type = ResponseType.NotFound,
                Succeeded = false,
                Message = message == null ? NotifiAndAlertsResources.NotFound : message
            };
        }

        public Response<T> Created<T>(T entity, object Meta = null)
        {
            return new Response<T>()
            {
                Data = entity,
                //StatusCode = System.Net.HttpStatusCode.Created,
                Type = ResponseType.Created,
                Succeeded = true,
                Message = NotifiAndAlertsResources.CreatedSuccessfully,
                Meta = Meta
            };
        }
    }


}
