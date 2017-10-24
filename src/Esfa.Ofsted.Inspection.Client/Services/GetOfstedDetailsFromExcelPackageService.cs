using System;
using System.Collections.Generic;
using System.Globalization;
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

        public InspectionOutcomesResponse ExtractOfstedInspections(ExcelPackage package)
        {
            var inspections = new List<InspectionOutcome>();
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
                var exception = new MissingInspectionOutcomesException(message);  
                _logger.Error(message, exception);
                throw exception;
            }

            for (var lineNumber = lineNumberStart; lineNumber <= keyWorksheet.Dimension.End.Row; lineNumber++)
            {
                var returnedStatusCode = ProcessLineIntoDetailsAsDetailOrError(keyWorksheet, lineNumber, inspections, errorSet);
                if (returnedStatusCode == InspectionsStatusCode.ProcessedWithErrors)
                    statusCode = InspectionsStatusCode.ProcessedWithErrors;
            }

            if (inspections.Count == 0)
            {
                const string message = "No inspections were processed successfully";
                var exception = new MissingInspectionOutcomesException(message); 
                foreach (var error in errorSet)
                {
                    exception.Data.Add(error.LineNumber.ToString(), error);
                }
                _logger.Error(message, exception);
                throw exception;
            }


            return new InspectionOutcomesResponse {InspectionOutcomes = inspections, InspectionOutcomeErrors = errorSet, StatusCode = statusCode};
        }

        private InspectionsStatusCode ProcessLineIntoDetailsAsDetailOrError(ExcelWorksheet keyWorksheet, int lineNumber, 
            ICollection<InspectionOutcome> inspections, ICollection<InspectionError> errorSet)
        {
            var error = new InspectionError {LineNumber = lineNumber};

            var ukprn = ProcessUkprnForError(Convert.ToString(keyWorksheet.Cells[lineNumber, UkprnPosition].Value), error);
            var url = _processExcelFormulaToLink.GetLinkFromFormula(keyWorksheet.Cells[lineNumber, WebLinkPosition].Formula, keyWorksheet.Cells[lineNumber, WebLinkPosition].Text);
            var overallEffectiveness = ProcessOverallEffectivenessForError(Convert.ToString(keyWorksheet.Cells[lineNumber, OverallEffectivenessPosition]?.Value), error);
            var datePublished = ProcessDatePublishedForError(keyWorksheet.Cells[lineNumber, DatePublishedPosition], error);

            if (ukprn != null && overallEffectiveness != null && datePublished != null)
            {
                AddInspectionData((int) ukprn, url, (DateTime) datePublished, (OverallEffectiveness) overallEffectiveness,
                    inspections);
                _logger.Debug($"Details processed successfully for line {lineNumber}: {ukprn}, {url}, {datePublished}, {overallEffectiveness}");
                return InspectionsStatusCode.Success;
            }

            error.Website = url;
            errorSet.Add(error);
            _logger.Warn($"Details processed unsuccessfully for line {lineNumber}: '{ukprn}', '{url}', '{datePublished}', '{overallEffectiveness}'");
            return InspectionsStatusCode.ProcessedWithErrors;

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
            OverallEffectiveness overallEffectiveness, ICollection<InspectionOutcome> inspections)
        {
            var inspectionData = new InspectionOutcome
            {
                Ukprn = ukprn,
                Website = url,
                DatePublished = datePublished,
                OverallEffectiveness =  overallEffectiveness
            };

            inspections.Add(inspectionData);
        }

        private static DateTime? ProcessDatePublishedForError(ExcelRange cell,
            InspectionError error)
        {
            
            var datePublished = GetDateTimeValue(cell);
            error.DatePublished = datePublished?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) 
                                  ?? cell.Value.ToString();
            return datePublished;
        }

        private OverallEffectiveness? ProcessOverallEffectivenessForError(string value, InspectionError error)
        {
            var overallEffectivenessString = value;
            var overallEffectiveness = _overallEffectivenessProcessor.GetOverallEffectiveness(overallEffectivenessString);
            error.OverallEffectiveness = overallEffectivenessString;
            return overallEffectiveness;
        }

        private static int? ProcessUkprnForError(string value, InspectionError error)
        {
            var ukprnString = value ?? string.Empty;
            error.Ukprn = ukprnString;
            int ukprn;
            if (int.TryParse(ukprnString, out ukprn)) return ukprn;

            return null;
        }


        private static DateTime? GetDateTimeValue(ExcelRange excelRange)
        {
            var value = excelRange?.Value;
            return value as DateTime?;
        }
    }
}
