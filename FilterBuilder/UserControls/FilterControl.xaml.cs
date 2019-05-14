using CmcScriptNet.FilterBuilder.Models;
using System.Windows;
using System.Windows.Controls;

namespace CmcScriptNet.FilterBuilder.UserControls
{
    /// <summary>
    /// Interaction logic for FilterControl.xaml
    /// </summary>
    public partial class FilterControl : UserControl
    {
        #region Contructors
        public FilterControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Dependency properties
        public static readonly DependencyProperty FilterControlModelProperty =
            DependencyProperty.Register(nameof(FilterControlModel),
            typeof(FilterControlModel),
            typeof(FilterControl),
            new FrameworkPropertyMetadata(null));
        #endregion

        public FilterControlModel FilterControlModel
        {
            get { return (FilterControlModel)GetValue(FilterControlModelProperty); }
            set { SetValue(FilterControlModelProperty, value); }
        }

        private void LvFieldList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) { return; }
            var item = e.AddedItems[0] as FieldListItem;
            if (item == null) { return; }

            if (item.IsField)
            {
                cboConnectedFieldList.IsEnabled = false;
                cboConnectedFieldList.Text = "(Field Value)";
            }
            else
            {
                cboConnectedFieldList.IsEnabled = true;
                cboConnectedFieldList.Text = "(Connection To Item)";
            }
        }

        private void Reset_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            ((FilterControlModel)DataContext).ResetFilter();
        }
    }
}
