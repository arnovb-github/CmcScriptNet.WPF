﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CmcScriptNet.FilterBuilder.Models
{
    public class FilterBuilderModel : INotifyPropertyChanged
    {
        #region Constructors
        public FilterBuilderModel() { }
        #endregion

        #region Properties
        public string CategoryName { get; set; }

        private IList<FilterListItem> _filterListItems;
        public IList<FilterListItem> FilterListItems
        {
            get { return _filterListItems; }
            private set
            {
                _filterListItems = value;
                OnPropertyChanged();
            }
        }

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
            this.FilterListItems = GetFilterListItems(CategoryName);
            _init = true;
        }

        private IList<FilterListItem> GetFilterListItems(string categoryName)
        {
            var retval = new List<FilterListItem>
            {
                new FilterListItem()
                {
                    DisplayName = "Summary 1-8",
                    Tag = "summary"
                }
            };
            // filters are numbered 1 to 8
            for (int i = 1; i < 2; i++) // just one, but left like this for future adaptations.
            {
                retval.Add(new FilterListItem()
                {
                    //DisplayName = "Filter " + i.ToString(),
                    DisplayName = "Create Filter",
                    Tag = "filter",
                    ClauseNumber = i,
                    FilterControlModel = new FilterControlModel(categoryName)
                    {
                        ClauseNumber = i,
                    }
                });
            }
            return retval;
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
