using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace AndroidTaskerPlugin
{
    public static class HttpListenerResponseExtension
    {
        public static async Task SendResponse(this HttpListenerResponse response, string message, HttpStatusCode httpStatusCode)
        {
            var isError = httpStatusCode != HttpStatusCode.Accepted ||
                httpStatusCode != HttpStatusCode.Created ||
                httpStatusCode != HttpStatusCode.Found ||
                httpStatusCode != HttpStatusCode.NoContent ||
                httpStatusCode != HttpStatusCode.OK;

            response.StatusCode = (int)httpStatusCode;
            using (var stream = new MemoryStream())
            {
                response.ContentType = "application/json;charset=utf-8";
                string json = JsonConvert.SerializeObject(new
                {
                    error = isError,
                    message = message
                });

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(json);
                stream.Write(buffer, 0, buffer.Length);
                byte[] bytes = stream.ToArray();
                if (response.OutputStream != null && response.OutputStream.CanWrite)
                {
                    await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                }
            }
        }

    }
}
