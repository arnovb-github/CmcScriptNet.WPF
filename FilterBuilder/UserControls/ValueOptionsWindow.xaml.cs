using CmcScriptNet.FilterBuilder.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CmcScriptNet.FilterBuilder.UserControls
{
    /// <summary>
    /// Interaction logic for ValueOptionsWindow.xaml
    /// </summary>
    public partial class ValueOptionsWindow : Window
    {
        private const string OR_SEPARATOR = " .OR. ";
        private const string AND_SEPARATOR = " .AND. ";

        public ValueOptionsWindow(string currentValue)
        {
            InitializeComponent();
            Result = currentValue;           
        }

        private void InitializeValues()
        {
            string s = Result;
            // what do we have in the fieldvalue now?
            if (string.IsNullOrEmpty(s)) { return; }
            if (s.Contains(OR_SEPARATOR))
            {
                rbAny.IsChecked = true;
                PopulateTextBoxes(s.Split(new[] { OR_SEPARATOR }, StringSplitOptions.None));
            }
            else if (s.Contains(AND_SEPARATOR))
            {
                rbAll.IsChecked = true;
                PopulateTextBoxes(s.Split(new[] { AND_SEPARATOR }, StringSplitOptions.None));
            }
            else
            {
                TextBoxes.First().Text = s;
            }
        }

        internal string Result { get; private set; }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // get all the textboxes
            var children = TextBoxes;

            // return the values in the correct filter syntax conjunction
            IEnumerable<string> values = GetTextBoxValues(children);

            Result = (bool)rbAny.IsChecked
                ? string.Join(OR_SEPARATOR, values)
                : string.Join(AND_SEPARATOR, values);

            if (!string.IsNullOrEmpty(Result))
            {
                this.DialogResult = true;
            }
        }

        private IList<TextBox> textBoxes = null;
        private IList<TextBox> TextBoxes
        {
            get
            {
                if (textBoxes == null)
                {
                    textBoxes = Utils.FindVisualChildren<TextBox>(this).ToList();
                }
                return textBoxes;
            }
        }

        // TODO make into generic method
        private IEnumerable<string> GetTextBoxValues(IEnumerable<TextBox> values)
        {
            foreach (TextBox tb in values)
            {
                if (!string.IsNullOrEmpty(tb.Text))
                {
                    yield return tb.Text;
                }
            }
        }

        private void PopulateTextBoxes(IEnumerable<string> values)
        {
            // if there are more values than we have boxes,
            // one or more of the values likely contains an embedded separator
            // this would screw up everything so just return;
            if (values.Count() > TextBoxes.Count()) { return; }

            int i = 0;
            foreach (string v in values)
            {
                TextBoxes[i].Text = v;
                i++;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeValues();
        }
    }
}
