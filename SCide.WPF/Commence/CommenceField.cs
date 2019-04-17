namespace SCide.WPF.Commence
{
    public class CommenceField
    {
        public CommenceField(string name, string category)
        {
            Name = name;
            Category = category;
        }
        public string Name { get; set; }
        public string Category { get; set; }
        public string FormString => "Form.Field(\"" + Name + "\")";

        public override string ToString()
        {
            return Name;
        }
    }
}
