using OfficeOpenXml;
using Sfa.Das.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    public interface IGetOfstedDetailsFromExcelPackageService
    {
        InspectionsDetail GetOsftedInspections(ExcelPackage package);
    }
}