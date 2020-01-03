using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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

        private ModFilesReader m_ModFilesReader;

        public Dictionary<string, string> TextDictionary = new Dictionary<string, string>();
        public MainWindow MainWindow { get; set; }
        public WaresWindow WaresWindow { get; set; }
        public UIModel UIModel { get; set; }
        public XmlExtractor m_XmlExtractor;
        public XmlWriter m_XmlWriter;
        public ReadWriteConfig m_ReadWriteConfig;
        public Calculations m_Calculations;

        public UIManager()
        {
            MainWindow = new MainWindow();
            WaresWindow = new WaresWindow();
            m_XmlExtractor = new XmlExtractor(this);
            m_XmlWriter = new XmlWriter(this);
            m_ReadWriteConfig = new ReadWriteConfig(this);
            m_Calculations = new Calculations(this);
            this.UIModel = new UIModel();
            MainWindow.DataContext = this.UIModel;
            WaresWindow.DataContext = this.UIModel;
            m_ModFilesReader = new ModFilesReader(this, m_XmlExtractor);
            MainWindow.Closing += OnClosing;
            
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.ReadAllVanillaFilesCommand, this.ExecuteReadAllVanillaFilesCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.ReadAllMod1FilesCommand, this.ExecuteReadAllMod1FilesCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.ReadAllMod2FilesCommand, this.ExecuteReadAllMod2FilesCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.FilterCommand, this.ExecuteFilterCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.WriteAllChangedFilesCommand, this.ExecuteWriteAllChangedFilesCommand));
            //MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.PackAllFilesCommand, this.ExecutePackAllFilesCommand));
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

            WaresWindow.CommandBindings.Add(new CommandBinding(X4Commands.OnWaresWindowCellRightClick, this.ExecuteOnWaresWindowCellRightClick));
            m_ReadWriteConfig.LoadConfig();
            this.TextDictionary = new Dictionary<string, string>();
            this.TextDictionary = m_XmlExtractor.ReadTextXml(this.UIModel.Path + this.PathToTexts + @"\0001-l044.xml", this.TextDictionary);
            MainWindow.Show();
            WaresWindow.Owner = this.MainWindow;
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
        private void ExecuteSelectMod1FolderCommand(object sender, ExecutedRoutedEventArgs e)
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
                    this.UIModel.ModPath1 = fbd.SelectedPath;
                    m_ReadWriteConfig.SaveConfig();
                }
            }
        }
        private void ExecuteSelectMod2FolderCommand(object sender, ExecutedRoutedEventArgs e)
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
                    this.UIModel.ModPath2 = fbd.SelectedPath;
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
            if (!WaresWindow.IsVisible)
            {
                this.WaresWindow = new WaresWindow();
                this.WaresWindow.Owner =this.MainWindow;
                this.WaresWindow.DataContext = this.UIModel;
                this.WaresWindow.CommandBindings.Add(new CommandBinding(X4Commands.OnWaresWindowCellRightClick, this.ExecuteOnWaresWindowCellRightClick));
                this.WaresWindow.Show();
            }
        }
        private void ExecuteFilterCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.UIModel.SetFilters();
        }

        #region calculations

        private void ExecuteSubstractFromValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.UIModel.MathParameter == 0)
            {
                MessageBox.Show("Please enter a value <> '0'", "Invalid operation");
                return;
            }
            this.AddCalculation(3);
        }

        private void ExecuteAddToValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.UIModel.MathParameter == 0)
            {
                MessageBox.Show("Please enter a value <> '0'", "Invalid operation");
                return;
            }
            this.AddCalculation(1);
        }
        private void AddCalculation(int operation)
        {
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

        
        /// <summary>
        /// Calculation function
        /// </summary>
        /// <param name="operation">1 = addition, 2 = multiplication, 3 = substraction, 4 = set fixed value</param>
        /// <param name="param1">unit value</param>
        
        private void ExecuteMultiplyToValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Counter counter = new Counter();

            DataGrid dg_Shields = null;
            dg_Shields = this.MainWindow.DG_Shields;

            if (dg_Shields.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Shields, 2, counter);
            }

            DataGrid dg_Engines = null;
            dg_Engines = this.MainWindow.DG_Engines;

            if (dg_Engines.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Engines, 2, counter);
            }

            DataGrid dg_Weapons = null;
            dg_Weapons = this.MainWindow.DG_Weapons;

            if (dg_Weapons.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Weapons, 2, counter);
            }

            DataGrid dg_Projectiles = null;
            dg_Projectiles = this.MainWindow.DG_Projectiles;

            if (dg_Projectiles.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Projectiles, 2, counter);
            }

            DataGrid dg_Missiles = null;
            dg_Missiles = this.MainWindow.DG_Missiles;

            if (dg_Missiles.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Missiles, 2, counter);
            }

            DataGrid dg_Ships = null;
            dg_Ships = this.MainWindow.DG_Ships;

            if (dg_Ships.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Ships, 2, counter);
            }

            DataGrid dg_Wares = null;
            dg_Wares = this.WaresWindow.DG_Wares;

            if (dg_Wares.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Wares, 2, counter);
            }
        }

        private void ExecuteDivideByValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.UIModel.MathParameter == 0 || this.UIModel.MathParameter < 0)
            {
                MessageBox.Show("Please enter a value > '0'", "Invalid operation");
                return;
            }

            Counter counter = new Counter();

            DataGrid dg_Shields = null;
            dg_Shields = this.MainWindow.DG_Shields;

            if (dg_Shields.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Shields, 5, counter);
            }

            DataGrid dg_Engines = null;
            dg_Engines = this.MainWindow.DG_Engines;

            if (dg_Engines.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Engines, 5, counter);
            }

            DataGrid dg_Weapons = null;
            dg_Weapons = this.MainWindow.DG_Weapons;

            if (dg_Weapons.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Weapons, 5, counter);
            }

            DataGrid dg_Projectiles = null;
            dg_Projectiles = this.MainWindow.DG_Projectiles;

            if (dg_Projectiles.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Projectiles, 5, counter);
            }

            DataGrid dg_Missiles = null;
            dg_Missiles = this.MainWindow.DG_Missiles;

            if (dg_Missiles.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Missiles, 5, counter);
            }

            DataGrid dg_Ships = null;
            dg_Ships = this.MainWindow.DG_Ships;

            if (dg_Ships.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Ships, 5, counter);
            }

            DataGrid dg_Wares = null;
            dg_Wares = this.WaresWindow.DG_Wares;

            if (dg_Wares.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Wares, 5, counter);
            }
        }

        private void ExecuteSetFixedValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Counter counter = new Counter();

            DataGrid dg_Shields = null;
            dg_Shields = this.MainWindow.DG_Shields;

            if (dg_Shields.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Shields, 4, counter);
            }

            DataGrid dg_Engines = null;
            dg_Engines = this.MainWindow.DG_Engines;

            if (dg_Engines.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Engines, 4, counter);
            }

            DataGrid dg_Projectiles = null;
            dg_Projectiles = this.MainWindow.DG_Projectiles;

            if (dg_Projectiles.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Projectiles, 4, counter);
            }

            DataGrid dg_Weapons = null;
            dg_Weapons = this.MainWindow.DG_Weapons;

            if (dg_Weapons.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Weapons, 4, counter);
            }

            DataGrid dg_Missiles = null;
            dg_Missiles = this.MainWindow.DG_Missiles;

            if (dg_Missiles.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Missiles, 4, counter);
            }

            DataGrid dg_Ships = null;
            dg_Ships = this.MainWindow.DG_Ships;

            if (dg_Ships.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Ships, 4, counter);
            }

            DataGrid dg_Wares = null;
            dg_Wares = this.WaresWindow.DG_Wares;

            if (dg_Wares.SelectedCells.Count > 0)
            {
                counter = m_Calculations.CalculateOverAll(dg_Wares, 4, counter);
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

                xmlList.AddRange(di.GetFiles("*.xml"));

                DirectoryInfo[] folders = di.GetDirectories();
                foreach(var item in folders)
                {
                    xmlList = GetAllXmlInSubFolders(item.FullName, xmlList);
                }
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
                    if (engine.Name.Length > 1)
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
                    if (shield.Name.Length > 1)
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
                if (projectile.Name.Length > 1)
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
                if (missile.Name.Length > 1)
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
        private void ExecuteReadAllMod1FilesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            m_ModFilesReader.ReadAllModFilesFromFolder(this.UIModel.ModPath1);
        }
        private void ExecuteReadAllMod2FilesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            m_ModFilesReader.ReadAllModFilesFromFolder(this.UIModel.ModPath2);
        }
    }
}
