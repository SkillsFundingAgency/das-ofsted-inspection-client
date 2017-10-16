using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using OfficeOpenXml;
using Esfa.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal class GetInspectionsService : IGetInspectionsService
    {
        private readonly IGetOfstedDetailsFromExcelPackageService _getOfstedDetailsFromExcelPackageService;

        public GetInspectionsService(IGetOfstedDetailsFromExcelPackageService getOfstedDetailsFromExcelPackageService)
        {
            _getOfstedDetailsFromExcelPackageService = getOfstedDetailsFromExcelPackageService;
        }

        public InspectionsDetail GetInspectionsDetail(string firstLinkUrl)
        {
            InspectionsDetail inspectionsDetail;

            try
            {
                using (var client = new WebClient())
                {
                    using (var stream =
                        new MemoryStream(client.DownloadData(new Uri(firstLinkUrl))))
                    {
                        using (var package = new ExcelPackage(stream))
                        {
                            inspectionsDetail = _getOfstedDetailsFromExcelPackageService.ExtractOfstedInspections(package);
                        }
                    }
                }
            }
            catch (UriFormatException ex)
            {
                return new InspectionsDetail
                {
                    Inspections = null,
                    StatusCode = InspectionsStatusCode.NotProcessed,
                    NotProcessedMessage = $"Error whilst trying to read url: [{firstLinkUrl}], message: [{ex.Message}]"
                  };
            }
           catch (COMException ex)
            {
                return new InspectionsDetail
                {
                    Inspections = null,
                    StatusCode = InspectionsStatusCode.NotProcessed,
                    NotProcessedMessage = $"Error whilst trying to read excel details from url: [{firstLinkUrl}], message: [{ex.Message}]"           
                };
            }

            return inspectionsDetail;
        }
    }
}
