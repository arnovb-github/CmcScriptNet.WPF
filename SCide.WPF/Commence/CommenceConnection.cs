using System;

namespace SCide.WPF.Commence
{
    public class CommenceConnection
    {
        public CommenceConnection(string connectionName, string toCategoryName)
        {
            this.Name = connectionName;
            this.Category = toCategoryName;
        }
        public string Name { get; }
        public string Category { get; }
        public string FormString => "Form.Connection(\"" + Name + "\",\"" + Category + "\")";

        public override string ToString()
        {
            return Name + " " + Category;
        }
    }
}
