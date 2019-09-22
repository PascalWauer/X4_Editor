﻿using System.Windows.Input;

namespace X4_Editor
{
    public static class X4Commands
    {
        public static readonly RoutedUICommand ReadAllVanillaFilesCommand;
        public static readonly RoutedUICommand ReadAllModFilesCommand;
        public static readonly RoutedUICommand WriteAllChangedFilesCommand;
        public static readonly RoutedUICommand AddToValueCommand;
        public static readonly RoutedUICommand MultiplyToValueCommand;
        public static readonly RoutedUICommand SetFixedValueCommand;
        public static readonly RoutedUICommand FilterCommand;
        public static readonly RoutedUICommand ShowWaresWindowCommand;
        public static readonly RoutedUICommand SubstractToValueCommand;
        public static readonly RoutedUICommand OnMainWindowCellRightClick;
        public static readonly RoutedUICommand OnWaresWindowCellRightClick;
        public static readonly RoutedUICommand SelectFolderCommand;
        public static readonly RoutedUICommand SelectModFolderCommand;
        public static readonly RoutedUICommand SelectExportFolderCommand;
        public static readonly RoutedUICommand RecalculatePriceCommand;
        public static readonly RoutedUICommand OnWeaponDoubleClick;
        public static readonly RoutedUICommand OnProjectileDoubleClick;

        static X4Commands()
        {
            ReadAllVanillaFilesCommand = new RoutedUICommand("Execute ReadAllVanillaFilesCommand", "ReadAllVanillaFilesCommand", typeof(X4Commands));
            ReadAllModFilesCommand = new RoutedUICommand("Execute ReadAllModFilesCommand", "ReadAllModFilesCommand", typeof(X4Commands));
            WriteAllChangedFilesCommand = new RoutedUICommand("Execute WriteAllChangedFilesCommand", "WriteAllChangedFilesCommand", typeof(X4Commands));
            AddToValueCommand = new RoutedUICommand("Execute AddToValueCommand", "AddToValueCommand", typeof(X4Commands));
            MultiplyToValueCommand = new RoutedUICommand("Execute MultiplyToValueCommand", "MultiplyToValueCommand", typeof(X4Commands));
            SetFixedValueCommand = new RoutedUICommand("Execute SetFixedValueCommand", "SetFixedValueCommand", typeof(X4Commands));
            FilterCommand = new RoutedUICommand("Execute FilterCommand", "FilterCommand", typeof(X4Commands));
            ShowWaresWindowCommand = new RoutedUICommand("Execute ShowWaresWindowCommand", "ShowWaresWindowCommand", typeof(X4Commands));
            SubstractToValueCommand = new RoutedUICommand("Execute SubstractToValueCommand", "SubstractToValueCommand", typeof(X4Commands));
            OnMainWindowCellRightClick = new RoutedUICommand("Execute OnMainWindowCellRightClick", "OnMainWindowCellRightClick", typeof(X4Commands));
            OnWaresWindowCellRightClick = new RoutedUICommand("Execute OnWaresWindowCellRightClick", "OnWaresWindowCellRightClick", typeof(X4Commands));
            SelectFolderCommand = new RoutedUICommand("Execute SelectFolderCommand", "SelectFolderCommand", typeof(X4Commands));
            SelectFolderCommand = new RoutedUICommand("Execute SelectFolderCommand", "SelectFolderCommand", typeof(X4Commands));
            SelectModFolderCommand = new RoutedUICommand("Execute SelectModFolderCommand", "SelectModFolderCommand", typeof(X4Commands));
            SelectExportFolderCommand = new RoutedUICommand("Execute SelectExportFolderCommand", "SelectExportFolderCommand", typeof(X4Commands));
            RecalculatePriceCommand = new RoutedUICommand("Execute RecalculatePriceCommand", "RecalculatePriceCommand", typeof(X4Commands));
            OnWeaponDoubleClick = new RoutedUICommand("Execute OnWeaponDoubleClick", "OnWeaponDoubleClick", typeof(X4Commands));
            OnProjectileDoubleClick = new RoutedUICommand("Execute OnProjectileDoubleClick", "OnProjectileDoubleClick", typeof(X4Commands));
        }
    }
}