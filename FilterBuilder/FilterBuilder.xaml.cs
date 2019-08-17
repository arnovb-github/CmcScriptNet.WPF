using CmcScriptNet.FilterBuilder.Models;
using FilterBuilder.Helpers;
using System.Windows;

namespace CmcScriptNet.FilterBuilder
{
    /// <summary>
    /// Interaction logic for FilterBuilderWindow.xaml
    /// </summary>
    public partial class FilterBuilderWindow : Window
    {
        public FilterBuilderWindow(string categoryName)
        {
            InitializeComponent();
            ((FilterBuilderModel)DataContext).Init(categoryName);
        }

        //// DEBUG category hardcoded
        //public FilterBuilderWindow()
        //{
        //    InitializeComponent();
        //    ((FilterBuilderModel)DataContext).Init("CategoryA");
        //}

        public string Result { get; private set; }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var model = (FilterBuilderModel)DataContext;
            var listItem = filterList.SelectedItem as FilterListItem;
            if (listItem == null) { return; }
            if (listItem.Tag.Equals("filter"))
            {
                Result = FilterStringCreator.ToString(model.CurrentFilterControlModel.CurrentFilter, model.CurrentFilterControlModel.OutputFormat);
            }
            else
            {
                Result = model.ViewConjunction;
            }
            DialogResult = true;
        }
    }
}
