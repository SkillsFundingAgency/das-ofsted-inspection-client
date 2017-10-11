using System.Collections.Generic;
using Sfa.Das.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.Services.interfaces
{
    public interface IGetInspectionsService
    {
        InspectionsDetail GetInspectionsDetail(string firstLinkUrl);
    }
}