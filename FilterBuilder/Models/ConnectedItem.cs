namespace CmcScriptNet.FilterBuilder.Models
{
    public class ConnectedItem
    {
        public ConnectedItem() { }

        public ConnectedItem(string displayName, string itemName, string clarifySeparator, string clarifyValue)
        {
            DisplayName = displayName;
            ItemName = itemName;
            ClarifySeparator = clarifySeparator;
            ClarifyValue = clarifyValue;
        }
        public string DisplayName { get; set; }
        public string ItemName { get; set; }
        public string ClarifySeparator { get; set; }
        public string ClarifyValue { get; set; }
    }
}
