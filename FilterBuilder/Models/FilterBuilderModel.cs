using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CmcScriptNet.FilterBuilder.Models
{
    public class FilterBuilderModel : INotifyPropertyChanged
    {
        #region Constructors
        public FilterBuilderModel()
        {
            PopulateFilterList();
        }
        #endregion

        #region Properties
        public string CategoryName { get; set; }
        public IList<FilterListItem> FilterList { get; private set; }

        private string _viewConjunction = "[ViewConjunction(AND,AND,AND,AND,AND,AND,AND)]";
        public string ViewConjunction
        {
            get
            {
                return _viewConjunction;
            }
            set
            {
                _viewConjunction = value;
                OnPropertyChanged();
            }
        }

        // TODO rethink this. We want to capture all FilterControlModel instances
        // so we can re-show the filter when you switch between filters
        private FilterControlModel _currentFilterControlModel;
        public FilterControlModel CurrentFilterControlModel
        {
            get { return _currentFilterControlModel; }
            set
            {
                _currentFilterControlModel = value;
                OnPropertyChanged();
            }
        }

        // TODO this is probably overly complicated
        // a better solution(?) is to have to filtercontrol figure out what values it needs,
        // in essence: pull, not push.
        // on the other hand, we want to keep some kind of state. Hmm.
        public IEnumerable<FilterControlModel> FilterControlModels { get; private set; }

        #endregion

        #region Methods
        private bool _init = false;
        /// <summary>
        /// Initializer, should be called once
        /// </summary>
        /// <param name="categoryName">Commence categoryname.</param>
        internal void Init(string categoryName)
        {
            if (_init) { return; }
            this.CategoryName = categoryName;
            IList<FilterControlModel> modelList = new List<FilterControlModel>();
            for (int i = 1; i < 2; i++) // just one now, but left like this for future adaptations.
            {
                modelList.Add(new FilterControlModel(CategoryName)
                {
                    ClauseNumber = i,
                });
            }
            this.FilterControlModels = modelList;
            // select the first filter
            this.CurrentFilterControlModel = FilterControlModels.First();
            _init = true;
        }

        private void PopulateFilterList()
        {
            FilterList = new List<FilterListItem>();
            FilterList.Add(new FilterListItem()
            {
                DisplayName = "Summary 1-8",
                Tag = "summary"
            });
            // filters are numbered 1 to 8
            for (int i = 1; i < 2; i++) // just one, but left like this for future adaptations.
            {
                FilterList.Add(new FilterListItem()
                {
                    //DisplayName = "Filter " + i.ToString(),
                    DisplayName = "Create Filter",
                    Tag = "filter",
                    ClauseNumber = i
                });
            }
        }

        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Event raisers
        // [CallerMemberName] eliminates the need to supply the propertyName
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
