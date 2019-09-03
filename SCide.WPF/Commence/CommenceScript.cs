using SCide.WPF.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Vovin.CmcLibNet.Database;
using Vovin.CmcLibNet.Database.Metadata;

namespace SCide.WPF.Commence
{
    /// <summary>
    /// Provide metadata of a Commence Item Detail Form script
    /// </summary>
    public class CommenceScript : INotifyPropertyChanged, ICommenceScript
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string CategoryName { get; internal set; }
        public string FormName { get; internal set; }
        public string DatabaseName { get; internal set; }
        public string DatabasePath { get; internal set; }
        public string FilePath { get; internal set; }

        private IList<CommenceField> list;
        public IList<CommenceField> Fields
        {
            get { return list; }
            private set
            {
                list = value;
                OnPropertyChanged();
            }
        }

        private CommenceField selectedField;
        public CommenceField SelectedField
        {
            get { return selectedField; }
            private set
            {
                selectedField = value;
                OnPropertyChanged();
            }
        }

        private IList<CommenceConnection> connections;
        public IList<CommenceConnection> Connections
        {
            get { return connections; }
            private set
            {
                connections = value;
                OnPropertyChanged();
            }
        }

        private CommenceConnection selectedConnection;
        public CommenceConnection SelectedConnection
        {
            get { return selectedConnection; }
            private  set
            {
                selectedConnection = value;
                OnPropertyChanged();
            }
        }

        private IList<IDFControl> controls;
        public IList<IDFControl> Controls
        {
            get { return controls; }
            private set
            {
                controls = value;
                OnPropertyChanged();
            }
        }

        private IDFControl selectedControl;
        public IDFControl SelectedControl
        {
            get { return selectedControl; }
            private set
            {
                selectedControl = value;
                OnPropertyChanged();
            }
        }

        public string FullName => this.CategoryName + " - " + this.FormName;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task GetMetaDataAsync()
        {
            if (string.IsNullOrEmpty(this.CategoryName) || string.IsNullOrEmpty(this.FormName)) { return; }
            using (ICommenceDatabase db = new CommenceDatabase())
            {
                var connectionList = db.GetConnectionNames(this.CategoryName);
                if (connectionList != null)
                {
                    Connections = GetConnections(connectionList).ToList();
                }
                var fields = db.GetFieldNames(this.CategoryName);
                Fields = GetFields(fields)?.ToList();
            }
            Controls = await Task.Run(() => GetControlList(this.CategoryName, this.FormName));
        }

        private IEnumerable<CommenceConnection> GetConnections(IEnumerable<ICommenceConnection> list)
        {
            foreach (ICommenceConnection c in list)
            {
                yield return new CommenceConnection(c.Name, c.ToCategory);
            }
        }

        private IEnumerable<CommenceField> GetFields(IEnumerable<string> fields)
        {
            foreach (string f in fields)
            {
                yield return new CommenceField(f, this.CategoryName);
            }
        }

        private IList<IDFControl> GetControlList(string categoryName, string formName)
        {
            IList<IDFControl> retval = new List<IDFControl>();
            var formfile = CommenceModel.FormFiles.FirstOrDefault(s => s.Name.Equals(formName));
            if (formfile == null) { return null; }
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(formfile.FileName);
            }
            catch
            {
                return retval; // just die
            }
            /* Expected format:
             * <form>
            *   <controls>
            *      <control>
            *          <name>
            *          </name>
            *      </control
            *   </controls>
            * </form>
            * 
            * so XPath = /form/controls/control/name
            */
            string xpath = "/FORM/CONTROLS/CONTROL"; // find all controls, should be ALL CAPS
            XmlNodeList controlNodes = doc.SelectNodes(xpath);
            // create a serializer
            // root attribute needed for XmlNodeReader to work
            XmlSerializer serializer = new XmlSerializer(typeof(IDFControl), new XmlRootAttribute("CONTROL"));
            if (controlNodes == null) { return retval; }
            // process nodes 
            foreach (XmlNode ControlElement in controlNodes)
            {
                // serialize nodes to object
                IDFControl c = (IDFControl)serializer.Deserialize(new XmlNodeReader(ControlElement));
                if (c != null && !string.IsNullOrEmpty(c.NAME))
                {
                    // slap on form file for good measure
                    c.FormFile = System.IO.Path.GetFileName(formfile.FileName);
                    retval.Add(c);
                }
            }
            retval = retval.OrderBy(o => o.NAME).ToList();
            return retval;
        }
    }
}