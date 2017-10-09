using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using Esfa.Ofsted.Inspection.Client.Services;
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

        private IProcessExcelFormulaToLink _processExcelFormulaToLink;

        public GetOfstedInspections(IProcessExcelFormulaToLink processExcelFormulaToLink)
        {
            _processExcelFormulaToLink = processExcelFormulaToLink;
        }

        public GetOfstedInspections()
        {
        }

        public List<Sfa.Das.Ofsted.Inspection.Types.Inspection> GetAll()
        {
            var inspections = new List<Sfa.Das.Ofsted.Inspection.Types.Inspection>();
            using (var client = new WebClient())
            {
                using (var stream =
                    new MemoryStream(client.DownloadData(
                        new Uri(
                            "https://www.gov.uk/government/uploads/system/uploads/attachment_data/file/643394/Management_information_-_further_education_and_skills_-_as_at_31_August_2017.xlsx")))
                )
                {
                    using (var package = new ExcelPackage(stream))
                    {

                        GetOsftedInspections(package, inspections);
                    }
                }
            }
            return inspections;
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

                var overallEffectiveness = OverallEffectivenessProcessor(keyWorksheet.Cells[i, OverallEffectivenessPosition]);


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

        private static OverallEffectiveness OverallEffectivenessProcessor(ExcelRange cell)
        {
            if (cell?.Value == null) return OverallEffectiveness.NotJudged;
            var overallEffectivessString = cell.Value.ToString();

            switch (overallEffectivessString)
            {
                case "1":
                    return OverallEffectiveness.Outstanding;
                case "2":
                    return OverallEffectiveness.Good;
                case "3":
                    return OverallEffectiveness.RequiresImprovement;
                case "4":
                    return OverallEffectiveness.Inadequate;
                case "9":
                    return OverallEffectiveness.RemainedGoodAtAShortInspectionThatDidNotConvert;
            }
            return OverallEffectiveness.NotJudged;
        }

        private static DateTime GetDateTimeValue(ExcelRange excelRange)
        {
            if (excelRange == null) throw new ArgumentNullException(nameof(excelRange));
            var value = excelRange.Value.ToString();
            return DateTime.Parse(value);
        }
    }


}