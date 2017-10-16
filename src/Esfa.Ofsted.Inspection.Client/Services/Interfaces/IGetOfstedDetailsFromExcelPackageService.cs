using OfficeOpenXml;
using Esfa.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    internal interface IGetOfstedDetailsFromExcelPackageService
    {
        InspectionsDetail ExtractOfstedInspections(ExcelPackage package);
    }
}