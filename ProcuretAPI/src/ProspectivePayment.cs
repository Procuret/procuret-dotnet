using System;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net.Http;


namespace ProcuretAPI
{
    public struct ProspectivePayment
    {
        internal const String Path = "/credit/prospective-payment";

        public readonly Decimal RecurringPayment;
        public readonly String SupplierId;
        public readonly Int16 PaymentCount;
        public readonly Period Period;
        public readonly Cycle Cycle;

        internal ProspectivePayment(
            Decimal recurringPayment,
            String supplierId,
            Int16 paymentCount,
            Cycle cycle
        )
        {
            this.RecurringPayment = recurringPayment;
            this.SupplierId = supplierId;
            this.PaymentCount = paymentCount;
            this.Period = Period.MONTH;  // Only .MONTH available at this time
            this.Cycle = cycle;
            return;
        }

        public static async Task<ProspectivePayment> Retrieve(
            Session session,
            String supplierId,
            Decimal principle,
            Int16 paymentCount,
            HttpClient httpClient = null
        )
        {

            QueryParameter[] parameters = {
                new QueryParameter(supplierId, "supplier_id"),
                new QueryParameter(principle.ToString(), "principle"),
                new QueryParameter(paymentCount, "periods"),
                new QueryParameter((Int16)Cycle.ADVANCE, "cycle")
            };

            QueryString query = new QueryString(parameters);

            String resultBody = await ApiRequest.Make(
                ProspectivePayment.Path,
                query,
                session,
                HttpMethod.Get,
                httpClient
            );

            var responsePayload = ApiRequest.DecodeResponse<ResponsePayload>(
                resultBody
            );

            return new ProspectivePayment(
                Convert.ToDecimal(responsePayload.payment),
                responsePayload.supplierId,
                responsePayload.periods,
                (Cycle)responsePayload.cycle
            );

        }


        [DataContract(Name="procuret_data", Namespace="")]
        internal struct ResponsePayload
        {
            [DataMember]
            internal readonly String payment;

            [DataMember]
            internal readonly Int16 cycle;

            [DataMember]
            internal readonly String supplierId;

            [DataMember]
            internal readonly Int16 periods;

        }
    }
}
