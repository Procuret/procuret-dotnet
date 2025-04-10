using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json; // <-- for JSON
using System.Text.Json.Serialization; // <-- only if you need property naming attributes

namespace ProcuretAPI
{
    public class ApiRequestException : Exception
    {
        public ApiRequestException() { }
        public ApiRequestException(string message) : base(message) { }
    }

    internal struct ApiRequest
    {
        private const string SignatureHeader = "x-procuret-signature";
        private const string IdHeader = "x-procuret-session-id";
        private const string ApiEndpoint = "https://procuret.com/api";

        private static readonly HttpClient SharedClient = new HttpClient();

        internal static async Task<string> Make<T>(
            string path,
            T body,
            Session? session,
            HttpMethod method,
            HttpClient httpClient = null
        )
        {
            string jsonBody = JsonSerializer.Serialize(body);

            return await Make(path, jsonBody, session, method, httpClient);
        }

        internal static async Task<string> Make(
            string path,
            string bodyJsonData,
            Session? session,
            HttpMethod method,
            HttpClient httpClient = null
        )
        {

            string requestPath = $"{ApiEndpoint}{path}";

            var content = new StringContent(
                bodyJsonData,
                Encoding.UTF8,
                "application/json"
            );

            HttpRequestMessage message = new HttpRequestMessage
            {
                Content = content
            };

            return await SendRequest(
                path,        // the base path
                path,        // pathAndQuery = no query used here
                message,
                session,
                method,
                httpClient
            );
        }

        internal static async Task<string> Make(
            string path,
            QueryString query,
            Session? session,
            HttpMethod method,
            HttpClient httpClient = null
        )
        {
            // Combine the path with the query string
            string pathAndQuery = path + query.Query;

            HttpRequestMessage message = new HttpRequestMessage();

            // Send request using the helper
            return await SendRequest(
                path,
                pathAndQuery,
                message,
                session,
                method,
                httpClient
            );
        }

        private static async Task<string> SendRequest(
            string path,
            string pathAndQuery,
            HttpRequestMessage message,
            Session? session,
            HttpMethod method,
            HttpClient httpClient = null
        )
        {
            message.Method = method;
            message.RequestUri = new Uri(ApiEndpoint + pathAndQuery);

            message.Headers.Add("accept", "application/json");

            if (session != null)
            {
                message.Headers.Add(IdHeader, session?.sessionId.ToString());
                message.Headers.Add(
                    SignatureHeader,
                    ComputeSignature(path, session?.apiKey)
                );
            }

            if (httpClient == null)
            {
                httpClient = SharedClient;
            }
    
            #if DEBUG
            // Simplify debugging in non-threaded dev environments
            httpClient.DefaultRequestHeaders.ConnectionClose = true;
            #endif

            HttpResponseMessage response = await httpClient.SendAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiRequestException($"API Error{response.StatusCode}");
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        private static string ComputeSignature(
            string path,
            string key
        )
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            using var hmac = new HMACSHA256(keyBytes);

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            // Round to nearest 900-second block, as before
            string time = (timestamp - (timestamp % 900)).ToString();
            string payload = time + path;

            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
            byte[] hashBytes = hmac.ComputeHash(payloadBytes);
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Decode a JSON payload into a strongly-typed object T.
        /// </summary>
        internal static T DecodeResponse<T>(string data)
        {
            return JsonSerializer.Deserialize<T>(data);
        }
    }
}