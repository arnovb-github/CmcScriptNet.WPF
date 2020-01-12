using System.Windows.Input;

namespace SCide.WPF.Commands
{
    /// <summary>
    /// Commands for the Ribbon
    /// </summary>
    /// <remarks>(Most) ribbon labels are bound to the command name,
    /// that is why not all declarations use nameof().</remarks>
    public static class UICommands
    {
        #region Ribbon commands
        #region File Commands
        public static readonly RoutedUICommand FileOpen = new RoutedUICommand
            (
                "Open (Ctrl+O)",
                "Open",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.O, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand FileNew = new RoutedUICommand
            (
                "New (Ctrl+N)",
                "New",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.N, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand FileSave = new RoutedUICommand
            (
                "Save (Ctrl+S)",
                "Save",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand FileSaveAs = new RoutedUICommand
            (
                "Save as… (Ctrl+Alt+S)",
                "Save as…",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Alt)
                }
            );

        public static readonly RoutedUICommand FileSaveAll = new RoutedUICommand
            (
                "Save all (Ctrl+Shift+S)",
                "Save all",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift)
                }
            );

        public static readonly RoutedUICommand FileClose = new RoutedUICommand
            (
                "Close (Ctrl+W)",
                "Close",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.W, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand Exit = new RoutedUICommand
            (
                "Exit (Alt+F4)",
                nameof(Exit),
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F4, ModifierKeys.Alt)
                }
            );
        #endregion

        #region Edit Commands
        public static readonly RoutedUICommand Undo = new RoutedUICommand
            (
                "Undo (Ctrl+Z)",
                nameof(Undo),
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.Z, ModifierKeys.Control )
                }
            );

        public static readonly RoutedUICommand Redo = new RoutedUICommand
            (
                "Redo (Ctrl+Y)",
                nameof(Redo),
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.Y, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand Paste = new RoutedUICommand
            (
                "Paste (Ctrl+V)",
                nameof(Paste),
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.V, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand Cut = new RoutedUICommand
            (
                "Cut",
                nameof(Cut),
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.X, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand Copy = new RoutedUICommand
            (
                "Copy (Ctrl+C)",
                nameof(Copy),
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.C, ModifierKeys.Control)
                }
            );

        #endregion

        #region Select Commands
        public static readonly RoutedUICommand SelectAll = new RoutedUICommand
            (
                "Select all (Ctrl+A)",
                "Select all",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.A, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand SelectLine = new RoutedUICommand
            (
                "Select line",
                "Select line",
                typeof(UICommands),
                new InputGestureCollection()
            );

        public static readonly RoutedUICommand ClearSelection = new RoutedUICommand
            (
                "Clear Selection",
                "Clear Selection",
                typeof(UICommands),
                new InputGestureCollection()
            );
        #endregion

        #region Find Commands
        public static readonly RoutedUICommand Find = new RoutedUICommand
            (
                "Find (Ctrl+F)",
                nameof(Find),
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand Replace = new RoutedUICommand
            (
                "Replace (Ctrl+H)",
                nameof(Replace),
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.H, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand IncrementalSearch = new RoutedUICommand
            (
                "Incremental search (Ctrl+I)",
                "Incremental search",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.I, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand GoToLine = new RoutedUICommand
            (
                "Goto line (Ctrl+G)",
                "Goto line",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.G, ModifierKeys.Control)
                }
            );
        #endregion

        #region Bookmark commands
        public static readonly RoutedUICommand BookmarkToggle = new RoutedUICommand
            (
                "Toggle (Ctrl+F2)",
                "Toggle",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F2, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand BookmarkPrevious = new RoutedUICommand
            (
                "Previous (F2)",
                "Previous",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F2)
                }
            );

        public static readonly RoutedUICommand BookmarkNext = new RoutedUICommand
            (
                "Next (Shift+F2)",
                "Next",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F2, ModifierKeys.Shift)
                }
            );

        public static readonly RoutedUICommand BookmarksClear = new RoutedUICommand
            (
                "Clear (Ctrl+Shift+F2)",
                "Clear",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F2, ModifierKeys.Shift | ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand MakeUpper = new RoutedUICommand
            (
                "Make Upper Case (Ctrl+Shift+U)",
                "Make Upper Case",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.U, ModifierKeys.Shift | ModifierKeys.Control)
                }
            );
        #endregion

        #region Advanced Commands
        public static readonly RoutedUICommand MakeLower = new RoutedUICommand
            (
                "Make Lower Case (Ctrl-U)",
                "Make Lower Case",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.U, ModifierKeys.Control)
                }
            );
        #endregion

        #region Editor Commands
        public static readonly RoutedUICommand ShowStatusbar = new RoutedUICommand
            (
                "Show Statusbar",
                "Show Statusbar",
                typeof(UICommands),
                new InputGestureCollection()
            );

        public static readonly RoutedUICommand ShowLineNumbers = new RoutedUICommand
            (
                "Show Line Numbers",
                "Show Line Numbers",
                typeof(UICommands),
                new InputGestureCollection()
            );

        public static readonly RoutedUICommand ShowWhitespace = new RoutedUICommand
            (
                "Show Whitespace",
                "Show Whitespace",
                typeof(UICommands),
                new InputGestureCollection()
            );

        public static readonly RoutedUICommand WordWrap = new RoutedUICommand
            (
                "Word Wrap",
                "Word Wrap",
                typeof(UICommands),
                new InputGestureCollection()
            );

        public static readonly RoutedUICommand ShowEol = new RoutedUICommand
            (
                "Show End of Line",
                "Show End of Line",
                typeof(UICommands),
                new InputGestureCollection()
            );

        public static readonly RoutedUICommand ShowOptions = new RoutedUICommand
            (
                "Editor options…",
                "Editor options…",
                typeof(UICommands),
                new InputGestureCollection()
            );
        #endregion

        #region Zoom Commands
        public static readonly RoutedUICommand ZoomIn = new RoutedUICommand
            (
                "Zoom In",
                "Zoom In",
                typeof(UICommands),
                new InputGestureCollection()
            );

        public static readonly RoutedUICommand ZoomOut = new RoutedUICommand
            (
                "Zoom Out",
                "Zoom Out",
                typeof(UICommands),
                new InputGestureCollection()
            );

        public static readonly RoutedUICommand ResetZoom = new RoutedUICommand
            (
                "Reset Zoom",
                "Reset Zoom",
                typeof(UICommands),
                new InputGestureCollection()
            );
        #endregion

        #region Fold Commands
        public static readonly RoutedUICommand FoldLevel = new RoutedUICommand
            (
                "Fold Level",
                "Fold Level",
                typeof(UICommands),
                new InputGestureCollection()
            );

        public static readonly RoutedUICommand UnfoldLevel = new RoutedUICommand
            (
                "Unfold Level",
                "Unfold Level",
                typeof(UICommands),
                new InputGestureCollection()
            );

        public static readonly RoutedUICommand FoldAll = new RoutedUICommand
            (
                "Fold All",
                "Fold All",
                typeof(UICommands),
                new InputGestureCollection()
            );

        public static readonly RoutedUICommand UnfoldAll = new RoutedUICommand
            (
                "Unfold All",
                "Unfold All",
                typeof(UICommands),
                new InputGestureCollection()
            );
        #endregion

        #region Window Commands
        public static readonly RoutedUICommand FocusScintilla = new RoutedUICommand
            (
                "Focus Editor (Ctrl+E)",
                "Focus Editor",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.E, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand About = new RoutedUICommand
            (
                "About",
                nameof(About),
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F1)
                }
            );

        public static readonly RoutedUICommand ToggleRibbon = new RoutedUICommand
            (
                "Toggle Ribbon (F12)",
                "Toggle Ribbon (F12)",
                typeof(UICommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F12)
                }
            );
        #endregion

        #endregion
    }
}
