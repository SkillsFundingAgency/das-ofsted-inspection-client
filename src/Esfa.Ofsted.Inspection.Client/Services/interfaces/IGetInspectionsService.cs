using Esfa.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    internal interface IGetInspectionsService
    {
        InspectionsDetail GetInspectionsDetail(string firstLinkUrl);
    }
}