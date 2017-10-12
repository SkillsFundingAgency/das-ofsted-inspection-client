using System;
using System.Collections.Generic;
using System.Linq;
using Esfa.Ofsted.Inspection.Client.Services;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using Esfa.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.ApplicationServices
{
    /// <summary>
    /// Get Latest Ofsted Inspection details available
    /// </summary>
    public class GetOfstedInspections : IGetOfstedInspections
    {
        private readonly IGetInspectionsService _getInspectionsService;
        private readonly IAngleSharpService _angleSharpService;
        private readonly IAppServiceSettings _appServiceSettings;

        /// <summary>
        /// default constructor
        /// </summary>
        public GetOfstedInspections() : this(new AngleSharpService(new HttpService()), 
                                            new AppServiceSettings(), 
                                            new GetInspectionsService(new GetOfstedDetailsFromExcelPackageService(
                                                                    new ProcessExcelFormulaToLink(), 
                                                                    new OverallEffectivenessProcessor())))
        {}

        internal GetOfstedInspections(IAngleSharpService angleSharpService, IAppServiceSettings appServiceSettings,IGetInspectionsService getInspectionsService)
        {
            _angleSharpService = angleSharpService;
            _appServiceSettings = appServiceSettings;
            _getInspectionsService = getInspectionsService;
        }


        /// <summary>
        /// Returns a list of ofsted inspection details 
        /// </summary>
        /// <returns>A set of inspection details giving website, ukrpn, date ofsted results published, and effectiveness rating</returns>
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