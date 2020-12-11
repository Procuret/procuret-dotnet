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
                supplierId: "4000",
                principle: Convert.ToDecimal("42"),
                12
            );

            return;

        }
    }
}
