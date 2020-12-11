using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;

namespace ProcuretAPI
{

    public class ApiRequestException : Exception
    {
        public ApiRequestException() { }

        public ApiRequestException(string message) : base(message) { }
    }

    internal struct ApiRequest { 

        private const String signatureHeader = "x-procuret-signature";
        private const String idHeader = "x-procuret-session-id";
        private const String apiEndpoint = "https://procuret.com/api";

        private static readonly HttpClient httpClient = new HttpClient();

        internal static async Task<String> Make<T> (
            String path,
            T body,
            Session? session,
            HttpMethod method,
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

            return await ApiRequest.Make(
                path,
                xmlBody,
                session,
                method,
                httpClient
            );

        }

        private static async Task<String> SendRequest(
            String path,
            String pathAndQuery,
            HttpRequestMessage message,
            Session? session,
            HttpMethod method,
            HttpClient httpClient = null
        )
        {

            message.Method = method;
            message.RequestUri = new Uri(
                ApiRequest.apiEndpoint + pathAndQuery
            );

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

            if (httpClient == null)
            {
                httpClient = ApiRequest.httpClient;
            }

            HttpResponseMessage response = await httpClient.SendAsync(
                message
            );

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiRequestException("API Error");
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;

        }

        internal static async Task<String> Make (
            String path,
            QueryString query,
            Session? session,
            HttpMethod method,
            HttpClient httpClient = null
        )
        {

            String pathAndQuery = path + query.Query;

            HttpRequestMessage message = new HttpRequestMessage();

            return await ApiRequest.SendRequest(
                path,
                pathAndQuery,
                message,
                session,
                method,
                httpClient
            );

        }


        internal static async Task<String> Make (
            String path,
            String bodyXmlData,
            Session? session,
            HttpMethod method,
            HttpClient httpClient = null
        )
        {

            String requestPath = "https://procuret.com/api" + path;
            StringContent content = new StringContent(
                bodyXmlData,
                System.Text.Encoding.UTF8,
                "application/xml"
            );

            HttpRequestMessage message = new HttpRequestMessage();

            message.Content = content;

            return await ApiRequest.SendRequest(
                path,
                path,
                message,
                session,
                method,
                httpClient
            );

            
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

        internal static T DecodeResponse<T>(String data)
        {
            var dcs = new DataContractSerializer(typeof(T));
            var reader = new StringReader(data);
            var xmlReader = XmlReader.Create(reader);
            var responsePayload = (T)dcs.ReadObject(xmlReader);
            xmlReader.Close();

            return responsePayload;

        }

    }
}
