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
        private readonly IConfigurationSettings _configurationSettings;

        /// <summary>
        /// default constructor
        /// </summary>
        public GetOfstedInspections() : this(new AngleSharpService(new HttpService()), 
                                            new ConfigurationSettings(), 
                                            new GetInspectionsService(new GetOfstedDetailsFromExcelPackageService(
                                                                                new ProcessExcelFormulaToLink(), 
                                                                                new OverallEffectivenessProcessor(), 
                                                                                new ConfigurationSettings())))
        {}

        internal GetOfstedInspections(IAngleSharpService angleSharpService, IConfigurationSettings configurationSettings,IGetInspectionsService getInspectionsService)
        {
            _angleSharpService = angleSharpService;
            _configurationSettings = configurationSettings;
            _getInspectionsService = getInspectionsService;
        }


        /// <summary>
        /// Returns a list of ofsted inspection details 
        /// </summary>
        /// <returns>A set of inspection details giving website, ukrpn, date ofsted results published, and effectiveness rating</returns>
        public InspectionsDetail GetAll()
        {
            var getFirstMatchingLink = _angleSharpService.GetLinks(_configurationSettings.InspectionSiteUrl, "a", _configurationSettings.LinkText).FirstOrDefault();
            if (getFirstMatchingLink==null)
                {
                return RaiseNotProcessedInspectionError($"Could not locate any links in page [{_configurationSettings.InspectionSiteUrl}] with text [{_configurationSettings.LinkText}]");
                }

            var firstLinkUrl = BuildFirstLinkUrl(getFirstMatchingLink);

            if (firstLinkUrl == string.Empty)
                {
                return RaiseNotProcessedInspectionError($"Could not build a valid url from url [{_configurationSettings.InspectionSiteUrl}], link [{getFirstMatchingLink}]");
                }

            return _getInspectionsService.GetInspectionsDetail(firstLinkUrl);
        }
        
        private string BuildFirstLinkUrl(string getFirstMatchingLink)
        {
            Uri uriResult;
            if (!Uri.TryCreate(_configurationSettings.InspectionSiteUrl, UriKind.Absolute, out uriResult))
                return string.Empty;

            var firstLinkUrl = $"{uriResult.Scheme}://{uriResult.Host}{getFirstMatchingLink}";
            return firstLinkUrl;
        }

        private static InspectionsDetail RaiseNotProcessedInspectionError(string message)
        {
            return new InspectionsDetail
            {
                StatusCode = InspectionsStatusCode.NotProcessed,
                NotProcessedMessage = message
            };
        }
    }
}