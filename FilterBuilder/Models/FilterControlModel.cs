using CmcScriptNet.FilterBuilder.Extensions;
using CmcScriptNet.FilterBuilder.Helpers;
using FilterBuilder.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Vovin.CmcLibNet.Database;
using Vovin.CmcLibNet.Database.Metadata;

namespace CmcScriptNet.FilterBuilder.Models
{
    /* Exposes all possible filter properties for binding
     * Lets a helper class construct the actual fiter
     * by taking the properties that pertain to the class
     */

    /// <summary>
    /// Model for the FilterControl usercontrol. Set this class as DataContext for the control when you use it.
    /// </summary>
    public class FilterControlModel : INotifyPropertyChanged
    {
        #region Fields
        // we'll create a dictionary of value/description pairs for all FilterQualifier enum values
        // we'll use that to get the appropriate enum values for a particular field type
        private readonly Dictionary<FilterQualifier, QualifierMember> _qualifierDictionary;
        private FilterConstructor _filterConstructor;

        #endregion
        #region Constructors
        public FilterControlModel(string categoryName)
        {
            CategoryName = categoryName;
            FieldList = PopulateFieldList(CategoryName);
            _qualifierDictionary = CreateQualifierDictionary();
        }

        public string CategoryName { get; set; }

        private int _clauseNumber = 1;
        public int ClauseNumber
        {
            get
            {
                return _clauseNumber;
            }
            set
            {
                _clauseNumber = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<int> ClauseNumbers => new[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        private IList<FieldListItem> _fieldListItems;
        public IList<FieldListItem> FieldList
        {
            get
            {
                return _fieldListItems;
            }
            set
            {
                _fieldListItems = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Represents the item selected in the 'main' field list (lvFieldList)
        /// </summary>
        private FieldListItem _selectedFieldListItem;
        public FieldListItem SelectedFieldListItem
        {
            get { return _selectedFieldListItem; }
            set
            {
                _selectedFieldListItem = value;
                if (value == null) { return; }
                CurrentFieldListItem = value;
                // do we have a field or category?
                if (!string.IsNullOrEmpty(_selectedFieldListItem.ToCategory)) // item is connection to category
                {
                    this.SelectedConnectedCategory = _selectedFieldListItem.ToCategory;
                    ConnectedFieldListItems = PopulateFieldList(_selectedFieldListItem.ToCategory);
                    // at this point, we should also be populating the connected item names list
                    // but only when it does not contain an insane number of items
                    // ideally this should be done async
                    this.ConnectedItemNames = PopulateConnectedItemNamesList();
                    InitializeFilter(FilterType.ConnectionToItem);
                }
                else
                {
                    InitializeFilter(FilterType.Field);
                }
                OnPropertyChanged();
            }
        }

        // we may have to introduce a separate SelectedFieldListItem property to bind to
        // currently we are binding to SelectedFieldListItem
        // but that goes wrong when we are dealing with a connection
        private FieldListItem _currentFieldListItem;
        public FieldListItem CurrentFieldListItem
        {
            get { return _currentFieldListItem; }
            set
            {
                _currentFieldListItem = value;
                if (_currentFieldListItem.IsField)
                {
                    this.QualifierMembers = QualifierLibrary.GetQualifierMembers(_qualifierDictionary, value.FieldDefinition.Type);
                }
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Represents the items shown in connected field list (lbxConnectedItemNames)
        /// </summary>
        private IList<FieldListItem> _connectedFieldListItems;
        public IList<FieldListItem> ConnectedFieldListItems
        {
            get { return _connectedFieldListItems; }
            set
            {
                _connectedFieldListItems = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Represents the item selected in the connected field list (lbxConnectedItemNames)
        /// </summary>
        // not called when FilterControlModel is re-assigned.
        private FieldListItem _selectedConnectionFieldListItem;
        public FieldListItem SelectedConnectionFieldListItem
        {
            get { return _selectedConnectionFieldListItem; }
            set
            { 
                _selectedConnectionFieldListItem = value;
                if (_selectedConnectionFieldListItem == null) { return; }
                this.CurrentFieldListItem = _selectedConnectionFieldListItem;
                this.SelectedConnectedCategory = _selectedConnectionFieldListItem.ToCategory;
                this.ConnectedItemNames = PopulateConnectedItemNamesList();
                if (!string.IsNullOrEmpty(value.ToCategory))
                {
                    InitializeFilter(FilterType.ConnectionToCategoryToItem);
                }
                else 
                {
                    this.CurrentFieldListItem = _selectedConnectionFieldListItem;
                    InitializeFilter(FilterType.ConnectionToCategoryField);
                }
                OnPropertyChanged(); 
            }
        }

        /// <summary>
        /// Represents an item in a connected category (both CTI and CTCTI)
        /// </summary>
        private ConnectedItem _currentConnectedItem;
        public ConnectedItem CurrentConnectedItem
        {
            get { return _currentConnectedItem; }
            set
            {
                _currentConnectedItem = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Represents the list of name files for a connected category (CTI and CTCTI)
        /// </summary>
        private IList<ConnectedItem> _connectedItemsNames;
        public IList<ConnectedItem> ConnectedItemNames
        {
            get { return _connectedItemsNames; }
            set
            {
                _connectedItemsNames = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Represents the directly connected category (CT*)
        /// </summary>
        private string _selectedConnectedCategory;
        public string SelectedConnectedCategory
        {
            get { return _selectedConnectedCategory; }
            set
            {
                _selectedConnectedCategory = value;
                this.CategoryDefinition = GetCategoryDefinition(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Search string for connected itemnames when > 1000
        /// </summary>
        private string _connectedItemSearchString;
        public string ConnectedItemSearchString
        {
            get
            {
                return _connectedItemSearchString;
            }
            set
            {
                _connectedItemSearchString = value;
                OnPropertyChanged();
                this.ConnectedItemNames = PopulateConnectedItemNamesList();
            }
        }

        private ICursorFilter _currentFilter;
        public ICursorFilter CurrentFilter
        {
            get
            {
                return _currentFilter;
            }
            set
            {
                _currentFilter = value;
                OnPropertyChanged();
            }
        }

        private bool _except;
        public bool Except
        {
            get { return _except; }
            set
            {
                _except = value;
                if (this.CurrentFilter == null) { return; }
                this.CurrentFilter.Except = value;
                OnPropertyChanged();
            }
        }

        private string _fieldValue;
        public string FieldValue
        {
            get { return _fieldValue; }
            set
            {
                _fieldValue = value;
                OnPropertyChanged();
            }
        }

        private string _betweenStart;
        public string BetweenStart
        {
            get { return _betweenStart; }
            set
            {
                _betweenStart = value;
                OnPropertyChanged();
            }
        }

        private string _betweenEnd;
        public string BetweenEnd
        {
            get { return _betweenEnd; }
            set
            {
                _betweenEnd = value;
                OnPropertyChanged();
            }
        }

        private bool _matchCase;
        public bool MatchCase
        {
            get { return _matchCase; }
            set
            {
                _matchCase = value;
                OnPropertyChanged();
            }
        }

        private IList<QualifierMember> _filterQualifierMembers;
        public IList<QualifierMember> QualifierMembers
        {
            get { return _filterQualifierMembers; }
            set
            {
                _filterQualifierMembers = value;
                OnPropertyChanged();
            }
        }

        private FilterQualifier _selectedFilterQualifier;
        public FilterQualifier SelectedFilterQualifier
        {
            get { return _selectedFilterQualifier; }
            set
            {
                _selectedFilterQualifier = value;
                OnPropertyChanged();
            }
        }

        private bool _isValid;
        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                _isValid = value;
                OnPropertyChanged();
            }
        }

        private FilterOutputFormat _filterOutputFormat;
        public FilterOutputFormat OutputFormat
        {
            get { return _filterOutputFormat; }
            set
            {
                _filterOutputFormat = value;
                OnPropertyChanged(); // TODO do we need this?
            }
        }

        internal ICategoryDef CategoryDefinition { get; set; }

        #endregion

        #region Methods
        // TODO: should be async
        private IList<FieldListItem> PopulateFieldList(string categoryName)
        {
            IList<FieldListItem> retval = new List<FieldListItem>();
            // TODO wrap in try-catch
            using (CommenceDatabase db = new CommenceDatabase())
            {
                foreach (string field in db.GetFieldNames(categoryName))
                {
                    retval.Add(new FieldListItem()
                    {
                        FieldName = field,
                        FieldDefinition = db.GetFieldDefinition(categoryName, field),
                        DisplayName = field
                    });
                }
                foreach (CommenceConnection conn in db.GetConnectionNames(categoryName))
                {
                    retval.Add(new FieldListItem()
                    {
                        DisplayName = conn.Name + " " + conn.ToCategory,
                        ConnectionName = conn.Name,
                        ToCategory = conn.ToCategory
                    });
                }
            }
            // Return only fields with a fielddefinition.
            // If they do not have a field definiton,
            // they most likely have a name that DDE commands trip over,
            // which means that using them in a filter is virtually guaranteed to fail as well.
            // If we would be really fancy we'd include them with a different color. That is too complex for now.
            return retval.Where(w => (w.FieldDefinition != null 
                        // exclude fields that cannot be used in a filter
                        && w.FieldDefinition.Type != CommenceFieldType.Image
                        && w.FieldDefinition.Type != CommenceFieldType.Datafile
                        && w.FieldDefinition.Type != CommenceFieldType.ExcelCell)
                        || !string.IsNullOrEmpty(w.ToCategory))
                    .OrderBy(o => o.FieldName).ToList();
        }
    

        private ICategoryDef GetCategoryDefinition(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName)) { return null; }
            using (ICommenceDatabase db = new CommenceDatabase())
            {
                return db.GetCategoryDefinition(categoryName);
            }
        }
        private IList<ConnectedItem> PopulateConnectedItemNamesList()
        {
            IList<ConnectedItem> retval = new List<ConnectedItem>();
            if (string.IsNullOrEmpty(this.SelectedConnectedCategory)) { return retval; }
                        
            using (ICommenceDatabase db = new CommenceDatabase())
            {
                using (ICommenceCursor cur = db.GetCursor(this.SelectedConnectedCategory))
                {
                    string nameField = db.GetNameField(this.SelectedConnectedCategory);
                    var columns = this.CategoryDefinition.Clarified
                        ? new[] { nameField, this.CategoryDefinition.ClarifyField }
                        : new[] { nameField };

                    if (!cur.SetColumns(columns)) { return retval; } // something went wrong bad
                    var number = cur.RowCount;
                    if (number < 1000)
                    {
                        return GetConnectedItems(cur).ToList();
                    }
                    else if (string.IsNullOrEmpty(this.ConnectedItemSearchString))
                    {
                        retval.Add(new ConnectedItem("(Too many items to display)", null, null, null));
                        return retval;
                    }
                    else
                    {
                        CursorFilterTypeF f = cur.Filters.Add(1, FilterType.Field);
                        f.FieldValue = this.ConnectedItemSearchString;
                        f.FieldName = nameField;
                        f.Qualifier = FilterQualifier.Contains;
                        int count = cur.Filters.Apply();
                        if (count > 1000)
                        {
                            retval.Add(new ConnectedItem("(Too many items to display)", null, null, null));
                            return retval;
                        }
                        else if (count == 0)
                        {
                            retval.Add(new ConnectedItem($"(No items match '{ this.ConnectedItemSearchString }')", null, null, null));
                            return retval;
                        }
                        else
                        {
                            return GetConnectedItems(cur).ToList();
                        }
                    }
                }
            }
        }

        private IEnumerable<ConnectedItem> GetConnectedItems(ICommenceCursor cur)
        {
            if (this.CategoryDefinition.Clarified)
            {
                foreach (List<string> row in cur.ReadAllRows())
                {
                    yield return new ConnectedItem()
                    {
                        DisplayName = row[0] + " " + this.CategoryDefinition.ClarifySeparator + " " + row[1],
                        ItemName = row[0],
                        ClarifyValue = row[1],
                        ClarifySeparator = this.CategoryDefinition.ClarifySeparator
                    };
                }
            }
            else
            {
                foreach (List<string> row in cur.ReadAllRows())
                {
                    yield return new ConnectedItem()
                    {
                        DisplayName = row[0],
                        ItemName = row[0],
                    };
                }
            }
        }

        // ideally we should do this async
        private Dictionary<FilterQualifier, QualifierMember> CreateQualifierDictionary()
        {
            Dictionary<FilterQualifier, QualifierMember> retval = new Dictionary<FilterQualifier, QualifierMember>();
            foreach (FilterQualifier value in Enum.GetValues(typeof(FilterQualifier)))
            {
                var da = value.GetAttribute<DescriptionAttribute>();
                var fa = value.GetAttribute<FilterValuesAttribute>();
                retval.Add(value, new QualifierMember()
                {
                    Description = da == null ? value.ToString() : da.Description,               
                    FieldValues = fa == null ? default(int) : fa.Number,
                    Value = value
                });
            }
            return retval;
        }

        private void InitializeFilter(FilterType filterType)
        {
            // explicitly kill any previous instance,
            // because it will have an event subscription and may not get garbage collected
            // not sure if this is needed.
            _filterConstructor = null; 
            _filterConstructor = new FilterConstructor(this, filterType);
        }

        internal void ResetFilter()
        {
            this.SelectedConnectedCategory = null;
            this.SelectedConnectionFieldListItem = null;
            this.SelectedFieldListItem = null;
            _filterConstructor = null;
            this.FieldValue = null;
            this.BetweenStart = null;
            this.BetweenEnd = null;
            this.CurrentFilter = null;
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Event raisers
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
