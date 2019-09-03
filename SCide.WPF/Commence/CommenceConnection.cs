namespace SCide.WPF.Commence
{
    public class CommenceConnection
    {
        public CommenceConnection(string connectionName, string toCategoryName)
        {
            Name = connectionName;
            Category = toCategoryName;
        }
        public string Name { get; }
        public string Category { get; }
        public string FullName => Name + ' ' + Category;
        public string FormString => "Form.Connection(\"" + Name + "\",\"" + Category + "\")";
    }
}