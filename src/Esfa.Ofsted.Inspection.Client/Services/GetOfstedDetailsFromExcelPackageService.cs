using System;
using System.Collections.Generic;
using System.Linq;
using Esfa.Ofsted.Inspection.Client.Extensions;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using OfficeOpenXml;
using Esfa.Ofsted.Inspection.Types;
using Esfa.Ofsted.Inspection.Types.Exceptions;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal class GetOfstedDetailsFromExcelPackageService : IGetOfstedDetailsFromExcelPackageService
    {
      
        private readonly IProcessExcelFormulaToLink _processExcelFormulaToLink;
        private readonly IOverallEffectivenessProcessor _overallEffectivenessProcessor;
        private readonly IConfigurationSettings _configurationSettings;
        private readonly ILogFunctions _logger;
        

        internal GetOfstedDetailsFromExcelPackageService(ILogFunctions logger) : this(logger,
            new ProcessExcelFormulaToLink(),
            new OverallEffectivenessProcessor(),
            new ConfigurationSettings())
        {}

        internal GetOfstedDetailsFromExcelPackageService() : this(new LogFunctions(),
                                                                new ProcessExcelFormulaToLink(),
                                                                new OverallEffectivenessProcessor(),
                                                                new ConfigurationSettings())
        {}

        internal GetOfstedDetailsFromExcelPackageService( ILogFunctions logger,
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

            var spreadsheetDetails = GetSpreadsheetColumnAndRowDetails(keyWorksheet);
           

            if (spreadsheetDetails.WebLinkColumn== 0
                || spreadsheetDetails.UkPrnColumn == 0
                || spreadsheetDetails.DatePublishedColumn == 0
                || spreadsheetDetails.OverallEffectivenessColumn == 0
                || spreadsheetDetails.DataStartsRow == 0)
            {
                const string message = "No details could be found when processing";
                var exception = new MissingInspectionOutcomesException(message);  
                _logger.Error(message, exception);
                throw exception;
            }

            for (var lineNumber = spreadsheetDetails.DataStartsRow; lineNumber <= keyWorksheet.Dimension.End.Row; lineNumber++)
            {
                var returnedStatusCode = ProcessLineIntoDetailsAsDetailOrError(keyWorksheet, 
                                                spreadsheetDetails, lineNumber, inspections, errorSet);
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

        private InspectionsStatusCode ProcessLineIntoDetailsAsDetailOrError(ExcelWorksheet keyWorksheet,
            SpreadsheetDetails spreadsheetDetails,
            int lineNumber, 
            ICollection<InspectionOutcome> inspections, ICollection<InspectionError> errorSet)
        {
            var error = new InspectionError {LineNumber = lineNumber};

            var ukprn = ProcessUkprnForError(Convert.ToString(keyWorksheet.Cells[lineNumber, spreadsheetDetails.UkPrnColumn].Value), error);
            var url = _processExcelFormulaToLink.GetLinkFromFormula(
                        keyWorksheet.Cells[lineNumber, spreadsheetDetails.WebLinkColumn].Formula, 
                        keyWorksheet.Cells[lineNumber, spreadsheetDetails.WebLinkColumn].Text);
            var overallEffectiveness = ProcessOverallEffectivenessForError(Convert.ToString(keyWorksheet.Cells[lineNumber, spreadsheetDetails.OverallEffectivenessColumn]?.Value), error);
            var datePublished = ProcessDatePublishedForError(keyWorksheet.Cells[lineNumber, spreadsheetDetails.DatePublishedColumn], error);

            if (ukprn != null && overallEffectiveness != null && datePublished != null)
            {
                AddInspectionData((int) ukprn, url, datePublished, (OverallEffectiveness) overallEffectiveness,
                    inspections);
                _logger.Debug($"Details processed successfully for line {lineNumber}: {ukprn}, {url}, {error.DatePublished}, {overallEffectiveness}");
                return InspectionsStatusCode.Success;
            }

            error.Website = url ?? string.Empty;

            errorSet.Add(error);
            _logger.Warn($"Details processed unsuccessfully for line {error.LineNumber}: '{error.Ukprn}', '{error.Website}', '{error.DatePublished}', '{error.OverallEffectiveness}'");
            return InspectionsStatusCode.ProcessedWithErrors;

        }

        private SpreadsheetDetails GetSpreadsheetColumnAndRowDetails(ExcelWorksheet keyWorksheet)
        {
            var spreadsheetDetails = new SpreadsheetDetails();
            var matchFound = false;
            var lineNumberStart = keyWorksheet.Dimension.Start.Row;
            while (!matchFound && lineNumberStart <= keyWorksheet.Dimension.End.Row)
            {
                for (var i = 1; i <= keyWorksheet.Dimension.End.Column; i++)
                {
                    if (keyWorksheet.Cells[lineNumberStart, i]?.Value?.ToString() == _configurationSettings.WebLinkHeading)
                    {
                        spreadsheetDetails.WebLinkColumn = i;
                        matchFound = true;
                    }

                    if (keyWorksheet.Cells[lineNumberStart, i]?.Value?.ToString() == _configurationSettings.UkPrnHeading)
                    {
                        spreadsheetDetails.UkPrnColumn = i;
                        matchFound = true;
                    }
                    
                    if (keyWorksheet.Cells[lineNumberStart, i]?.Value?.ToString() == _configurationSettings.DatePublishedHeading)
                    {
                        spreadsheetDetails.DatePublishedColumn = i;
                        matchFound = true;
                    }

                    if (keyWorksheet.Cells[lineNumberStart, i]?.Value?.ToString() == _configurationSettings.OverallEffectivenessHeading)
                    {
                        spreadsheetDetails.OverallEffectivenessColumn = i;
                        matchFound = true;
                    }
                }
                lineNumberStart++;
            }

            if (matchFound)
            {
                spreadsheetDetails.DataStartsRow = lineNumberStart;
            }
            
            return spreadsheetDetails;
        }
        
        private static void AddInspectionData(int ukprn, string url, DateTime? datePublished,
            OverallEffectiveness overallEffectiveness, ICollection<InspectionOutcome> inspections)
        {
            var inspectionData = new InspectionOutcome
            {
                Ukprn = ukprn,
                Website = url,
                DatePublished = datePublished == DateTime.MinValue ? null : datePublished,
                OverallEffectiveness =  overallEffectiveness
            };

            inspections.Add(inspectionData);
        }

        private static DateTime? ProcessDatePublishedForError(ExcelRange cell,
            InspectionError error)
        {

            var datePublished = GetDateTimeValue(cell);
            if (datePublished != null)
                {
                error.DatePublished = datePublished == DateTime.MinValue ? "NULL" : datePublished.ToDdmmyyyyString();
                }
            else
            {
                error.DatePublished = cell.Text;
                error.Message = error.Message + $@"Invalid value for Date Published [{error.DatePublished}]; ";
            }
        
        return datePublished;
        }

        private OverallEffectiveness? ProcessOverallEffectivenessForError(string value, InspectionError error)
        {
            var overallEffectivenessString = value;
            var overallEffectiveness = _overallEffectivenessProcessor.GetOverallEffectiveness(overallEffectivenessString);
            error.OverallEffectiveness = overallEffectivenessString;
            if (overallEffectiveness == null)
            {
                error.Message = error.Message + $@"Invalid value for Overall Effectiveness [{ error.OverallEffectiveness}]; ";
            }
            return overallEffectiveness;
        }

        private static int? ProcessUkprnForError(string value, InspectionError error)
        {
            var ukprnString = value ?? string.Empty;
            error.Ukprn = ukprnString;
            int ukprn;
            if (int.TryParse(ukprnString, out ukprn)) return ukprn;

            error.Message = error.Message + $@"Invalid value for ukprn [{error.Ukprn = ukprnString}]; ";
            return null;
        }

        private static DateTime? GetDateTimeValue(ExcelRange excelRange)
        {
            var value = excelRange?.Value;
            var valueToCheck = value as string;

            if (valueToCheck == "null" || valueToCheck == "NULL"|| valueToCheck == "Null")
            {
                return DateTime.MinValue;
            }
                     
            return value as DateTime?;
        }
    }

    internal struct SpreadsheetDetails
    {
        public int WebLinkColumn { get; set; }
        public int UkPrnColumn { get; set; }
        public int DatePublishedColumn { get; set; }
        public int OverallEffectivenessColumn { get; set; }
        public int DataStartsRow { get; set; }
    }
}
