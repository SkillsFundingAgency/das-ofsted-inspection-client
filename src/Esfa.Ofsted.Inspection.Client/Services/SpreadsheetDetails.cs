namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal struct SpreadsheetDetails
    {
        public int WebLinkColumn { get; set; }
        public int UkPrnColumn { get; set; }
        public int DatePublishedColumn { get; set; }
        public int OverallEffectivenessColumn { get; set; }
        public int DataStartsRow { get; set; }

        public bool AreAllColumnHeadingsMatched => WebLinkColumn>0 && UkPrnColumn>0 && DatePublishedColumn>0 && OverallEffectivenessColumn>0;
    }
}