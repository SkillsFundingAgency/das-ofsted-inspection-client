using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Esfa.Ofsted.Inspection.Client.Services;
using OfficeOpenXml;

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
        public GetOfstedInspections(IProcessExcelFormulaToLink processExcelFormulaToLink, IOverallEffectivenessProcessor overallEffectivenessProcessor)
        {
            _processExcelFormulaToLink = processExcelFormulaToLink;
            _overallEffectivenessProcessor = overallEffectivenessProcessor;
        }

        public GetOfstedInspections(IOverallEffectivenessProcessor overallEffectivenessProcessor)
        {
            _overallEffectivenessProcessor = overallEffectivenessProcessor;
        }

        public List<Sfa.Das.Ofsted.Inspection.Types.Inspection> GetAll()
        {
            var inspections = new List<Sfa.Das.Ofsted.Inspection.Types.Inspection>();

            var url = "https://www.gov.uk/government/statistical-data-sets/monthly-management-information-ofsteds-further-education-and-skills-inspections-outcomes-from-december-2015";

            var urlOfSpreadsheet = GetLinkForLatestSpreadsheet(url);

            using (var client = new WebClient())
            {
                using (var stream =
                    new MemoryStream(client.DownloadData(new Uri(urlOfSpreadsheet))))
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        GetOsftedInspections(package, inspections);
                    }
                }
            }
            return inspections;
        }

        private static string GetLinkForLatestSpreadsheet(string url)
        {
            var urlOfSpreadsheet = string.Empty;
            using (var client = new WebClient())
            {
                client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

                Uri uriResult;
                Uri.TryCreate(url, UriKind.Absolute, out uriResult);

                var absolutePath = uriResult.AbsolutePath;

                var urlDetails = client.DownloadString(url);
                var parser = new HtmlParser();
                var result = parser.Parse(urlDetails);
                var allAnchorTags = result.QuerySelectorAll("a").OfType<IHtmlAnchorElement>();

                var firstManagementInformationTag = allAnchorTags
                    .Where(x => x.InnerHtml.Contains("Management information")).Select(x => x.GetAttribute("href"))
                    .First();

                urlOfSpreadsheet =
                    $"{uriResult.Scheme}://{uriResult.Host}{firstManagementInformationTag}"; 
            }

            return urlOfSpreadsheet;
        }

        private void GetOsftedInspections(ExcelPackage package,
            List<Sfa.Das.Ofsted.Inspection.Types.Inspection> inspections)
        {
            var keyWorksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "D1 In-year inspection data");
            if (keyWorksheet == null) return;

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
        }

        private static DateTime GetDateTimeValue(ExcelRange excelRange)
        {
            if (excelRange?.Value == null) throw new ArgumentNullException(nameof(excelRange));
            var value = excelRange.Value.ToString();
            return DateTime.Parse(value);
        }
    }


}