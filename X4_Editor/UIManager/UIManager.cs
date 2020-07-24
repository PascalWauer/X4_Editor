using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using X4_Editor.Helper;
using DataGrid = System.Windows.Controls.DataGrid;
using MessageBox = System.Windows.MessageBox;

namespace X4_Editor
{
    public class Counter
    {
        public int successcounter { get; set; }
        public int outofrangecounter { get; set; }
        public Counter()
        {
            successcounter = 0;
            outofrangecounter = 0;
        }
    }

    public class UIManager
    {
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();

        public string PathToShields = @"\assets\props\SurfaceElements\macros";
        public string PathToShips = @"\assets\units";
        public string PathToProjectiles = @"\assets\fx\weaponFx\macros";
        public string PathToMissiles = @"\assets\props\WeaponSystems\missile\macros";
        public string PathToTurretsStandard = @"\assets\props\WeaponSystems\standard\macros";
        public string PathToTurretsHeavy = @"\assets\props\WeaponSystems\heavy\macros";
        public string PathToTurretsEnergy = @"\assets\props\WeaponSystems\energy\macros";
        public string PathToTurretsCapital = @"\assets\props\WeaponSystems\capital\macros";
        public string PathToTurretsGuided = @"\assets\props\WeaponSystems\guided\macros";
        public string PathToTurretsDumbfire = @"\assets\props\WeaponSystems\dumbfire\macros";
        public string PathToWares = @"\libraries\wares.xml";
        public string PathToEngines = @"\assets\props\Engines\macros";
        public string PathToTexts = @"\t";

        public List<string> VanillaXmlFiles;
        public List<string> Mod1XmlFiles;
        public List<string> Mod2XmlFiles;
        public List<string> Mod3XmlFiles;
        public List<string> Mod4XmlFiles;
        public List<string> Mod5XmlFiles;
        public List<string> Mod6XmlFiles;

        private ModFilesReader m_ModFilesReader;

        public Dictionary<string, string> TextDictionary = new Dictionary<string, string>();
        public MainWindow MainWindow { get; set; }
        public WaresWindow WaresWindow { get; set; }
        public ModManager ModManager { get; set; }
        public UIModel UIModel { get; set; }
        public XmlExtractor m_XmlExtractor;
        public XmlWriter m_XmlWriter;
        public ReadWriteConfig m_ReadWriteConfig;
        public Calculations m_Calculations;

        public UIManager()
        {
            MainWindow = new MainWindow();
            WaresWindow = new WaresWindow();
            ModManager = new ModManager();
            m_XmlExtractor = new XmlExtractor(this);
            m_XmlWriter = new XmlWriter(this);
            m_ReadWriteConfig = new ReadWriteConfig(this);
            m_Calculations = new Calculations(this);
            this.UIModel = new UIModel();
            MainWindow.DataContext = this.UIModel;
            WaresWindow.DataContext = this.UIModel;

            VanillaXmlFiles = new List<string>();
            Mod1XmlFiles = new List<string>();
            Mod2XmlFiles = new List<string>();
            Mod3XmlFiles = new List<string>();
            Mod4XmlFiles = new List<string>();
            Mod5XmlFiles = new List<string>();
            Mod6XmlFiles = new List<string>();

            m_ModFilesReader = new ModFilesReader(this, m_XmlExtractor);
            MainWindow.Closing += OnClosing;
            
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.ReadAllVanillaFilesCommand, this.ExecuteReadAllVanillaFilesCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.ReadAllModFilesCommand, this.ExecuteReadAllModFilesCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.OpenModPathManager, this.ExecuteOpenModPathManager));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.FilterCommand, this.ExecuteFilterCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.WriteAllChangedFilesCommand, this.ExecuteWriteAllChangedFilesCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.AddToValueCommand, this.ExecuteAddToValueCommand, CanExecuteCalculate));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.SubstractFromValueCommand, this.ExecuteSubstractFromValueCommand, CanExecuteCalculate));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.MultiplyToValueCommand, this.ExecuteMultiplyToValueCommand, CanExecuteCalculate));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.DivideByValueCommand, this.ExecuteDivideByValueCommand, CanExecuteCalculate));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.SetFixedValueCommand, this.ExecuteSetFixedValueCommand, CanExecuteCalculate));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.ShowWaresWindowCommand, this.ExecuteShowWaresWindowCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.OnMainWindowCellRightClick, this.ExecuteOnMainWindowCellRightClick));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.SelectFolderCommand, this.ExecuteSelectFolderCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.SelectMod1FolderCommand, this.ExecuteSelectMod1FolderCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.SelectMod2FolderCommand, this.ExecuteSelectMod2FolderCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.SelectExportFolderCommand, this.ExecuteSelectExportFolderCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.RecalculatePriceCommand, this.ExecuteRecalculatePriceCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.OnWeaponDoubleClick, this.ExecuteOnWeaponDoubleClick));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.OnProjectileDoubleClick, this.ExecuteOnProjectileDoubleClick));
            
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.ShowHelp, this.ExecuteShowHelp));

            ModManager = new ModManager();
            ModManager.DataContext = this.UIModel;
            ModManager.CommandBindings.Add(new CommandBinding(X4Commands.SelectFolderCommand, this.ExecuteSelectFolderCommand));
            ModManager.CommandBindings.Add(new CommandBinding(X4Commands.SelectMod1FolderCommand, this.ExecuteSelectMod1FolderCommand));
            ModManager.CommandBindings.Add(new CommandBinding(X4Commands.SelectMod2FolderCommand, this.ExecuteSelectMod2FolderCommand));
            ModManager.CommandBindings.Add(new CommandBinding(X4Commands.SelectMod3FolderCommand, this.ExecuteSelectMod3FolderCommand));
            ModManager.CommandBindings.Add(new CommandBinding(X4Commands.SelectMod4FolderCommand, this.ExecuteSelectMod4FolderCommand));
            ModManager.CommandBindings.Add(new CommandBinding(X4Commands.SelectMod5FolderCommand, this.ExecuteSelectMod5FolderCommand));
            ModManager.CommandBindings.Add(new CommandBinding(X4Commands.SelectMod6FolderCommand, this.ExecuteSelectMod6FolderCommand));
            ModManager.CommandBindings.Add(new CommandBinding(X4Commands.CloseModPathManager, this.ExecuteCloseModPathManager));

            m_ReadWriteConfig.LoadConfig();
            this.TextDictionary = new Dictionary<string, string>();
            this.TextDictionary = m_XmlExtractor.ReadTextXml(this.UIModel.Path + this.PathToTexts + @"\0001-l044.xml", this.TextDictionary);
            MainWindow.Show();

            this.WaresWindow = new WaresWindow();
            this.WaresWindow.Owner = this.MainWindow;
            this.WaresWindow.DataContext = this.UIModel;
            this.WaresWindow.CommandBindings.Add(new CommandBinding(X4Commands.OnWaresWindowCellRightClick, this.ExecuteOnWaresWindowCellRightClick));
            this.WaresWindow.CommandBindings.Add(new CommandBinding(X4Commands.RecalculatePriceCommand, this.ExecuteRecalculatePriceCommand));

            ModManager.Owner = this.MainWindow;
            WaresWindow.Owner = this.MainWindow;
        }

        private void ExecuteCloseModPathManager(object sender, ExecutedRoutedEventArgs e)
        {
            m_ReadWriteConfig.SaveConfig();
            ModManager.Hide();
        }

        private void ExecuteOpenModPathManager(object sender, ExecutedRoutedEventArgs e)
        {   
            ModManager.Show();
        }

        private void ExecuteShowHelp(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show
                (
                "Instructions for Use Version 1.00\r\r"
                + "Before you start!\r"
                + "ALWAYS backup your mods you use with this tool!\r"
                + "Before you can use this tool you must have extracted all X4 data into a separate folder. You can use the 'X Rebirth Catalog Tool 1.10' to do this.\r"
                + "The file will create diff files of all the values you have changed regarding to the read vanilla and mod files. As long as you dont click on export, nothing will be changed/written.\r\r"
                + "1. Enter the path to the exported X4 data folder into the vanilla folder path (orange) and click on 'Read Vanilla'.\r\r"
                + "2. You can read up to two additional mods. The base mod can be a mod on which the active mod depends. It can only be read from.\r"
                + "The second mod can be your active mod you are working on. Its path can be the same as the export path.\r\r"
                + "3. The order in which you read the mods is important! Always read vanilla first, then the order in which the mods will be loaded in game. That means the base mod before the active mod.\r\r\r"
                + "Mass editing:\r\r"
                + "- Mass editing works only on editable cells and only if you have selected only cells in one window.\r\r"
                + "Filters:\r\r"
                + "- You can filter all items by using the checkboxes or entering words into the seach box. Blanks will work as 'logical or' if you want to search for 'laser' and 'plasma' simply type 'laser plasma'.\r"
                + "- Hit the 'Filter' button or enter 'Return' in the search box to trigger the filter.\r"
                + "- Wares will automatically be filtered depending on the items you have filtered on the main window, except if you have checked 'All Wares'. Then the filter will parse over all existing wares.\r\r"
                + "Wares:\r\r"
                + "- Show Wares opens a separate window showing all wares you have filtered.\r"
                + "- The calculated price is based on the average costs of its components.\r\r"
                + "Export:\r\r"
                + "- Select an export path for your mod. By clicking on 'Export Changes' all necessary and changed files will be written into that folder. Keep in mind, that if your output path is one of the input paths, you might destroy your mod if you dont know exactly what you are doing!\r\r"
                + "Features:\r\r"
                + "- DPS values and effective ranges will automatically calculated.\r"
                + "- If available, the names of all items will be shown additionally to the IDs (file 't/0001.xml' is needed).\r"
                + "- Right Mouse button on a row will open the associated xml file. Priority is 'Active Mod', 'Base Mod', 'Vanilla'. So if 'Active Mod' has changed the file last this file will be opened.\r"
                + "- Double Click on either a Weapon/Turret row or a 'Projectile' row will filter the associated projectile or turret/weapon.\r\r"
                + "The author of this tool (Pascal Wauer) is not responsible for any damage you do to your files. Use it on your own risk."
                );
        }

        private void ExecuteOnWeaponDoubleClick(object sender, ExecutedRoutedEventArgs e)
        {

            {
                DataGridCellInfo dataGridCell = this.MainWindow.DG_Weapons.SelectedCells[0];
                var item = dataGridCell.Item as UIModelWeapon;
                if (this.MainWindow.DG_Weapons.SelectedCells.Count == 1 && this.UIModel.DoubleClickedWeapon != item.Projectile)
                {
                    this.UIModel.DoubleClickedWeapon = item.Projectile;
                    this.MainWindow.DG_Weapons.SelectedCells.Clear();
                    this.UIModel.SetProjectileFilter(item.Projectile);
                }
                else
                {
                    this.UIModel.SetProjectileFilter("");
                }
            }
        }
        private void ExecuteOnProjectileDoubleClick(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.MainWindow.DG_Projectiles.SelectedCells.Count == 1)
            {
                DataGridCellInfo dataGridCell = this.MainWindow.DG_Projectiles.SelectedCells[0];
                var item = dataGridCell.Item as UIModelProjectile;
                this.MainWindow.DG_Projectiles.SelectedCells.Clear();
                this.UIModel.SetWeaponsFilter(item.Name);
            }
        }
        private void OnClosing(object sender, CancelEventArgs e)
        {
            m_ReadWriteConfig.SaveConfig();
        }
        private void CanExecuteCalculate(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.UIModel.MathParameter.ToString().Contains(",") && this.UIModel.MathParameter.ToString().Split(',')[1].Length > 2)
            {
                this.UIModel.MathParameter = 0;
            }

            string s1 = String.Format(CultureInfo.InvariantCulture, "{0:0.00}", this.UIModel.MathParameter);
            string s2 = String.Format(CultureInfo.InvariantCulture, "{0:0,000.00}", this.UIModel.MathParameter);
            if (this.MainWindow.TextBoxMathParameter.Text == s1 || this.MainWindow.TextBoxMathParameter.Text == s2)
                {
                    e.Handled = true;
                    e.CanExecute = true;
                }
        }
        private void ExecuteSelectFolderCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the extracted folder containing the X4 folders.";
                fbd.ShowNewFolderButton = false;

                if (Directory.Exists(this.UIModel.Path))
                {
                    fbd.SelectedPath = this.UIModel.Path;
                }
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.UIModel.Path = fbd.SelectedPath;
                    m_ReadWriteConfig.SaveConfig();
                }
            }
        }
        private void ExecuteSelectExportFolderCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the folder of the mod you want to import.";
                fbd.ShowNewFolderButton = false;

                if (Directory.Exists(this.UIModel.Path))
                {
                    fbd.SelectedPath = this.UIModel.Path;
                }
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.UIModel.ExportPath = fbd.SelectedPath;
                    m_ReadWriteConfig.SaveConfig();
                }
            }
        }

        private string GetModFolder(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                return di.FullName;
            }
            else return String.Empty;
        }
        private void ExecuteSelectMod1FolderCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the extracted folder containing the X4 folders.";
                fbd.ShowNewFolderButton = false;

                if (Directory.Exists(GetModFolder(this.UIModel.ModPath1)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.ModPath1);
                }
                else if (Directory.Exists(GetModFolder(this.UIModel.Path)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.Path);
                }
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.UIModel.ModPath1 = fbd.SelectedPath;
                    m_ReadWriteConfig.SaveConfig();
                }
            }
        }
        private void ExecuteSelectMod2FolderCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the extracted folder containing the mod.";
                fbd.ShowNewFolderButton = false;

                if (Directory.Exists(GetModFolder(this.UIModel.ModPath2)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.ModPath2);
                }
                else if (Directory.Exists(GetModFolder(this.UIModel.ModPath1)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.ModPath1);
                }
                else if (Directory.Exists(GetModFolder(this.UIModel.Path)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.Path);
                }
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.UIModel.ModPath2 = fbd.SelectedPath;
                    m_ReadWriteConfig.SaveConfig();
                }
            }
        }
        private void ExecuteSelectMod3FolderCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the extracted folder containing the mod.";
                fbd.ShowNewFolderButton = false;

                if (Directory.Exists(GetModFolder(this.UIModel.ModPath3)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.ModPath3);
                }
                else if (Directory.Exists(GetModFolder(this.UIModel.ModPath2)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.ModPath2);
                }
                else if (Directory.Exists(GetModFolder(this.UIModel.Path)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.Path);
                }
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.UIModel.ModPath3 = fbd.SelectedPath;
                    m_ReadWriteConfig.SaveConfig();
                }
            }
        }
        private void ExecuteSelectMod4FolderCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the extracted folder containing the mod.";
                fbd.ShowNewFolderButton = false;

                if (Directory.Exists(GetModFolder(this.UIModel.ModPath4)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.ModPath4);
                }
                else if (Directory.Exists(GetModFolder(this.UIModel.ModPath3)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.ModPath3);
                }
                else if (Directory.Exists(GetModFolder(this.UIModel.Path)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.Path);
                }
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.UIModel.ModPath4 = fbd.SelectedPath;
                    m_ReadWriteConfig.SaveConfig();
                }
            }
        }
        private void ExecuteSelectMod5FolderCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the extracted folder containing the mod.";
                fbd.ShowNewFolderButton = false;

                if (Directory.Exists(GetModFolder(this.UIModel.ModPath5)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.ModPath5);
                }
                else if (Directory.Exists(GetModFolder(this.UIModel.ModPath4)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.ModPath4);
                }
                else if (Directory.Exists(GetModFolder(this.UIModel.Path)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.Path);
                }
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.UIModel.ModPath5 = fbd.SelectedPath;
                    m_ReadWriteConfig.SaveConfig();
                }
            }
        }
        private void ExecuteSelectMod6FolderCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the extracted folder containing the mod.";
                fbd.ShowNewFolderButton = false;

                if (Directory.Exists(GetModFolder(this.UIModel.ModPath6)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.ModPath6);
                }
                else if (Directory.Exists(GetModFolder(this.UIModel.ModPath5)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.ModPath5);
                }
                else if (Directory.Exists(GetModFolder(this.UIModel.Path)))
                {
                    fbd.SelectedPath = GetModFolder(this.UIModel.Path);
                }
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.UIModel.ModPath6 = fbd.SelectedPath;
                    m_ReadWriteConfig.SaveConfig();
                }
            }
        }
        private void ExecuteRecalculatePriceCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.UIModel.CalculateWarePrices();
        }
        private void ExecutePackAllFilesCommand(object sender, ExecutedRoutedEventArgs e)
        {
        }
        private void ExecuteOnMainWindowCellRightClick(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.MainWindow.DG_Engines.SelectedCells.Count == 1)
            {
                DataGridCellInfo dataGridCell = this.MainWindow.DG_Engines.SelectedCells[0];
                var item = dataGridCell.Item as UIModelEngine;
                Process.Start(item.File);
            }
            if (this.MainWindow.DG_Missiles.SelectedCells.Count == 1)
            {
                DataGridCellInfo dataGridCell = this.MainWindow.DG_Missiles.SelectedCells[0];
                var item = dataGridCell.Item as UIModelMissile;
                Process.Start(item.File);
            }
            if (this.MainWindow.DG_Shields.SelectedCells.Count == 1)
            {
                DataGridCellInfo dataGridCell = this.MainWindow.DG_Shields.SelectedCells[0];
                var item = dataGridCell.Item as UIModelShield;
                Process.Start(item.File);
            }
            if (this.MainWindow.DG_Ships.SelectedCells.Count == 1)
            {
                DataGridCellInfo dataGridCell = this.MainWindow.DG_Ships.SelectedCells[0];
                var item = dataGridCell.Item as UIModelShip;
                Process.Start(item.File);
            }
            if (this.MainWindow.DG_Projectiles.SelectedCells.Count == 1)
            {
                DataGridCellInfo dataGridCell = this.MainWindow.DG_Projectiles.SelectedCells[0];
                var item = dataGridCell.Item as UIModelProjectile;
                Process.Start(item.File);
            }
            if (this.MainWindow.DG_Weapons.SelectedCells.Count == 1)
            {
                DataGridCellInfo dataGridCell = this.MainWindow.DG_Weapons.SelectedCells[0];
                var item = dataGridCell.Item as UIModelWeapon;
                Process.Start(item.File);
            }
        }
        private void ExecuteOnWaresWindowCellRightClick(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.WaresWindow.DG_Wares.SelectedCells.Count == 1)
            {
                DataGridCellInfo dataGridCell = this.WaresWindow.DG_Wares.SelectedCells[0];
                var item = dataGridCell.Item as UIModelWare;
                Process.Start(item.File);
            }
        }
        private void ExecuteShowWaresWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            //if (!WaresWindow.IsVisible)
            //{
            //    this.WaresWindow = new WaresWindow();
            //    this.WaresWindow.Owner =this.MainWindow;
            //    this.WaresWindow.DataContext = this.UIModel;
            //    this.WaresWindow.CommandBindings.Add(new CommandBinding(X4Commands.OnWaresWindowCellRightClick, this.ExecuteOnWaresWindowCellRightClick));
            //    this.WaresWindow.Show();
            //}
            this.WaresWindow.Show();
        }
        private void ExecuteFilterCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MainWindow.DG_Engines.CancelEdit();
            MainWindow.DG_Missiles.CancelEdit();
            MainWindow.DG_Projectiles.CancelEdit();
            MainWindow.DG_Shields.CancelEdit();
            MainWindow.DG_Ships.CancelEdit();
            MainWindow.DG_Weapons.CancelEdit();
            WaresWindow.DG_Wares.CommitEdit();
            WaresWindow.DG_Wares.CancelEdit();

            this.UIModel.SetFilters();
        }

        #region calculations

        /// <summary>
        /// Calculation function
        /// </summary>
        /// <param name="operation">1 = addition, 2 = multiplication, 3 = substraction, 4 = set fixed value</param>
        /// <param name="param1">unit value</param>

        private void ExecuteAddToValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.UIModel.MathParameter == 0)
            {
                MessageBox.Show("Please enter a value <> '0'", "Invalid operation");
                return;
            }
            this.AddCalculation(1);
        }
        private void ExecuteMultiplyToValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            AddCalculation(2);
        }
        private void ExecuteSubstractFromValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.UIModel.MathParameter == 0)
            {
                MessageBox.Show("Please enter a value <> '0'", "Invalid operation");
                return;
            }
            this.AddCalculation(3);
        }
        private void ExecuteSetFixedValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            AddCalculation(4);
        }
        private void ExecuteDivideByValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.UIModel.MathParameter == 0 || this.UIModel.MathParameter < 0)
            {
                MessageBox.Show("Please enter a value > '0'", "Invalid operation");
                return;
            }

            AddCalculation(5);
        }
        /// <summary>
        /// Checks if the user has selected cells in main window and wares window
        /// </summary>
        /// <returns></returns>
        private bool OnlyOneWindowCellsSelected()
        {
            DataGrid dg_Shields = null;
            dg_Shields = this.MainWindow.DG_Shields;
            DataGrid dg_Engines = null;
            dg_Engines = this.MainWindow.DG_Engines;
            DataGrid dg_Projectiles = null;
            dg_Projectiles = this.MainWindow.DG_Projectiles;
            DataGrid dg_Missiles = null;
            dg_Missiles = this.MainWindow.DG_Missiles;
            DataGrid dg_Ships = null;
            dg_Ships = this.MainWindow.DG_Ships;
            DataGrid dg_Wares = null;
            dg_Wares = this.WaresWindow.DG_Wares;
            DataGrid dg_Weapons = null;
            dg_Weapons = this.MainWindow.DG_Weapons;

            if ((dg_Shields.SelectedCells.Count > 0 || dg_Engines.SelectedCells.Count > 0 || dg_Projectiles.SelectedCells.Count > 0 || dg_Missiles.SelectedCells.Count > 0 || dg_Ships.SelectedCells.Count > 0 || dg_Weapons.SelectedCells.Count > 0) && dg_Wares.SelectedCells.Count > 0)
            {
                MessageBox.Show("You must not select cells in Main window and Wares window at the same time for mass editing!\r\r" +
                    "Deselect all cells of one window first (you can use ctr + left click).", "Calculation not possible", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void AddCalculation(int operation)
        {
            if (!OnlyOneWindowCellsSelected())
                return;

            Counter counter = new Counter();

            DataGrid dg_Shields = null;
            dg_Shields = this.MainWindow.DG_Shields;

            if (dg_Shields.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Shields, operation, counter);
            }

            DataGrid dg_Engines = null;
            dg_Engines = this.MainWindow.DG_Engines;

            if (dg_Engines.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Engines, operation, counter);
            }

            DataGrid dg_Weapons = null;
            dg_Weapons = this.MainWindow.DG_Weapons;

            if (dg_Weapons.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Weapons, operation, counter);
            }

            DataGrid dg_Projectiles = null;
            dg_Projectiles = this.MainWindow.DG_Projectiles;

            if (dg_Projectiles.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Projectiles, operation, counter);
            }

            DataGrid dg_Missiles = null;
            dg_Missiles = this.MainWindow.DG_Missiles;

            if (dg_Missiles.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Missiles, operation, counter);
            }

            DataGrid dg_Ships = null;
            dg_Ships = this.MainWindow.DG_Ships;

            if (dg_Ships.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Ships, operation, counter);
            }

            DataGrid dg_Wares = null;
            dg_Wares = this.WaresWindow.DG_Wares;

            if (dg_Wares.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Wares, operation, counter);
            }

        }

        #endregion

        private void ExecuteWriteAllChangedFilesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!Directory.Exists(this.UIModel.ExportPath))
            {
                MessageBox.Show("Enter a valid export folder", "No valid export folder", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            DialogResult result = (DialogResult)MessageBox.Show("Directory found. Do you want to write the mod files? Existing files in the export path will be overwritten.\r\r\rBe sure to have a backup of your mod files.", "Write mod files to export path", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == DialogResult.Yes)
            {
                m_XmlWriter.WriteAllChanges();
            }
        }
        private List<FileInfo> GetAllXmlInSubFolders(string folderPath, List<FileInfo> xmlList)
        {
            if (Directory.Exists(folderPath))
            {
                DirectoryInfo di = new DirectoryInfo(folderPath);

                List<FileInfo> tmpXmlList = new List<FileInfo>();
                tmpXmlList.AddRange(di.GetFiles("*.xml", SearchOption.AllDirectories));
                xmlList.AddRange(tmpXmlList.Where(x => x.FullName.Contains("macros")));
                //DirectoryInfo[] folders = di.GetDirectories();

                //foreach(var item in folders)
                //{
                //        xmlList = GetAllXmlInSubFolders(item.FullName, xmlList);
                //}
            }
            return xmlList;
        }
        private void ExecuteReadAllVanillaFilesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            string folderPath = this.UIModel.Path.Replace(@"\\", @"\");
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("Enter a valid folder path for extracted vanilla files", "No valid folder", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            this.UIModel.UIModelProjectiles.Clear();
            this.UIModel.UIModelMissiles.Clear();
            this.UIModel.UIModelEngines.Clear();
            this.UIModel.UIModelShields.Clear();
            this.UIModel.UIModelWares.Clear();
            this.UIModel.UIModelShips.Clear();
            this.UIModel.UIModelWeapons.Clear();
            
            this.UIModel.AllWaresLoaded = false;

            
            string waresPath = folderPath + PathToWares;

            string weaponsPath = folderPath + PathToProjectiles;
            List<FileInfo> xmlWeaponsList = new List<FileInfo>();
            xmlWeaponsList = this.GetAllXmlInSubFolders(weaponsPath, xmlWeaponsList);
            this.ReadAllProjectiles(xmlWeaponsList);

            string turretsPath = folderPath + PathToTurretsStandard;
            List<FileInfo> xmlTurretsList = new List<FileInfo>();
            xmlTurretsList = this.GetAllXmlInSubFolders(folderPath + PathToTurretsStandard, xmlTurretsList);
            xmlTurretsList = this.GetAllXmlInSubFolders(folderPath + PathToTurretsEnergy, xmlTurretsList);
            xmlTurretsList = this.GetAllXmlInSubFolders(folderPath + PathToTurretsHeavy, xmlTurretsList);
            xmlTurretsList = this.GetAllXmlInSubFolders(folderPath + PathToTurretsCapital, xmlTurretsList);
            xmlTurretsList = this.GetAllXmlInSubFolders(folderPath + PathToTurretsGuided, xmlTurretsList);
            xmlTurretsList = this.GetAllXmlInSubFolders(folderPath + PathToTurretsDumbfire, xmlTurretsList);
            this.ReadAllWeapons(xmlTurretsList);

            string missilesPath = folderPath + PathToMissiles;
            //string torpedosPath = folderPath + @"\props\WeaponSystems\torpedo\macros";
            List<FileInfo> xmlMissilesList = new List<FileInfo>();
            xmlMissilesList = this.GetAllXmlInSubFolders(missilesPath, xmlMissilesList);

            this.ReadAllMissiles(xmlMissilesList);

            string shieldsPath = folderPath + PathToShields;
            List<FileInfo> xmlShieldsList = new List<FileInfo>();
            xmlShieldsList = this.GetAllXmlInSubFolders(shieldsPath, xmlShieldsList);
            this.ReadAllShields(xmlShieldsList);

            string enginesPath = folderPath + PathToEngines;
            List<FileInfo> xmlEnginesList = new List<FileInfo>();
            xmlEnginesList = this.GetAllXmlInSubFolders(enginesPath, xmlEnginesList);
            this.ReadAllEngines(xmlEnginesList);

            string shipsPath = folderPath + PathToShips;
            List<FileInfo> xmlShipsList = new List<FileInfo>();
            xmlShipsList = this.GetAllXmlInSubFolders(shipsPath, xmlShipsList);
            this.ReadAllShips(xmlShipsList);

            if (File.Exists(waresPath))
                m_XmlExtractor.ReadAllWares(waresPath);
            else
                MessageBox.Show("No valid wares found.", "No data found.");
        }
        public void ReadAllEngines(List<FileInfo> xmlEnginesList)
        {
            string folderPath = this.UIModel.Path.Replace(@"\\", @"\");
            if (!Directory.Exists(folderPath + this.PathToEngines))
            {
                MessageBox.Show("No valid engines found.", "No data found.");
            }
            else
            {
                foreach (var item in xmlEnginesList)
                {
                    UIModelEngine engine = m_XmlExtractor.ReadSingleEngineFile(item);
                    if (engine != null && engine.Name.Length > 1)
                        this.UIModel.UIModelEngines.Add(engine);
                }
                this.UIModel.UIModelEnginesVanilla.Clear();
                foreach (var item in this.UIModel.UIModelEngines)
                {
                    this.UIModel.UIModelEnginesVanilla.Add(item.Copy());
                }
            }
        }
        private void ReadAllShields(List<FileInfo> xmlShieldsList)
        {
            string folderPath = this.UIModel.Path.Replace(@"\\", @"\");
            if (!Directory.Exists(folderPath + PathToShields))
            {
                MessageBox.Show("No valid shields found.", "No data found.");
            }
            else
            {
                this.UIModel.UIModelShieldsVanilla.Clear();

                foreach (var item in xmlShieldsList)
                {
                    UIModelShield shield = m_XmlExtractor.ReadSingleShield(item);
                    if (shield != null &&shield.Name.Length > 1)
                        this.UIModel.UIModelShields.Add(shield);
                }
                foreach (var item in this.UIModel.UIModelShields)
                {
                    this.UIModel.UIModelShieldsVanilla.Add(item.Copy());
                }
            }
        }
        private void ReadAllWeapons(List<FileInfo> xmlWeaponsList)
        {
            string folderPath = this.UIModel.Path.Replace(@"\\", @"\");
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("No valid folder found.", "No data found.");
            }
            
            foreach (var item in xmlWeaponsList)
            {
                this.UIModel.UIModelWeapons.Add(m_XmlExtractor.ReadSingleWeapon(item));
            }

            this.UIModel.UIModelWeaponsVanilla.Clear();
            foreach (var item in this.UIModel.UIModelWeapons)
            {
                this.UIModel.UIModelWeaponsVanilla.Add(item.Copy());
            }
        }
        public void ReadAllProjectiles(List<FileInfo> xmlProjectilesList)
        {
            string folderPath = this.UIModel.Path.Replace(@"\\", @"\");
            if (!Directory.Exists(folderPath + this.PathToProjectiles))
            {
                MessageBox.Show("No valid weapons found.", "No data found.");
            }

            foreach (var item in xmlProjectilesList)
            {
                UIModelProjectile projectile = m_XmlExtractor.ReadSingleProjectile(item);
                if (projectile != null && projectile.Name.Length > 1)
                    this.UIModel.UIModelProjectiles.Add(projectile);
            }
            this.UIModel.UIModelProjectilesVanilla.Clear();
            foreach (var item in this.UIModel.UIModelProjectiles)
            {
                this.UIModel.UIModelProjectilesVanilla.Add(item.Copy());
            }
        }
        public void ReadAllMissiles(List<FileInfo> xmlMissilesList)
        {
            foreach (var item in xmlMissilesList)
            {
                UIModelMissile missile = m_XmlExtractor.ReadSingleMissile(item);
                if (missile != null && missile.Name.Length > 1)
                    this.UIModel.UIModelMissiles.Add(missile);
            }
            this.UIModel.UIModelMissilesVanilla.Clear();
            foreach (var item in this.UIModel.UIModelMissiles)
            {
                this.UIModel.UIModelMissilesVanilla.Add(item.Copy());
            }
        }
        private void ReadAllShips(List<FileInfo> xmlShipsList)
        {
            string folderPath = this.UIModel.Path.Replace(@"\\", @"\");
            if (!Directory.Exists(folderPath + PathToShips))
            {
                MessageBox.Show("No valid ships found.", "No data found.");
            }
            else
            {
                this.UIModel.UIModelShipsVanilla.Clear();
                foreach (var item in xmlShipsList)
                {
                    UIModelShip ship = m_XmlExtractor.ReadSingleShipFile(item);
                    if (ship != null && ship.Name.Length > 1 && ship.Class != "storage" && ship.Class != "cockpit")
                        this.UIModel.UIModelShips.Add(ship);
                }
                foreach (var item in this.UIModel.UIModelShips)
                {
                    this.UIModel.UIModelShipsVanilla.Add(item.Copy());
                }
            }
        }

        /// <summary>
        /// Updates all entities or creates new entities
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteReadAllModFilesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; 
            if (Directory.Exists(UIModel.Path))
            {
                VanillaXmlFiles.Clear();
                VanillaXmlFiles.AddRange(Directory.GetFiles(UIModel.Path, "*.xml", SearchOption.AllDirectories));
                m_ModFilesReader.ReadAllModFilesFromFolder(this.UIModel.Path);
            }
            this.ExecuteReadAllVanillaFilesCommand(null, null);
            if (Directory.Exists(UIModel.ModPath1))
            {
                Mod1XmlFiles.Clear();
                Mod1XmlFiles.AddRange(Directory.GetFiles(UIModel.ModPath1, "*.xml", SearchOption.AllDirectories));
                m_ModFilesReader.ReadAllModFilesFromFolder(this.UIModel.ModPath1);
            }
            if (Directory.Exists(UIModel.ModPath2))
            {
                Mod2XmlFiles.Clear();
                Mod2XmlFiles.AddRange(Directory.GetFiles(UIModel.ModPath2, "*.xml", SearchOption.AllDirectories));
                m_ModFilesReader.ReadAllModFilesFromFolder(this.UIModel.ModPath2);
            }
            if (Directory.Exists(UIModel.ModPath3))
            {
                Mod3XmlFiles.Clear();
                Mod3XmlFiles.AddRange(Directory.GetFiles(UIModel.ModPath3, "*.xml", SearchOption.AllDirectories));
                m_ModFilesReader.ReadAllModFilesFromFolder(this.UIModel.ModPath3);
            }
            if (Directory.Exists(UIModel.ModPath4))
            {
                Mod4XmlFiles.Clear();
                Mod4XmlFiles.AddRange(Directory.GetFiles(UIModel.ModPath4, "*.xml", SearchOption.AllDirectories));
                m_ModFilesReader.ReadAllModFilesFromFolder(this.UIModel.ModPath4);
            }
            if (Directory.Exists(UIModel.ModPath5))
            {
                Mod5XmlFiles.Clear();
                Mod5XmlFiles.AddRange(Directory.GetFiles(UIModel.ModPath5, "*.xml", SearchOption.AllDirectories));
                m_ModFilesReader.ReadAllModFilesFromFolder(this.UIModel.ModPath5);
            }
            if (Directory.Exists(UIModel.ModPath6))
            {
                Mod6XmlFiles.Clear();
                Mod6XmlFiles.AddRange(Directory.GetFiles(UIModel.ModPath6, "*.xml", SearchOption.AllDirectories));
                m_ModFilesReader.ReadAllModFilesFromFolder(this.UIModel.ModPath6);
            }
            this.UIModel.AllWaresLoaded = true;
            this.UIModel.CalculateWarePrices();
            Mouse.OverrideCursor = null;
        }
    }
}
