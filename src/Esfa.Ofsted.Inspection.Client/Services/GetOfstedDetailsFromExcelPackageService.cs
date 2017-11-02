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
           

            if (!spreadsheetDetails.AreAllColumnHeadingsMatched || spreadsheetDetails.DataStartsRow == 0)
            {
                const string message = "Could not find the start row or the column names";
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
            var rowNumber = keyWorksheet.Dimension.Start.Row;
            var maxRow = keyWorksheet.Dimension.End.Row;
            var maxCol = keyWorksheet.Dimension.End.Column;
            while (rowNumber <= maxRow)
            {
                for (var columnNumber = 1; columnNumber <= maxCol && !spreadsheetDetails.AreAllColumnHeadingsMatched; columnNumber++)
                {
                    var cellValue = keyWorksheet.Cells[rowNumber, columnNumber]?.Value?.ToString().Trim();

                    if (string.Equals(cellValue, _configurationSettings.WebLinkHeading, StringComparison.OrdinalIgnoreCase))
                    {
                        spreadsheetDetails.WebLinkColumn = columnNumber;
                    }

                    if (string.Equals(cellValue, _configurationSettings.UkPrnHeading, StringComparison.OrdinalIgnoreCase))
                    {
                        spreadsheetDetails.UkPrnColumn = columnNumber;
                    }

                    if (string.Equals(cellValue, _configurationSettings.DatePublishedHeading, StringComparison.OrdinalIgnoreCase))
                    {
                        spreadsheetDetails.DatePublishedColumn = columnNumber;
                    }

                    if (string.Equals(cellValue, _configurationSettings.OverallEffectivenessHeading, StringComparison.OrdinalIgnoreCase))
                    {
                        spreadsheetDetails.OverallEffectivenessColumn = columnNumber;
                    }
                }
                rowNumber++;

                if (spreadsheetDetails.AreAllColumnHeadingsMatched)
                {
                    spreadsheetDetails.DataStartsRow = rowNumber;
                    break;
                }

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

            if (string.Equals(valueToCheck, "null", StringComparison.OrdinalIgnoreCase))
            {
                return DateTime.MinValue;
            }
                     
            return value as DateTime?;
        }
    }
}
