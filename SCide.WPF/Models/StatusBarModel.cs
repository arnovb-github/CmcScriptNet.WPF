using SCide.WPF.Commence;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SCide.WPF.Models
{
    internal class StatusBarModel : INotifyPropertyChanged
    {
        
        #region Fields
        private string _statusText = string.Empty;
        private ICommenceScript _commenceScript;
        private ScintillaNET.Scintilla _scintilla;
        #endregion

        #region Properties

        internal ScintillaNET.Scintilla Scintilla
        {
            set
            {
                if (_scintilla != null)
                {
                    UnsubscribeEvents(_scintilla);
                }
                _scintilla = value;
                SubscribeEvents(_scintilla);
            }
        }

        internal ICommenceScript CurrentScript
        {
            get
            {
                return _commenceScript;
            }
            set
            {
                _commenceScript = value;
                OnPropertyChanged();
            }
        }

        internal int CurrentLine { get; private set; }

        internal int CurrentColumn { get; private set; }

        internal string StatusText
        {
            get
            {
                return _statusText;
            }
            set
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }

        internal string PositionInfo => "Ln: " + CurrentLine.ToString()
            + " Col: " + CurrentColumn.ToString();

        private bool overwriteMode;
        internal bool OverWrite
        {
            get { return overwriteMode; }
            set
            {
                overwriteMode = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Events

        private void SubscribeEvents(ScintillaNET.Scintilla scintilla)
        {
            _scintilla.UpdateUI += Scintilla_UpdateUI;
            _scintilla.MouseDown += Scintilla_MouseDown;
        }

        private void UnsubscribeEvents(ScintillaNET.Scintilla scintilla)
        {
            _scintilla.UpdateUI -= Scintilla_UpdateUI;
            _scintilla.MouseDown -= Scintilla_MouseDown;
        }

        private void Scintilla_UpdateUI(object sender, System.EventArgs e)
        {
            UpdatePositionInfo(sender as ScintillaNET.Scintilla);
        }

        private void UpdatePositionInfo(ScintillaNET.Scintilla scintilla)
        {
            CurrentColumn = scintilla.GetColumn(scintilla.CurrentPosition) + 1;
            CurrentLine = scintilla.CurrentLine + 1;
            OnPropertyChanged(nameof(PositionInfo));
        }

        private void Scintilla_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            UpdatePositionInfo(sender as ScintillaNET.Scintilla);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
