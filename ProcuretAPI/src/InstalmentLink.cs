using System;
using System.Threading.Tasks;


namespace ProcuretAPI
{
    public struct InstalmentLink
    {

        internal const String path = "/instalment-link";

        public static async Task<String> Create(
            Int64 supplierId,
            String customerEmail,
            String invoiceIdentifier,
            Decimal invoiceValue,
            CommunicationOption communication,
            Session session
        )
        {
            String emailCustomer;
            if (communication == CommunicationOption.NotifyCustomer)
            {
                emailCustomer = "true";
            } else
            {
                emailCustomer = "false";
            }

            String stringSupplierId = supplierId.ToString();
            String stringiInvoiceValue = invoiceValue.ToString();

            String requestBody = $@"
{{
    ""supplier_id"": ""{ stringSupplierId}"",
    ""invoice_amount"": ""{stringiInvoiceValue}"",
    ""invitee_email"": ""{customerEmail}"",
    ""invoice_identifier"": ""{invoiceIdentifier}"",
    ""communicate"": ""{emailCustomer}""
}}
";


            String resultBody = await ApiRequest.MakeAsyncPost(
                path: InstalmentLink.path,
                bodyJsonData: requestBody,
                session: session
            );


            return resultBody;
        }

    }
}
