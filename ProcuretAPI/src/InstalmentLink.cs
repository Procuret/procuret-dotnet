using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProcuretAPI
{
    public struct InstalmentLink
    {
        internal const string path = "/instalment-link";

        public readonly string PublicId;
        public readonly EntityHeadline Supplier;
        public readonly decimal InvoiceAmount;
        public readonly string InvoiceIdentifier;
        public readonly string InviteeEmail;
        public readonly Currency Denomination;

        internal InstalmentLink(
            string publicId,
            EntityHeadline supplier,
            string inviteeEmail,
            decimal invoiceAmount,
            string invoiceIdentifier,
            Currency denomination
        )
        {
            PublicId = publicId;
            Supplier = supplier;
            InviteeEmail = inviteeEmail;
            InvoiceAmount = invoiceAmount;
            InvoiceIdentifier = invoiceIdentifier;
            Denomination = denomination;
        }

        /// <summary>
        /// Creates a new InstalmentLink via the API.
        /// </summary>
        public static async Task<InstalmentLink> Create(
            long supplierId,
            string customerEmail,
            string invoiceIdentifier,
            decimal invoiceValue,
            CommunicationOption communication,
            Session session,
            Currency denomination
        )
        {
            string emailCustomer = (communication == CommunicationOption.NotifyCustomer)
                ? "true"
                : "false";

            string stringSupplierId = supplierId.ToString();
            string stringInvoiceValue = Math.Round(invoiceValue, 2).ToString();

            // Build our JSON payload
            CreatePayload payload = new CreatePayload(
                supplier_id: supplierId,
                invoice_amount: stringInvoiceValue,
                invitee_email: customerEmail,
                invoice_identifier: invoiceIdentifier,
                communicate: emailCustomer,
                denomination: (int)denomination
            );

            // Make the API request (now JSON-based)
            string resultBody = await ApiRequest.Make(
                path: InstalmentLink.path,
                body: payload,
                session: session,
                method: HttpMethod.Post
            );

            // Decode the JSON response
            DecodePayload decodePayload = ApiRequest.DecodeResponse<DecodePayload>(resultBody);

            // Convert the response payload into our struct
            var link = new InstalmentLink(
                decodePayload.public_id,
                decodePayload.supplier,
                decodePayload.invitee_email,
                Convert.ToDecimal(decodePayload.invoice_amount),
                decodePayload.invoice_identifier,
                (Currency)decodePayload.denomination_id
            );

            return link;
        }

        public enum OrderBy
        {
            CREATED = 1
        }

        /// <summary>
        /// Retrieves multiple InstalmentLinks from the API.
        /// </summary>
        public static async Task<InstalmentLink[]> RetrieveMany(
            Session session,
            long? supplierId = null,
            int offset = 0,
            int limit = 20,
            Order order = Order.ASCENDING,
            InstalmentLink.OrderBy orderBy = InstalmentLink.OrderBy.CREATED,
            string publicId = null
        )
        {
            var parameters = new List<QueryParameter>
            {
                new QueryParameter(offset, "offset"),
                new QueryParameter(limit, "limit"),
                new QueryParameter(order),
                new QueryParameter(orderBy)
            };

            if (supplierId != null)
            {
                parameters.Add(new QueryParameter(supplierId.Value, "supplier_id"));
            }

            if (publicId != null)
            {
                parameters.Add(new QueryParameter(publicId, "public_id"));
            }

            string resultBody = await ApiRequest.Make(
                path: InstalmentLink.path + "/list",
                query: new QueryString(parameters),
                session: session,
                method: HttpMethod.Get
            );

            // Decode an array of DecodePayload objects
            DecodeArrayPayload decodePayload = ApiRequest.DecodeResponse<DecodeArrayPayload>(resultBody);

            var resultList = new List<InstalmentLink>();
            foreach (DecodePayload link in decodePayload)
            {
                resultList.Add(new InstalmentLink(
                    link.public_id,
                    link.supplier,
                    link.invitee_email,
                    Convert.ToDecimal(link.invoice_amount),
                    link.invoice_identifier,
                    (Currency)link.denomination_id
                ));
            }

            return resultList.ToArray();
        }

        // --------------------------
        //      JSON Payload Types
        // --------------------------

        /// <summary>
        /// Represents the JSON body sent when creating a new InstalmentLink.
        /// The field names match exactly what the API expects.
        /// </summary>
        internal struct CreatePayload
        {
            // Keeping these as lowercase property names so JSON matches literally
            public long supplier_id { get; set; }
            public string invoice_amount { get; set; }
            public string invitee_email { get; set; }
            public string invoice_identifier { get; set; }
            public string communicate { get; set; }
            public int denomination { get; set; }

            public CreatePayload(
                long supplier_id,
                string invoice_amount,
                string invitee_email,
                string invoice_identifier,
                string communicate,
                int denomination
            )
            {
                this.supplier_id = supplier_id;
                this.invoice_amount = invoice_amount;
                this.invitee_email = invitee_email;
                this.invoice_identifier = invoice_identifier;
                this.communicate = communicate;
                this.denomination = denomination;
            }
        }

        /// <summary>
        /// Represents the JSON array returned by the API for multiple links.
        /// Inherits from List&lt;DecodePayload&gt; so we can keep the same usage pattern.
        /// </summary>
        internal class DecodeArrayPayload : List<DecodePayload>
        {
            // No extra fields needed
        }

        /// <summary>
        /// Represents the JSON structure returned by the API for a single InstalmentLink.
        /// The field names match exactly what the API sends.
        /// </summary>
        internal class DecodePayload
        {
            public string public_id { get; set; }
            public EntityHeadline supplier { get; set; }
            public string invitee_email { get; set; }
            public string invoice_amount { get; set; }
            public string invoice_identifier { get; set; }
            public int denomination_id { get; set; }
        }
    }
}