using System;

namespace Esfa.Ofsted.Inspection.Types
{
    public class OfstedInspection
    {
        public string Website { get; set; }
        public int Ukprn { get; set; }
        public DateTime DatePublished { get; set; }
        public OverallEffectiveness OverallEffectiveness { get; set; }
    }
}
