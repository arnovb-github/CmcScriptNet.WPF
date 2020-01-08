using CmcScriptNet.FilterBuilder;
using SCide.WPF.Commands;
using SCide.WPF.Commence;
using SCide.WPF.Extensions;
using SCide.WPF.Helpers;
using ScintillaNET;
using ScintillaNET.WPF;
using ScintillaNET_FindReplaceDialog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SCide.WPF
{
    public partial class MainWindow : Window
    {
        #region Fields
        private delegate bool BgwStateChecker(); // AVB

        private const string NEW_DOCUMENT_TEXT = "Untitled";
        private const int LINE_NUMBERS_MARGIN_WIDTH = 25; // TODO - don't hardcode this

        /// <summary>
        /// the background color of the text area
        /// </summary>
        //private const int BACK_COLOR = 0x2A211C;
        private const int BACK_COLOR = 0xE4E4E4;

        /// <summary>
        /// default text color of the text area
        /// </summary>
        //private const int FORE_COLOR = 0xB7B7B7;
        private const int FORE_COLOR = 0x777777;

        /// <summary>
        /// change this to whatever margin you want the line numbers to show in
        /// </summary>
        private const int NUMBER_MARGIN = 1;

        /// <summary>
        /// change this to whatever margin you want the bookmarks/breakpoints to show in
        /// </summary>
        private const int BOOKMARK_MARGIN = 2;

        private const int BOOKMARK_MARKER = 2;

        /// <summary>
        /// change this to whatever margin you want the code folding tree (+/-) to show in
        /// </summary>
        private const int FOLDING_MARGIN = 3;

        /// <summary>
        /// set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
        /// </summary>
        private const bool CODEFOLDING_CIRCULAR = false;

        private int _newDocumentCount;
        private int _zoomLevel;

        //private const string ProductName = "CmcScriptNet.WPF";

        private readonly FindReplace MyFindReplace;
        private readonly BackgroundWorker bgw;

        #endregion Fields

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            MyFindReplace = new FindReplace();
            // Tie in FindReplace event
            MyFindReplace.KeyPressed += MyFindReplace_KeyPressed;

            bgw = new BackgroundWorker()
            {
                WorkerSupportsCancellation = true
            };
            bgw.RunWorkerCompleted += Bgw_RunWorkerCompleted;
            bgw.DoWork += Bgw_DoWork;
        }

        #endregion Constructors

        private void ShowOptionsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var cw = new CustomizeWindow(this.DataContext);
            cw.UserPreferencesChanged += CustomizeWindow_UserPreferencesChanged;
            cw.ShowDialog();
        }

        private void CustomizeWindow_UserPreferencesChanged(object sender, EventArgs e)
        {
            foreach (DocumentForm doc in Documents)
            {
                InitSyntaxColoring(doc.Scintilla);
            }
        }
      
        private void UpdateAllScintillaZoom()
        {
            // Update zoom level for all files
            // TODO - DocumentsSource is null. This is probably supposed to zoom all windows, not just the document style windows.
            //foreach (DocumentForm doc in dockPanel.DocumentsSource)
            //    doc.Scintilla.Zoom = _zoomLevel;

            // TODO - Ideally remove this once the zoom for all windows is working.
            foreach (DocumentForm doc in documentsRoot.Children)
                doc.Scintilla.Zoom = _zoomLevel;
        }


        #region Properties

        public DocumentForm ActiveDocument
        {
            get { return documentsRoot.Children.FirstOrDefault(c => c.Content == dockPanel.ActiveContent) as DocumentForm; }
        }

        public IEnumerable<DocumentForm> Documents
        {
            get { return documentsRoot.Children.Cast<DocumentForm>(); }
        }

        #endregion Properties

        #region Methods

        private void DockPanel_ActiveContentChanged(object sender, EventArgs e)
        {
            // Update the main form _text to show the current document
            if (ActiveDocument != null)
            {
                this.Title = string.Format(CultureInfo.CurrentCulture, "{0} - {1}", ActiveDocument.Title, Program.Title);
                MyFindReplace.Scintilla = ActiveDocument.Scintilla.Scintilla;
                ActiveDocument.FindReplace = MyFindReplace;
                // AVB
                viewModel.CommenceModel.SelectedScript = ActiveDocument.CommenceScript;
                viewModel.CommenceModel.SelectedCategory = ActiveDocument.CommenceScript?.CategoryName;
                viewModel.CommenceModel.SelectedForm = ActiveDocument.CommenceScript?.FormName;
                if (ActiveDocument.CommenceScript == null) { viewModel.CommenceModel.Items = null; }
                RestartParsing();
                viewModel.StatusBarModel.Scintilla = ActiveDocument.Scintilla.Scintilla;
                viewModel.StatusBarModel.OverWrite = false;
                //viewModel.CanEdit = true;
            }
            else
            {
                this.Title = Program.Title;
            }
        }

        private void InitBookmarkMargin(ScintillaWPF ScintillaNet)
        {
            //TextArea.SetFoldMarginColor(true, IntToColor(BACK_COLOR));

            var margin = ScintillaNet.Margins[BOOKMARK_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = (1 << BOOKMARK_MARKER);
            //margin.Cursor = MarginCursor.Arrow;

            var marker = ScintillaNet.Markers[BOOKMARK_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            marker.SetBackColor(IntToColor(0xFF003B));
            marker.SetForeColor(IntToColor(0x000000));
            marker.SetAlpha(100);
        }

        private void InitCodeFolding(ScintillaWPF ScintillaNet)
        {
            //ScintillaNet.SetFoldMarginColor(true, IntToMediaColor(FORE_COLOR));
            ScintillaNet.SetFoldMarginColor(true, IntToMediaColor(0xf9f9f9));
            ScintillaNet.SetFoldMarginHighlightColor(true, IntToMediaColor(BACK_COLOR));

            // Enable code folding
            ScintillaNet.SetProperty("fold", "1");
            ScintillaNet.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            ScintillaNet.Margins[FOLDING_MARGIN].Type = MarginType.Symbol;
            ScintillaNet.Margins[FOLDING_MARGIN].Mask = Marker.MaskFolders;
            ScintillaNet.Margins[FOLDING_MARGIN].Sensitive = true;
            ScintillaNet.Margins[FOLDING_MARGIN].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                ScintillaNet.Markers[i].SetForeColor(IntToColor(BACK_COLOR)); // styles for [+] and [-]
                ScintillaNet.Markers[i].SetBackColor(IntToColor(FORE_COLOR)); // styles for [+] and [-]
            }

            // Configure folding markers with respective symbols
            ScintillaNet.Markers[Marker.Folder].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
            ScintillaNet.Markers[Marker.FolderOpen].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
            ScintillaNet.Markers[Marker.FolderEnd].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
            ScintillaNet.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            ScintillaNet.Markers[Marker.FolderOpenMid].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
            ScintillaNet.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            ScintillaNet.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            ScintillaNet.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }

        private void InitColors(ScintillaWPF ScintillaNet)
        {
            ScintillaNet.CaretForeColor = Colors.Black;
            ScintillaNet.CaretLineBackColor = IntToMediaColor(0xE8E8FF);
            ScintillaNet.CaretLineBackColorAlpha = 256;
            ScintillaNet.CaretLineVisible = true;
            ScintillaNet.SetSelectionBackColor(true, IntToMediaColor(0xE0E0E0));
            //FindReplace.Indicator.ForeColor = System.Drawing.Color.DarkOrange;
        }

        private void InitNumberMargin(ScintillaWPF ScintillaNet)
        {
            ScintillaNet.Styles[ScintillaNET.Style.LineNumber].BackColor = IntToColor(BACK_COLOR);
            ScintillaNet.Styles[ScintillaNET.Style.LineNumber].ForeColor = IntToColor(FORE_COLOR);
            ScintillaNet.Styles[ScintillaNET.Style.IndentGuide].ForeColor = IntToColor(FORE_COLOR);
            ScintillaNet.Styles[ScintillaNET.Style.IndentGuide].BackColor = IntToColor(BACK_COLOR);

            var nums = ScintillaNet.Margins[NUMBER_MARGIN];
            nums.Width = LINE_NUMBERS_MARGIN_WIDTH;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            ScintillaNet.MarginClick += TextArea_MarginClick;
        }

        private void InitSyntaxColoring(ScintillaWPF ScintillaNet)
        {
            #region VB / VBS
            ScintillaNet.StyleResetDefault();
            //ScintillaNet.Styles[ScintillaNET.Style.Default].Font = viewModel.EditorSettings.Font.FontFamily.Name;
            //ScintillaNet.Styles[ScintillaNET.Style.Default].Size = (int)viewModel.EditorSettings.Font.Size; // dangerous cast
            ScintillaNet.Styles[ScintillaNET.Style.Default].Font = viewModel.EditorSettings.FontFamily.ToString();
            ScintillaNet.Styles[ScintillaNET.Style.Default].Size = (int)(viewModel.EditorSettings.FontSize * (72.0/96.0));
            ScintillaNet.Styles[ScintillaNET.Style.Default].Italic = viewModel.EditorSettings.FontStyle == FontStyles.Italic;
            ScintillaNet.Styles[ScintillaNET.Style.Default].Weight = viewModel.EditorSettings.FontWeight.ToOpenTypeWeight();
            ScintillaNet.Styles[ScintillaNET.Style.Default].BackColor = viewModel.EditorSettings.BackColor.ToDrawingColor();
            ScintillaNet.Styles[ScintillaNET.Style.Default].ForeColor = viewModel.EditorSettings.ForeColor.ToDrawingColor();
            ScintillaNet.StyleClearAll(); // i.e. Apply to all
            // Configure the VbScript lexer styles
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
            #endregion
        }

        /// <summary>
        /// Converts a Win32 colour to a Drawing.Color
        /// </summary>
        /// <param name="rgb">A Win32 color.</param>
        /// <returns>A System.Drawing color.</returns>
        public static System.Drawing.Color IntToColor(int rgb)
        {
            return System.Drawing.Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        /// <summary>
        /// Converts a Win32 colour to a Media Color
        /// </summary>
        /// <param name="rgb">A Win32 color.</param>
        /// <returns>A System.Media color.</returns>
        public static Color IntToMediaColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        private void MyFindReplace_KeyPressed(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            ScintillaNet_KeyDown(sender, e);
        }

        private DocumentForm NewDocument()
        {
            DocumentForm doc = new DocumentForm();
            SetScintillaToCurrentOptions(doc);
            doc.Title = string.Format(CultureInfo.CurrentCulture, "{0}{1}", NEW_DOCUMENT_TEXT, ++_newDocumentCount);
            documentsRoot.Children.Add(doc);
            doc.DockAsDocument();
            doc.IsActive = true;
            return doc;
        }

        private void OpenFile()
        {
            bool? res = openFileDialog.ShowDialog();
            if (res == null || !(bool)res)
                return;

            foreach (string filePath in openFileDialog.FileNames)
            {
                // Ensure this file isn't already open
                bool isOpen = false;
                foreach (DocumentForm documentForm in Documents)
                {
                    if (filePath.Equals(documentForm.FilePath, StringComparison.OrdinalIgnoreCase))
                    {
                        documentForm.IsActive = true;
                        isOpen = true;
                        break;
                    }
                }

                // Open the files
                if (!isOpen)
                    OpenFile(filePath);
            }
        }

        private DocumentForm OpenFile(string filePath)
        {
            DocumentForm doc = new DocumentForm();
            SetScintillaToCurrentOptions(doc);
            doc.Scintilla.Text = File.ReadAllText(filePath, System.Text.Encoding.Default);
            doc.Scintilla.SetSavePoint(); // mark as unmodified
            //doc.Scintilla.UndoRedo.EmptyUndoBuffer();
            //doc.Scintilla.Modified = false;
            doc.Title = Path.GetFileName(filePath);
            doc.FilePath = filePath;
            documentsRoot.Children.Add(doc);
            doc.DockAsDocument();
            doc.IsActive = true;
            //incrementalSearcher.Scintilla = doc.Scintilla;

            return doc;
        }

        private DocumentForm OpenFile(ICommenceScript cs)
        {
            DocumentForm doc = new DocumentForm();
            SetScintillaToCurrentOptions(doc);
            doc.Scintilla.Text = File.ReadAllText(cs.FilePath, System.Text.Encoding.Default);
            doc.Scintilla.SetSavePoint(); // mark as unmodified
            //doc.Title = Path.GetFileName(cs.FilePath);
            doc.Title = cs.FullName;
            doc.FilePath = cs.FilePath;
            doc.CommenceScript = cs;
            documentsRoot.Children.Add(doc);
            doc.DockAsDocument();
            doc.IsActive = true;
            return doc;
        }

        private void SetScintillaToCurrentOptions(DocumentForm doc)
        {
            ScintillaWPF ScintillaNet = doc.Scintilla;
            ScintillaNet.KeyDown += ScintillaNet_KeyDown;
            ScintillaNet.CmdKeyPressed += ScintillaNet_CmdKeyPressed; // AVB
            ScintillaNet.TextChanged += Scintilla_TextChanged;

            // INITIAL VIEW CONFIG
            ScintillaNet.WrapMode = WrapMode.None;
            ScintillaNet.IndentationGuides = IndentView.LookBoth;

            // STYLING
            InitColors(ScintillaNet);
            InitSyntaxColoring(ScintillaNet);

            // NUMBER MARGIN
            InitNumberMargin(ScintillaNet);

            // BOOKMARK MARGIN
            InitBookmarkMargin(ScintillaNet);

            // CODE FOLDING MARGIN
            InitCodeFolding(ScintillaNet);

            // DRAG DROP
            // TODO - Enable InitDragDropFile
            //InitDragDropFile();

            // INIT HOTKEYS
            // TODO - Enable InitHotkeys
            //InitHotkeys(ScintillaNet);

            // Turn on line numbers?
            if (rbcbShowLineNumbers.IsChecked.HasValue && (bool)rbcbShowLineNumbers.IsChecked)
                doc.Scintilla.Margins[NUMBER_MARGIN].Width = LINE_NUMBERS_MARGIN_WIDTH;
            else
                doc.Scintilla.Margins[NUMBER_MARGIN].Width = 0;

            // Turn on white space?
            if (rbcbShowWhitespace.IsChecked.HasValue && (bool)rbcbShowWhitespace.IsChecked)
                doc.Scintilla.ViewWhitespace = WhitespaceMode.VisibleAlways;
            else
                doc.Scintilla.ViewWhitespace = WhitespaceMode.Invisible;

            // Turn on word wrap?
            if (rbcbWordWrap.IsChecked.HasValue && (bool)rbcbWordWrap.IsChecked)
                doc.Scintilla.WrapMode = WrapMode.Word;
            else
                doc.Scintilla.WrapMode = WrapMode.None;

            // Show EOL?
            if (rbcbShowEol.IsChecked.HasValue && (bool)rbcbShowEol.IsChecked)
                doc.Scintilla.ViewEol = true;
            else
                doc.Scintilla.ViewEol = false;

            // Set the zoom
            doc.Scintilla.Zoom = _zoomLevel;

        }

        // TODO this needs improvement
        private void Scintilla_TextChanged(object sender, EventArgs e)
        {
            //bgw.CancelAsync(); // stop any ongoing processing
            //if (!bgw.IsBusy)
            //{
            //    if (!string.IsNullOrEmpty(ActiveDocument?.Scintilla.Text))
            //        bgw.RunWorkerAsync(ActiveDocument.Scintilla.Text);
            //}
            RestartParsing();
        }

        private void RestartParsing()
        {
            bgw.CancelAsync(); // stop any ongoing processing
            if (!bgw.IsBusy)
            {
                if (!string.IsNullOrEmpty(ActiveDocument?.Scintilla.Text))
                    bgw.RunWorkerAsync(ActiveDocument.Scintilla.Text);
            }
        }

        // Intercept certain keypress combo's that would otherwise be eaten by Scintilla
        // Looks exactly similar to the ScintillaNet_KeyDown event,
        // but the difference is that CmdKeyPressed is processed deeper down in ScintillaNET.WPF.
        private void ScintillaNet_CmdKeyPressed(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == System.Windows.Forms.Keys.N)
            {
                if (UICommands.FileNew.CanExecute(null, this))
                {
                    UICommands.FileNew.Execute(null, this);
                }
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.O)
            {
                if (UICommands.FileOpen.CanExecute(null, this))
                {
                    UICommands.FileOpen.Execute(null, this);
                }
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.W)
            {
                if (UICommands.FileClose.CanExecute(null, this))
                {
                    UICommands.FileClose.Execute(null, this);
                }
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.Shift && e.KeyCode == System.Windows.Forms.Keys.S)
            {
                if (UICommands.FileSaveAll.CanExecute(null, this))
                {
                    UICommands.FileSaveAll.Execute(null, this);
                }
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.S)
            {
                ActiveDocument?.Save();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.Alt && e.KeyCode == System.Windows.Forms.Keys.F7)
            {
                CommenceCommands.CheckInScriptAndFocusCommence.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.Shift && e.KeyCode == System.Windows.Forms.Keys.F7)
            {
                CommenceCommands.CheckInScriptAndOpenForm.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.F7)
            {
                CommenceCommands.CheckInScript.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.Alt && e.KeyCode == System.Windows.Forms.Keys.F7)
            {
                CommenceCommands.FocusCommence.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.Shift && e.KeyCode == System.Windows.Forms.Keys.F4)
            {
                CommenceCommands.GetCategoryNames.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F4)
            {
                CommenceCommands.OpenCategoryList.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.Shift && e.KeyCode == System.Windows.Forms.Keys.F5)
            {
                CommenceCommands.GetFormNames.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F5)
            {
                CommenceCommands.OpenFormList.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.Shift && e.KeyCode == System.Windows.Forms.Keys.F6)
            {
                CommenceCommands.GetFieldNames.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F6)
            {
                CommenceCommands.OpenFieldList.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.Shift && e.KeyCode == System.Windows.Forms.Keys.F8)
            {
                CommenceCommands.GetConnectionNames.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F8)
            {
                CommenceCommands.OpenConnectionList.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.Shift && e.KeyCode == System.Windows.Forms.Keys.F9)
            {
                CommenceCommands.GetControlNames.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F9)
            {
                CommenceCommands.OpenControlList.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F11)
            {
                CommenceCommands.OpenGotoSectionList.Execute(null, this);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F12)
            {
                UICommands.ToggleRibbon.Execute(null, this);
                e.SuppressKeyPress = true;
            }
        }

        // original implementation
        private void ScintillaNet_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Insert)
            {
                viewModel.StatusBarModel.OverWrite = !viewModel.StatusBarModel.OverWrite;
            }
            if (e.Control && e.KeyCode == System.Windows.Forms.Keys.F)
            {
                MyFindReplace.ShowFind();
                e.SuppressKeyPress = true;
            }
            else if (e.Shift && e.KeyCode == System.Windows.Forms.Keys.F3)
            {
                MyFindReplace.Window.FindPrevious();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F3)
            {
                MyFindReplace.Window.FindNext();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.H)
            {
                MyFindReplace.ShowReplace();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.I)
            {
                MyFindReplace.ShowIncrementalSearch();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.G)
            {
                GoTo MyGoTo = new GoTo((Scintilla)sender);
                MyGoTo.ShowGoToDialog();
                e.SuppressKeyPress = true;
            }
        }

        private void TextArea_MarginClick(object sender, MarginClickEventArgs e)
        {
            ScintillaWPF TextArea = ActiveDocument.Scintilla;

            if (e.Margin == BOOKMARK_MARGIN)
            {
                // Do we have a marker for this line?
                const uint mask = (1 << BOOKMARK_MARKER);
                var line = TextArea.Lines[TextArea.LineFromPosition(e.Position)];
                if ((line.MarkerGet() & mask) > 0)
                {
                    // Remove existing bookmark
                    line.MarkerDelete(BOOKMARK_MARKER);
                }
                else
                {
                    // Add bookmark
                    line.MarkerAdd(BOOKMARK_MARKER);
                }
            }
        }

        private static Visibility Toggle(Visibility v)
        {
            if (v == Visibility.Visible)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            // DEFAULT FILE
            //OpenFile("../../SCide.WPF/MainWindow.xaml.cs");
            //OpenFile("../../SCide.WPF/DocumentForm.xaml.cs");
#endif
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            TempFileTracker.DeleteAll();
        }

        // If the document being closed belongs to the same category
        // as the currently selected category and form,
        // set the selected category to null
        // (unless another form from the same category is open)
        // This way, you can open the same category again
        // without having to switch between categories first.
        private void DockPanel_DocumentClosed(object sender, Xceed.Wpf.AvalonDock.DocumentClosedEventArgs e)
        {
            try
            {
                if (!(e.Document is DocumentForm doc)) { return; }
                if (doc.CommenceScript != null)
                {
                    // check all other docs except yourself
                    var otherDocs = Documents.Where(s => s != doc); // all other documents except the one being closed
                    if (otherDocs.Any(a => a.CommenceScript.CategoryName.Equals(doc.CommenceScript.CategoryName))) // are other forms from the same category openened?
                    {
                        viewModel.CommenceModel.SelectedForm = null;
                        return;
                    }
                }

                if (viewModel.CommenceModel.SelectedCategory.Equals(doc.CommenceScript.CategoryName)
                        && viewModel.CommenceModel.SelectedForm.Equals(doc.CommenceScript.FormName))
                {
                    viewModel.CommenceModel.SelectedCategory = null;
                    viewModel.CommenceModel.SelectedForm = null;
                    return;
                }

                if (Documents.Count() == 1)
                {
                    viewModel.CommenceModel.SelectedCategory = null;
                }
            }
            catch { }
        }

        private void FocusScintilla(int offSet = 0)
        {
            if (ActiveDocument == null) { return; }
            ActiveDocument.Scintilla.Scintilla.CurrentPosition = ActiveDocument.Scintilla.CurrentPosition + offSet;
            ActiveDocument.Scintilla.Scintilla.AnchorPosition = ActiveDocument.scintilla.CurrentPosition;
            ActiveDocument.Scintilla.Scintilla.Focus();
        }

        // This event handler is where the actual,
        // potentially time-consuming work is done.
        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // check if cancel was issued
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            e.Result = Parser.ParseVbScript((string)e.Argument);
        }

        private void Bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // The background process is complete. We need to inspect
            // our response to see if an error occurred, a cancel was
            // requested or if we completed successfully.  
            if (e.Cancelled) { return; }
            if (e.Result == null) { return; }

            List<IdentifierMatch> list = new List<IdentifierMatch>();
            foreach (IdentifierMatch m in (IdentifierMatches)e.Result)
            {
                list.Add(m);
            }
            viewModel.IdentifierMatches = list;
        }

        #endregion Methods

        #region Ribbon command handlers

        #region File commands
        private void FileNewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewDocument();
        }

        private void FileNewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FileOpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFile();
            FocusScintilla();
        }

        private void FileOpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FileCloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.FileClose.CanExecute(null, this))
            {
                ActiveDocument.Close();
            }
        }

        private void FileCloseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FileSaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        { 
            if (UICommands.FileSave.CanExecute(null, this))
            {
                ActiveDocument.Save();
            }
        }

        private void FileSaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.FileSaveAs.CanExecute(null, this))
            {
                ActiveDocument.SaveAs();
            }
        }

        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void FileSaveAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.FileSaveAll.CanExecute(null, this))
            {
                foreach (DocumentForm doc in Documents)
                {
                    doc.Save();
                }
            }
        }

        private void FileSaveAllCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        #endregion

        #region Undo/redo commands
        private void UndoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.Undo.CanExecute(null, this))
            {
                ActiveDocument.Scintilla.Undo();
            }
        }

        private void UndoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void RedoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.Redo.CanExecute(null, this))
            {
                ActiveDocument.Scintilla.Redo();
            }
        }

        private void RedoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }
        #endregion

        #region Copy/Paste commands
        private void PasteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.Paste.CanExecute(null, this))
            {
                ActiveDocument.Scintilla.Paste();
            }
        }

        private void PasteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void CopyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.Copy.CanExecute(null, this))
            {
                ActiveDocument.Scintilla.Copy();
            }
        }

        private void CopyCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void CutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.Cut.CanExecute(null, this))
            {
                ActiveDocument.Scintilla.Cut();
            }
        }

        private void CutCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void SelectAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.SelectAll.CanExecute(null, this))
            {
                ActiveDocument.Scintilla.SelectAll();
            }
        }
        #endregion

        #region Select commands
        private void SelectAllCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void SelectLineCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.SelectLine.CanExecute(null, this))
            {
                Line line = ActiveDocument.Scintilla.Lines[ActiveDocument.Scintilla.CurrentLine];
                ActiveDocument.Scintilla.SetSelection(line.Position + line.Length, line.Position);
            }
        }

        private void SelectLineCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void ClearSelectionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.ClearSelection.CanExecute(null, this))
            {
                ActiveDocument.Scintilla.SetEmptySelection(0);
            }
        }

        private void ClearSelectionCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }
        #endregion

        #region Find/replace commands
        private void FindCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.Find.CanExecute(null, this))
            {
                ActiveDocument.FindReplace.ShowFind();
            }
        }

        private void FindCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void ReplaceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.Replace.CanExecute(null, this))
            {
                ActiveDocument.FindReplace.ShowReplace();
            }
        }

        private void ReplaceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void IncrementalSearchCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.IncrementalSearch.CanExecute(null, this))
            {
                ActiveDocument.FindReplace.ShowIncrementalSearch();
            }
        }

        private void IncrementalSearchCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void GoToLineCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.GoToLine.CanExecute(null, this))
            {
                GoTo MyGoTo = new GoTo(ActiveDocument.Scintilla.Scintilla);
                MyGoTo.ShowGoToDialog();
            }

        }

        private void GoToLineCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }
        #endregion

        #region Bookmark commands
        private void BookmarkToggleCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.BookmarkToggle.CanExecute(null, this))
            {
                Line currentLine = ActiveDocument.Scintilla.Lines[ActiveDocument.Scintilla.CurrentLine];
                const uint mask = (1 << BOOKMARK_MARKER);
                uint markers = currentLine.MarkerGet();
                if ((markers & mask) > 0)
                {
                    currentLine.MarkerDelete(BOOKMARK_MARKER);
                }
                else
                {
                    currentLine.MarkerAdd(BOOKMARK_MARKER);
                }
            }
        }

        private void BookmarkToggleCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void BookmarkNextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.BookmarkNext.CanExecute(null, this))
            {
                int lineNumber = ActiveDocument.Scintilla.Lines[ActiveDocument.Scintilla.CurrentLine + 1].MarkerNext(1 << BOOKMARK_MARKER);
                if (lineNumber != -1)
                    ActiveDocument.Scintilla.Lines[lineNumber].Goto();
            }
        }

        private void BookmarkNextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void BookmarkPreviousCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.BookmarkPrevious.CanExecute(null, this))
            {
                int lineNumber = ActiveDocument.Scintilla.Lines[ActiveDocument.Scintilla.CurrentLine - 1].MarkerPrevious(1 << BOOKMARK_MARKER);
                if (lineNumber != -1)
                    ActiveDocument.Scintilla.Lines[lineNumber].Goto();
            }
        }

        private void BookmarkPreviousCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void BookmarksClearCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.BookmarksClear.CanExecute(null, this))
            {
                ActiveDocument.Scintilla.MarkerDeleteAll(BOOKMARK_MARKER);
            }
        }

        private void BookmarksClearCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }
        #endregion

        #region Advanced commands
        private void MakeUpperCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.MakeUpper.CanExecute(null, this))
            {
                ActiveDocument?.Scintilla.ExecuteCmd(Command.Uppercase);
            }
        }

        private void MakeUpperCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void MakeLowerCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.MakeLower.CanExecute(null, this))
            {
                ActiveDocument?.Scintilla.ExecuteCmd(Command.Lowercase);
            }
        }

        private void MakeLowerCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }
        #endregion

        #region View commands
        private void ShowStatusbarCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.ShowStatusbar.CanExecute(null, this))
            {
                // Toggle the visibility of the status bar
                statusStrip.Visibility = Toggle(statusStrip.Visibility);
            }
        }

        private void ShowStatusbarCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ShowShowLineNumbersCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.ShowLineNumbers.CanExecute(null, this))
            {
                foreach (DocumentForm docForm in Documents)
                {
                    if ((bool)e.Parameter)
                        docForm.Scintilla.Margins[NUMBER_MARGIN].Width = LINE_NUMBERS_MARGIN_WIDTH;
                    else
                        docForm.Scintilla.Margins[NUMBER_MARGIN].Width = 0;
                }
            }
        }

        private void ShowShowLineNumbersCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void ShowWhitespaceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.ShowWhitespace.CanExecute(null, this))
            {
                // Toggle the whitespace mode for all open files
                foreach (DocumentForm doc in Documents)
                {
                    if ((bool)e.Parameter)
                        doc.Scintilla.ViewWhitespace = WhitespaceMode.VisibleAlways;
                    else
                        doc.Scintilla.ViewWhitespace = WhitespaceMode.Invisible;
                }
            }
        }

        private void ShowWhitespaceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void ShowEolCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.ShowEol.CanExecute(null, this))
            {
                // Toggle EOL visibility for all open files
                foreach (DocumentForm doc in Documents)
                {
                    doc.Scintilla.ViewEol = (bool)e.Parameter;
                }
            }
        }

        private void ShowEolCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void WordWrapCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.WordWrap.CanExecute(null, this))
            {
                // Toggle word wrap for all open files
                foreach (DocumentForm doc in Documents)
                {
                    if ((bool)e.Parameter)
                        doc.Scintilla.WrapMode = WrapMode.Word;
                    else
                        doc.Scintilla.WrapMode = WrapMode.None;
                }
            }
        }

        private void WordWrapCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void ZoomInCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.ZoomIn.CanExecute(null, this))
            {
                // Increase the zoom for all open files
                _zoomLevel++;
                UpdateAllScintillaZoom();
            }
        }
        #endregion

        #region Zoom commands
        private void ZoomInCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void ZoomOutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.ZoomOut.CanExecute(null, this))
            {
                // Decrease the zoom for all open files
                _zoomLevel--;
                UpdateAllScintillaZoom();
            }
        }

        private void ZoomOutCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void ResetZoomCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.ResetZoom.CanExecute(null, this))
            {
                _zoomLevel = 0;
                UpdateAllScintillaZoom();
            }
        }

        private void ResetZoomCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }
        #endregion

        #region Fold commands
        private void FoldLevelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.FoldLevel.CanExecute(null, this))
            {
                ActiveDocument?.Scintilla.Lines[ActiveDocument.Scintilla.CurrentLine].FoldLine(FoldAction.Contract);
            }
        }

        private void FoldLevelCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void UnfoldLevelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.UnfoldLevel.CanExecute(null, this))
            {
                ActiveDocument?.Scintilla.Lines[ActiveDocument.Scintilla.CurrentLine].FoldLine(FoldAction.Expand);
            }
        }

        private void UnfoldLevelCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void FoldAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.FoldAll.CanExecute(null, this))
            {
                ActiveDocument?.Scintilla.FoldAll(FoldAction.Contract);
            }
        }

        private void FoldAllCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void UnfoldAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.UnfoldAll.CanExecute(null, this))
            {
                ActiveDocument.Scintilla.FoldAll(FoldAction.Expand);
            }
        }

        private void UnfoldAllCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        #endregion

        #region Window commands
        private void FocusScintillaCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (UICommands.FocusScintilla.CanExecute(null, this))
            {
                FocusScintilla(); // works from here
            }
        }

        private void FocusScintillaCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentsActive;
        }

        private void AboutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private void ToggleRibbonCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ribbon.Visibility = Toggle(ribbon.Visibility);
        }
        #endregion
        #endregion

        #region Commence
        private void LoadFormCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = viewModel.CommenceModel.IsRunning;
        }

        private async void LoadFormCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string categoryName = viewModel.CommenceModel.SelectedCategory;
            string formName = viewModel.CommenceModel.SelectedForm;

            // check to see if form is not already open
            foreach (DocumentForm documentForm in Documents)
            {
                if (documentForm.CommenceScript?.CategoryName == categoryName
                    && documentForm.CommenceScript?.FormName == formName)
                {
                    documentForm.IsActive = true;
                    return;
                }
            }

            // open the script
            string scriptFile = viewModel.CommenceModel.CheckOutFormScript(categoryName, formName);
            if (!string.IsNullOrEmpty(scriptFile))
            {
                ICommenceScript cs = new CommenceScript()
                {
                    CategoryName = categoryName,
                    DatabaseName = viewModel.CommenceModel.Name,
                    DatabasePath = viewModel.CommenceModel.Path,
                    FilePath = scriptFile,
                    FormName = formName
                };
                await cs.GetMetaDataAsync(); 
                OpenFile(cs);

                // (re)start code analyzer
                BgwStateChecker stillWorking = () => { return bgw.IsBusy; };
                if (bgw.IsBusy)
                {
                    bgw.CancelAsync();
                    while ((bool)this.Dispatcher.Invoke(stillWorking, null)) { Thread.Sleep(15); }
                }
                bgw.RunWorkerAsync(ActiveDocument.scintilla.Text);
            }
        }

        private void SaveCommenceScript()
        {
            if (ActiveDocument == null) { return; }
            if (ActiveDocument.CommenceScript != null && ActiveDocument.Save())
            {
                try
                {
                    viewModel.CommenceModel.CheckInFormScript(ActiveDocument.CommenceScript);
                }
                catch (Vovin.CmcLibNet.CommenceNotRunningException)
                {
                    MessageBox.Show("Commence is not running",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                catch (Vovin.CmcLibNet.CommenceDDEException)
                {
                    MessageBox.Show("Unable to check in Commence script." +
                        "\n\nCommon reasons are:\n\n" +
                        " - you have multiple Commence databases open\n" +
                        " - you have switched to another database in Commence\n\n" +
                        "Please make sure you have a single Commence database opened and it matches the database that the script belongs to.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }

        private void CheckInScriptCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.CheckInScript.CanExecute(null, this))
            {
                SaveCommenceScript();
            }
        }

        private void CheckInScriptCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!DocumentsActive || ActiveDocument?.CommenceScript == null) { return; }
            e.CanExecute = viewModel.CommenceModel.IsRunning;
        }

        private void CheckInScriptAndOpenFormCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.CheckInScriptAndOpenForm.CanExecute(null, this))
            {
                rbsbItems.IsDropDownOpen = true;
            }
        }

        private void CheckInScriptAndOpenFormCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rbsbItems.Items.Count > 0;
        }

        // returns true if there are any open documents
        private bool DocumentsActive
        {
            get
            {
                if (!this.IsInitialized) { return false; }
                else
                {
                    return Convert.ToBoolean(Documents?.Any());
                }
            }
        }

        private void CheckInScriptAndFocusCommenceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.CheckInScriptAndFocusCommence.CanExecute(null, this))
            {
                SaveCommenceScript();
                viewModel.CommenceModel.Focus();
            }
        }

        private void CheckInScriptAndFocusCommenceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!DocumentsActive || ActiveDocument?.CommenceScript == null) { return; }
            e.CanExecute = viewModel.CommenceModel.IsRunning;
        }

        private void FocusCommenceCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = viewModel.CommenceModel.IsRunning;
        }

        private void FocusCommenceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.FocusCommence.CanExecute(null, this))
            {
                viewModel.CommenceModel.Focus();
            }
        }

        private async void GetCategoryNamesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.GetCategoryNames.CanExecute(null, this))
            {
                await viewModel.CommenceModel.InitializeModelAsync().ConfigureAwait(false); // no need to switch context
            }
        }

        private void GetCategoryNamesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = viewModel.CommenceModel.IsRunning;
        }

        private void GetFormNamesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.GetFormNames.CanExecute(null, this))
            {
                viewModel.CommenceModel.GetFormNames(viewModel.CommenceModel.SelectedCategory);
            }
        }

        private void GetFormNamesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = viewModel.CommenceModel.SelectedCategory != null;
        }

        private async void GetFieldNamesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.GetFieldNames.CanExecute(null, this))
            {
                await viewModel.CommenceModel.SelectedScript.GetMetaDataAsync();
            }
        }

        private void GetFieldNamesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = viewModel.CommenceModel.SelectedForm != null;
        }

        private async void GetConnectionNamesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.GetConnectionNames.CanExecute(null, this))
            {
                await viewModel.CommenceModel.SelectedScript.GetMetaDataAsync();
            }
        }

        private void GetConnectionNamesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = viewModel.CommenceModel.SelectedForm != null;
        }

        private async void GetControlNamesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.GetControlNames.CanExecute(null, this))
            {
                await viewModel.CommenceModel.SelectedScript.GetMetaDataAsync();
            }
        }

        private void GetControlNamesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = viewModel.CommenceModel.SelectedForm != null;
        }

        private void InsertFieldCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rbsbFields.Items.Count > 0;
        }

        private void InsertFieldCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.InsertField.CanExecute(null, this))
            {
                CommenceField cf = (CommenceField)e.Parameter;
                ActiveDocument.Scintilla.InsertText(ActiveDocument.Scintilla.CurrentPosition, cf.FormString);
                FocusScintilla(cf.FormString.Length);
            }
        }

        private void InsertConnectionCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rbsbConnections.Items.Count > 0;
        }

        private void InsertConnectionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.InsertConnection.CanExecute(null, this))
            {
                CommenceConnection cc = (CommenceConnection)e.Parameter;
                ActiveDocument.Scintilla.InsertText(ActiveDocument.Scintilla.CurrentPosition, cc.FormString);
                FocusScintilla(cc.FormString.Length);
            }
        }

        private void InsertControlCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rbsbControls.Items.Count > 0;
        }

        private void InsertControlCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.InsertControl.CanExecute(null, this))
            {
                IDFControl cc = (IDFControl)e.Parameter;
                ActiveDocument.Scintilla.InsertText(ActiveDocument.Scintilla.CurrentPosition, cc.FormString);
                FocusScintilla(cc.FormString.Length);
            }
        }

        private void GotoSectionCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rbsbGotoSection.Items.Count > 0;
        }

        private void GotoSectionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.GotoSection.CanExecute(null, this))
            {
                IdentifierMatch m = (IdentifierMatch)e.Parameter;
                ActiveDocument.Scintilla.Scintilla.CurrentPosition = ActiveDocument.Scintilla.Scintilla.Lines[m.Line - 1].Position;
                ActiveDocument.Scintilla.Scintilla.AnchorPosition = ActiveDocument.Scintilla.Scintilla.CurrentPosition; // if we don't do this, we also select the positions we moved
                ActiveDocument.Scintilla.Scintilla.FirstVisibleLine = ActiveDocument.Scintilla.Scintilla.CurrentPosition;
                ActiveDocument.Scintilla.Scintilla.ScrollCaret();
                ActiveDocument.Scintilla.Scintilla.Focus();
            }
        }

        private void OpenCategoryListCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rbsbCategories.Items.Count > 0;
        }

        private void OpenCategoryListCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.OpenCategoryList.CanExecute(null, this))
            {
                rbsbCategories.IsDropDownOpen = true;
            }
        }

        private void OpenFormListCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rbsbForms.Items.Count > 0;
        }

        private void OpenFormListCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.OpenFormList.CanExecute(null, this))
            {
                rbsbForms.IsDropDownOpen = true;
            }
        }

        private void OpenFieldListCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rbsbFields.Items.Count > 0;
        }

        private void OpenFieldListCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.OpenFieldList.CanExecute(null, this))
            {
                rbsbFields.IsDropDownOpen = true;
            }
        }

        private void OpenConnectionListCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rbsbConnections.Items.Count > 0;
        }

        private void OpenConnectionListCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.OpenConnectionList.CanExecute(null, this))
            {
                rbsbConnections.IsDropDownOpen = true;
            }
        }

        private void OpenControlListCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rbsbControls.Items.Count > 0;
        }

        private void OpenControlListCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.OpenControlList.CanExecute(null, this))
            {
                rbsbControls.IsDropDownOpen = true;
            }
        }

        private void OpenGotoSectionListCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rbsbGotoSection.Items.Count > 0;
        }

        private void OpenGotoSectionListCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CommenceCommands.GotoSection.CanExecute(null, this))
            {
                rbsbGotoSection.IsDropDownOpen = true;
            }
        }

        private void ShowFilterBuilderCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // We must be on a form script and commence must be running the correct database
            // in order to be able to show the filter builder
            e.CanExecute = DocumentsActive
                && ActiveDocument?.CommenceScript != null
                && viewModel.CommenceModel.IsRunning
                && viewModel.CommenceModel.Name.Equals(ActiveDocument.CommenceScript.DatabaseName);
        }

        private void ShowFilterBuilderCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FilterBuilderWindow fbw = new FilterBuilderWindow(ActiveDocument.CommenceScript.CategoryName);
            if ((bool)fbw.ShowDialog())
            {
                ActiveDocument.Scintilla.InsertText(ActiveDocument.Scintilla.CurrentPosition, fbw.Result);
                FocusScintilla(fbw.Result.Length);
            }
        }
        #endregion

        // TODO make this work :)
        // the purpose of this handler is to put the focus on the scintilla editor
        // haven't been able to make it work so far
        private void RibbonComboBox_DropDownClosed(object sender, EventArgs e)
        {
            FocusScintilla(); // does not work here.
            // it does work from the ribbon with the FocusScintilla UICommand
            // that makes me think that this might actually work,
            // but the focus is taken back by the ribbon
        }


    }
}