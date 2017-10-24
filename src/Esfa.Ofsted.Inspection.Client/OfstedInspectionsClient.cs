using System;
using System.Linq;
using Esfa.Ofsted.Inspection.Client.Services;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using Esfa.Ofsted.Inspection.Types;
using Esfa.Ofsted.Inspection.Types.Exceptions;

namespace Esfa.Ofsted.Inspection.Client
{
    /// <summary>
    /// Get Latest Ofsted Inspection details available
    /// </summary>
    public class OfstedInspectionsClient : IOfstedInspectionsClient
    {
        private readonly IGetInspectionsService _getInspectionsService;
        private readonly IAngleSharpService _angleSharpService;
        private readonly IConfigurationSettings _configurationSettings;
        private readonly ILogFunctions _logger;

        /// <summary>
        /// OfstedInspectionsClient with default logging
        /// </summary>
        public OfstedInspectionsClient() : this(new LogFunctions())
        {}

        /// <summary>
        /// OfstedInspectionsClient with logging overrides available
        /// </summary>
        public OfstedInspectionsClient(ILogFunctions logger) : 
            this(logger,
                new AngleSharpService(),
                new ConfigurationSettings(),
                new GetInspectionsService(logger)
                )
        {
            _logger = logger;
        }

        internal OfstedInspectionsClient(ILogFunctions logger, IAngleSharpService angleSharpService, IConfigurationSettings configurationSettings,IGetInspectionsService getInspectionsService)
        {
            _angleSharpService = angleSharpService;
            _configurationSettings = configurationSettings;
            _getInspectionsService = getInspectionsService;
            _logger = logger;
        }


        /// <summary>
        /// Returns a list of ofsted inspection details 
        /// </summary>
        /// <returns>A set of inspection details giving website, ukrpn, date ofsted results published, and effectiveness rating</returns>
        public InspectionOutcomesResponse GetOfstedInspectionOutcomes()
        {
            _logger.Info("Start: gathering of Ofsted details");
             var getFirstMatchingLink = GetFirstMatchingLink();
            _logger.Debug($"First Link retrieved {getFirstMatchingLink}");
            var firstLinkUrl = BuildFullLinkFromRelativeFirstLink(getFirstMatchingLink);
            _logger.Debug($"First link built: '{firstLinkUrl}'");
            var result = _getInspectionsService.GetInspectionsDetail(firstLinkUrl);
            _logger.Info("End: All Ofsted details returned");
            return result;
        }

        private string BuildFullLinkFromRelativeFirstLink(string getFirstMatchingLink)
        {
            var firstLinkUrl = BuildFirstLinkUrl(getFirstMatchingLink);

            if (firstLinkUrl != string.Empty) return firstLinkUrl;

            var noFirstLinkValid =
                $"Could not build a valid url from url [{_configurationSettings.InspectionSiteUrl}], link [{getFirstMatchingLink}]";
            _logger.Error(noFirstLinkValid, new InvalidLinkException(noFirstLinkValid));
            throw new InvalidLinkException(noFirstLinkValid);
        }

        private string GetFirstMatchingLink()
        {
            var getFirstMatchingLink = _angleSharpService
                .GetLinks(_configurationSettings.InspectionSiteUrl, "a", _configurationSettings.LinkText).FirstOrDefault();

            if (getFirstMatchingLink != null) return getFirstMatchingLink;

            var noLinksInPageMessage =
                $"Could not locate any links in page [{_configurationSettings.InspectionSiteUrl}] with text [{_configurationSettings.LinkText}]";
            _logger.Error(noLinksInPageMessage, new NoLinksInPageException(noLinksInPageMessage));
            throw new NoLinksInPageException(noLinksInPageMessage);
        }

        private string BuildFirstLinkUrl(string getFirstMatchingLink)
        {
            Uri uriResult;
            if (!Uri.TryCreate(_configurationSettings.InspectionSiteUrl, UriKind.Absolute, out uriResult))
                return string.Empty;

            var firstLinkUrl = $"{uriResult.Scheme}://{uriResult.Host}{getFirstMatchingLink}";
            return firstLinkUrl;
        }

    }


}