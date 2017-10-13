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

            for (var lineNumber = keyWorksheet.Dimension.Start.Row + 1; lineNumber <= keyWorksheet.Dimension.End.Row; lineNumber++)
            {
                var ukprnString = keyWorksheet.Cells[lineNumber, UkprnPosition].Value != null
                    ? keyWorksheet.Cells[lineNumber, UkprnPosition].Value.ToString()
                    : string.Empty;
                int ukprn;
                var isUkprnValid = int.TryParse(ukprnString, out ukprn);

                var url = _processExcelFormulaToLink.GetLinkFromFormula(keyWorksheet.Cells[lineNumber, WebLinkPosition].Formula);
                var overallEffectivenessString = keyWorksheet.Cells[lineNumber, OverallEffectivenessPosition]?.Value?.ToString();
                var overallEffectiveness = _overallEffectivenessProcessor.GetOverallEffectiveness(overallEffectivenessString);
                var datePublishedString = keyWorksheet.Cells[lineNumber, DatePublishedPosition]?.Value?.ToString();
                var datePublished = GetDateTimeValue(keyWorksheet.Cells[lineNumber, DatePublishedPosition]);

                var errorMessage = ProcessInspectionErrorMessages(overallEffectiveness, overallEffectivenessString,
                                                                  isUkprnValid, ukprnString, 
                                                                  datePublished, datePublishedString);

                if (errorMessage != string.Empty)
                {
                    AddErrorToErrorSet(errorSet, lineNumber, errorMessage, ukprnString, url, datePublishedString, overallEffectivenessString);
                    statusCode = InspectionsStatusCode.ProcessedWithErrors;
                }
                else
                {
                    var inspectionData = new OfstedInspection
                    {
                        Ukprn = ukprn,
                        Website = url,
                        DatePublished = (DateTime) datePublished,
                        OverallEffectiveness = (OverallEffectiveness) overallEffectiveness
                    };

                    inspections.Add(inspectionData);
                }
            }

            if (inspections.Count == 0)
            {
                statusCode = InspectionsStatusCode.NotProcessed;
            }

            return new InspectionsDetail { Inspections = inspections, ErrorSet = errorSet, StatusCode = statusCode };
        }

        private static void AddErrorToErrorSet(ICollection<InspectionError> errorSet, int lineNumber, string errorMessage,
            string ukprnString, string url, string datePublishedString, string overallEffectivenessString)
        {
            errorSet.Add(new InspectionError
            {
                LineNumber = lineNumber,
                Message = errorMessage,
                Ukprn = ukprnString,
                Website = url,
                DatePublished = datePublishedString,
                OverallEffectiveness = overallEffectivenessString
            });
        }

        private static string ProcessInspectionErrorMessages(OverallEffectiveness? overallEffectiveness, 
            string overallEffectivenessString, bool isUkprnValid, string ukprnString, DateTime? datePublished,
            string datePublishedString)
        {
            var message = string.Empty;
            if (overallEffectiveness == null)
            {
                message = $"Overall Effectiveness is not a valid value: [{overallEffectivenessString}]; ";
            }

            if (!isUkprnValid)
            {
                message += $"ukprn not a valid int: [{ukprnString}]; ";
            }

            if (datePublished == null)
            {
                message += $"Date published is invalid: [{datePublishedString}]; ";
            }

            return message;
        }

        private static DateTime? GetDateTimeValue(ExcelRange excelRange)
        {
            if (excelRange?.Value == null) return null;
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
