using SCide.WPF.Configuration;
using SCide.WPF.Extensions;
using SCide.WPF.Models;
using ScintillaNET;
using ScintillaNET.WPF;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SCide.WPF
{
    /// <summary>
    /// Interaction logic for CustomizeWindow.xaml
    /// </summary>
    /// <remarks>Some of this code may seem convoluted, because of the use of (and conversion to/from)
    /// System.Drawing.* and System.Windows.*. This is because the editor is a WinForms control.
    /// We could have included WPF-native color- and font-pickers like Xceed.Wpf.Toolkit,
    /// but that would significantly increase the size of the executable.
    /// Since we need WinForms anyway, I decided to favour small code over elegance.
    /// </remarks>
    public partial class CustomizeWindow : Window
    {
        public event EventHandler<EventArgs> UserPreferencesChanged;

        #region Fields
        private readonly object _dataContext;
        public static RoutedCommand ChangeForegroundColorCommand = new RoutedCommand();
        public static RoutedCommand ChangeBackgroundColorCommand = new RoutedCommand();
        public static RoutedCommand ChangeFontCommand = new RoutedCommand();
        public static RoutedCommand ApplySettingsCommand = new RoutedCommand();
        public static RoutedCommand SaveSettingsCommand = new RoutedCommand();

        MainWindowModel viewModel;
        #endregion
        public CustomizeWindow(object dataContext)
        {
            InitializeComponent();
            _dataContext = dataContext;
            this.DataContext = _dataContext;
            viewModel = DataContext as MainWindowModel;
            InitCodeHighlighting();
            viewModel.EditorSettings.PropertyChanged += EditorSettings_PropertyChanged;
            PopulateListBox(propertiesList, viewModel.EditorSettings);
        }

        private void EditorSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                InitCodeHighlighting();
            }
            catch { }
        }

        private void SaveSettingsCommand_Executed(object sender, RoutedEventArgs e)
        {
            var dc = (MainWindowModel)_dataContext;
            dc.EditorSettings.Save();
            this.Close();
        }

        private void ApplySettingsCommand_Executed(object sender, RoutedEventArgs e)
        {
            // we need to somehow re-initialize our scintilla(s) now
            UserPreferencesChanged?.Invoke(this, new EventArgs());
        }

        private void PopulateListBox(ListBox listBox, EditorSettings settings)
        {
            listBox.ItemsSource = settings.ColorOptions;
        }

        private void ChangeForegroundColorCommand_Executed(object sender, RoutedEventArgs e)
        {
            var option = propertiesList.SelectedItem as SettingsOption;
            if (option == null) { return; }
            Color newColor = GetColorFromDialog(option.ForeColor.ToDrawingColor()).ToMediaColor();
            option.ForeColor = newColor;
            viewModel.EditorSettings.GetType().GetProperty(option.Name).SetValue(viewModel.EditorSettings, newColor);
        }

        private void ChangeBackgroundColorCommand_Executed(object sender, RoutedEventArgs e)
        {
            Color currentColor = viewModel.EditorSettings.BackColor;
            viewModel.EditorSettings.BackColor = GetColorFromDialog(currentColor.ToDrawingColor()).ToMediaColor();
        }

        private void ChangeFontCommand_Executed(object sender, RoutedEventArgs e)
        {
            FontInfo fi = new FontInfo();
            fi.FamilyName = viewModel.EditorSettings.FontFamily.ToString();
            fi.FontFamily = viewModel.EditorSettings.FontFamily;
            fi.SizePx = viewModel.EditorSettings.FontSize;
            fi.SizePt = (float)((72.0 / 96.0) * fi.SizePx);
            fi.Style = viewModel.EditorSettings.FontStyle;
            fi.Weight = viewModel.EditorSettings.FontWeight;
            var result = GetFontInfoFromDialog(fi);
            viewModel.EditorSettings.FontFamily = result.FontFamily;
            viewModel.EditorSettings.FontSize = result.SizePx;
            viewModel.EditorSettings.FontStyle = result.Style;
            viewModel.EditorSettings.FontWeight = result.Weight;
        }

        private FontInfo GetFontInfoFromDialog(FontInfo fontInfo)
        {
            FontInfo fi = fontInfo;
            System.Drawing.FontStyle style = new System.Drawing.FontStyle();
            style = fi.Bold | fi.Italic;
            System.Drawing.Font f = new System.Drawing.Font(fi.FamilyName, fi.SizePt, style);
            System.Windows.Forms.FontDialog fd = new System.Windows.Forms.FontDialog();
            fd.Font = f;
            System.Windows.Forms.DialogResult dr = fd.ShowDialog();
            if (dr != System.Windows.Forms.DialogResult.Cancel)
            {
                fi.FamilyName = fd.Font.Name;
                fi.SizePt = fd.Font.Size;
                fi.SizePx = (int)(fd.Font.Size * 96.0 / 72.0);
                fi.Style = fd.Font.Italic ? FontStyles.Italic : FontStyles.Normal;
                fi.FontFamily = new FontFamily(fd.Font.Name);
                fi.Weight = fd.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
            }
            return fi;
        }
        private System.Drawing.Color GetColorFromDialog(System.Drawing.Color currentColor)
        {
            System.Windows.Forms.ColorDialog colDiag = new System.Windows.Forms.ColorDialog();
            colDiag.Color = currentColor;
            if (colDiag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return colDiag.Color;
            }
            return currentColor;
        }

        private void InitCodeHighlighting()
        {
            ScintillaWPF ScintillaNet = scintilla;
            ScintillaNet.StyleResetDefault();
            ScintillaNet.Styles[ScintillaNET.Style.Default].Font = viewModel.EditorSettings.FontFamily.ToString();
            ScintillaNet.Styles[ScintillaNET.Style.Default].Size = (int)(viewModel.EditorSettings.FontSize * (72.0 / 96.0));
            ScintillaNet.Styles[ScintillaNET.Style.Default].Italic = viewModel.EditorSettings.FontStyle == FontStyles.Italic;
            ScintillaNet.Styles[ScintillaNET.Style.Default].Weight = viewModel.EditorSettings.FontWeight.ToOpenTypeWeight();
            ScintillaNet.Styles[ScintillaNET.Style.Default].BackColor = viewModel.EditorSettings.BackColor.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.Default].ForeColor = viewModel.EditorSettings.ForeColor.ToDrawingColor();
            ScintillaNet.StyleClearAll(); // i.e. Apply to all

            ScintillaNet.Styles[ScintillaNET.Style.VbScript.Asm].ForeColor = viewModel.EditorSettings.Asm.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.BinNumber].ForeColor = viewModel.EditorSettings.BinNumber.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.Comment].ForeColor = viewModel.EditorSettings.Comment.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.CommentBlock].ForeColor = viewModel.EditorSettings.CommentBlock.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.Constant].ForeColor = viewModel.EditorSettings.Constant.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.Default].ForeColor = viewModel.EditorSettings.Default.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.Error].ForeColor = viewModel.EditorSettings.Error.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.HexNumber].ForeColor = viewModel.EditorSettings.HexNumber.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.Identifier].ForeColor = viewModel.EditorSettings.Identifier.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.Keyword].ForeColor = viewModel.EditorSettings.Keyword0.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.Keyword2].ForeColor = viewModel.EditorSettings.Keyword1.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.Keyword3].ForeColor = viewModel.EditorSettings.Keyword2.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.Keyword4].ForeColor = viewModel.EditorSettings.Keyword3.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.Label].ForeColor = viewModel.EditorSettings.Label.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.Number].ForeColor = viewModel.EditorSettings.Number.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.Operator].ForeColor = viewModel.EditorSettings.Operator.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.Preprocessor].ForeColor = viewModel.EditorSettings.PreProcessor.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.String].ForeColor = viewModel.EditorSettings.String.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.VbScript.StringEol].BackColor = viewModel.EditorSettings.StringEol.ToDrawingColor();
            //// Set the lexer
            ScintillaNet.Lexer = Lexer.VbScript;
            ScintillaNet.SetKeywords(0, "addhandler addressof andalso alias and ansi as assembly attribute auto begin boolean byref byte byval call case catch cbool cbyte cchar cdate cdec cdbl char cint class clng cobj compare const continue cshort csng cstr ctype currency date decimal declare default delegate dim do double each else elseif end enum erase error event exit explicit false finally for friend function get gettype global gosub goto handles if implement implements imports in inherits integer interface is let lib like load long loop lset me mid mod module mustinherit mustoverride mybase myclass namespace new next not nothing notinheritable notoverridable object on option optional or orelse overloads overridable overrides paramarray preserve private property protected public raiseevent readonly redim rem removehandler rset resume return select set shadows shared short single static step stop string structure sub synclock then throw to true try type typeof unload unicode until variant wend when while with withevents writeonly xor");
            ScintillaNet.SetKeywords(1, "form_onload form_onsave form_onentertab form_onleavetab form_onenterfield form_onleavefield form_oncancel form_onactivexcontrolevent");
        }

        // quick and dirty solution to pass font info around between System.Windows.* and System.Drawing.*
        private class FontInfo
        {
            public string FamilyName { get; set; }
            public double SizePx { get; set; }
            public float SizePt { get; set; }
            public FontStyle Style { get; set; } = FontStyles.Normal;
            public FontWeight Weight { get; set; } = FontWeights.Normal;
            public FontFamily FontFamily { get; set; }
            public System.Drawing.FontStyle Bold =>
                Weight == FontWeights.Bold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular;
            public System.Drawing.FontStyle Italic =>
                Style == FontStyles.Italic ? System.Drawing.FontStyle.Italic : System.Drawing.FontStyle.Regular;
        }
    }
}