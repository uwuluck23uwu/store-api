using System.Net;

namespace ClassLibrary.Models.Response
{
    public class ResponseMessage : ResponseBase
    {
        public ResponseMessage(HttpStatusCode statusCode, bool taskStatus, string message)
            : base(statusCode, taskStatus, message) { }
    }
}
