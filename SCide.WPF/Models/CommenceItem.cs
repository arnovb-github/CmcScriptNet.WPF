namespace SCide.WPF.Models
{
    internal class CommenceItem : ICommenceItem
    {
        public string ItemName { get; set; }
        public string ClarifyValue { get; set; }
        public string ClarifyFieldName { get; set; }
        public string ClarifySeparator { get; set; }
        public override string ToString()
        {
            return ItemName + ClarifySeparator + ClarifyValue;
        }
    }
}
