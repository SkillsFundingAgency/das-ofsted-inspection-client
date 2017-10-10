using System.Collections.Generic;

namespace Esfa.Ofsted.Inspection.Client.Services.interfaces
{
    public interface IAngleSharpService
    {
        IList<string> GetLinks(string url, string selector, string textInTitle);
    }
}