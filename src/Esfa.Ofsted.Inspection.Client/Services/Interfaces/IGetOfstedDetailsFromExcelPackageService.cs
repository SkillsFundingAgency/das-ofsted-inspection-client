using OfficeOpenXml;
using Esfa.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    internal interface IGetOfstedDetailsFromExcelPackageService
    {
        InspectionOutcomesResponse ExtractOfstedInspections(ExcelPackage package);
    }
}