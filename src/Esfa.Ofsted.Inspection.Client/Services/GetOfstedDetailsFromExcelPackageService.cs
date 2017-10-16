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
        private readonly IProcessExcelFormulaToLink _processExcelFormulaToLink;
        private readonly IOverallEffectivenessProcessor _overallEffectivenessProcessor;
        private readonly IConfigurationSettings _configurationSettings;

        public GetOfstedDetailsFromExcelPackageService(IProcessExcelFormulaToLink processExcelFormulaToLink, 
                                                        IOverallEffectivenessProcessor overallEffectivenessProcessor,
                                                        IConfigurationSettings configurationSettings)
        {
            _processExcelFormulaToLink = processExcelFormulaToLink;
            _overallEffectivenessProcessor = overallEffectivenessProcessor;
            _configurationSettings = configurationSettings;
        }

        public InspectionsDetail ExtractOfstedInspections(ExcelPackage package)
        {
            var inspections = new List<OfstedInspection>();
            var errorSet = new List<InspectionError>();
            var statusCode = InspectionsStatusCode.Success;
            var worksheetOfSpreadsheetToUse = _configurationSettings.WorksheetName;

            var keyWorksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == worksheetOfSpreadsheetToUse);
            if (keyWorksheet == null)
            {
                return ReturnKeyworksheetNotFoundInspectionsDetail();
            }

            var lineNumberStart = FindStartingLineNumber(keyWorksheet);

            for (var lineNumber = lineNumberStart; lineNumber <= keyWorksheet.Dimension.End.Row; lineNumber++)
            {
                var error = new InspectionError {LineNumber = lineNumber};
                
                var ukprn = ProcessUkprnForError(keyWorksheet, lineNumber, error);
                var url = _processExcelFormulaToLink.GetLinkFromFormula(keyWorksheet.Cells[lineNumber, WebLinkPosition].Formula);
                var overallEffectiveness = ProcessOverallEffectivenessForError(keyWorksheet, lineNumber, error);
                var datePublished = ProcessDatePublishedForError(keyWorksheet, lineNumber, error);

                if (ukprn != null && overallEffectiveness != null && datePublished != null)
                {
                    AddInspectionData((int) ukprn, url, (DateTime) datePublished, (OverallEffectiveness) overallEffectiveness, inspections);
                }
                else
                {
                    errorSet.Add(error);
                    statusCode = InspectionsStatusCode.ProcessedWithErrors;
                }
            }

            if (inspections.Count == 0)
            {
                statusCode = InspectionsStatusCode.NotProcessed;
            }

            return new InspectionsDetail { Inspections = inspections, ErrorSet = errorSet, StatusCode = statusCode };
        }

        private static int FindStartingLineNumber(ExcelWorksheet keyWorksheet)
        {
            var lineNumberStart = keyWorksheet.Dimension.Start.Row;
            var intValue = 0;
            while (!int.TryParse(keyWorksheet.Cells[lineNumberStart, UkprnPosition]?.Value?.ToString(), out intValue) &&
                   GetDateTimeValue(keyWorksheet.Cells[lineNumberStart, DatePublishedPosition]) == null)
            {
                lineNumberStart++;
            }
            return lineNumberStart;
        }

        private static void AddInspectionData(int ukprn, string url, DateTime datePublished,
            OverallEffectiveness overallEffectiveness, ICollection<OfstedInspection> inspections)
        {
            var inspectionData = new OfstedInspection
            {
                Ukprn = ukprn,
                Website = url,
                DatePublished = datePublished,
                OverallEffectiveness =  overallEffectiveness
            };

            inspections.Add(inspectionData);
        }

        private static DateTime? ProcessDatePublishedForError(ExcelWorksheet keyWorksheet, int lineNumber,
            InspectionError error)
        {
            var datePublishedString = keyWorksheet.Cells[lineNumber, DatePublishedPosition]?.Value?.ToString();
            var datePublished = GetDateTimeValue(keyWorksheet.Cells[lineNumber, DatePublishedPosition]);
            if (datePublished == null)
            {
                error.Message += $"Date published is invalid: [{datePublishedString}]; ";
            }
            return datePublished;
        }

        private OverallEffectiveness? ProcessOverallEffectivenessForError(ExcelWorksheet keyWorksheet, int lineNumber, InspectionError error)
        {
            var overallEffectivenessString = keyWorksheet.Cells[lineNumber, OverallEffectivenessPosition]?.Value?.ToString();
            var overallEffectiveness = _overallEffectivenessProcessor.GetOverallEffectiveness(overallEffectivenessString);

            if (overallEffectiveness == null)
            {
                error.Message += $"Overall Effectiveness is not a valid value: [{overallEffectivenessString}]; ";
                error.OverallEffectiveness = overallEffectivenessString;
            }
            return overallEffectiveness;
        }

        private static int? ProcessUkprnForError(ExcelWorksheet keyWorksheet, int lineNumber, InspectionError error)
        {
            var ukprnString = keyWorksheet.Cells[lineNumber, UkprnPosition].Value != null
                ? keyWorksheet.Cells[lineNumber, UkprnPosition].Value.ToString()
                : string.Empty;
            int ukprn;
            var isUkprnValid = int.TryParse(ukprnString, out ukprn);
            error.Ukprn = ukprnString;
            if (!isUkprnValid)
            {
                error.Message += $"ukprn not a valid int: [{ukprnString}]; ";
            }
            return ukprn;
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

        private InspectionsDetail ReturnKeyworksheetNotFoundInspectionsDetail()
        {
            return new InspectionsDetail
            {
                Inspections = null,
                StatusCode = InspectionsStatusCode.NotProcessed,
                NotProcessedMessage = $"No worksheet found in the datasource that matches '{_configurationSettings.WorksheetName}'"
            };
        }

    }
}
