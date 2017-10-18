using System;
using System.Collections.Generic;
using System.Linq;
using Esfa.Ofsted.Inspection.Client.ApplicationServices;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using OfficeOpenXml;
using Esfa.Ofsted.Inspection.Types;
using Esfa.Ofsted.Inspection.Types.Exceptions;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal class GetOfstedDetailsFromExcelPackageService : IGetOfstedDetailsFromExcelPackageService
    {
        private const int WebLinkPosition = 1;
        private const int UkprnPosition = 3;
        private const int DatePublishedPosition = 16;
        private const int OverallEffectivenessPosition = 17;
        private readonly IProcessExcelFormulaToLink _processExcelFormulaToLink;
        private readonly IOverallEffectivenessProcessor _overallEffectivenessProcessor;
        private readonly IConfigurationSettings _configurationSettings;
        private readonly ILogFunctions _logger;

        public GetOfstedDetailsFromExcelPackageService(ILogFunctions logger) : this(logger,
            new ProcessExcelFormulaToLink(),
            new OverallEffectivenessProcessor(),
            new ConfigurationSettings())
        {}

        public GetOfstedDetailsFromExcelPackageService() : this(new LogFunctions(),
                                                                new ProcessExcelFormulaToLink(),
                                                                new OverallEffectivenessProcessor(),
                                                                new ConfigurationSettings())
        {}

        public GetOfstedDetailsFromExcelPackageService( ILogFunctions logger,
                                                        IProcessExcelFormulaToLink processExcelFormulaToLink, 
                                                        IOverallEffectivenessProcessor overallEffectivenessProcessor,
                                                        IConfigurationSettings configurationSettings)
        {
            _processExcelFormulaToLink = processExcelFormulaToLink;
            _overallEffectivenessProcessor = overallEffectivenessProcessor;
            _configurationSettings = configurationSettings;
            _logger = logger;
        }

        public InspectionsDetail ExtractOfstedInspections(ExcelPackage package)
        {
            var inspections = new List<OfstedInspection>();
            var errorSet = new List<InspectionError>();
            var statusCode = InspectionsStatusCode.Success;
    
            var keyWorksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == _configurationSettings.WorksheetName);
            if (keyWorksheet == null)
            {
                var message = $@"No worksheet found in the datasource that matches '{_configurationSettings.WorksheetName}'";
                var exception = new NoWorksheetPresentException(message);
                _logger.Error(message, exception);
                throw exception;
            }

            var lineNumberStart = FindStartingLineNumber(keyWorksheet);

            if (lineNumberStart == 0)
            {
                const string message = "No details could be found when processing";
                var exception = new NoDetailsException(message);
                _logger.Error(message, exception);
                throw exception;
            }

            for (var lineNumber = lineNumberStart; lineNumber <= keyWorksheet.Dimension.End.Row; lineNumber++)
            {
                statusCode = ProcessLineIntoDetailsAsDetailOrError(keyWorksheet, lineNumber, inspections, errorSet, statusCode);
            }

            if (inspections.Count == 0)
            {
                const string message = "No inspections were processed successfully";
                var exception = new NoDetailsException(message);
                _logger.Error(message, exception);
                throw exception;
            }

            return new InspectionsDetail { Inspections = inspections, ErrorSet = errorSet, StatusCode = statusCode};
        }

        private InspectionsStatusCode ProcessLineIntoDetailsAsDetailOrError(ExcelWorksheet keyWorksheet, int lineNumber, 
            ICollection<OfstedInspection> inspections, ICollection<InspectionError> errorSet, InspectionsStatusCode statusCode)
        {
            var error = new InspectionError {LineNumber = lineNumber};

            var ukprn = ProcessUkprnForError(keyWorksheet, lineNumber, error);
            var url = _processExcelFormulaToLink.GetLinkFromFormula(keyWorksheet.Cells[lineNumber, WebLinkPosition].Formula);
            var overallEffectiveness = ProcessOverallEffectivenessForError(keyWorksheet, lineNumber, error);
            var datePublished = ProcessDatePublishedForError(keyWorksheet, lineNumber, error);

            if (ukprn != null && overallEffectiveness != null && datePublished != null)
            {
                AddInspectionData((int) ukprn, url, (DateTime) datePublished, (OverallEffectiveness) overallEffectiveness,
                    inspections);
                _logger.Debug($"Details processed successfully for line {lineNumber}: {ukprn}, {url}, {datePublished}, {overallEffectiveness}");
            }
            else
            {
                errorSet.Add(error);
                statusCode = InspectionsStatusCode.ProcessedWithErrors;
                _logger.Debug($"Details processed unsuccessfully for line {lineNumber}: '{ukprn}', '{url}', '{datePublished}', '{overallEffectiveness}'");
            }
            return statusCode;
        }

       

        private static int FindStartingLineNumber(ExcelWorksheet keyWorksheet)
        {
            var lineNumberStart = keyWorksheet.Dimension.Start.Row;
            int intValue;
            while (!int.TryParse(keyWorksheet.Cells[lineNumberStart, UkprnPosition]?.Value?.ToString(), out intValue) &&
                   GetDateTimeValue(keyWorksheet.Cells[lineNumberStart, DatePublishedPosition]) == null && lineNumberStart<= keyWorksheet.Dimension.End.Row)
            {
                lineNumberStart++;
            }
            return lineNumberStart > keyWorksheet.Dimension.End.Row ? 0 : lineNumberStart;
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
            error.DatePublished = datePublishedString;
            error.Message = $"'{datePublishedString}' could not be converted to date";
            return datePublished;
        }

        private OverallEffectiveness? ProcessOverallEffectivenessForError(ExcelWorksheet keyWorksheet, int lineNumber, InspectionError error)
        {
            var overallEffectivenessString = keyWorksheet.Cells[lineNumber, OverallEffectivenessPosition]?.Value?.ToString();
            var overallEffectiveness = _overallEffectivenessProcessor.GetOverallEffectiveness(overallEffectivenessString);
            error.OverallEffectiveness = overallEffectivenessString;
            error.Message = $"'{overallEffectivenessString}' could not be converted to overallEffectiveness";
            return overallEffectiveness;
        }

        private static int? ProcessUkprnForError(ExcelWorksheet keyWorksheet, int lineNumber, InspectionError error)
        {
            var ukprnString = keyWorksheet.Cells[lineNumber, UkprnPosition].Value != null
                ? keyWorksheet.Cells[lineNumber, UkprnPosition].Value.ToString()
                : string.Empty;
            error.Ukprn = ukprnString;
            int ukprn;
            if (!int.TryParse(ukprnString, out ukprn))
            {
                error.Message = $"'{ukprnString}' could not be converted to overallEffectiveness";
                return null;
            }
        
        return ukprn;
        }


        private static DateTime? GetDateTimeValue(ExcelRange excelRange)
        {
            var value = excelRange?.Value;
            return value as DateTime?;
        }
    }
}
