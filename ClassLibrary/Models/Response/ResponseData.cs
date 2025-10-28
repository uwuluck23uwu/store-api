using System.Net;
using System.Text.Json.Serialization;

namespace ClassLibrary.Models.Response
{
    public class ResponseData : ResponseBase
    {
        [JsonPropertyName("data")]
        public object? Data { get; set; }

        public ResponseData(HttpStatusCode statusCode, bool taskStatus, string message, object? data = null)
            : base(statusCode, taskStatus, message)
        {
            Data = data;
        }
    }
}
