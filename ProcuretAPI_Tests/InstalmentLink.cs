using System;
using Xunit;
using ProcuretAPI;
using System.Threading.Tasks;


namespace ProcuretAPI_Tests
{
    public class CreateInstalmentLink
    {
        [Fact]
        public async Task TestCreateInstalmentLink()
        {

            Session session = new Session(
                apiKey: "placebo",
                sessionId: 123
            );

            InstalmentLink link = await InstalmentLink.Create(
                supplierId: 4000,
                customerEmail: "someone@procuret-test-domain.org",
                invoiceIdentifier: "Test ID",
                invoiceValue: Convert.ToDecimal("422.22"),
                communication: CommunicationOption.NotifyCustomer,
                session: session
            );

            return;

        }
    }
}
