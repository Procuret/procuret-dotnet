using System;
using ProcuretAPI;
using Xunit;
using System.Threading.Tasks;
using ProcuretAPI_Tests.Variants;


namespace ProcuretAPI_Tests
{
    public class RetrieveProspectivePayment: WithSession
    {
        [Fact]
        public async Task TestRetreiveProspectivePayment()
        {

            ProspectivePayment payment = await ProspectivePayment.Retrieve(
                session: this.Session,
                supplierId: 4000,
                principle: Convert.ToDecimal("42"),
                12
            );

            Assert.True(payment.PaymentCount == 12);

            return;

        }

        [Fact]
        public async Task TestListProspectivePayment()
        {
            var payments = await ProspectivePayment.RetrieveMany(
                session: this.Session,
                supplierId: 4000,
                principle: Convert.ToDecimal("1000")
            );

            Assert.True(payments.Length > 0);

            return;
        }
    }
}
