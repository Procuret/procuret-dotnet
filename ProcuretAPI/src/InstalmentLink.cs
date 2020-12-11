using System;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net.Http;


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
            String stringInvoiceValue = invoiceValue.ToString();

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

        [DataContract(Name="procuret_data", Namespace="")]
        internal struct DecodePayload
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
