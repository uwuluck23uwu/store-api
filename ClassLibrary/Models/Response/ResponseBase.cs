using System.Net;
using System.Text.Json.Serialization;

namespace ClassLibrary.Models.Response
{
    public abstract class ResponseBase
    {
        [JsonPropertyOrder(-3)]
        [JsonPropertyName("statusCode")]
        public HttpStatusCode StatusCode { get; set; }

        [JsonPropertyOrder(-2)]
        [JsonPropertyName("isSuccess")]
        public bool TaskStatus { get; set; }

        [JsonPropertyOrder(-1)]
        [JsonPropertyName("message")]
        public string Message { get; set; }

        protected ResponseBase(HttpStatusCode statusCode, bool taskStatus, string message)
        {
            StatusCode = statusCode;
            TaskStatus = taskStatus;
            Message = message;
        }
    }
}
