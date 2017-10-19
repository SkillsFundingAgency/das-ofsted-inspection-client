using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    internal interface IWebClient: IDisposable
    {
        byte[] DownloadData(Uri address);
    }
}
