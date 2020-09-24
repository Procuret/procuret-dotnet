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
                sessionId: 111
            );

            String response = await InstalmentLink.Create(
                supplierId: 111,
                customerEmail: "someone@procuret-test-domain.org",
                invoiceIdentifier: "Test ID",
                invoiceValue: Convert.ToDecimal("422.22"),
                communication: CommunicationOption.NotifyCustomer,
                session: session
            );

            Console.WriteLine(response);
            Assert.True(response.Length > 0);

            return;

        }
    }
}
