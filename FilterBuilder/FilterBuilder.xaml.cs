using CmcScriptNet.FilterBuilder.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CmcScriptNet.FilterBuilder
{
    // TODO this is all really messy and in dire need of refactoring and reorganizing

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
            var listItem = filterList.SelectedValue as FilterListItem;
            if (listItem == null) { return; }
            if (listItem.Tag.Equals("filter"))
            {
                Result = model.CurrentFilterControlModel.CurrentFilter.ToString() ?? string.Empty;
            }
            else
            {
                Result = model.ViewConjunction;
            }
            DialogResult = true;
        }

        // TODO simplify this, we should be able to get rid of it
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitialized) { return; }
            if (e.AddedItems.Count == 0) { return; }
            var item = e.AddedItems[0] as FilterListItem;
            if (item == null) { return; }
            filterBuilderModel.CurrentFilterControlModel =
                filterBuilderModel.FilterControlModels.SingleOrDefault(s => s.ClauseNumber.Equals(item.ClauseNumber));
        }
    }
}
