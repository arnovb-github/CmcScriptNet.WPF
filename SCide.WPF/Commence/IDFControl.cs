using System.Text;

namespace SCide.WPF.Commence
{
    public class IDFControl
    {
        // propertynames have to match the case of the node element
        // for deserialization to work
        public string NAME { set; get; }
        public string DATAFIELD { set; get; }
        public string CAPTION { set; get; }

        public string ToolTipText
        {
            get
            {
                StringBuilder sb = new StringBuilder("Control name: " + this.NAME + "; Caption: ");
                sb.Append(string.IsNullOrEmpty(this.CAPTION) ? "[null]" : '"' + this.CAPTION + '"');
                sb.Append("; Field: ");
                sb.Append(string.IsNullOrEmpty(this.DATAFIELD) ? "[unbound]" : '"' + this.DATAFIELD + '"');
                return sb.ToString();
            }
        }

        public string FormString => "Form.RunTime.GetControl(\"" + NAME + "\")";

        public override string ToString()
        {
            return NAME; 
        }
    }
}
