using CmcScriptNet.FilterBuilder.Helpers;
using CmcScriptNet.FilterBuilder.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CmcScriptNet.FilterBuilder.UserControls
{
    /// <summary>
    /// Interaction logic for FilterConjunctionControl.xaml
    /// </summary>
    public partial class FilterConjunctionControl : UserControl
    {
        public FilterConjunctionControl()
        {
            InitializeComponent();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            // we are not going to bother with datacontexts and binding and models
            // we will just create a string on the fly by evaluating all radiobuttons
            var AndButtons = Utils.FindVisualChildren<RadioButton>(this)
                .Where(w => w.Content.Equals("And"))
                .OrderBy(o => o.Name);
            IList<string> list = new List<string>();
            foreach (RadioButton rb in AndButtons)
            {
                list.Add((bool)rb.IsChecked ? "AND" : "OR");
            }
            StringBuilder sb = new StringBuilder("[ViewConjunction(");
            sb.Append(string.Join(",", list));
            sb.Append(")]");
            ((FilterBuilderModel)DataContext).ViewConjunction = sb.ToString();
        }

    }
}
