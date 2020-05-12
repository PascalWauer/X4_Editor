using System.Windows.Input;

namespace X4_Editor
{
    public static class X4Commands
    {
        public static readonly RoutedUICommand ReadAllVanillaFilesCommand;
        public static readonly RoutedUICommand ReadAllModFilesCommand;
        public static readonly RoutedUICommand ReadAllActiveModFilesCommand;
        public static readonly RoutedUICommand WriteAllChangedFilesCommand;
        public static readonly RoutedUICommand AddToValueCommand;
        public static readonly RoutedUICommand MultiplyToValueCommand;
        public static readonly RoutedUICommand DivideByValueCommand;
        public static readonly RoutedUICommand SetFixedValueCommand;
        public static readonly RoutedUICommand FilterCommand;
        public static readonly RoutedUICommand ShowWaresWindowCommand;
        public static readonly RoutedUICommand SubstractFromValueCommand;
        public static readonly RoutedUICommand OnMainWindowCellRightClick;
        public static readonly RoutedUICommand OnWaresWindowCellRightClick;
        public static readonly RoutedUICommand SelectFolderCommand;
        public static readonly RoutedUICommand SelectMod1FolderCommand;
        public static readonly RoutedUICommand SelectMod2FolderCommand;
        public static readonly RoutedUICommand SelectMod3FolderCommand;
        public static readonly RoutedUICommand SelectMod4FolderCommand;
        public static readonly RoutedUICommand SelectMod5FolderCommand;
        public static readonly RoutedUICommand SelectMod6FolderCommand;
        public static readonly RoutedUICommand SelectExportFolderCommand;
        public static readonly RoutedUICommand RecalculatePriceCommand;
        public static readonly RoutedUICommand OnWeaponDoubleClick;
        public static readonly RoutedUICommand OnProjectileDoubleClick;
        public static readonly RoutedUICommand ShowHelp;
        public static readonly RoutedUICommand OpenModPathManager;
        public static readonly RoutedUICommand CloseModPathManager;

        static X4Commands()
        {
            ReadAllVanillaFilesCommand = new RoutedUICommand("Execute ReadAllVanillaFilesCommand", "ReadAllVanillaFilesCommand", typeof(X4Commands));
            ReadAllModFilesCommand = new RoutedUICommand("Execute ReadAllModFilesCommand", "ReadAllModFilesCommand", typeof(X4Commands));
            WriteAllChangedFilesCommand = new RoutedUICommand("Execute WriteAllChangedFilesCommand", "WriteAllChangedFilesCommand", typeof(X4Commands));
            AddToValueCommand = new RoutedUICommand("Execute AddToValueCommand", "AddToValueCommand", typeof(X4Commands));
            MultiplyToValueCommand = new RoutedUICommand("Execute MultiplyToValueCommand", "MultiplyToValueCommand", typeof(X4Commands));
            DivideByValueCommand = new RoutedUICommand("Execute DivideByValueCommand", "DivideByValueCommand", typeof(X4Commands));
            SetFixedValueCommand = new RoutedUICommand("Execute SetFixedValueCommand", "SetFixedValueCommand", typeof(X4Commands));
            FilterCommand = new RoutedUICommand("Execute FilterCommand", "FilterCommand", typeof(X4Commands));
            ShowWaresWindowCommand = new RoutedUICommand("Execute ShowWaresWindowCommand", "ShowWaresWindowCommand", typeof(X4Commands));
            SubstractFromValueCommand = new RoutedUICommand("Execute SubstractFromValueCommand", "SubstractFromValueCommand", typeof(X4Commands));
            OnMainWindowCellRightClick = new RoutedUICommand("Execute OnMainWindowCellRightClick", "OnMainWindowCellRightClick", typeof(X4Commands));
            OnWaresWindowCellRightClick = new RoutedUICommand("Execute OnWaresWindowCellRightClick", "OnWaresWindowCellRightClick", typeof(X4Commands));
            SelectFolderCommand = new RoutedUICommand("Execute SelectFolderCommand", "SelectFolderCommand", typeof(X4Commands));
            SelectFolderCommand = new RoutedUICommand("Execute SelectFolderCommand", "SelectFolderCommand", typeof(X4Commands));
            SelectMod1FolderCommand = new RoutedUICommand("Execute SelectMod1FolderCommand", "SelectMod1FolderCommand", typeof(X4Commands));
            SelectMod2FolderCommand = new RoutedUICommand("Execute SelectMod2FolderCommand", "SelectMod2FolderCommand", typeof(X4Commands));
            SelectMod3FolderCommand = new RoutedUICommand("Execute SelectMod3FolderCommand", "SelectMod3FolderCommand", typeof(X4Commands));
            SelectMod4FolderCommand = new RoutedUICommand("Execute SelectMod4FolderCommand", "SelectMod4FolderCommand", typeof(X4Commands));
            SelectMod5FolderCommand = new RoutedUICommand("Execute SelectMod5FolderCommand", "SelectMod5FolderCommand", typeof(X4Commands));
            SelectMod6FolderCommand = new RoutedUICommand("Execute SelectMod6FolderCommand", "SelectMod6FolderCommand", typeof(X4Commands));
            SelectExportFolderCommand = new RoutedUICommand("Execute SelectExportFolderCommand", "SelectExportFolderCommand", typeof(X4Commands));
            RecalculatePriceCommand = new RoutedUICommand("Execute RecalculatePriceCommand", "RecalculatePriceCommand", typeof(X4Commands));
            OnWeaponDoubleClick = new RoutedUICommand("Execute OnWeaponDoubleClick", "OnWeaponDoubleClick", typeof(X4Commands));
            OnProjectileDoubleClick = new RoutedUICommand("Execute OnProjectileDoubleClick", "OnProjectileDoubleClick", typeof(X4Commands));
            OpenModPathManager = new RoutedUICommand("Execute OpenModPathManager", "OpenModPathManager", typeof(X4Commands));
            CloseModPathManager = new RoutedUICommand("Execute CloseModPathManager", "CloseModPathManager", typeof(X4Commands));
            ShowHelp = new RoutedUICommand("Execute ShowHelp", "ShowHelp", typeof(X4Commands));
        }
    }
}
