using Esfa.Ofsted.Inspection.Client.ApplicationServices;
using NUnit.Framework;

namespace Esfa.Ofsted.Inspection.UnitTests
{
    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        public void runtest()
        {
           var x = new GetOfstedInspections().GetAll();           
        }
    }
}
