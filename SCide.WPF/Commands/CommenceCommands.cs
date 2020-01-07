using System.Windows.Input;

namespace SCide.WPF.Commands
{
    public static class CommenceCommands
    {

        public static readonly RoutedUICommand CheckInScript = new RoutedUICommand
            (
                "Save script (Ctrl+F7)",
                "Save",
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F7, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand CheckInScriptAndFocusCommence = new RoutedUICommand
            (
                "Save script and switch to Commence (Ctrl+Alt+F7)",
                "Save and switch to Commence",
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F7, ModifierKeys.Alt | ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand CheckInScriptAndOpenForm = new RoutedUICommand
            (
                "Save script and open form (Ctrl+Shift+F7)",
                "Save script and open form",
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F7, ModifierKeys.Shift | ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand FocusCommence = new RoutedUICommand
            (
                "Switch to Commence (Alt+F7)",
                "Switch to Commence",
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F7, ModifierKeys.Alt)
                }
            );

        public static readonly RoutedCommand GetCategoryNames = new RoutedCommand
            (
                nameof(GetCategoryNames),
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F4, ModifierKeys.Shift)
                }
            );

        public static readonly RoutedCommand GetFormNames = new RoutedCommand
            (
                nameof(GetFormNames),
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F5, ModifierKeys.Shift)
                }
            );

        public static readonly RoutedCommand GetFieldNames = new RoutedCommand
            (
                nameof(GetFieldNames),
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F6, ModifierKeys.Shift)
                }
            );

        public static readonly RoutedCommand GetConnectionNames = new RoutedCommand
            (
                nameof(GetConnectionNames),
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F8, ModifierKeys.Shift)
                }
            );

        public static readonly RoutedCommand GetControlNames = new RoutedCommand
            (
                nameof(GetControlNames),
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F9, ModifierKeys.Shift)
                }
            );

        public static readonly RoutedCommand GotoSection = new RoutedCommand
            (
                nameof(GotoSection),
                typeof(CommenceCommands),
                new InputGestureCollection()
            );

        public static readonly RoutedCommand LoadForm = new RoutedCommand
            (
                nameof(LoadForm),
                typeof(CommenceCommands),
                new InputGestureCollection()
            );

        public static readonly RoutedCommand InsertField = new RoutedCommand
            (
                nameof(InsertField),
                typeof(CommenceCommands),
                new InputGestureCollection()
            );

        public static readonly RoutedCommand InsertConnection = new RoutedCommand
            (
                nameof(InsertConnection),
                typeof(CommenceCommands),
                new InputGestureCollection()
            );
        public static readonly RoutedCommand InsertControl = new RoutedCommand
            (
                nameof(InsertControl),
                typeof(CommenceCommands),
                new InputGestureCollection()
            );
        public static readonly RoutedUICommand OpenCategoryList = new RoutedUICommand
            (
                nameof(OpenCategoryList),
                nameof(OpenCategoryList),
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F4)
                }
            );
        public static readonly RoutedUICommand OpenFormList = new RoutedUICommand
            (
                nameof(OpenFormList),
                nameof(OpenFormList),
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F5)
                }
            );
        public static readonly RoutedUICommand OpenFieldList = new RoutedUICommand
            (
                nameof(OpenFieldList),
                nameof(OpenFieldList),
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F6)
                }
            );
        public static readonly RoutedUICommand OpenConnectionList = new RoutedUICommand
            (
                nameof(OpenConnectionList),
                nameof(OpenConnectionList),
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F8)
                }
            );
        public static readonly RoutedUICommand OpenControlList = new RoutedUICommand
            (
                nameof(OpenControlList),
                nameof(OpenControlList),
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F9)
                }
            );
        public static readonly RoutedUICommand OpenGotoSectionList = new RoutedUICommand
            (
                nameof(OpenGotoSectionList),
                nameof(OpenGotoSectionList),
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F11)
                }
            );

        public static readonly RoutedUICommand ShowFilterBuilder = new RoutedUICommand
            (
                nameof(ShowFilterBuilder),
                nameof(ShowFilterBuilder),
                typeof(CommenceCommands),
                new InputGestureCollection()
                {
                    //new KeyGesture(Key.F12)
                }
            );
    }
}
