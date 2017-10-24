using System;
using System.IO;
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
        private readonly IWebClientFactory _webClientFactory;

        public GetInspectionsService() : this(new LogFunctions())
        { }


        public GetInspectionsService(ILogFunctions logger) : this(logger, new GetOfstedDetailsFromExcelPackageService(), new SystemWebClientFactory())
        {
            _logger = logger;
        }

        public GetInspectionsService(ILogFunctions logger, IGetOfstedDetailsFromExcelPackageService getOfstedDetailsFromExcelPackageService, IWebClientFactory webClientFactory)
        {
            _logger = logger;
            _getOfstedDetailsFromExcelPackageService = getOfstedDetailsFromExcelPackageService;
            _webClientFactory = webClientFactory;
        }

        public InspectionOutcomesResponse GetInspectionsDetail(string firstLinkUrl)
        {
            InspectionOutcomesResponse inspectionOutcomesResponse;
            try
            {
                _logger.Debug("Opening web client");

                var webClient = _webClientFactory.Create();

                using (var client = webClient)
                {
                    _logger.Debug("Opening memory stream");
                    using (var stream =
                        new MemoryStream(client.DownloadData(new Uri(firstLinkUrl))))
                    {
                        _logger.Debug("Opened memory stream");

                        using (var package = new ExcelPackage(stream))
                        {
                            _logger.Debug("Opened excel package");
                            inspectionOutcomesResponse = _getOfstedDetailsFromExcelPackageService.ExtractOfstedInspections(package);
                        }
                        _logger.Debug("Closed excel package");

                    }
                    _logger.Debug("Closed memory stream");

                }
                _logger.Debug($"Closed web client");

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
            catch (Exception ex)
            {
                var message = $"Error whilst trying to read excel details";
                var exception = new UrlReadingException(message, ex);
                _logger.Error(message, exception);
                throw exception;
            }

            return inspectionOutcomesResponse;
        }
    }
}
