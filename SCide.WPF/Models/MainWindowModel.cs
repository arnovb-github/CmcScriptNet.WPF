using SCide.WPF.Commence;
using SCide.WPF.Configuration;
using SCide.WPF.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// test iets

namespace SCide.WPF.Models
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowModel()
        {
            CommenceModel = new CommenceModel(new CommenceMonitor());
            StatusBarModel = new StatusBarModel();
            EditorSettings = EditorSettings.Instance;
        }

        public CommenceModel CommenceModel { get; }
        public StatusBarModel StatusBarModel { get; }


        // TODO move this to CommenceScript
        private List<IdentifierMatch> list;
        public List<IdentifierMatch> IdentifierMatches
        {
            get { return list; }
            set
            {
                list = value;
                OnPropertyChanged();
            }
        }

        // TODO move this to CommenceModel? Or even CommenceScript?
        private IdentifierMatch selectedIdentifierMatch;
        public IdentifierMatch SelectedIdentifierMatch
        {
            get { return selectedIdentifierMatch; }
            set
            {
                selectedIdentifierMatch = value;
                OnPropertyChanged();
            }
        }

        private EditorSettings editorSettings;
        public EditorSettings EditorSettings
        {
            get
            {
                return editorSettings;
            }
            set
            {
                editorSettings = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
