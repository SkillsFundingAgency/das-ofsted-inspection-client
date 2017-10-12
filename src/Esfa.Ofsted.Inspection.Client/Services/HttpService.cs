using System.Net;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal class HttpService: IHttpGet
    {
        public string Get(string url)
        {
            using (var client = new WebClient())
            {
                client.CachePolicy =
                    new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                return client.DownloadString(url);
            }
        }
    }
}
