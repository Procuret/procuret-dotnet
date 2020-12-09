using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Serialization;

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

        internal static async Task<String> MakeAsyncPost<T> (
            String path,
            T body,
            Session? session,
            HttpClient httpClient = null
        )
        {

            String xmlBody;
            using (MemoryStream stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(stream, body);
                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(stream))
                {
                    xmlBody = reader.ReadToEnd();
                }


            }

            return await ApiRequest.MakeAsyncPost(
                path,
                xmlBody,
                session,
                httpClient
            );

        }


        internal static async Task<String> MakeAsyncPost (
            String path,
            String bodyXmlData,
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
                bodyXmlData,
                System.Text.Encoding.UTF8,
                "application/xml"
            );

            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri(requestPath);
            message.Content = content;

            message.Headers.Add("accept", "application/xml");

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

            HttpResponseMessage response = await httpClient.SendAsync(
                message
            );

            if (!response.IsSuccessStatusCode) {
                throw new ApiRequestException("API Error");
            }

            string responseBody = await response.Content.ReadAsStringAsync();

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

            String time = (timestamp - (timestamp % 900)).ToString();
            String payload = time + path;

            byte[] payloadBytes = Encoding.UTF8.GetBytes(
                payload
            );

            byte[] hashBytes = hmac.ComputeHash(payloadBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
