using System.Net;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal class SystemWebClient: WebClient, IWebClient
    {
        public IWebClient Create()
        {
            return new SystemWebClient();
        }
    }
}
