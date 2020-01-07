namespace SCide.WPF.Models
{
    public interface ICommenceItem
    {
        string ClarifyFieldName { get; set; }
        string ClarifySeparator { get; set; }
        string ClarifyValue { get; set; }
        string ItemName { get; set; }
    }
}