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


        public Dictionary<string, string> TextDictionary = new Dictionary<string, string>();
        public MainWindow MainWindow { get; set; }
        public WaresWindow WaresWindow { get; set; }
        public UIModel UIModel { get; set; }
        public XmlExtractor m_XmlExtractor;

        public UIManager()
        {
            MainWindow = new MainWindow();
            WaresWindow = new WaresWindow();
            m_XmlExtractor = new XmlExtractor(this);
            this.UIModel = new UIModel();
            MainWindow.DataContext = this.UIModel;
            WaresWindow.DataContext = this.UIModel;
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

            if (Directory.Exists(this.UIModel.Path))
                this.ExecuteReadAllVanillaFilesCommand(this, null);
            if (Directory.Exists(this.UIModel.ModPath1))
                this.ExecuteReadAllMod1FilesCommand(this, null);
            if (Directory.Exists(this.UIModel.ModPath2))
                this.ExecuteReadAllMod2FilesCommand(this, null);
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
            var check = Utility.ParseToDouble(this.UIModel.MathParameter.ToString());
            if (check == null)
            {
                MessageBox.Show("Please enter numeric values only", "Invalid operation");
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
            var check = Utility.ParseToDouble(this.UIModel.MathParameter.ToString());
            if (check == null)
            {
                MessageBox.Show("Please enter numeric values only", "Invalid operation");
                return;
            }
            this.AddCalculation(1);
        }
        private void AddCalculation(int operation)
        {
            Counter counter = new Counter();
            
            var check = Utility.ParseToDouble(this.UIModel.MathParameter.ToString());

            DataGrid dg_Shields = null;
            dg_Shields = this.MainWindow.DG_Shields;

            if (dg_Shields.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Shields.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Shields, operation, counter);
            }

            DataGrid dg_Engines = null;
            dg_Engines = this.MainWindow.DG_Engines;

            if (dg_Engines.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Engines.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Engines, operation, counter);
            }

            DataGrid dg_Projectiles = null;
            dg_Projectiles = this.MainWindow.DG_Projectiles;

            if (dg_Projectiles.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Projectiles.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Projectiles, operation, counter);
            }

            DataGrid dg_Missiles = null;
            dg_Missiles = this.MainWindow.DG_Missiles;

            if (dg_Missiles.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Missiles.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Missiles, operation, counter);
            }

            DataGrid dg_Ships = null;
            dg_Ships = this.MainWindow.DG_Ships;

            if (dg_Ships.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Ships.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Ships, operation, counter);
            }

            DataGrid dg_Wares = null;
            dg_Wares = this.WaresWindow.DG_Wares;

            if (dg_Wares.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Wares.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Wares, operation, counter);
            }

            //if (counter.outofrangecounter > 0 && (selectedCells != counter.successcounter))
            //    MessageBox.Show(counter.successcounter + " values have been changed.\r " 
            //        + (selectedCells - counter.successcounter) + " cells could not be changed\r" 
            //        + counter.outofrangecounter + " values were out of range and have been set to default.", 
            //        "Calculation Errors");
            //else if (counter.outofrangecounter > 0 && selectedCells.Equals(counter.successcounter))
            //    MessageBox.Show(counter.successcounter + " values have been changed.\r" +
            //        +counter.outofrangecounter + " values were out of range and have been set to default.",
            //        "Calculation Errors");
            //else if (counter.outofrangecounter == 0 && !selectedCells.Equals(counter.successcounter))
            //    MessageBox.Show(counter.successcounter + " values have been changed.\r" +
            //        +(selectedCells - counter.successcounter) + " cells could not be changed.\r",
            //        "Calculation Mixed Success");
            //else
            //    MessageBox.Show("All " + counter.successcounter + " values have been changed successfully.", "Calculation Success");
        }

        private Counter CalculateOverAll(DataGrid dg, int operation, Counter counter)
        {
            foreach (DataGridCellInfo dataGridCell in dg.SelectedCells)
            {
                var shield = dataGridCell.Item as UIModelShield;
                if (shield != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        shield.Max = (int)Calculate(operation, shield.Max);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 6)
                    {
                        shield.Rate = Calculate(operation, shield.Rate);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        shield.Delay = Calculate(operation, shield.Delay);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        shield.MaxHull = (int)Calculate(operation, shield.MaxHull);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 9)
                    {
                        shield.Threshold = Calculate(operation, shield.Threshold);
                        counter.successcounter++;
                    }
                }

                var engine = dataGridCell.Item as UIModelEngine;
                if (engine != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        engine.Forward = Calculate(operation, engine.Forward);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 6)
                    {
                        engine.Reverse = Calculate(operation, engine.Reverse);
                        counter.successcounter++;
                    }

                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        engine.BoostThrust = Calculate(operation, engine.BoostThrust);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        engine.BoostDuration = Calculate(operation, engine.BoostDuration);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 9)
                    {
                        engine.BoostAttack = Calculate(operation, engine.BoostAttack);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 10)
                    {
                        engine.BoostRelease = Calculate(operation, engine.BoostRelease);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 11)
                    {
                        engine.TravelThrust = Calculate(operation, engine.TravelThrust);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 12)
                    {
                        engine.TravelCharge = (int)Calculate(operation, engine.TravelCharge);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 13)
                    {
                        engine.TravelAttack = Calculate(operation, engine.TravelAttack);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        engine.TravelRelease = Calculate(operation, engine.TravelRelease);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 15)
                    {
                        engine.Strafe = Calculate(operation, engine.Strafe);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 16)
                    {
                        engine.Yaw = Calculate(operation, engine.Yaw);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 17)
                    {
                        engine.Pitch = Calculate(operation, engine.Pitch);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 18)
                    {
                        engine.Roll = Calculate(operation, engine.Roll);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 19)
                    {
                        engine.AngularPitch = Calculate(operation, engine.AngularPitch);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 20)
                    {
                        engine.AngularRoll = Calculate(operation, engine.AngularRoll);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 21)
                    {
                        engine.MaxHull = (int)Calculate(operation, engine.MaxHull);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 22)
                    {
                        engine.Threshold = (int)Calculate(operation, engine.Threshold);
                        counter.successcounter++;
                    }
                }

                var projectile = dataGridCell.Item as UIModelProjectile;
                if (projectile != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 2)
                    {
                        projectile.Damage = Calculate(operation, projectile.Damage);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 3)
                    {
                        projectile.ReloadRate = Calculate(operation, projectile.ReloadRate);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 4)
                    {
                        projectile.ReloadTime = Calculate(operation, projectile.ReloadTime);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 6)
                    {
                        projectile.Shield = Calculate(operation, projectile.Shield);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        projectile.Amount = (int)Calculate(operation, projectile.Amount);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 9)
                    {
                        projectile.Speed = (int)Calculate(operation, projectile.Speed);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 10)
                    {
                        projectile.Range = (int)Calculate(operation, projectile.Range);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 11)
                    {
                        projectile.Lifetime = Calculate(operation, projectile.Lifetime);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 12)
                    {
                        projectile.ChargeTime = Calculate(operation, projectile.ChargeTime);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 13)
                    {
                        projectile.Ammunition = (int)Calculate(operation, projectile.Ammunition);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        projectile.AmmunitionReload = Calculate(operation, projectile.AmmunitionReload);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 15)
                    {
                        projectile.Angle = Calculate(operation, projectile.Angle);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 16)
                    {
                        projectile.TimeDiff = Calculate(operation, projectile.TimeDiff);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 17)
                    {
                        projectile.MaxHits = (int)Calculate(operation, projectile.MaxHits);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 18)
                    {
                        projectile.Scale = Calculate(operation, projectile.Scale);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 19)
                    {
                        projectile.Ricochet = (int)Calculate(operation, projectile.Ricochet);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 20)
                    {
                        projectile.HeatValue = (int)Calculate(operation, projectile.HeatValue);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 21)
                    {
                        projectile.HeatInitial = (int)Calculate(operation, projectile.HeatInitial);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 22)
                    {
                        projectile.Repair = Calculate(operation, projectile.Repair);
                        counter.successcounter++;
                    }
                }

                var weapon = dataGridCell.Item as UIModelWeapon;
                if (weapon != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        weapon.RotationSpeed = Calculate(operation, weapon.RotationSpeed);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 6)
                    {
                        weapon.RotationAcceleration = Calculate(operation, weapon.RotationAcceleration);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        weapon.ReloadRate = Calculate(operation, weapon.ReloadRate);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        weapon.ReloadTime = Calculate(operation, weapon.ReloadTime);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 9)
                    {
                        weapon.Overheat = (int)Calculate(operation, weapon.Overheat);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 10)
                    {
                        weapon.CoolDelay = Calculate(operation, weapon.CoolDelay);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 11)
                    {
                        weapon.CoolRate = (int)Calculate(operation, weapon.CoolRate);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 12)
                    {
                        weapon.Reenable = (int)Calculate(operation, weapon.Reenable);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 13)
                    {
                        weapon.HullMax = (int)Calculate(operation, weapon.HullMax);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        weapon.HullThreshold = Calculate(operation, weapon.HullThreshold);
                        counter.successcounter++;
                    }
   
                }

                var missile = dataGridCell.Item as UIModelMissile;
                if (missile != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 2)
                    {
                        missile.Damage = (int)Calculate(operation, missile.Damage);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 3)
                    {
                        missile.Reload = Calculate(operation, missile.Reload);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        missile.Range = (int)Calculate(operation, missile.Range);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 6)
                    {
                        missile.Lifetime = Calculate(operation, missile.Lifetime);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        missile.Forward = Calculate(operation, missile.Forward);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        missile.Reverse = Calculate(operation, missile.Reverse);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 9)
                    {
                        missile.Guided = (int)Calculate(operation, missile.Guided);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 10)
                    {
                        missile.Swarm = (int)Calculate(operation, missile.Swarm);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 11)
                    {
                        missile.Retarget = (int)Calculate(operation, missile.Retarget);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 13)
                    {
                        missile.Hull = (int)Calculate(operation, missile.Hull);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        missile.Ammunition = (int)Calculate(operation, missile.Ammunition);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 15)
                    {
                        missile.MissileAmount = (int)Calculate(operation, missile.MissileAmount);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 16)
                    {
                        missile.Mass = Calculate(operation, missile.Mass);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 17)
                    {
                        missile.Horizontal = Calculate(operation, missile.Horizontal);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 18)
                    {
                        missile.Vertical = Calculate(operation, missile.Vertical);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 19)
                    {
                        missile.Pitch = Calculate(operation, missile.Pitch);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 20)
                    {
                        missile.Yaw = Calculate(operation, missile.Yaw);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 21)
                    {
                        missile.Roll = Calculate(operation, missile.Roll);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 21)
                    {
                        missile.InertiaPitch = Calculate(operation, missile.InertiaPitch);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 22)
                    {
                        missile.InertiaYaw = Calculate(operation, missile.InertiaYaw);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 23)
                    {
                        missile.InertiaRoll = Calculate(operation, missile.InertiaRoll);
                        counter.successcounter++;
                    }
                }

                var ship = dataGridCell.Item as UIModelShip;
                if (ship != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        ship.HullMax = (int)Calculate(operation, ship.HullMax);
                        counter.successcounter++;
                    }
                    
                    //if (dataGridCell.Column.DisplayIndex == 9)
                    //{
                    //    ship.Cargo.CargoMax = (int)Calculate(operation, ship.Cargo.CargoMax);
                    //    counter.successcounter++;
                    //}

                    //if (dataGridCell.Column.DisplayIndex == 11)
                    //{
                    //    ship.StorageUnits = (int)Calculate(operation, ship.StorageUnits);
                    //    counter.successcounter++;
                    //}
                    if (dataGridCell.Column.DisplayIndex == 12)
                    {
                        ship.StorageMissiles = (int)Calculate(operation, ship.StorageMissiles);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 13)
                    {
                        ship.People = (int)Calculate(operation, ship.People);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        ship.ExplosionDamage = (int)Calculate(operation, ship.ExplosionDamage);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 15)
                    {
                        ship.Secrecy = (int)Calculate(operation, ship.Secrecy);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 16)
                    {
                        ship.GatherRrate = Calculate(operation, ship.GatherRrate);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 17)
                    {
                        ship.Mass = Calculate(operation, ship.Mass);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 18)
                    {
                        ship.Forward = Calculate(operation, ship.Forward);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 19)
                    {
                        ship.Reverse = Calculate(operation, ship.Reverse);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 20)
                    {
                        ship.Horizontal = Calculate(operation, ship.Horizontal);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 21)
                    {
                        ship.Vertical = Calculate(operation, ship.Vertical);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 22)
                    {
                        ship.Pitch = Calculate(operation, ship.Pitch);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 23)
                    {
                        ship.Yaw = Calculate(operation, ship.Yaw);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 24)
                    {
                        ship.Roll = Calculate(operation, ship.Roll);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 25)
                    {
                        ship.InertiaPitch = Calculate(operation, ship.InertiaPitch);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 26)
                    {
                        ship.InertiaYaw = Calculate(operation, ship.InertiaYaw);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 27)
                    {
                        ship.InertiaRoll = Calculate(operation, ship.InertiaRoll);
                        counter.successcounter++;
                    }
                }

                var ware = dataGridCell.Item as UIModelWare;
                if (ware != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 2)
                    {
                        ware.Min = (int)Calculate(operation, ware.Min);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 3)
                    {
                        ware.Avg = (int)Calculate(operation, ware.Avg);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 4)
                    {
                        ware.Max = (int)Calculate(operation, ware.Max);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        ware.Amount = (int)Calculate(operation, ware.Amount);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        ware.Time = (int)Calculate(operation, ware.Time);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 10)
                    {
                        ware.Amount1 = (int)Calculate(operation, ware.Amount1);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 12)
                    {
                        ware.Amount2 = (int)Calculate(operation, ware.Amount2);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        ware.Amount3 = (int)Calculate(operation, ware.Amount3);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 16)
                    {
                        ware.Amount4 = (int)Calculate(operation, ware.Amount4);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 18)
                    {
                        ware.Amount5 = (int)Calculate(operation, ware.Amount5);
                        counter.successcounter++;
                    }
                }

            }

            if (counter.successcounter > 0 && dg.SelectedCells.Count == counter.successcounter)
                MessageBox.Show("All " + counter.successcounter + " values have been changed successfully.", "Calculation Success");
            else if (counter.successcounter == 0)
                MessageBox.Show("No cells could be changed. \rBe sure to select only cells you can use calculations on.", "Calculation Failed");
            else if (counter.successcounter > 0 && dg.SelectedCells.Count != counter.successcounter)
                MessageBox.Show("Only " + counter.successcounter + " of " + dg.SelectedCells.Count + " cells could be changed succesfully. \rBe sure to select only cells you can use calculations on.", "Calculation partially Success.");

            return counter;
        }
        /// <summary>
        /// Calculation function
        /// </summary>
        /// <param name="operation">1 = addition, 2 = multiplication, 3 = substraction, 4 = set fixed value</param>
        /// <param name="param1">unit value</param>
        private double Calculate(int operation, double param1)
        {
            switch (operation)
            {
                case 1:
                    return param1 + this.UIModel.MathParameter;
                case 2:
                    return param1 * this.UIModel.MathParameter;
                case 3:
                    return param1 - this.UIModel.MathParameter;
                case 4:
                    return this.UIModel.MathParameter;
                default:
                    return param1;
            }
        }
        private void ExecuteMultiplyToValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Counter counter = new Counter();
            var check = Utility.ParseToDouble(this.UIModel.MathParameter.ToString());

            DataGrid dg_Shields = null;
            dg_Shields = this.MainWindow.DG_Shields;

            if (dg_Shields.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Shields.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Shields, 2, counter);
            }

            DataGrid dg_Engines = null;
            dg_Engines = this.MainWindow.DG_Engines;

            if (dg_Engines.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Engines.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Engines, 2, counter);
            }

            DataGrid dg_Projectiles = null;
            dg_Projectiles = this.MainWindow.DG_Projectiles;

            if (dg_Projectiles.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Projectiles.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Projectiles, 2, counter);
            }

            DataGrid dg_Missiles = null;
            dg_Missiles = this.MainWindow.DG_Missiles;

            if (dg_Missiles.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Missiles.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Missiles, 2, counter);
            }

            DataGrid dg_Ships = null;
            dg_Ships = this.MainWindow.DG_Ships;

            if (dg_Ships.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Ships.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Ships, 2, counter);
            }

            DataGrid dg_Wares = null;
            dg_Wares = this.WaresWindow.DG_Wares;

            if (dg_Wares.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Wares.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Wares, 2, counter);
            }
            //if (counter.outofrangecounter > 0 && (selectedCells != counter.successcounter))
            //    MessageBox.Show(counter.successcounter + " values have been changed.\r "
            //        + (selectedCells - counter.successcounter) + " cells could not be changed\r"
            //        + counter.outofrangecounter + " values were out of range and have been set to default.",
            //        "Calculation Errors");
            //else if (counter.outofrangecounter > 0 && selectedCells.Equals(counter.successcounter))
            //    MessageBox.Show(counter.successcounter + " values have been changed.\r" +
            //        +counter.outofrangecounter + " values were out of range and have been set to default.",
            //        "Calculation Errors");
            //else if (counter.outofrangecounter == 0 && !selectedCells.Equals(counter.successcounter))
            //    MessageBox.Show(counter.successcounter + " values have been changed.\r" +
            //        +(selectedCells - counter.successcounter) + " cells could not be changed.\r",
            //        "Calculation Mixed Success");
            //else
            //    MessageBox.Show("All " + counter.successcounter + " values have been changed successfully.", "Calculation Success");
        }

        private void ExecuteSetFixedValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Counter counter = new Counter();
            var check = Utility.ParseToDouble(this.UIModel.MathParameter.ToString());

            DataGrid dg_Shields = null;
            dg_Shields = this.MainWindow.DG_Shields;

            if (dg_Shields.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Shields.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Shields, 4, counter);
            }

            DataGrid dg_Engines = null;
            dg_Engines = this.MainWindow.DG_Engines;

            if (dg_Engines.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Engines.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Engines, 4, counter);
            }

            DataGrid dg_Projectiles = null;
            dg_Projectiles = this.MainWindow.DG_Projectiles;

            if (dg_Projectiles.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Projectiles.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Projectiles, 4, counter);
            }

            DataGrid dg_Weapons = null;
            dg_Weapons = this.MainWindow.DG_Weapons;

            if (dg_Weapons.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Weapons.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Weapons, 4, counter);
            }

            DataGrid dg_Missiles = null;
            dg_Missiles = this.MainWindow.DG_Missiles;

            if (dg_Missiles.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Missiles.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Missiles, 4, counter);
            }

            DataGrid dg_Ships = null;
            dg_Ships = this.MainWindow.DG_Ships;

            if (dg_Ships.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Ships.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Ships, 4, counter);
            }

            DataGrid dg_Wares = null;
            dg_Wares = this.WaresWindow.DG_Wares;

            if (dg_Wares.SelectedCells.Count > 0)
            {
                int selectedCells = dg_Wares.SelectedCells.Count;
                counter = this.CalculateOverAll(dg_Wares, 4, counter);
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

                foreach (var item in this.UIModel.UIModelModulesShields)
                {
                    var vanillaItem = this.UIModel.UIModelModuleShieldsVanilla.Where(x => x.Name == item.Name).FirstOrDefault();

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

                foreach (var item in this.UIModel.UIModelModulesEngines)
                {
                    var vanillaItem = this.UIModel.UIModelModuleEnginesVanilla.Where(x => x.Name == item.Name).FirstOrDefault();

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

            using (StreamWriter sw = new StreamWriter(outputPath))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sw.WriteLine("<diff> ");

                foreach (var item in this.UIModel.UIModelWares)
                {
                    var vanillaItem = this.UIModel.UIModelWaresVanilla.Where(x => x.Name == item.Name).ToList()[0];

                    if (item.Changed)
                    {
                        if (vanillaItem.Max != item.Max)
                            sw.WriteLine("\t<replace sel=\"//wares/ware[@id='"+ item.ID + "']/price/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Max) + "</replace>");
                        if (vanillaItem.Min != item.Min)
                            sw.WriteLine("\t<replace sel=\"//wares/ware[@id='" + item.ID + "']/price/@min\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Min) + "</replace>");
                        if (vanillaItem.Avg != item.Avg)
                            sw.WriteLine("\t<replace sel=\"//wares/ware[@id='" + item.ID + "']/price/@average\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Avg) + "</replace>");
                        if (vanillaItem.Amount != item.Amount)
                            sw.WriteLine("\t<replace sel=\"//wares/ware[@id='" + item.ID + "']/production/@amount\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Amount) + "</replace>");
                        if (vanillaItem.Time != item.Time)
                            sw.WriteLine("\t<replace sel=\"//wares/ware[@id='" + item.ID + "']/production/@time\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Time) + "</replace>");
                    }
                }

                sw.WriteLine("</diff> ");

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
            this.UIModel.UIModelModulesEngines.Clear();
            this.UIModel.UIModelModulesShields.Clear();
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
                        this.UIModel.UIModelModulesEngines.Add(engine);
                }

                this.UIModel.UIModelModuleEnginesVanilla.Clear();
                foreach (var item in this.UIModel.UIModelModulesEngines)
                {
                    this.UIModel.UIModelModuleEnginesVanilla.Add(item.Copy());
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
                this.UIModel.UIModelModuleShieldsVanilla.Clear();

                foreach (var item in xmlShieldsList)
                {
                    UIModelShield shield = m_XmlExtractor.ReadSingleShield(item);
                    if (shield.Name.Length > 1)
                        this.UIModel.UIModelModulesShields.Add(shield);
                }

                foreach (var item in this.UIModel.UIModelModulesShields)
                {
                    this.UIModel.UIModelModuleShieldsVanilla.Add(item.Copy());
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
            this.ReadAllModFilesFromFolder(this.UIModel.ModPath1);
        }

        private void ExecuteReadAllMod2FilesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.ReadAllModFilesFromFolder(this.UIModel.ModPath2);
        }
        private void ReadAllModFilesFromFolder(string modPath)
        {

            if (!Directory.Exists(modPath))
            {
                MessageBox.Show("Enter a valid folder path for mod files", "No valid mod folder");
            }
            else
            {
                List<string> ModDirectories = Directory.GetDirectories(modPath, "*", SearchOption.AllDirectories).ToList();

                Dictionary<string, string> ModTexts = new Dictionary<string, string>();
                ModTexts = m_XmlExtractor.ReadTextXml(modPath + this.PathToTexts + @"\0001.xml", ModTexts);

                while (ModTexts.Count > 0)
                {
                    if (this.TextDictionary.ContainsKey(ModTexts.First().Key))
                        this.TextDictionary[ModTexts.First().Key] = ModTexts.First().Value;
                    else
                        this.TextDictionary.Add(ModTexts.First().Key, ModTexts.First().Value);

                    ModTexts.Remove(ModTexts.First().Key);
                }

                // read wares of mod
                m_XmlExtractor.ReadAllWares(modPath + @"\libraries\wares.xml" );

                foreach (string dir in ModDirectories)
                {
                    //shields
                    if (dir.Contains("assets\\props\\SurfaceElements\\macros"))
                    {
                        List<string> files = Directory.GetFiles(dir).ToList();

                        foreach (string file in files)
                        {
                            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            string[] fileDepth = fileStream.Name.Split('\\');
                            string fileName = fileDepth[fileDepth.Count() - 1];
                            using (StreamReader sr = new StreamReader(fileStream))
                            {
                                var shield = this.UIModel.UIModelModulesShields.FirstOrDefault(x => x.File.Contains(fileName));
                                if (shield != null)
                                {
                                    int index = this.UIModel.UIModelModulesShields.IndexOf(shield);
                                    string line;

                                    while (!sr.EndOfStream)
                                    {
                                        line = sr.ReadLine();

                                        if (line.Contains(@"<replace") && line.Contains("sel") && line.Contains(@"/macros") && !line.Contains("@"))
                                        {
                                            this.UIModel.UIModelModulesShields[index] = m_XmlExtractor.ReadSingleShield(new FileInfo(file));
                                            break;
                                        }

                                        if (line.Contains("@max") && line.Contains("recharge"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            shield.Max = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@rate"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            shield.Rate = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@delay"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            shield.Delay = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@max") && line.Contains("hull"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            shield.MaxHull = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@threshold") && line.Contains("hull"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            shield.Threshold = Utility.ParseToDouble(value);
                                        }
                                    }
                                }
                                else
                                {
                                    UIModelShield extractedShield = m_XmlExtractor.ReadSingleShield(new FileInfo(file));
                                    if (extractedShield.Name.Length > 1)
                                        this.UIModel.UIModelModulesShields.Add(extractedShield);
                                }
                            }
                        }
                    }
                    //engines
                    if (dir.Contains("assets\\props\\Engines\\macros"))
                    {
                        List<string> files = Directory.GetFiles(dir).ToList();

                        foreach (string file in files)
                        {
                            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            string[] fileDepth = fileStream.Name.Split('\\');
                            string fileName = fileDepth[fileDepth.Count() - 1];
                            using (StreamReader sr = new StreamReader(fileStream))
                            {
                                var engine = this.UIModel.UIModelModulesEngines.FirstOrDefault(x => x.File.Contains(fileName));
                                if (engine != null)
                                {
                                    int index = this.UIModel.UIModelModulesEngines.IndexOf(engine);
                                    string line;

                                    while (!sr.EndOfStream)
                                    {
                                        line = sr.ReadLine();

                                        if (line.Contains(@"<replace") && line.Contains("sel") && line.Contains(@"/macros") && !line.Contains("@"))
                                        {
                                            this.UIModel.UIModelModulesEngines[index] = m_XmlExtractor.ReadSingleEngineFile(new FileInfo(file));
                                            break;
                                        }

                                        if (line.Contains("@duration") && line.Contains("boost"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.BoostDuration = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@thrust") && line.Contains("boost"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.BoostThrust = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@attack") && line.Contains("boost"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.BoostAttack = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@release") && line.Contains("boost"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.BoostRelease = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@charge") && line.Contains("travel"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.TravelCharge = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@thrust") && line.Contains("travel"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.TravelThrust = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@attack") && line.Contains("travel"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.TravelAttack = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@release") && line.Contains("travel"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.TravelRelease = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@forward"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.Forward = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@reverse"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.Reverse = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@strafe") && line.Contains("thrust"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.Strafe = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@pitch") && line.Contains("thrust"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.Pitch = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@yaw") && line.Contains("thrust"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.Yaw = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@roll") && line.Contains("thrust"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.Roll = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@roll") && line.Contains("angular"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.AngularRoll = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@pitch") && line.Contains("angular"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.AngularPitch = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@max") && line.Contains("hull"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.MaxHull = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@threshold") && line.Contains("hull"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            engine.Threshold = Utility.ParseToDouble(value);
                                        }
                                    }
                                }
                                else
                                {
                                    UIModelEngine extractedEngine = m_XmlExtractor.ReadSingleEngineFile(new FileInfo(file));
                                    if (extractedEngine.Name.Length > 1)
                                        this.UIModel.UIModelModulesEngines.Add(extractedEngine);
                                }
                            }
                        }
                    }
                    //projectiles 
                    if (dir.Contains("assets\\fx\\weaponFx\\macros"))
                    {
                        List<string> files = Directory.GetFiles(dir).ToList();

                        foreach (string file in files)
                        {
                            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            string[] fileDepth = fileStream.Name.Split('\\');
                            string fileName = fileDepth[fileDepth.Count() - 1];
                            using (StreamReader sr = new StreamReader(fileStream))
                            {
                                // Projectiles
                                if (this.UIModel.UIModelProjectiles.Any(x => x.File.Contains(fileName)))
                                {
                                    var weaponProjectile = this.UIModel.UIModelProjectiles.FirstOrDefault(x => x.File.Contains(fileName));
                                    if (weaponProjectile != null)
                                    {
                                        int index = this.UIModel.UIModelProjectiles.IndexOf(weaponProjectile);

                                        string line;
                                        while (!sr.EndOfStream)
                                        {
                                            line = sr.ReadLine();

                                            if (line.Contains(@"<replace") && line.Contains("sel") && line.Contains("/macros") && !line.Contains("@"))
                                            {
                                                this.UIModel.UIModelProjectiles[index] = m_XmlExtractor.ReadSingleProjectile(new FileInfo(file));
                                                break;
                                            }

                                            if (line.Contains("@value") && line.Contains("ammunition"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.Ammunition = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("@reload") && line.Contains("ammunition"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.AmmunitionReload = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("@speed") && line.Contains("bullet"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.Speed = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("@lifetime") && line.Contains("bullet"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.Lifetime = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("@amount") && line.Contains("bullet"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.Amount = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("@barrelamount") && line.Contains("bullet"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.BarrelAmount = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("@timediff") && line.Contains("bullet"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.TimeDiff = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("@angle") && line.Contains("bullet"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.Angle = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("@scale") && line.Contains("bullet"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.Scale = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("@ricochet") && line.Contains("bullet"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.Ricochet = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("@chargetime") && line.Contains("bullet"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.ChargeTime = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("@value") && line.Contains("heat"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.HeatValue = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("@initial") && line.Contains("heat"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.HeatInitial = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("@rate") && line.Contains("reload"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.ReloadRate = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("@time") && line.Contains("reload"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.ReloadTime = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("@value") && line.Contains("damage"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.Damage = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("@shield") && line.Contains("damage"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.Shield = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("@repair") && line.Contains("damage"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weaponProjectile.Repair = Utility.ParseToDouble(value);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    UIModelProjectile extractedProjectile = m_XmlExtractor.ReadSingleProjectile(new FileInfo(file));
                                    if (extractedProjectile.Name.Length > 1)
                                        this.UIModel.UIModelProjectiles.Add(extractedProjectile);
                                }
                            }
                        }
                    }
                    // weapons
                    if (dir.Contains("assets\\props\\WeaponSystems"))
                    {
                        List<string> files = new List<string>();

                        if(Directory.Exists(modPath + PathToTurretsStandard))
                            files.AddRange(Directory.GetFiles(modPath + PathToTurretsStandard).ToList());
                        if (Directory.Exists(modPath + PathToTurretsEnergy))
                            files.AddRange(Directory.GetFiles(modPath + PathToTurretsEnergy).ToList());
                        if (Directory.Exists(modPath + PathToTurretsCapital))
                            files.AddRange(Directory.GetFiles(modPath + PathToTurretsCapital).ToList());
                        if (Directory.Exists(modPath + PathToTurretsHeavy))
                            files.AddRange(Directory.GetFiles(modPath + PathToTurretsHeavy).ToList());
                        if (Directory.Exists(modPath + PathToTurretsGuided))
                            files.AddRange(Directory.GetFiles(modPath + PathToTurretsGuided).ToList());
                        if (Directory.Exists(modPath + PathToTurretsDumbfire))
                            files.AddRange(Directory.GetFiles(modPath + PathToTurretsDumbfire).ToList());

                        foreach (string file in files)
                        {
                            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            string[] fileDepth = fileStream.Name.Split('\\');
                            string fileName = fileDepth[fileDepth.Count() - 1];
                            using (StreamReader sr = new StreamReader(fileStream))
                            {
                                if (this.UIModel.UIModelWeapons.Any(x => x.File.Contains(fileName)))
                                {
                                    var weapon = this.UIModel.UIModelWeapons.FirstOrDefault(x => x.File.Contains(fileName));
                                    
                                    if (weapon != null)
                                    {
                                        int index = this.UIModel.UIModelWeapons.IndexOf(weapon);
                                        string line;
                                        while (!sr.EndOfStream)
                                        {
                                            line = sr.ReadLine();
                                            
                                            if (line.Contains(@"<replace") && line.Contains("sel") && line.Contains(@"/macros") && !line.Contains("@"))
                                            {
                                                this.UIModel.UIModelWeapons[index] = m_XmlExtractor.ReadSingleWeapon(new FileInfo(file));
                                                break;
                                            }

                                            if (line.Contains("class") && line.Contains("bullet"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Projectile = value;
                                            }
                                            if (line.Contains("rotationspeed") && line.Contains("@max"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.RotationSpeed = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("rotationacceleration") && line.Contains("@max"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.RotationAcceleration = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("reload") && line.Contains("@rate"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.ReloadRate = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("reload") && line.Contains("@time"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.ReloadTime = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("hull") && line.Contains("@max"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.HullMax = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("reload") && line.Contains("@threshold"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.HullThreshold = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("heat") && line.Contains("@overheat"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Overheat = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("heat") && line.Contains("@coolDelay"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.CoolDelay = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("heat") && line.Contains("@coolrate"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.CoolRate = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("heat") && line.Contains("@reenable"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Reenable = Convert.ToInt32(value);
                                            }
                                        }
                                    }
                                    else
                                         this.UIModel.UIModelWeapons.Add(m_XmlExtractor.ReadSingleWeapon(new FileInfo(file)));
                                }

                            }
                        }
                    }
                    // missiles
                    if (dir.Contains(@"assets\props\WeaponSystems\missile\macros"))
                    {
                        List<string> files = Directory.GetFiles(dir).ToList();

                        foreach (string file in files)
                        {
                            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            string[] fileDepth = fileStream.Name.Split('\\');
                            string fileName = fileDepth[fileDepth.Count() - 1];
                            using (StreamReader sr = new StreamReader(fileStream))
                            {

                                var weapon = this.UIModel.UIModelMissiles.FirstOrDefault(x => x.File.Contains(fileName));
                                if (weapon != null)
                                    {
                                        int index = this.UIModel.UIModelMissiles.IndexOf(weapon);
                                        string line;
                                        while (!sr.EndOfStream)
                                        {
                                            line = sr.ReadLine();

                                            if (line.Contains(@"<replace") && line.Contains("sel") && line.Contains(@"/macros") && !line.Contains("@"))
                                            {
                                                this.UIModel.UIModelMissiles[index] = m_XmlExtractor.ReadSingleMissile(new FileInfo(file));
                                                break;
                                            }

                                            if (line.Contains("ammunition") && line.Contains("@value"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Ammunition = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("missile") && line.Contains("@amount"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.MissileAmount = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("missile") && line.Contains("@barrelamount"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.BarrelAmount = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("missile") && line.Contains("@lifetime"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Lifetime = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("missile") && line.Contains("@range"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Range = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("missile") && line.Contains("@guided"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Guided = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("missile") && line.Contains("@swarm"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Swarm = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("missile") && line.Contains("@retarget"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Retarget = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("explosiondamage") && line.Contains("@value"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Damage = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("reload") && line.Contains("@time"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Reload = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("hull") && line.Contains("@max"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Hull = Convert.ToInt32(value);
                                            }
                                            if (line.Contains("drag") && line.Contains("@forward"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Forward = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("drag") && line.Contains("@forward"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Forward = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("drag") && line.Contains("@reverse"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Reverse = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("drag") && line.Contains("@horizontal"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Horizontal = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("drag") && line.Contains("@vertical"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Vertical = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("drag") && line.Contains("@pitch"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Pitch = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("drag") && line.Contains("@yaw"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Yaw = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("drag") && line.Contains("@roll"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Roll = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("physics") && line.Contains("@mass"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.Mass = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("inertia") && line.Contains("@pitch"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.InertiaPitch = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("inertia") && line.Contains("@roll"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.InertiaRoll = Utility.ParseToDouble(value);
                                            }
                                            if (line.Contains("inertia") && line.Contains("@yaw"))
                                            {
                                                string value = line.Split('>')[1].Split('<')[0];
                                                weapon.InertiaYaw = Utility.ParseToDouble(value);
                                            }
                                        }
                                    }
                                
                                else
                                {
                                    UIModelMissile extractedMissile = m_XmlExtractor.ReadSingleMissile(new FileInfo(file));
                                    if (extractedMissile.Name.Length > 1)
                                        this.UIModel.UIModelMissiles.Add(extractedMissile);
                                }
                            }
                        }
                    }
                    //ships
                    if (dir.Contains("assets\\units\\size"))
                    {
                        List<string> files = Directory.GetFiles(dir, "*.xml", SearchOption.AllDirectories).ToList();

                        foreach (string file in files)
                        {
                            if (!file.Contains(@"\macros\"))
                                continue;
                            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            string[] fileDepth = fileStream.Name.Split('\\');
                            string fileName = fileDepth[fileDepth.Count() - 1];
                            using (StreamReader sr = new StreamReader(fileStream))
                            {
                                UIModelShip ship = null;
                                if (fileName.Contains("storage"))
                                {
                                    ship = this.UIModel.UIModelShips.FirstOrDefault(x => x.File.Contains(fileName.Replace("storage", "ship")));
                                }
                                else
                                    ship = this.UIModel.UIModelShips.FirstOrDefault(x => x.File.Contains(fileName));

                                if (ship != null)
                                {
                                    int index = this.UIModel.UIModelShips.IndexOf(ship);
                                    string line;
                                    while (!sr.EndOfStream)
                                    {
                                        line = sr.ReadLine();

                                        if (line.Contains(@"<replace") && line.Contains("sel") && line.Contains(@"/macros") && !line.Contains("@"))
                                        {
                                            this.UIModel.UIModelShips[index] = m_XmlExtractor.ReadSingleShipFile(new FileInfo(file));
                                            break;
                                        }

                                        if (line.Contains("@max") && line.Contains("hull"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.HullMax = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@value") && line.Contains("explosiondamage"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.ExplosionDamage = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@missile") && line.Contains("storage"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.StorageMissiles = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@unit") && line.Contains("storage"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.StorageUnits = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@level") && line.Contains("secrecy"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Secrecy = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@gas") && line.Contains("gatherrate"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.GatherRrate = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@capacity") && line.Contains("people"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.People = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@mass") && line.Contains("physics"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Mass = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@pitch") && line.Contains("inertia"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.InertiaPitch = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@yaw") && line.Contains("inertia"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.InertiaYaw = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@roll") && line.Contains("inertia"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.InertiaRoll = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@forward") && line.Contains("drag"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Forward = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@reverse") && line.Contains("drag"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Reverse = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@horizontal") && line.Contains("drag"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Horizontal = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@vertical") && line.Contains("drag"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Vertical = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@pitch") && line.Contains("drag"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Pitch = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@yaw") && line.Contains("drag"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Yaw = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@roll") && line.Contains("drag"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Roll = Utility.ParseToDouble(value);
                                        }
                                        if (line.Contains("@max") && line.Contains("cargo"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Cargo.CargoMax = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@tags") && line.Contains("cargo"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Cargo.CargoTags = value;
                                        }
                                    }
                                }
                                else
                                {
                                    UIModelShip extractedShip = m_XmlExtractor.ReadSingleShipFile(new FileInfo(file));
                                    if (extractedShip != null && extractedShip.Name.Length > 1 && extractedShip.Class != "storage")
                                        this.UIModel.UIModelShips.Add(extractedShip);
                                    //this.UIModel.UIModelShips.Add(m_XmlExtractor.ReadSingleShipFile(new FileInfo(file)));
                                }
                            }
                        }
                    }
                }
            }
        }
        //private double GetDoubleValue(string input)
        //{
        //    string[] inputArray = input.Split(';');
        //    string[] inputArray2 = inputArray[0].Split('/');
        //    string[] inputArray3 = inputArray[0].Split('=');
        //    string output = inputArray3[1];
        //    if (output.StartsWith("."))
        //    {
        //        output = "0" + output; 
        //    }
        //    return double.Parse(output, new NumberFormatInfo() { NumberDecimalSeparator = "." });
        //}
        //private string GetStringValue(string input)
        //{
        //    string[] inputArray = input.Split(';');
        //    string[] inputArray2 = inputArray[0].Split('/');
        //    string[] inputArray3 = inputArray[0].Split('=');
        //    string output = inputArray3[1];
        //    return output;
        //}
    }
}
