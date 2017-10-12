using System.Collections.Generic;

namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    public interface IAngleSharpService
    {
        IList<string> GetLinks(string url, string selector, string textInTitle);
    }
}