using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Esfa.Ofsted.Inspection.Client.Services;
using Esfa.Ofsted.Inspection.Client.Services.interfaces;
using OfficeOpenXml;
using Sfa.Das.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.ApplicationServices
{
    public class GetOfstedInspections : IGetOfstedInspections
    {
        //private readonly ILog _log;
        private readonly IGetInspectionsService _getInspectionsService;
        private readonly IAngleSharpService _angleSharpService;
        private readonly IAppServiceSettings _appServiceSettings;

        public GetOfstedInspections(IAngleSharpService angleSharpService, IAppServiceSettings appServiceSettings,IGetInspectionsService getInspectionsService)
        {
            _angleSharpService = angleSharpService;
            _appServiceSettings = appServiceSettings;
            _getInspectionsService = getInspectionsService;
        }


        public InspectionsDetail GetAll()
        {
            var getFirstMatchingLink = _angleSharpService.GetLinks(_appServiceSettings.InspectionSiteUrl, "a", _appServiceSettings.LinkText).FirstOrDefault();
            if (getFirstMatchingLink==null)
                {
                return RaiseNotProcessedInspectionError($"Could not locate any links in page [{_appServiceSettings.InspectionSiteUrl}] with text [{_appServiceSettings.LinkText}]");
                }

            var firstLinkUrl = BuildFirstLinkUrl(getFirstMatchingLink);

            if (firstLinkUrl == string.Empty)
                {
                return RaiseNotProcessedInspectionError($"Could not build a valid url from url [{_appServiceSettings.InspectionSiteUrl}], link [{getFirstMatchingLink}]");
                }

            return _getInspectionsService.GetInspectionsDetail(firstLinkUrl);
        }
        
        private string BuildFirstLinkUrl(string getFirstMatchingLink)
        {
            Uri uriResult;
            if (!Uri.TryCreate(_appServiceSettings.InspectionSiteUrl, UriKind.Absolute, out uriResult))
                return string.Empty;

            var firstLinkUrl = $"{uriResult.Scheme}://{uriResult.Host}{getFirstMatchingLink}";
            return firstLinkUrl;
        }

        private static InspectionsDetail RaiseNotProcessedInspectionError(string message)
        {
            return new InspectionsDetail
            {
                StatusCode = InspectionsStatusCode.NotProcessed,
                ErrorSet = new List<InspectionError>
                {
                    new InspectionError
                    {
                        LineNumber = 0,
                        Message = message
                    }
                }
            };
        }
    }
}