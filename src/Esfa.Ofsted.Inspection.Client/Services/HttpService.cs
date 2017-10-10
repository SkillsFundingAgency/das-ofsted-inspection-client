using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Esfa.Ofsted.Inspection.Client.Services.interfaces;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    public class HttpService: IHttpGet
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
