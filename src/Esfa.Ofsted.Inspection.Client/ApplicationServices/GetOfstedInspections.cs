using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Esfa.Ofsted.Inspection.Client.Services;
using Esfa.Ofsted.Inspection.Client.Services.interfaces;
using OfficeOpenXml;
using Sfa.Das.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.ApplicationServices
{
    public class GetOfstedInspections : IGetOfstedInspections
    {
        //private readonly ILog _log;
        private const int UkprnPosition = 2;
        private const int WebLinkPosition = 1;
        private const int DatePublishedPosition = 16;
        private const int OverallEffectivenessPosition = 17;

        private readonly IProcessExcelFormulaToLink _processExcelFormulaToLink;
        private readonly IOverallEffectivenessProcessor _overallEffectivenessProcessor;
        private readonly IAngleSharpService _angleSharpService;

        private const string Url = "https://www.gov.uk/government/statistical-data-sets/monthly-management-information-ofsteds-further-education-and-skills-inspections-outcomes-from-december-2015";
        private const string TextOfLink = "Management information";
        private const string WorksheetOfSpreadsheetToUse = "D1 In-year inspection data";

        public GetOfstedInspections(IProcessExcelFormulaToLink processExcelFormulaToLink, 
                                    IOverallEffectivenessProcessor overallEffectivenessProcessor, 
                                    IAngleSharpService angleSharpService)
        {
            _processExcelFormulaToLink = processExcelFormulaToLink;
            _overallEffectivenessProcessor = overallEffectivenessProcessor;
            _angleSharpService = angleSharpService;
        }

        public InspectionsDetail GetAll()
        {
            var inspections = new List<Sfa.Das.Ofsted.Inspection.Types.Inspection>();        
            var getFirstMatchingLink = _angleSharpService.GetLinks(Url, "a",TextOfLink).First();
            var firstLinkUrl = BuildFirstLinkUrl(getFirstMatchingLink);

            using (var client = new WebClient())
            {
                using (var stream =
                    new MemoryStream(client.DownloadData(new Uri(firstLinkUrl))))
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        inspections = GetOsftedInspections(package);
                    }
                }
            }
            return new InspectionsDetail { Inspections = inspections, StatusCode = InspectionsStatusCode.Success, ErrorSet = null};
        }

        private static string BuildFirstLinkUrl(string getFirstMatchingLink)
        {
            Uri uriResult;
            Uri.TryCreate(Url, UriKind.Absolute, out uriResult);
            var firstLinkUrl = $"{uriResult.Scheme}://{uriResult.Host}{getFirstMatchingLink}";
            return firstLinkUrl;
        }


        private List<Sfa.Das.Ofsted.Inspection.Types.Inspection> GetOsftedInspections(ExcelPackage package)
        {
            var inspections = new List<Sfa.Das.Ofsted.Inspection.Types.Inspection>();
            var keyWorksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == WorksheetOfSpreadsheetToUse);
            if (keyWorksheet == null) return inspections;

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

                var inspectionData = new Sfa.Das.Ofsted.Inspection.Types.Inspection
                {
                    Ukprn = ukprn,
                    Website = url,
                    DatePublished = GetDateTimeValue(keyWorksheet.Cells[i, DatePublishedPosition]),
                    OverallEffectiveness = overallEffectiveness
                };

                inspections.Add(inspectionData);
            }

            return inspections;
        }

        private static DateTime GetDateTimeValue(ExcelRange excelRange)
        {
            if (excelRange?.Value == null) throw new ArgumentNullException(nameof(excelRange));
            var value = excelRange.Value.ToString();
            return DateTime.Parse(value);
        }
    }


}