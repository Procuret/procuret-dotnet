using System;
using Xunit;
using ProcuretAPI;
using System.Threading.Tasks;
using ProcuretAPI_Tests.Variants;


namespace ProcuretAPI_Tests
{
    public class ExerciseInstalmentLink : WithSession
    {
        [Fact]
        public async Task TestCreateInstalmentLink()
        {

            InstalmentLink link = await InstalmentLink.Create(
                supplierId: 4000,
                customerEmail: "someone@procuret-test-domain.org",
                invoiceIdentifier: "Test ID",
                invoiceValue: Convert.ToDecimal("422.22"),
                communication: CommunicationOption.NotifyCustomer,
                session: this.Session
            );

            return;

        }

        [Fact]
        public async Task TestListInstalmentLink()
        {

            InstalmentLink link = await InstalmentLink.Create(
                supplierId: 4000,
                customerEmail: "someone@procuret-test-domain.org",
                invoiceIdentifier: "Test ID",
                invoiceValue: Convert.ToDecimal("422.22"),
                communication: CommunicationOption.NotifyCustomer,
                session: this.Session
            );

            InstalmentLink[] links = await InstalmentLink.RetrieveMany(
                supplierId: 4000,
                limit: 2,
                session: this.Session
            );

            Assert.True(
                links.Length > 0,
                "Actual length: " + links.Length.ToString()
            );


            return;

        }
    }
}
