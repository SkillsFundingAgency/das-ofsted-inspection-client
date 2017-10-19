using Esfa.Ofsted.Inspection.Client.Services.Interfaces;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal class SystemWebClientFactory : IWebClientFactory
    {
        public IWebClient Create()
        {
            return new SystemWebClient();
        }

    }
}