using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ScintillaNET.WPF
{
    /// <summary>
    /// Custom ScintillaNet implementation trapping certain keyboard events
    /// to prevent them being added to the text in the editor.
    /// </summary>
    public class MyScintilla : ScintillaNET.Scintilla
    {
        public event KeyEventHandler CmdKeyPressed;

        private readonly List<Keys> keysToTrap;
        private List<Keys> KeysToTrap { get; set; }

        public MyScintilla()
        {
            keysToTrap = new List<Keys>()
            {
                Keys.Control | Keys.E, // focus editor
                Keys.Control | Keys.N, // new
                Keys.Control | Keys.O, // open
                Keys.Control | Keys.S, // save
                Keys.Control | Keys.W, // close
                Keys.Alt | Keys.Control | Keys.F7, // check in script and focus Commence
                Keys.Shift | Keys.Control | Keys.F7, // check in script and open form
                Keys.Alt | Keys.F7, // focus commence
                Keys.Control | Keys.F7, // check in script
                Keys.Control | Keys.Shift | Keys.S, // save all
                Keys.Control | Keys.Alt | Keys.S, // save as
                Keys.F4, // load categories
                Keys.Shift | Keys.F4, // refresh categories
                Keys.F5, // load form
                Keys.Shift | Keys.F5, // refresh forms
                Keys.F6, // insert field
                Keys.Shift | Keys.F6, // refresh fields
                Keys.F8, // insert connection
                Keys.Shift | Keys.F8, // refresh connections
                Keys.F9, // insert control
                Keys.Shift | Keys.F9, // refresh controls
                Keys.F11, // goto
                Keys.F12 // toggle ribbon
            };
            this.KeysToTrap = keysToTrap;
        }

        // trap specific keyboard combinations
        // so as to prevent them being entered into the document text
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.KeysToTrap.Any(a => a == keyData))
            {
                CmdKeyPressed?.Invoke(this, new KeyEventArgs(keyData));
                return true;
            }
            // not caught, just pass on
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
