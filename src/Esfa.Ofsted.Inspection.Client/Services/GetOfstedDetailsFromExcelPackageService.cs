using System;
using System.Collections.Generic;
using System.Linq;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using OfficeOpenXml;
using Esfa.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal class GetOfstedDetailsFromExcelPackageService : IGetOfstedDetailsFromExcelPackageService
    {
        private const int UkprnPosition = 2;
        private const int WebLinkPosition = 1;
        private const int DatePublishedPosition = 16;
        private const int OverallEffectivenessPosition = 17;
        private const string WorksheetOfSpreadsheetToUse = "D1 In-year inspection data";
        private readonly IProcessExcelFormulaToLink _processExcelFormulaToLink;
        private readonly IOverallEffectivenessProcessor _overallEffectivenessProcessor;

        public GetOfstedDetailsFromExcelPackageService(IProcessExcelFormulaToLink processExcelFormulaToLink, IOverallEffectivenessProcessor overallEffectivenessProcessor)
        {
            _processExcelFormulaToLink = processExcelFormulaToLink;
            _overallEffectivenessProcessor = overallEffectivenessProcessor;
        }

        public InspectionsDetail GetOsftedInspections(ExcelPackage package)
        {
            var inspections = new List<OfstedInspection>();
            var errorSet = new List<InspectionError>();
            var statusCode = InspectionsStatusCode.Success;

            var keyWorksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == WorksheetOfSpreadsheetToUse);
            if (keyWorksheet == null)
            {
                return ReturnKeyworksheetNotFoundInspectionsDetail();
            }

            for (var i = keyWorksheet.Dimension.Start.Row + 1; i <= keyWorksheet.Dimension.End.Row; i++)
            {
                var ukprnString = keyWorksheet.Cells[i, UkprnPosition].Value != null
                    ? keyWorksheet.Cells[i, UkprnPosition].Value.ToString()
                    : string.Empty;
                int ukprn;

                if (string.IsNullOrEmpty(ukprnString) || !int.TryParse(ukprnString, out ukprn)) continue;

                var url = _processExcelFormulaToLink.GetLinkFromFormula(keyWorksheet.Cells[i, WebLinkPosition].Formula);
                var overallEffectivenessString = keyWorksheet.Cells[i, OverallEffectivenessPosition]?.Value?.ToString();
                var overallEffectiveness = _overallEffectivenessProcessor.GetOverallEffectiveness(overallEffectivenessString);
                var datePublished = GetDateTimeValue(keyWorksheet.Cells[i, DatePublishedPosition]);

                if (datePublished != null)
                {
                    var inspectionData = new OfstedInspection
                    {
                        Ukprn = ukprn,
                        Website = url,
                        DatePublished = (DateTime)datePublished,
                        OverallEffectiveness = overallEffectiveness
                    };

                    inspections.Add(inspectionData);
                }
                else
                {
                    errorSet.Add(new InspectionError { LineNumber = i, Message = $"Date Published is invalid: {keyWorksheet.Cells[i, DatePublishedPosition]}" });
                    statusCode = InspectionsStatusCode.ProcessedWithErrors;
                }
            }
            if (inspections.Count == 0)
            {
                statusCode = InspectionsStatusCode.NotProcessed;
            }

            return new InspectionsDetail { Inspections = inspections, ErrorSet = errorSet, StatusCode = statusCode };
        }


        private static DateTime? GetDateTimeValue(ExcelRange excelRange)
        {
            if (excelRange?.Value == null) throw new ArgumentNullException(nameof(excelRange));
            var value = excelRange.Value.ToString();
            DateTime dateResult;
            if (DateTime.TryParse(value, out dateResult))
            {
                return dateResult;
            }

            return null;
        }

        private static InspectionsDetail ReturnKeyworksheetNotFoundInspectionsDetail()
        {
            return new InspectionsDetail
            {
                ErrorSet = new List<InspectionError>
                {
                    new InspectionError
                    {
                        LineNumber = 0,
                        Message = $"No worksheet found that matches '{WorksheetOfSpreadsheetToUse}'"
                    }
                },
                Inspections = null,
                StatusCode = InspectionsStatusCode.NotProcessed
            };
        }

    }
}
