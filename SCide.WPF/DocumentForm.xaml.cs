using SCide.WPF.Commence;
using ScintillaNET.WPF;
using ScintillaNET_FindReplaceDialog;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;

namespace SCide.WPF
{
    /// <summary>
    /// Interaction logic for DocumentForm.xaml
    /// </summary>
    public partial class DocumentForm : LayoutDocument
    {
        public FindReplace FindReplace { get; set; }
        public ICommenceScript CommenceScript { get; set; }

        public DocumentForm()
        {
            InitializeComponent();
            this.Title = "";
            this.Closing += new EventHandler<CancelEventArgs>(DocumentForm_Closing);
            Scintilla.MouseDown += Scintilla_MouseDown;
            Scintilla.Scintilla.MouseDoubleClick += Scintilla_MouseDoubleClick; // AVB
            Scintilla.SavePointLeft += Scintilla_SavePointLeft;
        }

        // AVB new      
        private void Scintilla_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:

                    if (FindReplace == null) break;
                    FindReplace.ClearAllHighlights();
                    string sWord = Scintilla.GetWordFromPosition(Scintilla.CurrentPosition);
                    if (!string.IsNullOrEmpty(sWord))
                        FindReplace.FindAll(sWord, false, true);
                    break;

                default:
                    break;
            }
        }

        private void Scintilla_SavePointLeft(object sender, EventArgs e)
        {
            AddOrRemoveAsteric();
        }

        // AVB edited
        private void Scintilla_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            {
                switch (e.Button)
                {
                    case System.Windows.Forms.MouseButtons.Left:
                        if (FindReplace == null) break;

                        //Clear current highlights
                        FindReplace.ClearAllHighlights();
                        break;

                    default:
                        break;
                }
            }
        }

        public ScintillaWPF Scintilla
        {
            get { return scintilla; }
        }

        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        private void AddOrRemoveAsteric()
        {
            if (scintilla.Modified)
            {
                if (!Title.EndsWith(" *", StringComparison.InvariantCulture))
                    Title += " *";
            }
            else
            {
                if (Title.EndsWith(" *", StringComparison.InvariantCulture))
                    Title = Title.Substring(0, Title.Length - 2);
            }
        }

        private void DocumentForm_Closing(object sender, CancelEventArgs e)
        {
            if (Scintilla.Modified)
            {
                // Prompt if not saved
                string message = string.Format(CultureInfo.CurrentCulture, "The content of file '{0}' has changed.{1}{2}Do you want to save the changes?", Title.TrimEnd(' ', '*'), Environment.NewLine, Environment.NewLine);

                MessageBoxResult dr = MessageBox.Show(message, Program.Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                if (dr == MessageBoxResult.Cancel)
                {
                    // Stop closing
                    e.Cancel = true;
                    return;
                }
                else if (dr == MessageBoxResult.Yes)
                {
                    // Try to save before closing
                    e.Cancel = !Save();
                    return;
                }
            }

            // Close as normal
        }

        public bool Save()
        {
            if (string.IsNullOrEmpty(_filePath))
                return SaveAs();

            return Save(_filePath);
        }

        //// AVB This is the original implementation of the Save method
        //// There are 2 issues:
        //// - it omits the last character
        //// - it doesn't deal well with characters like the copyright character (further testing needed)
        //public bool Save(string filePath)
        //{
        //    using (FileStream fs = File.Create(filePath))
        //    {
        //        using (BinaryWriter bw = new BinaryWriter(fs))
        //        {
        //            bw.Write(scintilla.Text.ToCharArray(), 0, scintilla.Text.Length - 1); // Omit trailing NULL
        //        }
        //    }
        //    scintilla.SetSavePoint();
        //    return true;
        //}

        // AVB: A simpler saving method
        // this may have it's own issues but I do not care
        public bool Save(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.Default))
            {
                sw.Write(scintilla.Text);
            }

            if (this.CommenceScript != null)
            {
                this.Title = this.CommenceScript.FullName;
            }
            else
            {
                this.Title = Path.GetFileName(filePath);
            }

            scintilla.SetSavePoint();
            return true;
        }

        public bool SaveAs()
        {
            bool? res = saveFileDialog.ShowDialog();
            if (res != null && (bool)res)
            {
                _filePath = saveFileDialog.FileName;
                return Save(_filePath);
            }

            return false;
        }
    }
}
