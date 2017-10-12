using Sfa.Das.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    public interface IGetInspectionsService
    {
        InspectionsDetail GetInspectionsDetail(string firstLinkUrl);
    }
}