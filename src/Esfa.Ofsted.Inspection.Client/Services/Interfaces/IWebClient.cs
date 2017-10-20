using System;

namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    internal interface IWebClient: IDisposable
    {
        byte[] DownloadData(Uri address);
    }
}
