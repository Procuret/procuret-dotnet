using System;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net.Http;
using System.Collections.Generic;


namespace ProcuretAPI
{
    public struct InstalmentLink
    {

        internal const String path = "/instalment-link";

        public readonly String PublicId;
        public readonly EntityHeadline Supplier;
        public readonly Decimal InvoiceAmount;
        public readonly String InvoiceIdentifier;
        public readonly String InviteeEmail;

        internal InstalmentLink(
            String publicId,
            EntityHeadline supplier,
            String inviteeEmail,
            Decimal invoiceAmount,
            String invoiceIdentifier
        )
        {
            this.PublicId = publicId;
            this.Supplier = supplier;
            this.InviteeEmail = inviteeEmail;
            this.InvoiceAmount = invoiceAmount;
            this.InvoiceIdentifier = invoiceIdentifier;

            return;
        }

        public static async Task<InstalmentLink> Create(
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
            String stringInvoiceValue = Math.Round(invoiceValue, 2).ToString();

            CreatePayload payload = new CreatePayload(
                stringSupplierId,
                stringInvoiceValue,
                customerEmail,
                invoiceIdentifier,
                emailCustomer
            );

            String resultBody = await ApiRequest.Make<CreatePayload>(
                path: InstalmentLink.path,
                body: payload,
                method: HttpMethod.Post,
                session: session
            );

            var decodePayload = ApiRequest.DecodeResponse<DecodePayload>(
                resultBody
            );

            var link = new InstalmentLink(
                decodePayload.public_id,
                decodePayload.supplier,
                decodePayload.invitee_email,
                Convert.ToDecimal(decodePayload.invoice_amount),
                decodePayload.invoice_identifier
            );

            return link;
        }

        public enum OrderBy
        {
            CREATED = 1
        }

        public static async Task<InstalmentLink[]> RetrieveMany(
            Session session,
            Int64? supplierId = null,
            Int32 offset = 0,
            Int32 limit = 20,
            Order order = Order.ASCENDING,
            InstalmentLink.OrderBy orderBy = InstalmentLink.OrderBy.CREATED,
            String publicId = null
        )
        {

            List<QueryParameter> parameters = new List<QueryParameter>();

            parameters.Add(new QueryParameter(offset, "offset"));
            parameters.Add(new QueryParameter(limit, "limit"));
            parameters.Add(new QueryParameter(order));
            parameters.Add(new QueryParameter(orderBy));

            if (supplierId != null)
            {
                parameters.Add(
                    new QueryParameter(supplierId ?? 0, "supplier_id")
                );
            }

            if (publicId != null)
            {
                parameters.Add(
                    new QueryParameter(publicId, "public_id")
                );
            }


            String resultBody = await ApiRequest.Make(
                path: InstalmentLink.path + "/list",
                query: new QueryString(parameters),
                session: session,
                method: HttpMethod.Get
            );

            var decodePayload = ApiRequest.DecodeResponse<DecodeArrayPayload>(
                resultBody
            );

            var resultList = new List<InstalmentLink>();

            foreach (InstalmentLink.DecodePayload link in decodePayload)
            {

                resultList.Add(new InstalmentLink(
                    link.public_id,
                    link.supplier,
                    link.invitee_email,
                    Convert.ToDecimal(link.invoice_amount),
                    link.invoice_identifier
                ));
                continue;
            }

            return resultList.ToArray();

        }

        [DataContract(Name="procuret_data", Namespace="")]
        internal struct CreatePayload
        {
            [DataMember]
            private readonly String supplier_id;
            [DataMember]
            private readonly String invoice_amount;
            [DataMember]
            private readonly String invitee_email;
            [DataMember]
            private readonly String invoice_identifier;
            [DataMember]
            private readonly String communicate;

            internal CreatePayload(
                String supplierId,
                String invoiceAmount,
                String inviteeEmail,
                String invoiceIdentifier,
                String communicate
            )
            {
                this.supplier_id = supplierId;
                this.invoice_amount = invoiceAmount;
                this.invitee_email = inviteeEmail;
                this.invoice_identifier = invoiceIdentifier;
                this.communicate = communicate;
                return;
            }
        }


        [CollectionDataContract(Name = "procuret_data", Namespace = "")]
        internal class DecodeArrayPayload : List<DecodePayload> { }

        [DataContract(Name="procuret_data", Namespace="")]
        internal class DecodePayload
        {
            [DataMember]
            internal readonly String public_id;

            [DataMember]
            internal readonly EntityHeadline supplier;

            [DataMember]
            internal readonly String invitee_email;

            [DataMember]
            internal readonly String invoice_amount;

            [DataMember]
            internal readonly String invoice_identifier;

        }
    }
}
