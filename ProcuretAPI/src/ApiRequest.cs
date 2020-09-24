using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace ProcuretAPI
{

    public class ApiRequestException : Exception
    {
        public ApiRequestException() { }

        public ApiRequestException(string message) : base(message) { }
    }

    internal class ApiRequest { 

        private const String signatureHeader = "x-procuret-signature";
        private const String idHeader = "x-procuret-session-id";

        private static readonly HttpClient httpClient = new HttpClient();


        internal static async Task<String> MakeAsyncPost (
            String path,
            String bodyJsonData,
            Session? session,
            HttpClient httpClient = null
        )
        {

            if (httpClient == null)
            {
                httpClient = ApiRequest.httpClient;
            }

            String requestPath = "https://procuret.com/api" + path;
            StringContent content = new StringContent(
                bodyJsonData,
                System.Text.Encoding.UTF8,
                "application/json"
            );

            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri(requestPath);
            message.Content = content;

            if (session != null)
            {
                message.Headers.Add(idHeader, session?.sessionId.ToString());
                message.Headers.Add(
                    signatureHeader,
                    ApiRequest.ComputeSignature(
                        path: path,
                        key: session?.apiKey
                    )
                );
            }

            System.Diagnostics.Debug.WriteLine("Before request");
            HttpResponseMessage response = await httpClient.SendAsync(
                message
            );
            System.Diagnostics.Debug.WriteLine("After request");

            if (!response.IsSuccessStatusCode) {
                throw new ApiRequestException("API Error");
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine("After Read");

            return responseBody;
            
        }

        private static String ComputeSignature(
            String path,
            String key
        )
        {

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            HMACSHA256 hmac = new HMACSHA256(keyBytes);

            DateTime foo = DateTime.UtcNow;
            long timestamp = ((DateTimeOffset)foo).ToUnixTimeSeconds();

            System.Diagnostics.Debug.WriteLine("### TIMESTAMP ###");
            System.Diagnostics.Debug.WriteLine(timestamp);

            String time = (timestamp - (timestamp % 900)).ToString();
            String payload = time + path;

            System.Diagnostics.Debug.WriteLine("### PAYLOAD ###");
            System.Diagnostics.Debug.WriteLine(payload);

            byte[] payloadBytes = Encoding.UTF8.GetBytes(
                payload
            );

            byte[] hashBytes = hmac.ComputeHash(payloadBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
