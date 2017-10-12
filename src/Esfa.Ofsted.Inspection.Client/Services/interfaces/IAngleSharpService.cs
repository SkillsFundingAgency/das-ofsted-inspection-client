using System.Collections.Generic;

namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    internal interface IAngleSharpService
    {
        IList<string> GetLinks(string url, string selector, string textInTitle);
    }
}