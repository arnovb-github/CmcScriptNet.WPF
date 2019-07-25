using SCide.WPF.Commence;
using SCide.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using Vovin.CmcLibNet.Database;

namespace SCide.WPF.Models
{
    public class CommenceModel : ICommenceModel
    {
        #region Event declarations
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Fields
        private const string TEMPLATES_FOLDER = "tmplts"; // subfolder holding detail forms, report views etc.
        private static List<IDFFile> _formfiles;
        private readonly ICommenceMonitor _monitor;
        private IList<string> _tempFiles = new List<string>();
        private IList<string> _categories;
        private IList<string> _forms;
        private string _name = "Commence is not running";
        private string _path;

        #endregion

        #region Constructors
        public CommenceModel()
        {
            _monitor = new CommenceMonitor();
            _monitor.CommenceProcessExited += Monitor_CommenceProcessExited;
            _monitor.CommenceProcessStarted += Monitor_CommenceProcessStarted;
            if (_monitor.CommenceIsRunning)
            {
                InitializeModel();
            }
        }

        #endregion

        #region Event handlers
        private void Monitor_CommenceProcessStarted(object sender, EventArgs e)
        {
            InitializeModel();
        }

        private void Monitor_CommenceProcessExited(object sender, EventArgs e)
        {
            Name = "Commence is not running";
            Path = string.Empty;
            Categories = null;
            IsRunning = false;
        }
        #endregion

        #region Properties
        internal static List<IDFFile> FormFiles
            {
            get
            {
                return _formfiles;
            }
            private set
            {
                _formfiles = value;
            }
        }

        public IList<string> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        private string _selectedCategory;

        public string SelectedCategory
        {
            get => this._selectedCategory;
            set
            {
                this._selectedCategory = value;
                OnPropertyChanged();
                this.Forms = null; // clear forms
                this.SelectedForm = null; // clear selected form
                GetFormNames(this.SelectedCategory);
                if (Forms?.Count == 1)
                {
                    this.SelectedForm = Forms.First();
                }
            }
        }

        private string _selectedForm;

        public string SelectedForm
        {
            get { return _selectedForm; }
            set
            {
                _selectedForm = value;
                OnPropertyChanged();
            }
        }

        private CommenceScript _currentScript;
        public CommenceScript CurrentScript
        {
            get => _currentScript;
            set
            {
                _currentScript = value;
                OnPropertyChanged();
            }
        }

        public IList<string> Forms
        {
            get
            {
                return _forms;
            }
            set
            {
                _forms = value;
                OnPropertyChanged();
            }
        }

        public IList<string> Fields { get; set; }

        public IList<string> Connections { get; set; }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }

        private bool _isRunning;

        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Event raisers
        // [CallerMemberName] eliminates the need to supply the propertyName
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Methods

        public async void InitializeModel() // TODO async void should be avoided
        {
            using (ICommenceDatabase db = new CommenceDatabase())
            {
                Name = db.Name;
                Path = db.Path;
                Categories = db.GetCategoryNames();
                IsRunning = true;
            }
            FormFiles = await GetDetailFormFilesAsync();
        }

        public bool CheckInFormScript(CommenceScript cs)
        {
            using (ICommenceDatabase db = new CommenceDatabase())
            {
                if (!this.CanSave(cs.DatabasePath))
                {
                    db.Close();
                    throw new Vovin.CmcLibNet.CommenceDDEException("Unable to check in script in Commence");
                }
                return db.CheckInFormScript(cs.CategoryName, cs.FormName, cs.FilePath);
            }
        }

        public bool CanSave(string path)
        {
            if (!_monitor.CommenceIsRunning) { return false; }
            return path.Equals(this.Path);
        }

        public void GetFormNames(string categoryName)
        {
            if (!_monitor.CommenceIsRunning)
            {
                Forms = null;
                return;
            }
            using (ICommenceDatabase db = new CommenceDatabase())
            {
                if (string.IsNullOrEmpty(categoryName)) { return; }
                Forms = db.GetFormNames(categoryName);
            }
        }

        public string CheckOutFormScript(string categoryName, string formName)
        {
            if (!_monitor.CommenceIsRunning) { return string.Empty; }
            string path = System.IO.Path.GetTempFileName();
            TempFileTracker.Add(path);
            using (ICommenceDatabase db = new CommenceDatabase())
            {
                if (!db.CheckOutFormScript(categoryName, formName, path)) // tell commence to save form to textfile
                {
                    path = string.Empty;
                }
            }
            return path;
        }

        private async Task<List<IDFFile>> GetDetailFormFilesAsync()
        {
            List<IDFFile> retval = new List<IDFFile>();
            string[] parts = {this.Path, TEMPLATES_FOLDER };
            string path = System.IO.Path.Combine(parts);
            string[] files = Directory.GetFiles(path, "fm*.xml"); // returns empty array if no files
            foreach (string f in files)
            {
                using (XmlReader reader = XmlReader.Create(f, new XmlReaderSettings() { Async = true }))
                {
                    try
                    {
                        while (await reader.ReadAsync())
                        {
                            // Only detect start elements.
                            if (reader.IsStartElement())
                            {
                                // Get element name and switch on it.
                                switch (reader.Name.ToLower())
                                {
                                    case "form":
                                        IDFFile idf = new IDFFile();
                                        // we want to read attributes
                                        idf.Name = reader.GetAttribute("Name");
                                        idf.Category = reader.GetAttribute("CategoryName");
                                        idf.FileName = f;
                                        retval.Add(idf);
                                        break;
                                } //switch
                            } //if
                        } // while
                    } // try
                    catch { }
                } // using
            } // foreach
            return retval;
        }

        public void Focus()
        {
            _monitor.Focus(Name);
        }
        #endregion
    }
}
