using Esfa.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    internal interface IGetInspectionsService
    {
        InspectionOutcomesResponse GetInspectionsDetail(string firstLinkUrl);
    }
}