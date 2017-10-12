namespace Esfa.Ofsted.Inspection.Types
{
   /// <summary>
   /// Details on line errors in processing the ofsted details (line = 0 is a file error)
   /// </summary>
   public  class InspectionError
    {
        public int LineNumber { get; set; }
        public string Message { get; set; }
    }
}
