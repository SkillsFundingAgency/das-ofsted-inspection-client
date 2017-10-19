using System.ComponentModel;

namespace Esfa.Ofsted.Inspection.Types
{
    /// <summary>
    /// Overall Effectiveness Rating
    /// </summary>
    public enum OverallEffectiveness
    {
        /// <summary>
        /// Outstanding
        /// </summary>
        [Description("Outstanding")]
        Outstanding,
        /// <summary>
        /// Good
        /// </summary>
        [Description("Good")]
        Good,
        /// <summary>
        /// Requires improvement (known as 'satisfactory' prior to 1 September 2012)
        /// </summary>
        [Description("Requires improvement (known as 'satisfactory' prior to 1 September 2012)")]
        RequiresImprovement,
        /// <summary>
        /// Inadequate
        /// </summary>
        [Description("Inadequate")]
        Inadequate,
        /// <summary>
        /// Remained good at a short inspection that did not convert
        /// </summary>
        [Description("Remained good at a short inspection that did not convert")]
        RemainedGoodAtAShortInspectionThatDidNotConvert,
        /// <summary>
        /// Not judged
        /// </summary>
        [Description("Not judged")]
        NotJudged
    }
}
