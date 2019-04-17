using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace SCide.WPF.Configuration
{
    
    public class SettingsOption : INotifyPropertyChanged
    {
        #region Fields
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        public SettingsOption(EditorSettings settings)
        {
            settings.PropertyChanged += EditorSettings_PropertyChanged;
        }
        #endregion

        #region Event handlers
        private void EditorSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // try to update matching property
            try
            {
                if (this[e.PropertyName] != null)
                {
                    this[e.PropertyName] = EditorSettings.Instance.GetType().GetProperty(e.PropertyName).GetValue(EditorSettings.Instance, null);
                }
            }
            catch(NullReferenceException) { }
        }
        #endregion

        #region Properties
        public string Name { get; set; }

        private Color foregroundColor;
        public Color ForeColor
        {
            get { return foregroundColor; }
            set
            {
                foregroundColor = value;
                OnPropertyChanged();
            }
        }

        private Color backgroundColor;
        public Color BackColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                OnPropertyChanged();
            }
        }

        private FontFamily fontFamily;
        public FontFamily FontFamily
        {
            get { return fontFamily; }
            set
            {
                fontFamily = value;
                OnPropertyChanged();
            }
        }

        private double fontSize;
        public double FontSize
        {
            get { return fontSize; }
            set
            {
                fontSize = value;
                OnPropertyChanged();
            }
        }

        private FontStyle fontStyle;
        public FontStyle FontStyle
        {
            get { return fontStyle; }
            set
            {
                fontStyle = value;
                OnPropertyChanged();
            }
        }

        private FontWeight fontWeight;
        public FontWeight FontWeight
        {
            get { return fontWeight; }
            set
            {
                fontWeight = value;
                OnPropertyChanged();
            }
        }

        private string toolTipText;
        public string ToolTipText
        {   
            get { return toolTipText; }
            set
            {
                toolTipText = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Event raisers
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Indexer
        public object this[string propertyName]
        {
            get
            {
                PropertyInfo property = GetType().GetProperty(propertyName);
                return property?.GetValue(this, null);
            }
            set
            {
                PropertyInfo property = GetType().GetProperty(propertyName);
                property?.SetValue(this, value, null);
            }
        }
        #endregion
    }
}
