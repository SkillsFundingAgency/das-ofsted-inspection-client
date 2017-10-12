using OfficeOpenXml;
using Esfa.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    internal interface IGetOfstedDetailsFromExcelPackageService
    {
        InspectionsDetail GetOsftedInspections(ExcelPackage package);
    }
}