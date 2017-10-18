﻿using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Esfa.Ofsted.Inspection.Client.ApplicationServices;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using OfficeOpenXml;
using Esfa.Ofsted.Inspection.Types;
using Esfa.Ofsted.Inspection.Types.Exceptions;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal class GetInspectionsService : IGetInspectionsService
    {
        private readonly IGetOfstedDetailsFromExcelPackageService _getOfstedDetailsFromExcelPackageService;
        private readonly ILogFunctions _logger;

        public GetInspectionsService() : this(new LogFunctions(), new GetOfstedDetailsFromExcelPackageService())
        { }
        
        public GetInspectionsService(ILogFunctions logger, IGetOfstedDetailsFromExcelPackageService getOfstedDetailsFromExcelPackageService)
        {
            _logger = logger;
            _getOfstedDetailsFromExcelPackageService = getOfstedDetailsFromExcelPackageService;
        }

        public InspectionsDetail GetInspectionsDetail(string firstLinkUrl)
        {
            InspectionsDetail inspectionsDetail;
            try
            {
                _logger.Debug("Opening web client");

                using (var client = new WebClient())
                {
                    _logger.Debug("Opening memory stream");
                    using (var stream =
                        new MemoryStream(client.DownloadData(new Uri(firstLinkUrl))))
                    {
                        _logger.Debug("Opened memory stream");

                        using (var package = new ExcelPackage(stream))
                        {
                            _logger.Debug("Opened excel package");
                            inspectionsDetail = _getOfstedDetailsFromExcelPackageService.ExtractOfstedInspections(package);
                        }
                    }
                }
            }
            catch (UriFormatException ex)
            {
                var message = $"Error whilst trying to read url: [{firstLinkUrl}]";
                var exception = new UrlReadingException(message, ex);
                _logger.Error(message, exception);
                throw exception;
            }
           catch (COMException ex)
           {
               var message = $"Error whilst trying to read excel details from url: [{firstLinkUrl}], message: [{ex.Message}]";
               var exception = new UrlReadingException(message, ex);
               _logger.Error(message, exception);
               throw exception;
            }

            return inspectionsDetail;
        }
    }
}
