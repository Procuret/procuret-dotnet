using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json.Serialization; // If you want [JsonPropertyName]

namespace ProcuretAPI
{
    public struct ProspectivePayment
    {
        internal const string Path = "/credit/prospective-payment";
        internal const string ListPath = Path + "/list";

        public readonly decimal RecurringPayment;
        public readonly long SupplierId;
        public readonly short PaymentCount;
        public readonly Period Period;
        public readonly Cycle Cycle;

        internal ProspectivePayment(
            decimal recurringPayment,
            long supplierId,
            short paymentCount,
            Cycle cycle
        )
        {
            RecurringPayment = recurringPayment;
            SupplierId = supplierId;
            PaymentCount = paymentCount;
            Period = Period.MONTH; // Only MONTH is used
            Cycle = cycle;
        }

        /// <summary>
        /// Retrieve a single ProspectivePayment from the API.
        /// </summary>
        public static async Task<ProspectivePayment> Retrieve(
            Session session,
            long supplierId,
            decimal principle,
            short paymentCount,
            HttpClient httpClient = null
        )
        {
            QueryParameter[] parameters = {
                new QueryParameter($"{supplierId}", "supplier_id"),
                new QueryParameter(principle.ToString(), "principle"),
                new QueryParameter(paymentCount, "periods"),
                new QueryParameter((short)Cycle.ADVANCE, "cycle")
            };

            QueryString query = new QueryString(parameters);

            string resultBody = await ApiRequest.Make(
                Path,
                query,
                session,
                HttpMethod.Get,
                httpClient
            );

            // Decode JSON response
            var responsePayload = ApiRequest.DecodeResponse<ResponsePayload>(resultBody);

            return new ProspectivePayment(
                Convert.ToDecimal(responsePayload.payment),
                responsePayload.supplier_id,
                responsePayload.periods,
                (Cycle)responsePayload.cycle
            );
        }

        /// <summary>
        /// Retrieve multiple ProspectivePayments from the API.
        /// </summary>
        public static async Task<ProspectivePayment[]> RetrieveMany(
            Session session,
            long supplierId,
            decimal principle,
            HttpClient httpClient = null
        )
        {
            QueryParameter[] parameters = {
                new QueryParameter(supplierId, "supplier_id"),
                new QueryParameter(principle.ToString(), "principle"),
                new QueryParameter((short)Cycle.ADVANCE, "cycle")
            };

            QueryString query = new QueryString(parameters);

            string resultBody = await ApiRequest.Make(
                ListPath,
                query,
                session,
                HttpMethod.Get,
                httpClient
            );

            // Decode an array of ResponsePayload
            var responsePayloads = ApiRequest.DecodeResponse<ResponsePayload[]>(resultBody);

            var resultList = new List<ProspectivePayment>();
            foreach (ResponsePayload payload in responsePayloads)
            {
                resultList.Add(new ProspectivePayment(
                    Convert.ToDecimal(payload.payment),
                    payload.supplier_id,
                    payload.periods,
                    (Cycle)payload.cycle
                ));
            }

            return resultList.ToArray();
        }

        /// <summary>
        /// This class matches the JSON the server returns. 
        /// We'll parse it into a struct or class, then 
        /// create ProspectivePayment from those fields.
        /// </summary>
        internal class ResponsePayload
        {
            // If the API’s JSON keys differ from these property names, 
            // you can annotate them with [JsonPropertyName("json_name")].
            public string payment { get; set; }
            public short cycle { get; set; }
            public long supplier_id { get; set; }
            public short periods { get; set; }
        }
    }
}