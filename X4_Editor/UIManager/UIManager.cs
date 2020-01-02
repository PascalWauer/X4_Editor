using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using X4_Editor.Helper;
using DataGrid = System.Windows.Controls.DataGrid;
using DataGridCell = System.Windows.Controls.DataGridCell;
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
        public Calculations m_Calculations;

        public UIManager()
        {
            MainWindow = new MainWindow();
            WaresWindow = new WaresWindow();
            m_XmlExtractor = new XmlExtractor(this);
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
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.SubstractToValueCommand, this.ExecuteSubstractToValueCommand, CanExecuteCalculate));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.MultiplyToValueCommand, this.ExecuteMultiplyToValueCommand, CanExecuteCalculate));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.DivideFromValueCommand, this.ExecuteDivideFromValueCommand, CanExecuteCalculate));
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
            this.LoadConfig();
            this.TextDictionary = new Dictionary<string, string>();
            this.TextDictionary = m_XmlExtractor.ReadTextXml(this.UIModel.Path + this.PathToTexts + @"\0001-l044.xml", this.TextDictionary);
            MainWindow.Show();
            WaresWindow.Owner = this.MainWindow;

            //if (Directory.Exists(this.UIModel.Path))
            //    this.ExecuteReadAllVanillaFilesCommand(this, null);
            //if (Directory.Exists(this.UIModel.ModPath1))
            //    this.ExecuteReadAllMod1FilesCommand(this, null);
            //if (Directory.Exists(this.UIModel.ModPath2))
            //    this.ExecuteReadAllMod2FilesCommand(this, null);
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
            this.SaveConfig();
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
                    this.SaveConfig();
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
                    this.SaveConfig();
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
                    this.SaveConfig();
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
                    this.SaveConfig();
                }
            }
        }

        private void SaveConfig()
        {
            string path = Environment.CurrentDirectory;

            using (StreamWriter sw = new StreamWriter(path + "\\X4_Editor.cfg"))
            {
                //if (!Directory.Exists(this.UIModel.Path))
                //{
                //    MessageBox.Show("You need to enter at least the vanilla folder path...", "No valid folder");
                //    return;
                //}
                if (Directory.Exists(this.UIModel.Path))
                    sw.WriteLine("Vanilla X4 Path: " + this.UIModel.Path);
                else
                    sw.WriteLine("Vanilla X4 Path: ");
                if (Directory.Exists(this.UIModel.ModPath1))
                    sw.WriteLine("Mod 1 Path: " + this.UIModel.ModPath1);
                else
                    sw.WriteLine("Mod 1 Path: ");
                if (Directory.Exists(this.UIModel.ModPath2))
                    sw.WriteLine("Mod 2 Path: " + this.UIModel.ModPath2);
                else
                    sw.WriteLine("Mod 2 Path: ");
                if (Directory.Exists(this.UIModel.ExportPath))
                    sw.WriteLine("Mod export Path: " + this.UIModel.ExportPath);
                else
                    sw.WriteLine("Mod export Path: ");


                // filters
                if (this.UIModel.Ships)
                    sw.WriteLine("Ships = 1");
                else
                    sw.WriteLine("Ships = 0");
                if (this.UIModel.Shields)
                    sw.WriteLine("Shields = 1");
                else
                    sw.WriteLine("Shields = 0");
                if (this.UIModel.Engines)
                    sw.WriteLine("Engines = 1");
                else
                    sw.WriteLine("Engines = 0");
                if (this.UIModel.Weapons)
                    sw.WriteLine("Weapons = 1");
                else
                    sw.WriteLine("Weapons = 0");
                if (this.UIModel.Wares)
                    sw.WriteLine("Wares = 1");
                else
                    sw.WriteLine("Wares = 0");
                if (this.UIModel.Size_S)
                    sw.WriteLine("Size_S = 1");
                else
                    sw.WriteLine("Size_S = 0");
                if (this.UIModel.Size_M)
                    sw.WriteLine("Size_M = 1");
                else
                    sw.WriteLine("Size_M = 0");
                if (this.UIModel.Size_L)
                    sw.WriteLine("Size_L = 1");
                else
                    sw.WriteLine("Size_L = 0");
                if (this.UIModel.Size_XL)
                    sw.WriteLine("Size_XL = 1");
                else
                    sw.WriteLine("Size_XL = 0");
                if (this.UIModel.Size_Other)
                    sw.WriteLine("Size_Other = 1");
                else
                    sw.WriteLine("Size_Other = 0");
            }
        }

        private void LoadConfig()
        {
            string path = Environment.CurrentDirectory;

            List<string> config = new List<string>();
            if (File.Exists(path + "\\X4_Editor.cfg"))
            {
                using (StreamReader sr = new StreamReader(path + "\\X4_Editor.cfg"))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line.Contains("Vanilla X4 Path: "))
                        {
                            line = line.Replace("Vanilla X4 Path: ", "");
                            if (line.Length > 0)
                                this.UIModel.Path = line;
                        }
                        else if (line.Contains("Mod 1 Path: "))
                        {
                            line = line.Replace("Mod 1 Path: ", "");
                            if (line.Length > 0)
                                this.UIModel.ModPath1 = line;
                        }
                        else if (line.Contains("Mod 2 Path: "))
                        {
                            line = line.Replace("Mod 2 Path: ", "");
                            if (line.Length > 0)
                                this.UIModel.ModPath2 = line;
                        }
                        else if (line.Contains("Mod export Path: "))
                        {
                            line = line.Replace("Mod export Path: ", "");
                            if (line.Length > 0)
                                this.UIModel.ExportPath = line;
                        }

                        // filters
                        if (line.Contains("Ships = 1"))
                            this.UIModel.Ships = true;
                        if (line.Contains("Ships = 0"))
                            this.UIModel.Ships = false;
                        if (line.Contains("Shields = 1"))
                            this.UIModel.Shields = true;
                        if (line.Contains("Shields = 0"))
                            this.UIModel.Shields = false;
                        if (line.Contains("Engines = 1"))
                            this.UIModel.Engines = true;
                        if (line.Contains("Engines = 0"))
                            this.UIModel.Engines = false;
                        if (line.Contains("Weapons = 1"))
                            this.UIModel.Weapons = true;
                        if (line.Contains("Weapons = 0"))
                            this.UIModel.Weapons = false;
                        if (line.Contains("Wares = 1"))
                            this.UIModel.Wares = true;
                        if (line.Contains("Wares = 0"))
                            this.UIModel.Wares = false;
                        if (line.Contains("Size_S = 1"))
                            this.UIModel.Size_S = true;
                        if (line.Contains("Size_S = 0"))
                            this.UIModel.Size_S = false;
                        if (line.Contains("Size_M = 1"))
                            this.UIModel.Size_M = true;
                        if (line.Contains("Size_M = 0"))
                            this.UIModel.Size_M = false;
                        if (line.Contains("Size_L = 1"))
                            this.UIModel.Size_L = true;
                        if (line.Contains("Size_L = 0"))
                            this.UIModel.Size_L = false;
                        if (line.Contains("Size_XL = 1"))
                            this.UIModel.Size_XL = true;
                        if (line.Contains("Size_XL = 0"))
                            this.UIModel.Size_XL = false;
                        if (line.Contains("Size_Other = 1"))
                            this.UIModel.Size_Other = true;
                        if (line.Contains("Size_Other = 0"))
                            this.UIModel.Size_Other = false;
                    }
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

        private void ExecuteSubstractToValueCommand(object sender, ExecutedRoutedEventArgs e)
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

        private void ExecuteDivideFromValueCommand(object sender, ExecutedRoutedEventArgs e)
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
                MessageBox.Show("Enter a valid export folder", "No valid export folder");
            }

            DialogResult result = (DialogResult)MessageBox.Show("Directory found. Do you want to write the mod files? Existing files in the export path will be overwritten.", "Write mod files to export path", System.Windows.MessageBoxButton.YesNo);

            if (result == DialogResult.Yes)
            {
                bool fileswritten = false;

                foreach (var item in this.UIModel.UIModelShields)
                {
                    var vanillaItem = this.UIModel.UIModelShieldsVanilla.Where(x => x.Name == item.Name).FirstOrDefault();

                    if (item.Changed)
                    {
                        if (!Directory.Exists(this.UIModel.ExportPath + PathToShields))
                        {
                            Directory.CreateDirectory(this.UIModel.ExportPath + PathToShields);
                        }

                        string outputPath = this.UIModel.ExportPath + PathToShields + item.File.Split(new[] { "macros" }, StringSplitOptions.None)[1];
                        using (StreamWriter sw = new StreamWriter(outputPath))
                        {
                            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                            sw.WriteLine("<diff> ");
                            if (vanillaItem.Max != item.Max)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/recharge/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Max) + "</replace>");
                            if (vanillaItem.Rate != item.Rate)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/recharge/@rate\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Rate) + "</replace>");
                            if (vanillaItem.Delay != item.Delay)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/recharge/@delay\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Delay) + "</replace>");
                            if (vanillaItem.MaxHull != item.MaxHull)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.MaxHull) + "</replace>");
                            if (vanillaItem.Threshold != item.Threshold)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@threshold\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Threshold) + "</replace>");
                            sw.WriteLine("</diff> ");
                        }
                        fileswritten = true;
                    }
                }

                foreach (var item in this.UIModel.UIModelEngines)
                {
                    var vanillaItem = this.UIModel.UIModelEnginesVanilla.Where(x => x.Name == item.Name).FirstOrDefault();

                    if (item.Changed)
                    {
                        if (!Directory.Exists(this.UIModel.ExportPath + PathToEngines))
                        {
                            Directory.CreateDirectory(this.UIModel.ExportPath + PathToEngines);
                        }

                        string outputPath = this.UIModel.ExportPath + PathToEngines + item.File.Split(new[] { "macros" }, StringSplitOptions.None)[1];
                        using (StreamWriter sw = new StreamWriter(outputPath))
                        {
                            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                            sw.WriteLine("<diff> ");
                            if (vanillaItem.BoostDuration != item.BoostDuration)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/boost/@duration\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.BoostDuration) + "</replace>");
                            if (vanillaItem.BoostAttack != item.BoostAttack)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/boost/@attack\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.BoostAttack) + "</replace>");
                            if (vanillaItem.BoostThrust != item.BoostThrust)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/boost/@thrust\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.BoostThrust) + "</replace>");
                            if (vanillaItem.BoostRelease != item.BoostRelease)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/boost/@release\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.BoostRelease) + "</replace>");
                            if (vanillaItem.TravelAttack != item.TravelAttack)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/travel/@attack\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.TravelAttack) + "</replace>");
                            if (vanillaItem.TravelCharge != item.TravelCharge)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/travel/@charge\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.TravelCharge) + "</replace>");
                            if (vanillaItem.TravelRelease != item.TravelRelease)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/travel/@release\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.TravelRelease) + "</replace>");
                            if (vanillaItem.TravelThrust != item.TravelThrust)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/travel/@thrust\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.TravelThrust) + "</replace>");
                            if (vanillaItem.AngularPitch != item.AngularPitch)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/angular/@pitch\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.AngularPitch) + "</replace>");
                            if (vanillaItem.AngularRoll != item.AngularRoll)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/angular/@roll\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.AngularRoll) + "</replace>");
                            if (vanillaItem.Forward != item.Forward)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/thrust/@forward\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Forward) + "</replace>");
                            if (vanillaItem.Reverse != item.Reverse)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/thrust/@reverse\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Reverse) + "</replace>");
                            if (vanillaItem.Pitch != item.Pitch)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/thrust/@pitch\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Pitch) + "</replace>");
                            if (vanillaItem.Strafe != item.Strafe)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/thrust/@Strafe\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Strafe) + "</replace>");
                            if (vanillaItem.Yaw != item.Yaw)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/thrust/@yaw\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Yaw) + "</replace>");
                            if (vanillaItem.Roll != item.Roll)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/thrust/@roll\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Roll) + "</replace>");
                            if (vanillaItem.MaxHull != item.MaxHull)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.MaxHull) + "</replace>");
                            if (vanillaItem.Threshold != item.Threshold)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@threshold\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Threshold) + "</replace>");
                            sw.WriteLine("</diff> ");

                            fileswritten = true;
                        }
                    }
                }

                foreach (var item in this.UIModel.UIModelShips)
                {
                    var vanillaItem = this.UIModel.UIModelShipsVanilla.Where(x => x.Name == item.Name).FirstOrDefault();

                    string shipClasssFolder = item.File.Split(new[] { "units" }, StringSplitOptions.None)[1].Split(new[] { "macros" }, StringSplitOptions.None)[0];

                    if (!Directory.Exists(this.UIModel.ExportPath + PathToShips + shipClasssFolder + @"\macros"))
                    {
                        Directory.CreateDirectory(this.UIModel.ExportPath + PathToShips + shipClasssFolder + @"\macros");
                    }

                    string outputPath = this.UIModel.ExportPath + PathToShips + shipClasssFolder + "macros" + item.File.Split(new[] { "macros" }, StringSplitOptions.None)[1];

                    if (item.Changed)
                    {
                        using (StreamWriter sw = new StreamWriter(outputPath))
                        {
                            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                            sw.WriteLine("<diff> ");
                            if (vanillaItem == null || vanillaItem.HullMax != item.HullMax)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@max\">" + item.HullMax + "</replace>");
                            if (vanillaItem == null || vanillaItem.ExplosionDamage != item.ExplosionDamage)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/boost/@explosiondamage\">" + item.ExplosionDamage + "</replace>");
                            if (vanillaItem == null || vanillaItem.StorageMissiles != item.StorageMissiles)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/storage/@missile\">" + item.StorageMissiles + "</replace>");
                            if (vanillaItem == null || vanillaItem.StorageUnits != item.StorageUnits)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/storage/@unit\">" + item.StorageUnits + "</replace>");
                            if (vanillaItem == null || vanillaItem.Secrecy != item.Secrecy)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/secrecy/@level\">" + item.Secrecy + "</replace>");
                            if (vanillaItem == null || vanillaItem.People != item.People)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/people/@capacity\">" + item.People + "</replace>");
                            if (vanillaItem == null || vanillaItem.Mass != item.Mass)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/@mass\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Mass) + "</replace>");
                            if (vanillaItem == null || vanillaItem.InertiaPitch != item.InertiaPitch)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@pitch\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaPitch) + "</replace>");
                            if (vanillaItem == null || vanillaItem.InertiaYaw != item.InertiaYaw)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@yaw\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaYaw) + "</replace>");
                            if (vanillaItem == null || vanillaItem.InertiaRoll != item.InertiaRoll)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@roll\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaRoll) + "</replace>");
                            if (vanillaItem == null || vanillaItem.Forward != item.Forward)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@forward\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Forward) + "</replace>");
                            if (vanillaItem == null || vanillaItem.Reverse != item.Reverse)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@reverse\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Reverse) + "</replace>");
                            if (vanillaItem == null || vanillaItem.Horizontal != item.Horizontal)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@horizontal\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Horizontal) + "</replace>");
                            if (vanillaItem == null || vanillaItem.Vertical != item.Vertical)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@vertical\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Vertical) + "</replace>");
                            if (vanillaItem == null || vanillaItem.Pitch != item.Pitch)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@pitch\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Pitch) + "</replace>");
                            if (vanillaItem == null || vanillaItem.Yaw != item.Yaw)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@yaw\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Yaw) + "</replace>");
                            if (vanillaItem == null || vanillaItem.Roll != item.Roll)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@roll\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Roll) + "</replace>");
                            sw.WriteLine("</diff> ");
                        }


                    }
                    if (item.Cargo != null && item.Cargo.Changed)
                    {
                        using (StreamWriter sw = new StreamWriter(outputPath.Replace("ship", "storage")))
                        {
                            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                            sw.WriteLine("<diff> ");
                            if (vanillaItem == null || vanillaItem.Cargo.CargoMax != item.Cargo.CargoMax)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/cargo/@max\">" + item.Cargo.CargoMax + "</replace>");
                            if (vanillaItem == null || vanillaItem.Cargo.CargoTags != item.Cargo.CargoTags)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/cargo/@tags\">" + item.Cargo.CargoTags + "</replace>");
                            sw.WriteLine("</diff> ");

                            fileswritten = true;
                        }
                    }
                }

                foreach (var item in this.UIModel.UIModelProjectiles)
                {
                    var vanillaItem = this.UIModel.UIModelProjectilesVanilla.Where(x => x.Name == item.Name).FirstOrDefault();

                    if (item.Changed)
                    {
                        if (!Directory.Exists(this.UIModel.ExportPath + PathToProjectiles))
                        {
                            Directory.CreateDirectory(this.UIModel.ExportPath + PathToProjectiles);
                        }

                        string outputPath = this.UIModel.ExportPath + PathToProjectiles + item.File.Split(new[] { "macros" }, StringSplitOptions.None)[1];
                        using (StreamWriter sw = new StreamWriter(outputPath))
                        {
                            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                            sw.WriteLine("<diff> ");
                            if (vanillaItem.Speed != item.Speed)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@speed\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Speed) + "</replace>");
                            if (vanillaItem.Lifetime != item.Lifetime)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@lifetime\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Lifetime) + "</replace>");
                            if (vanillaItem.Range != item.Range)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@range\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Range) + "</replace>");
                            if (vanillaItem.Amount != item.Amount)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@amount\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Amount) + "</replace>");
                            if (vanillaItem.BarrelAmount != item.BarrelAmount)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@barrelamount\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.BarrelAmount) + "</replace>");
                            if (vanillaItem.TimeDiff != item.TimeDiff)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@timediff\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.TimeDiff) + "</replace>");
                            if (vanillaItem.Angle != item.Angle)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@angle\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.000}", item.Angle) + "</replace>");
                            if (vanillaItem.MaxHits != item.MaxHits)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@maxhits\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.MaxHits) + "</replace>");
                            if (vanillaItem.Ricochet != item.Ricochet)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@ricochet\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Ricochet) + "</replace>");
                            if (vanillaItem.Scale != item.Scale)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@scale\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Scale) + "</replace>");
                            if (vanillaItem.HeatInitial != item.HeatInitial)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/heat/@initial\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.HeatInitial) + "</replace>");
                            if (vanillaItem.HeatValue != item.HeatValue)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/heat/@value\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.HeatValue) + "</replace>");
                            if (vanillaItem.Damage != item.Damage)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/damage/@value\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Damage) + "</replace>");
                            if (vanillaItem.Repair != item.Repair)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/damage/@repair\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Repair) + "</replace>");
                            if (vanillaItem.Shield != item.Shield)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/damage/@shield\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Shield) + "</replace>");
                            if (vanillaItem.Hull != item.Hull)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/damage/@hull\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Hull) + "</replace>");
                            if (vanillaItem.ReloadRate != item.ReloadRate)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/reload/@rate\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.ReloadRate) + "</replace>");

                            sw.WriteLine("</diff> ");

                            fileswritten = true;
                        }
                    }
                }

                foreach (var item in this.UIModel.UIModelWeapons)
                {
                    var vanillaItem = this.UIModel.UIModelWeaponsVanilla.Where(x => x.Name == item.Name).FirstOrDefault();

                    if (item.Changed)
                    {
                        if (!Directory.Exists(this.UIModel.ExportPath + PathToTurretsStandard))
                        {
                            Directory.CreateDirectory(this.UIModel.ExportPath + PathToTurretsStandard);
                        }
                        if (!Directory.Exists(this.UIModel.ExportPath + PathToTurretsHeavy))
                        {
                            Directory.CreateDirectory(this.UIModel.ExportPath + PathToTurretsHeavy);
                        }
                        if (!Directory.Exists(this.UIModel.ExportPath + PathToTurretsGuided))
                        {
                            Directory.CreateDirectory(this.UIModel.ExportPath + PathToTurretsGuided);
                        }
                        if (!Directory.Exists(this.UIModel.ExportPath + PathToTurretsEnergy))
                        {
                            Directory.CreateDirectory(this.UIModel.ExportPath + PathToTurretsEnergy);
                        }
                        if (!Directory.Exists(this.UIModel.ExportPath + PathToTurretsCapital))
                        {
                            Directory.CreateDirectory(this.UIModel.ExportPath + PathToTurretsCapital);
                        }
                        if (!Directory.Exists(this.UIModel.ExportPath + PathToTurretsDumbfire))
                        {
                            Directory.CreateDirectory(this.UIModel.ExportPath + PathToTurretsDumbfire);
                        }

                        string outputPath = item.File.Replace(this.UIModel.Path, this.UIModel.ExportPath);
                        using (StreamWriter sw = new StreamWriter(outputPath))
                        {
                            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                            sw.WriteLine("<diff> ");
                            if (vanillaItem.Projectile != item.Projectile)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@class\">" + item.Projectile + "</replace>");
                            if (vanillaItem.RotationSpeed != item.RotationSpeed)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/rotationspeed/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.RotationSpeed) + "</replace>");
                            if (vanillaItem.RotationAcceleration != item.RotationAcceleration)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/rotationacceleration/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.RotationAcceleration) + "</replace>");
                            if (vanillaItem.ReloadRate != item.ReloadRate)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/reload/@rate\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.ReloadRate) + "</replace>");
                            if (vanillaItem.ReloadTime != item.ReloadTime)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/reload/@time\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.ReloadTime) + "</replace>");
                            if (vanillaItem.HullMax != item.HullMax)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.HullMax) + "</replace>");
                            if (vanillaItem.HullThreshold != item.HullThreshold)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/reload/@threshold\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.HullThreshold) + "</replace>");
                            if (vanillaItem.Overheat != item.Overheat)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/heat/@overheat\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Overheat) + "</replace>");
                            if (vanillaItem.CoolDelay != item.CoolDelay)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/heat/@cooldelay\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.CoolDelay) + "</replace>");
                            if (vanillaItem.CoolRate != item.CoolRate)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/heat/@coolrate\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.CoolRate) + "</replace>");
                            if (vanillaItem.Reenable != item.Reenable)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/heat/@reenable\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Reenable) + "</replace>");
                            sw.WriteLine("</diff> ");

                            fileswritten = true;
                        }
                    }
                }

                foreach (var item in this.UIModel.UIModelMissiles)
                {
                    var vanillaItem = this.UIModel.UIModelMissilesVanilla.Where(x => x.Name == item.Name).FirstOrDefault();

                    if (item.Changed)
                    {
                        if (!Directory.Exists(this.UIModel.ExportPath + PathToMissiles))
                        {
                            Directory.CreateDirectory(this.UIModel.ExportPath + PathToMissiles);
                        }

                        string outputPath = this.UIModel.ExportPath + PathToMissiles + item.File.Split(new[] { "macros" }, StringSplitOptions.None)[1];
                        using (StreamWriter sw = new StreamWriter(outputPath))
                        {
                            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                            sw.WriteLine("<diff> ");
                            if (vanillaItem.Ammunition != item.Ammunition)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/ammunition/@value\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Ammunition) + "</replace>");
                            if (vanillaItem.MissileAmount != item.MissileAmount)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/missile/@amount\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.MissileAmount) + "</replace>");
                            if (vanillaItem.BarrelAmount != item.BarrelAmount)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/missile/@barrelamount\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.BarrelAmount) + "</replace>");
                            if (vanillaItem.Lifetime != item.Lifetime)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/missile/@lifetime\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Lifetime) + "</replace>");
                            if (vanillaItem.Range != item.Range)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/missile/@range\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Range) + "</replace>");
                            if (vanillaItem.Guided != item.Guided)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/missile/@guided\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Guided) + "</replace>");
                            if (vanillaItem.Swarm != item.Swarm)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/missile/@swarm\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Swarm) + "</replace>");
                            if (vanillaItem.Retarget != item.Retarget)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/missile/@retarget\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Retarget) + "</replace>");
                            if (vanillaItem.Damage != item.Damage)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/explosiondamage/@value\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Damage) + "</replace>");
                            if (vanillaItem.Reload != item.Reload)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/reload/@time\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Reload) + "</replace>");
                            if (vanillaItem.Hull != item.Hull)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Hull) + "</replace>");
                            if (vanillaItem.Forward != item.Forward)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@forward\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Forward) + "</replace>");
                            if (vanillaItem.Reverse != item.Reverse)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@reverse\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Reverse) + "</replace>");
                            if (vanillaItem.Horizontal != item.Horizontal)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@horizontal\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Horizontal) + "</replace>");
                            if (vanillaItem.Vertical != item.Vertical)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@vertical\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Vertical) + "</replace>");
                            if (vanillaItem.Pitch != item.Pitch)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@pitch\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Pitch) + "</replace>");
                            if (vanillaItem.Yaw != item.Yaw)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@yaw\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Yaw) + "</replace>");
                            if (vanillaItem.Roll != item.Roll)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@roll\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Roll) + "</replace>");

                            if (vanillaItem.Mass != item.Mass)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/@mass\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Mass) + "</replace>");
                            if (vanillaItem.InertiaPitch != item.InertiaPitch)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@pitch\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaPitch) + "</replace>");
                            if (vanillaItem.InertiaRoll != item.InertiaRoll)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@roll\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaRoll) + "</replace>");
                            if (vanillaItem.InertiaYaw != item.InertiaYaw)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@yaw\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaYaw) + "</replace>");
                            sw.WriteLine("</diff> ");

                            fileswritten = true;
                        }
                    }
                }

                this.WriteWaresFile();

                if (fileswritten)
                    MessageBox.Show("Mod files have been created.");
                else
                    MessageBox.Show("No changes detected - no files written.");
            }
        }

        private void WriteWaresFile()
        {
            string outputPath = this.UIModel.ExportPath + PathToWares;

            if (!Directory.Exists(this.UIModel.ExportPath + @"\libraries"))
            {
                Directory.CreateDirectory(this.UIModel.ExportPath + @"\libraries");
            }

            if (this.UIModel.UIModelWares.Any(x => x.Changed))
            {
                using (StreamWriter sw = new StreamWriter(outputPath, true))
                {
                    sw.WriteLine("<!-- Code below has been added at " + DateTime.Now + " -->");
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    sw.WriteLine("<diff> ");

                    foreach (var item in this.UIModel.UIModelWares)
                    {
                        var vanillaItem = this.UIModel.UIModelWaresVanilla.Where(x => x.Name == item.Name).ToList()[0];

                        if (item.Changed)
                        {
                            if (vanillaItem.Max != item.Max || vanillaItem.Avg != item.Avg || vanillaItem.Min != item.Min)
                            {
                                sw.WriteLine("\t<replace sel=\"/wares/ware[@id='" + item.ID + "']/price\">");
                                sw.WriteLine("\t\t<price min=\"" + item.Min + "\" average=\"" + item.Avg + "\" max=\"" + item.Max + "\" /> ");
                                sw.WriteLine("\t</replace>");
                            }
                            if (vanillaItem.Amount != item.Amount)
                                sw.WriteLine("\t<replace sel=\"/wares/ware[@id='" + item.ID + "']/production/@amount\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Amount) + "</replace>");
                            if (vanillaItem.Time != item.Time)
                                sw.WriteLine("\t<replace sel=\"/wares/ware[@id='" + item.ID + "']/production/@time\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Time) + "</replace>");

                            if (vanillaItem.Amount1 != item.Amount1 || vanillaItem.Amount2 != item.Amount2 || vanillaItem.Amount3 != item.Amount3 || vanillaItem.Amount4 != item.Amount4 || vanillaItem.Amount5 != item.Amount5)
                            {
                                sw.WriteLine("\t<replace sel=\"/wares/ware[@id='" + item.ID + "']/production/primary\">");
                                sw.WriteLine("\t\t<primary> ");
                                if (item.Amount1 > 0)
                                    sw.WriteLine("\t\t\t<ware ware=\"" + item.Ware1 + "\" amount=\"" + item.Amount1 + "\" />");
                                if (item.Amount2 > 0)
                                    sw.WriteLine("\t\t\t<ware ware=\"" + item.Ware2 + "\" amount=\"" + item.Amount2 + "\" />");
                                if (item.Amount3 > 0)
                                    sw.WriteLine("\t\t\t<ware ware=\"" + item.Ware3 + "\" amount=\"" + item.Amount3 + "\" />");
                                if (item.Amount4 > 0)
                                    sw.WriteLine("\t\t\t<ware ware=\"" + item.Ware4 + "\" amount=\"" + item.Amount4 + "\" />");
                                if (item.Amount5 > 0)
                                    sw.WriteLine("\t\t\t<ware ware=\"" + item.Ware5 + "\" amount=\"" + item.Amount5 + "\" />");
                                sw.WriteLine("\t\t</primary> ");
                                sw.WriteLine("\t</replace>");
                                sw.WriteLine("");
                            }
                        }
                    }
                    sw.WriteLine("</diff> ");

                }
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
            this.UIModel.UIModelProjectiles.Clear();
            this.UIModel.UIModelMissiles.Clear();
            this.UIModel.UIModelEngines.Clear();
            this.UIModel.UIModelShields.Clear();
            this.UIModel.UIModelWares.Clear();
            this.UIModel.UIModelShips.Clear();
            this.UIModel.UIModelWeapons.Clear();
            
            this.UIModel.AllWaresLoaded = false;

            string folderPath = this.UIModel.Path.Replace(@"\\", @"\");

            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("Enter a valid folder path for extracted vanilla files", "No valid folder");
            }
            string waresPath = folderPath + PathToWares;
            m_XmlExtractor.ReadAllWares(waresPath);

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
