using CmcScriptNet.FilterBuilder.Models;
using System.Windows.Controls;
using System.Windows.Input;

namespace CmcScriptNet.FilterBuilder.UserControls
{
    /// <summary>
    /// Interaction logic for SingleStringValueControl.xaml
    /// </summary>
    public partial class SingleStringValueControl : UserControl
    {
        public SingleStringValueControl()
        {
            InitializeComponent();
        }

        private void ShowFieldValueOptions_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FilterControlModel model = (FilterControlModel)this.DataContext;
            ValueOptionsWindow vow = new ValueOptionsWindow(model.FieldValue);
            if ((bool)vow.ShowDialog())
            {
                model.FieldValue = vow.Result;
            }
        }
    }
}
