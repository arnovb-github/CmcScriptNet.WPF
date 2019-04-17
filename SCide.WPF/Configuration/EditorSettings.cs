using SCide.WPF.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using SC = System.Configuration;

namespace SCide.WPF.Configuration
{
    public sealed class EditorSettings : ConfigurationSection, INotifyPropertyChanged
    {
        static SC.Configuration configuration =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public event PropertyChangedEventHandler PropertyChanged;

        #region Singleton pattern
        private static readonly EditorSettings _instance = new EditorSettings();

        private EditorSettings() { }

        public static EditorSettings Instance
        {
            get
            {
                // this may fail if the config file differs
                // from the configuration settings
                // the application then simply fails to load,
                // which is hard to troubleshoot
                try
                {
                    // make sure there is a section
                    if (configuration.GetSection(nameof(EditorSettings)) == null)
                    {
                        configuration.Sections.Add(nameof(EditorSettings), _instance);
                    }
                    return (EditorSettings)configuration.GetSection(nameof(EditorSettings));
                }
                catch (ConfigurationErrorsException)
                {
                    MessageBox.Show(string.Format("The configuration file '{0}' does not match the expected configuration.\n\n" +
                        "Delete it and restart the application. " +
                        "The application will revert to default settings.", configuration.FilePath),
                        "Configuration error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                    return null;
                }
            }
        }

        public void Save()
        {
            configuration.Save(ConfigurationSaveMode.Full);
        }
        #endregion

        #region Application settings
        [UserScopedSetting]
        [ConfigurationProperty(nameof(Statusbar), DefaultValue = true)]
        public bool Statusbar
        {
            get { return (bool)base[nameof(Statusbar)]; }
            set { base[nameof(Statusbar)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting]
        [ConfigurationProperty(nameof(LineNumbers), DefaultValue = true)]
        public bool LineNumbers
        {
            get { return (bool)base[nameof(LineNumbers)]; }
            set { base[nameof(LineNumbers)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting]
        [ConfigurationProperty(nameof(Whitespace), DefaultValue = false)]
        public bool Whitespace
        {
            get { return (bool)base[nameof(Whitespace)]; }
            set { base[nameof(Whitespace)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting]
        [ConfigurationProperty(nameof(Eol), DefaultValue = false)]
        public bool Eol
        {
            get { return (bool)base[nameof(Eol)]; }
            set { base[nameof(Eol)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting]
        [ConfigurationProperty(nameof(WordWrap), DefaultValue = false)]
        public bool WordWrap
        {
            get { return (bool)base[nameof(WordWrap)]; }
            set { base[nameof(WordWrap)] = value; OnPropertyChanged(); }
        }
        #endregion

        #region Scintilla editor settings
        [UserScopedSetting()]
        [ConfigurationProperty(nameof(FontFamily), DefaultValue = "Consolas")]
        [EditorScope(true)]
        public System.Windows.Media.FontFamily FontFamily
        {
            get { return (System.Windows.Media.FontFamily)base[nameof(FontFamily)]; }
            set { base[nameof(FontFamily)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(FontSize), DefaultValue = 12.0)]
        [EditorScope(true)]
        public double FontSize
        {
            get { return (double)base[nameof(FontSize)]; }
            set { base[nameof(FontSize)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting]
        [ConfigurationProperty(nameof(FontStyle), DefaultValue = "Normal")]
        [EditorScope(true)]
        public FontStyle FontStyle
        {
            get { return (FontStyle)base[nameof(FontStyle)]; }
            set { base[nameof(FontStyle)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting]
        [ConfigurationProperty(nameof(FontWeight), DefaultValue = "Normal")]
        [EditorScope(true)]
        public FontWeight FontWeight
        {
            get { return (  FontWeight)base[nameof(FontWeight)]; }
            set { base[nameof(FontWeight)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(BackColor), DefaultValue = "GhostWhite")]
        [EditorScope(true)]
        public System.Windows.Media.Color BackColor
        {
            get { return (System.Windows.Media.Color)base[nameof(BackColor)]; }
            set { base[nameof(BackColor)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(ForeColor), DefaultValue = "#202020")]
        [EditorScope(true)]
        public System.Windows.Media.Color ForeColor
        {
            get { return (System.Windows.Media.Color)base[nameof(ForeColor)]; }
            set { base[nameof(ForeColor)] = value; OnPropertyChanged(); }
        }

        // default means whitespace(?)
        [UserScopedSetting()]
        [ConfigurationProperty(nameof(Default), DefaultValue = "silver")]
        [EditorScope(true)]
        public System.Windows.Media.Color Default
        {
            get { return (System.Windows.Media.Color)base[nameof(Default)]; }
            set { base[nameof(Default)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(Asm), DefaultValue = "black")] // use string of the hex value
        public System.Windows.Media.Color Asm
        {
            get { return (System.Windows.Media.Color)base[nameof(Asm)]; }
            set { base[nameof(Asm)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(BinNumber), DefaultValue = "lightblue")] // use string of the hex value
        public System.Windows.Media.Color BinNumber
        {
            get { return (System.Windows.Media.Color)base[nameof(BinNumber)]; }
            set { base[nameof(BinNumber)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(Comment), DefaultValue = "#008000")] // use string of the hex value
        public System.Windows.Media.Color Comment
        {
            get { return (System.Windows.Media.Color)base[nameof(Comment)]; }
            set { base[nameof(Comment)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(CommentBlock), DefaultValue = "#008000")] // use string of the hex value
        public System.Windows.Media.Color CommentBlock
        {
            get { return (System.Windows.Media.Color)base[nameof(CommentBlock)]; }
            set { base[nameof(CommentBlock)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(Constant), DefaultValue = "orange")] // use string of the hex value
        public System.Windows.Media.Color Constant
        {
            get { return (System.Windows.Media.Color)base[nameof(Constant)]; }
            set { base[nameof(Constant)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(Date), DefaultValue = "#008000")] // use string of the hex value
        public System.Windows.Media.Color Date
        {
            get { return (System.Windows.Media.Color)base[nameof(Date)]; }
            set { base[nameof(Date)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(Error), DefaultValue = "#008000")] // use string of the hex value
        public System.Windows.Media.Color Error
        {
            get { return (System.Windows.Media.Color)base[nameof(Error)]; }
            set { base[nameof(Error)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(HexNumber), DefaultValue = "red")]
        public System.Windows.Media.Color HexNumber
        {
            get { return (System.Windows.Media.Color)base[nameof(HexNumber)]; }
            set { base[nameof(HexNumber)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(Identifier), DefaultValue = "black")]
        public System.Windows.Media.Color Identifier
        {
            get { return (System.Windows.Media.Color)base[nameof(Identifier)]; }
            set { base[nameof(Identifier)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(Keyword0), DefaultValue = "blue")]
        public System.Windows.Media.Color Keyword0
        {
            get { return (System.Windows.Media.Color)base[nameof(Keyword0)]; }
            set { base[nameof(Keyword0)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(Keyword1), DefaultValue = "orangered")]
        public System.Windows.Media.Color Keyword1
        {
            get { return (System.Windows.Media.Color)base[nameof(Keyword1)]; }
            set { base[nameof(Keyword1)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(Keyword2), DefaultValue = "purple")]
        public System.Windows.Media.Color Keyword2
        {
            get { return (System.Windows.Media.Color)base[nameof(Keyword2)]; }
            set { base[nameof(Keyword2)] = value; OnPropertyChanged(); }
        }
        [UserScopedSetting()]
        [ConfigurationProperty(nameof(Keyword3), DefaultValue = "pink")]
        public System.Windows.Media.Color Keyword3
        {
            get { return (System.Windows.Media.Color)base[nameof(Keyword3)]; }
            set { base[nameof(Keyword3)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(Label), DefaultValue = "#808000")]
        public System.Windows.Media.Color Label
        {
            get { return (System.Windows.Media.Color)base[nameof(Label)]; }
            set { base[nameof(Label)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(Number), DefaultValue = "red")]
        public System.Windows.Media.Color Number
        {
            get { return (System.Windows.Media.Color)base[nameof(Number)]; }
            set { base[nameof(Number)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(Operator), DefaultValue = "purple")]
        public System.Windows.Media.Color Operator
        {
            get { return (System.Windows.Media.Color)base[nameof(Operator)]; }
            set { base[nameof(Operator)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(PreProcessor), DefaultValue = "maroon")]
        public System.Windows.Media.Color PreProcessor
        {
            get { return (System.Windows.Media.Color)base[nameof(PreProcessor)]; }
            set { base[nameof(PreProcessor)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(String), DefaultValue = "#a31515")]
        public System.Windows.Media.Color String
        {
            get { return (System.Windows.Media.Color)base[nameof(String)]; }
            set { base[nameof(String)] = value; OnPropertyChanged(); }
        }

        [UserScopedSetting()]
        [ConfigurationProperty(nameof(StringEol), DefaultValue = "pink")]
        public System.Windows.Media.Color StringEol
        {
            get { return (System.Windows.Media.Color)base[nameof(StringEol)]; }
            set { base[nameof(StringEol)] = value; OnPropertyChanged(); }
        }
        #endregion

        #region Event raisers
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private List<SettingsOption> GetSettingsOptions<T>()
        {
            List<SettingsOption> list = new List<SettingsOption>();

            foreach (var p in this.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(T)))
            {
                SettingsOption so = new SettingsOption(Instance)
                {
                    Name = p.Name,
                    ForeColor = (System.Windows.Media.Color)this[p.Name],
                    BackColor = BackColor,
                    FontFamily = FontFamily,
                    FontSize = FontSize,
                    FontStyle = FontStyle,
                    FontWeight = FontWeight
                };
                // set tooltip
                var attr = (EditorScopeAttribute[])p.GetCustomAttributes(typeof(EditorScopeAttribute), true);
                if (attr.Length >0)
                {
                    so.ToolTipText = attr[0].Global ? "Applies to all options" : string.Empty;
                }
                list.Add(so);
            }
            return list;
        }

        // transform properties into a list of objects
        public List<SettingsOption> ColorOptions =>
            GetSettingsOptions<System.Windows.Media.Color>();
    }
}
