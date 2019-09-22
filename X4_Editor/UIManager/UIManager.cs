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

    class UIManager
    {
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();

        private string PathToShields = @"\assets\props\SurfaceElements\macros";
        private string PathToShips = @"\assets\units";
        private string PathToProjectiles = @"\assets\fx\weaponFx\macros";
        private string PathToMissiles = @"\assets\props\WeaponSystems\missile\macros";
        private string PathToTurretsStandard = @"\assets\props\WeaponSystems\standard\macros";
        private string PathToTurretsHeavy = @"\assets\props\WeaponSystems\heavy\macros";
        private string PathToTurretsEnergy = @"\assets\props\WeaponSystems\energy\macros";
        private string PathToTurretsCapital = @"\assets\props\WeaponSystems\capital\macros";
        private string PathToTurretsGuided = @"\assets\props\WeaponSystems\guided\macros";
        private string PathToTurretsDumbfire = @"\assets\props\WeaponSystems\dumbfire\macros";
        private string PathToWares = @"\libraries\wares.xml";
        private string PathToEngines = @"\assets\props\Engines\macros";

        public MainWindow MainWindow { get; set; }
        public WaresWindow WaresWindow { get; set; }
        public UIModel UIModel { get; set; }

        //protected CommandBindingCollection m_CommandBindings;
        //protected virtual CommandBindingCollection CommandBindings
        //{
        //    get
        //    {
        //        if (m_CommandBindings == null)
        //        {
        //            m_CommandBindings = new CommandBindingCollection();
        //        }
        //        return m_CommandBindings;
        //    }
        //}

        public UIManager()
        {
            MainWindow = new MainWindow();
            WaresWindow = new WaresWindow();
            this.UIModel = new UIModel();
            MainWindow.DataContext = this.UIModel;
            WaresWindow.DataContext = this.UIModel;
            MainWindow.Closing += OnClosing;
            
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.ReadAllVanillaFilesCommand, this.ExecuteReadAllVanillaFilesCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.ReadAllModFilesCommand, this.ExecuteReadAllModFilesCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.FilterCommand, this.ExecuteFilterCommand));
            //MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.ReadAllFBIFilesCommand, this.ExecuteReadAllFBIFilesCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.WriteAllChangedFilesCommand, this.ExecuteWriteAllChangedFilesCommand));
            //MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.PackAllFilesCommand, this.ExecutePackAllFilesCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.AddToValueCommand, this.ExecuteAddToValueCommand, CanExecuteCalculate));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.SubstractToValueCommand, this.ExecuteSubstractToValueCommand, CanExecuteCalculate));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.MultiplyToValueCommand, this.ExecuteMultiplyToValueCommand, CanExecuteCalculate));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.SetFixedValueCommand, this.ExecuteSetFixedValueCommand, CanExecuteCalculate));

            //MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.FilterUnitsCommand, this.ExecuteFilterUnitsCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.ShowWaresWindowCommand, this.ExecuteShowWaresWindowCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.OnMainWindowCellRightClick, this.ExecuteOnMainWindowCellRightClick));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.SelectFolderCommand, this.ExecuteSelectFolderCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.SelectModFolderCommand, this.ExecuteSelectModFolderCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.SelectExportFolderCommand, this.ExecuteSelectExportFolderCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.RecalculatePriceCommand, this.ExecuteRecalculatePriceCommand));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.OnWeaponDoubleClick, this.ExecuteOnWeaponDoubleClick));
            MainWindow.CommandBindings.Add(new CommandBinding(X4Commands.OnProjectileDoubleClick, this.ExecuteOnProjectileDoubleClick));

            WaresWindow.CommandBindings.Add(new CommandBinding(X4Commands.OnWaresWindowCellRightClick, this.ExecuteOnWaresWindowCellRightClick));
            this.LoadConfig();
            MainWindow.Show();
            WaresWindow.Owner = this.MainWindow;
        }

        private void ExecuteOnWeaponDoubleClick(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.MainWindow.DG_Weapons.SelectedCells.Count == 1)
            {
                DataGridCellInfo dataGridCell = this.MainWindow.DG_Weapons.SelectedCells[0];
                var item = dataGridCell.Item as UIModelWeapon;
                this.MainWindow.DG_Weapons.SelectedCells.Clear();
                this.UIModel.SetProjectileFilter(item.Projectile);
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
            if (Directory.Exists(this.UIModel.Path))
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

        private void ExecuteSelectModFolderCommand(object sender, ExecutedRoutedEventArgs e)
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
                    this.UIModel.ModPath = fbd.SelectedPath;
                    this.SaveConfig();
                }
            }
        }

        private void SaveConfig()
        {
            string path = Environment.CurrentDirectory;

            using (StreamWriter sw = new StreamWriter(path + "\\X4_Editor.cfg"))
            {
                if (!Directory.Exists(this.UIModel.Path))
                {
                    MessageBox.Show("You need to enter at least the vanilla folder path...", "No valid folder");
                    return;
                }
                sw.WriteLine(this.UIModel.Path);
                if (Directory.Exists(this.UIModel.ModPath))
                {
                    sw.WriteLine(this.UIModel.ModPath);
                }
                else
                    sw.WriteLine();
                if (Directory.Exists(this.UIModel.ExportPath))
                {
                    sw.WriteLine(this.UIModel.ExportPath);
                }
                else
                    sw.WriteLine();
            }
        }

        private void LoadConfig()
        {
            string path = Environment.CurrentDirectory;

            List<string> config = new List<string>();
            using (StreamReader sr = new StreamReader(path + "\\X4_Editor.cfg"))
            {
                if (!sr.EndOfStream)
                    config.Add(sr.ReadLine());
                if (!sr.EndOfStream)
                    config.Add(sr.ReadLine());
                if (!sr.EndOfStream)
                    config.Add(sr.ReadLine());
            }
            if (config.Count > 0)
                this.UIModel.Path = config[0];
            if (config.Count > 1)
                this.UIModel.ModPath = config[1];
            if (config.Count > 2)
                this.UIModel.ExportPath = config[2];
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
            var check = ParseToDouble(this.UIModel.MathParameter.ToString());
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
            var check = ParseToDouble(this.UIModel.MathParameter.ToString());
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
            
            var check = ParseToDouble(this.UIModel.MathParameter.ToString());

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
            dg_Projectiles = this.MainWindow.DG_Weapons;

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
                    if (dataGridCell.Column.DisplayIndex == 4)
                    {
                        shield.Max = (int)Calculate(operation, shield.Max);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        shield.Rate = Calculate(operation, shield.Rate);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 6)
                    {
                        shield.Delay = Calculate(operation, shield.Delay);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        shield.MaxHull = (int)Calculate(operation, shield.MaxHull);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        shield.Threshold = Calculate(operation, shield.Threshold);
                        counter.successcounter++;
                    }
                }

                var engine = dataGridCell.Item as UIModelEngine;
                if (engine != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 4)
                    {
                        engine.Forward = Calculate(operation, engine.Forward);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        engine.Reverse = Calculate(operation, engine.Reverse);
                        counter.successcounter++;
                    }

                    if (dataGridCell.Column.DisplayIndex == 6)
                    {
                        engine.BoostThrust = Calculate(operation, engine.BoostThrust);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        engine.BoostDuration = Calculate(operation, engine.BoostDuration);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        engine.BoostAttack = Calculate(operation, engine.BoostAttack);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 9)
                    {
                        engine.BoostRelease = Calculate(operation, engine.BoostRelease);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 10)
                    {
                        engine.TravelThrust = Calculate(operation, engine.TravelThrust);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 11)
                    {
                        engine.TravelCharge = (int)Calculate(operation, engine.TravelCharge);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 12)
                    {
                        engine.TravelAttack = Calculate(operation, engine.TravelAttack);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 13)
                    {
                        engine.TravelRelease = Calculate(operation, engine.TravelRelease);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        engine.Strafe = Calculate(operation, engine.Strafe);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 15)
                    {
                        engine.Yaw = Calculate(operation, engine.Yaw);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 16)
                    {
                        engine.Pitch = Calculate(operation, engine.Pitch);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 17)
                    {
                        engine.Roll = Calculate(operation, engine.Roll);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 18)
                    {
                        engine.AngularPitch = Calculate(operation, engine.AngularPitch);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 19)
                    {
                        engine.AngularRoll = Calculate(operation, engine.AngularRoll);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 20)
                    {
                        engine.MaxHull = (int)Calculate(operation, engine.MaxHull);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 21)
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
                        projectile.ReloadRate = Calculate(operation, projectile.ReloadRate);
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
                    if (dataGridCell.Column.DisplayIndex == 4)
                    {
                        weapon.RotationSpeed = Calculate(operation, weapon.RotationSpeed);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        weapon.RotationAcceleration = Calculate(operation, weapon.RotationAcceleration);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 6)
                    {
                        weapon.ReloadRate = Calculate(operation, weapon.ReloadRate);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        weapon.ReloadTime = Calculate(operation, weapon.ReloadTime);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        weapon.Overheat = (int)Calculate(operation, weapon.Overheat);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 9)
                    {
                        weapon.CoolDelay = Calculate(operation, weapon.CoolDelay);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 10)
                    {
                        weapon.CoolRate = (int)Calculate(operation, weapon.CoolRate);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 11)
                    {
                        weapon.Reenable = (int)Calculate(operation, weapon.Reenable);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 12)
                    {
                        weapon.HullMax = (int)Calculate(operation, weapon.HullMax);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 13)
                    {
                        weapon.HullThreshold = Calculate(operation, weapon.HullThreshold);
                        counter.successcounter++;
                    }
   
                }

                var missile = dataGridCell.Item as UIModelMissile;
                if (missile != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 1)
                    {
                        missile.Damage = (int)Calculate(operation, missile.Damage);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 2)
                    {
                        missile.Reload = Calculate(operation, missile.Reload);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 3)
                    {
                        missile.Range = (int)Calculate(operation, missile.Range);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 4)
                    {
                        missile.Lifetime = Calculate(operation, missile.Lifetime);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        missile.Forward = Calculate(operation, missile.Forward);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 6)
                    {
                        missile.Reverse = Calculate(operation, missile.Reverse);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        missile.Guided = (int)Calculate(operation, missile.Guided);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        missile.Swarm = (int)Calculate(operation, missile.Swarm);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 9)
                    {
                        missile.Retarget = (int)Calculate(operation, missile.Retarget);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 10)
                    {
                        missile.Hull = (int)Calculate(operation, missile.Hull);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 11)
                    {
                        missile.Ammunition = (int)Calculate(operation, missile.Ammunition);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 12)
                    {
                        missile.MissileAmount = (int)Calculate(operation, missile.MissileAmount);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 13)
                    {
                        missile.Mass = Calculate(operation, missile.Mass);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        missile.Horizontal = Calculate(operation, missile.Horizontal);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 15)
                    {
                        missile.Vertical = Calculate(operation, missile.Vertical);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 16)
                    {
                        missile.Pitch = Calculate(operation, missile.Pitch);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 17)
                    {
                        missile.Yaw = Calculate(operation, missile.Yaw);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 18)
                    {
                        missile.Roll = Calculate(operation, missile.Roll);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 19)
                    {
                        missile.InertiaPitch = Calculate(operation, missile.InertiaPitch);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 20)
                    {
                        missile.InertiaYaw = Calculate(operation, missile.InertiaYaw);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 21)
                    {
                        missile.InertiaRoll = Calculate(operation, missile.InertiaRoll);
                        counter.successcounter++;
                    }
                }

                var ship = dataGridCell.Item as UIModelShip;
                if (ship != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 4)
                    {
                        ship.HullMax = (int)Calculate(operation, ship.HullMax);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        ship.Cargo.CargoMax = (int)Calculate(operation, ship.Cargo.CargoMax);
                        counter.successcounter++;
                    }

                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        ship.StorageUnits = (int)Calculate(operation, ship.StorageUnits);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        ship.StorageMissiles = (int)Calculate(operation, ship.StorageMissiles);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 9)
                    {
                        ship.People = (int)Calculate(operation, ship.People);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 10)
                    {
                        ship.ExplosionDamage = (int)Calculate(operation, ship.ExplosionDamage);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 11)
                    {
                        ship.Secrecy = (int)Calculate(operation, ship.Secrecy);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 12)
                    {
                        ship.GatherRrate = Calculate(operation, ship.GatherRrate);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 13)
                    {
                        ship.Mass = Calculate(operation, ship.Mass);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        ship.Forward = Calculate(operation, ship.Forward);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 15)
                    {
                        ship.Reverse = Calculate(operation, ship.Reverse);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 16)
                    {
                        ship.Horizontal = Calculate(operation, ship.Horizontal);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 17)
                    {
                        ship.Vertical = Calculate(operation, ship.Vertical);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 18)
                    {
                        ship.Pitch = Calculate(operation, ship.Pitch);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 19)
                    {
                        ship.Yaw = Calculate(operation, ship.Yaw);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 20)
                    {
                        ship.Roll = Calculate(operation, ship.Roll);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 21)
                    {
                        ship.InertiaPitch = Calculate(operation, ship.InertiaPitch);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 22)
                    {
                        ship.InertiaYaw = Calculate(operation, ship.InertiaYaw);
                        counter.successcounter++;
                    }
                    if (dataGridCell.Column.DisplayIndex == 23)
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
            var check = ParseToDouble(this.UIModel.MathParameter.ToString());

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
            dg_Projectiles = this.MainWindow.DG_Weapons;

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
            var check = ParseToDouble(this.UIModel.MathParameter.ToString());

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
                    var vanillaItem = this.UIModel.UIModelModuleShieldsVanilla.Where(x => x.Name == item.Name).ToList()[0];

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
                    var vanillaItem = this.UIModel.UIModelModuleEnginesVanilla.Where(x => x.Name == item.Name).ToList()[0];

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
                    var vanillaItem = this.UIModel.UIModelShipsVanilla.Where(x => x.Name == item.Name).ToList()[0];

                    string shipClasssFolder = item.File.Split(new[] { "units" }, StringSplitOptions.None)[1].Split(new[] { "macros" }, StringSplitOptions.None)[0];

                    if (!Directory.Exists(this.UIModel.ExportPath + PathToShips + shipClasssFolder))
                    {
                        Directory.CreateDirectory(this.UIModel.ExportPath + PathToShips + shipClasssFolder);
                    }

                    string outputPath = this.UIModel.ExportPath + PathToShips + shipClasssFolder + item.File.Split(new[] { "macros" }, StringSplitOptions.None)[1];

                    if (item.Changed)
                    {
                        using (StreamWriter sw = new StreamWriter(outputPath))
                        {
                            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                            sw.WriteLine("<diff> ");
                            if (vanillaItem.HullMax != item.HullMax)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@max\">" + item.HullMax + "</replace>");
                            if (vanillaItem.ExplosionDamage != item.ExplosionDamage)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/boost/@explosiondamage\">" + item.ExplosionDamage + "</replace>");
                            if (vanillaItem.StorageMissiles != item.StorageMissiles)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/storage/@missile\">" + item.StorageMissiles + "</replace>");
                            if (vanillaItem.StorageUnits != item.StorageUnits)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/storage/@unit\">" + item.StorageUnits + "</replace>");
                            if (vanillaItem.Secrecy != item.Secrecy)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/secrecy/@level\">" + item.Secrecy + "</replace>");
                            if (vanillaItem.People != item.People)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/people/@capacity\">" + item.People + "</replace>");
                            if (vanillaItem.Mass != item.Mass)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/@mass\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Mass) + "</replace>");
                            if (vanillaItem.InertiaPitch != item.InertiaPitch)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@pitch\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaPitch) + "</replace>");
                            if (vanillaItem.InertiaYaw != item.InertiaYaw)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@yaw\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaYaw) + "</replace>");
                            if (vanillaItem.InertiaRoll != item.InertiaRoll)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@roll\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaRoll) + "</replace>");
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
                            sw.WriteLine("</diff> ");
                        }


                    }
                    if (item.Cargo != null && item.Cargo.Changed)
                    {
                        using (StreamWriter sw = new StreamWriter(outputPath.Replace("ship", "storage")))
                        {
                            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                            sw.WriteLine("<diff> ");
                            if (vanillaItem.Cargo.CargoMax != item.Cargo.CargoMax)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/cargo/@max\">" + item.Cargo.CargoMax + "</replace>");
                            if (vanillaItem.Cargo.CargoTags != item.Cargo.CargoTags)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/cargo/@tags\">" + item.Cargo.CargoTags + "</replace>");
                            sw.WriteLine("</diff> ");

                            fileswritten = true;
                        }
                    }
                }

                foreach (var item in this.UIModel.UIModelProjectiles)
                {
                    var vanillaItem = this.UIModel.UIModelProjectilesVanilla.Where(x => x.Name == item.Name).ToList()[0];

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
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@angle\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Angle) + "</replace>");
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

                            sw.WriteLine("</diff> ");

                            fileswritten = true;
                        }
                    }
                }

                foreach (var item in this.UIModel.UIModelWeapons)
                {
                    var vanillaItem = this.UIModel.UIModelWeaponsVanilla.Where(x => x.Name == item.Name).ToList()[0];

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

                foreach (var item in this.UIModel.UIModelWares)
                {
                    var vanillaItem = this.UIModel.UIModelWaresVanilla.Where(x => x.Name == item.Name).ToList()[0];

                    //if (item.Changed)
                    //{
                    //    if (!Directory.Exists(this.UIModel.ExportPath + PathToWares))
                    //    {
                    //        Directory.CreateDirectory(this.UIModel.ExportPath + PathToWares);
                    //    }

                    //    string outputPath = this.UIModel.ExportPath + PathToWares + item.File.Split(new[] { "macros" }, StringSplitOptions.None)[1];
                    //    using (StreamWriter sw = new StreamWriter(outputPath))
                    //    {
                    //        sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    //        sw.WriteLine("<diff> ");
                    //        if (vanillaItem.Max != item.Max)
                    //            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/recharge/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Max) + "</replace>");
                    //        if (vanillaItem.Rate != item.Rate)
                    //            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/recharge/@rate\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Rate) + "</replace>");
                    //        if (vanillaItem.Delay != item.Delay)
                    //            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/recharge/@delay\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Delay) + "</replace>");
                    //        if (vanillaItem.MaxHull != item.MaxHull)
                    //            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.MaxHull) + "</replace>");
                    //        if (vanillaItem.Threshold != item.Threshold)
                    //            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@threshold\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Threshold) + "</replace>");
                    //        sw.WriteLine("</diff> ");
                    //    }
                    //}
                }

                foreach (var item in this.UIModel.UIModelMissiles)
                {
                    var vanillaItem = this.UIModel.UIModelMissilesVanilla.Where(x => x.Name == item.Name).ToList()[0];

                    if (item.Changed)
                    {
                        if (!Directory.Exists(this.UIModel.ExportPath + PathToMissiles))
                        {
                            Directory.CreateDirectory(this.UIModel.ExportPath + PathToMissiles);
                        }

                        string outputPath = this.UIModel.ExportPath + PathToShields + item.File.Split(new[] { "macros" }, StringSplitOptions.None)[1];
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
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/reload/@time\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Damage) + "</replace>");
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

                            if (vanillaItem.InertiaMass != item.InertiaMass)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@mass\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaMass) + "</replace>");
                            if (vanillaItem.InertiaPitch != item.InertiaPitch)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@inertiapitch\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaPitch) + "</replace>");
                            if (vanillaItem.InertiaRoll != item.InertiaRoll)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@inertiaroll\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaRoll) + "</replace>");
                            if (vanillaItem.InertiaYaw != item.InertiaYaw)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@inertiayaw\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaYaw) + "</replace>");
                            sw.WriteLine("</diff> ");

                            fileswritten = true;
                        }
                    }
                }
                if (fileswritten)
                    MessageBox.Show("Mod files have been created.");
                else
                    MessageBox.Show("No changes detected - no files written.");
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

            this.UIModel.AllWaresLoaded = false;

            string folderPath = this.UIModel.Path.Replace(@"\\", @"\");

            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("Enter a valid folder path for extracted vanilla files", "No valid folder");
            }
            string waresPath = folderPath + PathToWares;
            ReadAllWares(waresPath);

            string weaponsPath = folderPath + PathToProjectiles;
            List<FileInfo> xmlWeaponsList = new List<FileInfo>();
            xmlWeaponsList = this.GetAllXmlInSubFolders(weaponsPath, xmlWeaponsList);
            ReadAllProjectiles(xmlWeaponsList);

            string turretsPath = folderPath + PathToTurretsStandard;
            List<FileInfo> xmlTurretsList = new List<FileInfo>();
            xmlTurretsList = this.GetAllXmlInSubFolders(PathToTurretsStandard, xmlTurretsList);
            xmlTurretsList = this.GetAllXmlInSubFolders(folderPath + PathToTurretsEnergy, xmlTurretsList);
            xmlTurretsList = this.GetAllXmlInSubFolders(folderPath + PathToTurretsHeavy, xmlTurretsList);
            xmlTurretsList = this.GetAllXmlInSubFolders(folderPath + PathToTurretsCapital, xmlTurretsList);
            xmlTurretsList = this.GetAllXmlInSubFolders(folderPath + PathToTurretsGuided, xmlTurretsList);
            xmlTurretsList = this.GetAllXmlInSubFolders(folderPath + PathToTurretsDumbfire, xmlTurretsList);
            ReadAllWeapons(xmlTurretsList);

            string missilesPath = folderPath + PathToMissiles;
            //string torpedosPath = folderPath + @"\props\WeaponSystems\torpedo\macros";
            List<FileInfo> xmlMissilesList = new List<FileInfo>();
            xmlMissilesList = this.GetAllXmlInSubFolders(missilesPath, xmlMissilesList);
            //xmlMissilesList.AddRange(this.GetAllXmlInSubFolders(torpedosPath, xmlMissilesList));
            ReadAllMissiles(xmlMissilesList);

            string shieldsPath = folderPath + PathToShields;
            List<FileInfo> xmlShieldsList = new List<FileInfo>();
            xmlShieldsList = this.GetAllXmlInSubFolders(shieldsPath, xmlShieldsList);
            ReadAllShields(xmlShieldsList);

            string enginesPath = folderPath + PathToEngines;
            List<FileInfo> xmlEnginesList = new List<FileInfo>();
            xmlEnginesList = this.GetAllXmlInSubFolders(enginesPath, xmlEnginesList);
            ReadAllEngines(xmlEnginesList);

            string shipsPath = folderPath + PathToShips;
            List<FileInfo> xmlShipsList = new List<FileInfo>();
            xmlShipsList = this.GetAllXmlInSubFolders(shipsPath, xmlShipsList);
            ReadAllShips(xmlShipsList);

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
                foreach (var item in xmlShieldsList)
                {
                    this.ReadSingleShield(item);
                    //using (XmlReader reader = XmlReader.Create(item.FullName))
                    //{
                    //    while (reader.Read())
                    //    {
                    //        if (reader.NodeType == XmlNodeType.Element && reader.Name == "macros")
                    //        {
                    //            XDocument doc = XDocument.Load(item.FullName);

                    //            var shields = doc.Descendants("macro")
                    //                .Where(p => (string)p.Attribute("class") == "shieldgenerator");

                    //            foreach (var shield in shields)
                    //            {
                    //                XmlDocument xD = new XmlDocument();
                    //                xD.LoadXml(shield.ToString());
                    //                XmlNode xN = XmlHelper.ToXmlNode(shield);
                    //                XmlNodeList shielComponentNode = xN.SelectNodes("//component");
                    //                XmlNodeList shielMacroNode = xN.SelectNodes("//macro");
                    //                XmlNodeList shielIdentificationNode = xN.SelectNodes("//properties/identification");
                    //                XmlNodeList shielRechargedNode = xN.SelectNodes("//properties/recharge");
                    //                XmlNodeList shieldHullNode = xN.SelectNodes("//properties/hull");

                    //                UIModelShield uiModelShield = new UIModelShield()
                    //                {
                    //                    File = item.FullName,
                    //                    Name = shielMacroNode[0].Attributes["name"].Value,
                    //                    Faction = shielIdentificationNode[0].Attributes["makerrace"].Value,
                    //                    MK = shielIdentificationNode[0].Attributes["mk"].Value,
                    //                    Max = Convert.ToInt32(shielRechargedNode[0].Attributes["max"].Value),
                    //                    Rate = ParseToDouble(shielRechargedNode[0].Attributes["rate"].Value),
                    //                    Delay = ParseToDouble(shielRechargedNode[0].Attributes["delay"].Value)

                    //                };
                    //                if (shieldHullNode.Count > 0)
                    //                {
                    //                    if (shieldHullNode[0].Attributes["max"] != null)
                    //                        uiModelShield.MaxHull = ParseToDouble(shieldHullNode[0].Attributes["max"].Value);
                    //                    if (shieldHullNode[0].Attributes["threshold"] != null)
                    //                        uiModelShield.Threshold = ParseToDouble(shieldHullNode[0].Attributes["threshold"].Value);
                    //                }
                    //                uiModelShield.Changed = false;
                    //                this.UIModel.UIModelModulesShields.Add(uiModelShield);
                    //            }
                    //        }
                    //    }
                    //}
                }
                this.UIModel.UIModelModuleShieldsVanilla.Clear();
                foreach (var item in this.UIModel.UIModelModulesShields)
                {
                    this.UIModel.UIModelModuleShieldsVanilla.Add(item.Copy());
                }
            }
        }

        private void ReadSingleShield(FileInfo file)
        {
            using (XmlReader reader = XmlReader.Create(file.FullName))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "macros")
                    {
                        XDocument doc = XDocument.Load(file.FullName);

                        var shields = doc.Descendants("macro")
                            .Where(p => (string)p.Attribute("class") == "shieldgenerator");

                        foreach (var shield in shields)
                        {
                            XmlDocument xD = new XmlDocument();
                            xD.LoadXml(shield.ToString());
                            XmlNode xN = XmlHelper.ToXmlNode(shield);
                            XmlNodeList shielComponentNode = xN.SelectNodes("//component");
                            XmlNodeList shielMacroNode = xN.SelectNodes("//macro");
                            XmlNodeList shielIdentificationNode = xN.SelectNodes("//properties/identification");
                            XmlNodeList shielRechargedNode = xN.SelectNodes("//properties/recharge");
                            XmlNodeList shieldHullNode = xN.SelectNodes("//properties/hull");

                            UIModelShield uiModelShield = new UIModelShield()
                            {
                                File = file.FullName,
                                Name = shielMacroNode[0].Attributes["name"].Value,
                                //Faction = shielIdentificationNode[0].Attributes["makerrace"].Value,
                                //MK = shielIdentificationNode[0].Attributes["mk"].Value,
                                Max = Convert.ToInt32(shielRechargedNode[0].Attributes["max"].Value),
                                Rate = ParseToDouble(shielRechargedNode[0].Attributes["rate"].Value),
                                Delay = ParseToDouble(shielRechargedNode[0].Attributes["delay"].Value)

                            };
                            // not neccessary
                            if (shielIdentificationNode.Count > 0)
                            {
                                uiModelShield.Faction = shielIdentificationNode[0].Attributes["makerrace"].Value;
                                uiModelShield.MK = shielIdentificationNode[0].Attributes["mk"].Value;
                            }
                            if (shieldHullNode.Count > 0)
                            {
                                if (shieldHullNode[0].Attributes["max"] != null)
                                    uiModelShield.MaxHull = ParseToDouble(shieldHullNode[0].Attributes["max"].Value);
                                if (shieldHullNode[0].Attributes["threshold"] != null)
                                    uiModelShield.Threshold = ParseToDouble(shieldHullNode[0].Attributes["threshold"].Value);
                            }
                            uiModelShield.Changed = false;
                            this.UIModel.UIModelModulesShields.Add(uiModelShield);
                        }
                    }
                }
            }
        }

        private void ReadAllEngines(List<FileInfo> xmlEnginesList)
        {
            string folderPath = this.UIModel.Path.Replace(@"\\", @"\");
            if (!Directory.Exists(folderPath + PathToEngines))
            {
                MessageBox.Show("No valid engines found.", "No data found.");
            }
            else
            {
                foreach (var item in xmlEnginesList)
                {
                    using (XmlReader reader = XmlReader.Create(item.FullName))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "macros")
                            {
                                XDocument doc = XDocument.Load(item.FullName);

                                var engines = doc.Descendants("macro")
                                    .Where(p => (string)p.Attribute("class") == "engine");

                                foreach (var engine in engines)
                                {
                                    XmlDocument xD = new XmlDocument();
                                    xD.LoadXml(engine.ToString());
                                    XmlNode xN = XmlHelper.ToXmlNode(engine);
                                    XmlNodeList engineComponentNode = xN.SelectNodes("//component");
                                    XmlNodeList engineMacroNode = xN.SelectNodes("//macro");
                                    XmlNodeList engineIdentificationNode = xN.SelectNodes("//properties/identification");
                                    XmlNodeList engineBoostNode = xN.SelectNodes("//properties/boost");
                                    XmlNodeList engineTravelNode = xN.SelectNodes("//properties/travel");
                                    XmlNodeList engineThrustlNode = xN.SelectNodes("//properties/thrust");
                                    XmlNodeList engineAngularNode = xN.SelectNodes("//properties/angular");
                                    XmlNodeList engineHullNode = xN.SelectNodes("//properties/hull");

                                    UIModelEngine uiModelEngine = new UIModelEngine()
                                    {
                                        File = item.FullName,
                                        Name = engineMacroNode[0].Attributes["name"].Value,


                                    };
                                    if (engineIdentificationNode.Count > 0)
                                    {
                                        if (engineIdentificationNode[0].Attributes["makerrace"] != null)
                                            uiModelEngine.Faction = engineIdentificationNode[0].Attributes["makerrace"].Value;
                                        if (engineIdentificationNode[0].Attributes["mk"] != null)
                                            uiModelEngine.MK = engineIdentificationNode[0].Attributes["mk"].Value;
                                    }
                                    if (engineBoostNode.Count > 0)
                                    {
                                        if (engineBoostNode[0].Attributes["attack"] != null)
                                            uiModelEngine.BoostAttack = ParseToDouble(engineBoostNode[0].Attributes["attack"].Value);
                                        if (engineBoostNode[0].Attributes["attack"] != null)
                                            uiModelEngine.BoostThrust = ParseToDouble(engineBoostNode[0].Attributes["thrust"].Value);
                                        if (engineBoostNode[0].Attributes["duration"] != null)
                                            uiModelEngine.BoostRelease = ParseToDouble(engineBoostNode[0].Attributes["release"].Value);
                                        if (engineBoostNode[0].Attributes["duration"] != null)
                                            uiModelEngine.BoostDuration = ParseToDouble(engineBoostNode[0].Attributes["duration"].Value);
                                    }
                                    if (engineThrustlNode.Count > 0)
                                    {
                                        if (engineThrustlNode[0].Attributes["forward"] != null)
                                            uiModelEngine.Forward = ParseToDouble(engineThrustlNode[0].Attributes["forward"].Value);
                                        if (engineThrustlNode[0].Attributes["reverse"] != null)
                                            uiModelEngine.Reverse = ParseToDouble(engineThrustlNode[0].Attributes["reverse"].Value);
                                        if (engineThrustlNode[0].Attributes["strafe"] != null)
                                            uiModelEngine.Strafe = ParseToDouble(engineThrustlNode[0].Attributes["strafe"].Value);
                                        if (engineThrustlNode[0].Attributes["pitch"] != null)
                                            uiModelEngine.Pitch = ParseToDouble(engineThrustlNode[0].Attributes["pitch"].Value);
                                        if (engineThrustlNode[0].Attributes["yaw"] != null)
                                            uiModelEngine.Yaw = ParseToDouble(engineThrustlNode[0].Attributes["yaw"].Value);
                                        if (engineThrustlNode[0].Attributes["roll"] != null)
                                            uiModelEngine.Roll = ParseToDouble(engineThrustlNode[0].Attributes["roll"].Value);
                                    }
                                    if (engineTravelNode.Count > 0)
                                    {
                                        if (engineTravelNode[0].Attributes["attack"] != null)
                                            uiModelEngine.TravelAttack = ParseToDouble(engineTravelNode[0].Attributes["attack"].Value);
                                        if (engineTravelNode[0].Attributes["charge"] != null)
                                            uiModelEngine.TravelCharge = Convert.ToInt32(engineTravelNode[0].Attributes["charge"].Value);
                                        if (engineTravelNode[0].Attributes["release"] != null)
                                            uiModelEngine.TravelRelease = ParseToDouble(engineTravelNode[0].Attributes["release"].Value);
                                        if (engineTravelNode[0].Attributes["thrust"] != null)
                                            uiModelEngine.TravelThrust = ParseToDouble(engineTravelNode[0].Attributes["thrust"].Value);
                                    }
                                    if (engineAngularNode.Count > 0)
                                    {
                                        if (engineAngularNode[0].Attributes["roll"] != null)
                                            uiModelEngine.AngularPitch = ParseToDouble(engineAngularNode[0].Attributes["roll"].Value);
                                        if (engineAngularNode[0].Attributes["pitch"] != null)
                                            uiModelEngine.AngularRoll = ParseToDouble(engineAngularNode[0].Attributes["pitch"].Value);
                                    }
                                    if (engineHullNode.Count > 0)
                                    {
                                        if (engineHullNode[0].Attributes["max"] != null)
                                            uiModelEngine.MaxHull = ParseToDouble(engineHullNode[0].Attributes["max"].Value);
                                        if (engineHullNode[0].Attributes["threshold"] != null)
                                            uiModelEngine.Threshold = ParseToDouble(engineHullNode[0].Attributes["threshold"].Value);
                                    }
                                    uiModelEngine.Changed = false;

                                    this.UIModel.UIModelModulesEngines.Add(uiModelEngine);
                                }
                            }
                        }
                    }
                }
                this.UIModel.UIModelModuleEnginesVanilla.Clear();
                foreach (var item in this.UIModel.UIModelModulesEngines)
                {
                    this.UIModel.UIModelModuleEnginesVanilla.Add(item.Copy());
                }
            }
        }

        private void ReadAllWares(string waresPath)
        {
            if (!File.Exists(waresPath))
            {
                MessageBox.Show("No valid wares found.", "No data found.");
            }
            else
            {
                using (XmlReader reader = XmlReader.Create(waresPath))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "wares")
                        {
                            XDocument doc = XDocument.Load(waresPath);

                            var wares = doc.Descendants("wares");

                            foreach (var ware in wares)
                            {
                                XmlDocument xD = new XmlDocument();
                                xD.LoadXml(ware.ToString());
                                XmlNode xN = XmlHelper.ToXmlNode(ware);
                                XmlNodeList wareNodes = xN.SelectNodes("//ware");

                                foreach (XmlNode item in wareNodes)
                                {

                                    if (item.Attributes["id"] != null)
                                    {

                                        XmlNode priceNode = null;
                                        foreach (XmlNode child in item.SelectNodes("price"))
                                        {
                                            priceNode = child;
                                        }

                                        UIModelWare uiModelWare = new UIModelWare()
                                        {
                                            File = waresPath,
                                            Name = item.Attributes["id"].Value,
                                            Max = Convert.ToInt32(priceNode.Attributes["max"].Value),
                                            Min = Convert.ToInt32(priceNode.Attributes["min"].Value),
                                            Avg = Convert.ToInt32(priceNode.Attributes["average"].Value),
                                        };

                                        if (item.SelectNodes("./production").Count == 1)
                                        {
                                            XmlNodeList productionNode = item.SelectNodes("./production");
                                            if (productionNode.Count > 0)
                                            {
                                                uiModelWare.Time = Convert.ToInt32(productionNode[0].Attributes["time"].Value);
                                                uiModelWare.Amount = Convert.ToInt32(productionNode[0].Attributes["amount"].Value);
                                            }
                                            for (int i = 0; i < item.SelectNodes("./production/primary/ware").Count; i++)
                                            {
                                                if (i == 0)
                                                {
                                                    uiModelWare.Ware1 = item.SelectNodes("./production/primary/ware")[i].Attributes["ware"].Value;
                                                    uiModelWare.Amount1 = Convert.ToInt32(item.SelectNodes("./production/primary/ware")[i].Attributes["amount"].Value);
                                                }
                                                if (i == 1)
                                                {
                                                    uiModelWare.Ware2 = item.SelectNodes("./production/primary/ware")[i].Attributes["ware"].Value;
                                                    uiModelWare.Amount2 = Convert.ToInt32(item.SelectNodes("./production/primary/ware")[i].Attributes["amount"].Value);
                                                }
                                                if (i == 2)
                                                {
                                                    uiModelWare.Ware3 = item.SelectNodes("./production/primary/ware")[i].Attributes["ware"].Value;
                                                    uiModelWare.Amount3 = Convert.ToInt32(item.SelectNodes("./production/primary/ware")[i].Attributes["amount"].Value);
                                                }
                                                if (i == 3)
                                                {
                                                    uiModelWare.Ware4 = item.SelectNodes("./production/primary/ware")[i].Attributes["ware"].Value;
                                                    uiModelWare.Amount4 = Convert.ToInt32(item.SelectNodes("./production/primary/ware")[i].Attributes["amount"].Value);
                                                }
                                                if (i == 4)
                                                {
                                                    uiModelWare.Ware5 = item.SelectNodes("./production/primary/ware")[i].Attributes["ware"].Value;
                                                    uiModelWare.Amount5 = Convert.ToInt32(item.SelectNodes("./production/primary/ware")[i].Attributes["amount"].Value);
                                                }
                                            }
                                        }
                                        else if (item.SelectNodes("./production").Count == 2)
                                        {
                                            int noXenon = 0;
                                            bool xenon = false;

                                            XmlNodeList productionNodes = item.SelectNodes("./production");
                                            if (item.SelectNodes("./production")[0].Attributes["method"].Value == "xenon" && item.SelectNodes("./production")[1].Attributes["method"].Value == "default")
                                            {
                                                noXenon = 1;
                                                xenon = true;
                                            }
                                            if (item.SelectNodes("./production")[1].Attributes["method"].Value == "xenon" && item.SelectNodes("./production")[0].Attributes["method"].Value == "default")
                                            {
                                                noXenon = 0;
                                                xenon = true;
                                            }
                                            // only if one of the two production ways is xenon and the other one is default, show default
                                            if (xenon)
                                            {
                                                productionNodes = item.SelectNodes("./production")[noXenon].ChildNodes[0].ChildNodes;

                                                if (productionNodes.Count > 0)
                                                {
                                                    uiModelWare.Time = ParseToDouble(item.SelectNodes("./production")[noXenon].Attributes["time"].Value);
                                                    uiModelWare.Amount = Convert.ToInt32(item.SelectNodes("./production")[noXenon].Attributes["amount"].Value);
                                                }
                                                for (int i = 0; i < item.SelectNodes("./production/primary/ware").Count; i++)
                                                {
                                                    if (i == 0 && productionNodes[i] != null)
                                                    {
                                                        uiModelWare.Ware1 = productionNodes[i].Attributes["ware"].Value;
                                                        uiModelWare.Amount1 = Convert.ToInt32(productionNodes[i].Attributes["amount"].Value);
                                                    }
                                                    if (i == 1 && productionNodes[i] != null)
                                                    {
                                                        uiModelWare.Ware2 = productionNodes[i].Attributes["ware"].Value;
                                                        uiModelWare.Amount2 = Convert.ToInt32(productionNodes[i].Attributes["amount"].Value);
                                                    }
                                                    if (i == 2 && productionNodes[i] != null)
                                                    {
                                                        uiModelWare.Ware3 = productionNodes[i].Attributes["ware"].Value;
                                                        uiModelWare.Amount3 = Convert.ToInt32(productionNodes[i].Attributes["amount"].Value);
                                                    }
                                                    if (i == 3 && productionNodes[i] != null)
                                                    {
                                                        uiModelWare.Ware4 = productionNodes[i].Attributes["ware"].Value;
                                                        uiModelWare.Amount4 = Convert.ToInt32(productionNodes[i].Attributes["amount"].Value);
                                                    }
                                                    if (i == 4 && productionNodes[i] != null)
                                                    {
                                                        uiModelWare.Ware5 = productionNodes[i].Attributes["ware"].Value;
                                                        uiModelWare.Amount5 = Convert.ToInt32(productionNodes[i].Attributes["amount"].Value);
                                                    }
                                                }
                                            }
                                        }
                                        uiModelWare.Changed = false;

                                        this.UIModel.UIModelWares.Add(uiModelWare);
                                    }
                                }
                            }
                        }
                    }
                }
                this.UIModel.AllWaresLoaded = true;
                this.UIModel.CalculateWarePrices();

                this.UIModel.UIModelWaresVanilla.Clear();
                foreach (var item in this.UIModel.UIModelWares)
                {
                    this.UIModel.UIModelWaresVanilla.Add(item.Copy());
                }
            }
        }

        private void ReadAllProjectiles(List<FileInfo> xmlProjectilesList)
        {
            string folderPath = this.UIModel.Path.Replace(@"\\", @"\");
            if (!Directory.Exists(folderPath + PathToProjectiles))
            {
                MessageBox.Show("No valid weapons found.", "No data found.");
            }

            foreach (var item in xmlProjectilesList)
            {
                using (XmlReader reader = XmlReader.Create(item.FullName))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "macros")
                        {
                            XDocument doc = XDocument.Load(item.FullName);

                            var weapons = doc.Descendants("macro").Where(p => (string)p.Attribute("class") == "bullet");

                            foreach (var weapon in weapons)
                            {
                                XmlDocument xD = new XmlDocument();
                                xD.LoadXml(weapon.ToString());
                                XmlNode xN = XmlHelper.ToXmlNode(weapon);
                                XmlNodeList weaponMacroNode = xN.SelectNodes("//macro");
                                XmlNodeList weaponComponentNode = xN.SelectNodes("//component");
                                XmlNodeList weaponAmmunitionNode = xN.SelectNodes("//properties/ammunition");
                                XmlNodeList weaponBulletNode = xN.SelectNodes("//properties/bullet");
                                XmlNodeList weaponHeatNode = xN.SelectNodes("//properties/heat");
                                XmlNodeList weaponReloadNode = xN.SelectNodes("//properties/reload");
                                XmlNodeList weaponDamageNode = xN.SelectNodes("//properties/damage");

                                UIModelProjectile uiModelWeapon = new UIModelProjectile()
                                {
                                    File = item.FullName,
                                    Name = weaponMacroNode[0].Attributes["name"].Value
                                };
                                if (weaponAmmunitionNode.Count > 0)
                                {
                                    if (weaponAmmunitionNode[0].Attributes["value"] != null)
                                        uiModelWeapon.Ammunition = Convert.ToInt32(weaponAmmunitionNode[0].Attributes["value"].Value);
                                    if (weaponAmmunitionNode[0].Attributes["reload"] != null)
                                        uiModelWeapon.AmmunitionReload = ParseToDouble(weaponAmmunitionNode[0].Attributes["reload"].Value);
                                }
                                if (weaponBulletNode.Count > 0)
                                {
                                    if (weaponBulletNode[0].Attributes["speed"] != null)
                                        uiModelWeapon.Speed = Convert.ToInt32(weaponBulletNode[0].Attributes["speed"].Value);
                                    if (weaponBulletNode[0].Attributes["lifetime"] != null)
                                        uiModelWeapon.Lifetime = ParseToDouble(weaponBulletNode[0].Attributes["lifetime"].Value);
                                    if (weaponBulletNode[0].Attributes["amount"] != null)
                                        uiModelWeapon.Amount = Convert.ToInt32(weaponBulletNode[0].Attributes["amount"].Value);
                                    if (weaponBulletNode[0].Attributes["range"] != null)
                                        uiModelWeapon.Range = Convert.ToInt32(weaponBulletNode[0].Attributes["range"].Value);
                                    if (weaponBulletNode[0].Attributes["maxhits"] != null)
                                        uiModelWeapon.MaxHits = Convert.ToInt32(weaponBulletNode[0].Attributes["maxhits"].Value);
                                    if (weaponBulletNode[0].Attributes["ricochet"] != null)
                                        uiModelWeapon.Ricochet = ParseToDouble(weaponBulletNode[0].Attributes["ricochet"].Value);
                                    if (weaponBulletNode[0].Attributes["scale"] != null)
                                        uiModelWeapon.Scale = Convert.ToInt32(weaponBulletNode[0].Attributes["scale"].Value);
                                    if (weaponBulletNode[0].Attributes["chargetime"] != null)
                                        uiModelWeapon.ChargeTime = ParseToDouble(weaponBulletNode[0].Attributes["chargetime"].Value);
                                    if (weaponBulletNode[0].Attributes["timediff"] != null)
                                        uiModelWeapon.TimeDiff = ParseToDouble(weaponBulletNode[0].Attributes["timediff"].Value);
                                    if (weaponBulletNode[0].Attributes["angle"] != null)
                                        uiModelWeapon.Angle = ParseToDouble(weaponBulletNode[0].Attributes["angle"].Value);
                                }
                                if (weaponHeatNode.Count > 0)
                                {
                                    if (weaponHeatNode[0].Attributes["initial"] != null)
                                        uiModelWeapon.HeatInitial = Convert.ToInt32(weaponHeatNode[0].Attributes["initial"].Value);
                                    if (weaponHeatNode[0].Attributes["value"] != null)
                                        uiModelWeapon.HeatValue = Convert.ToInt32(weaponHeatNode[0].Attributes["value"].Value);
                                    
                                }
                                if (weaponReloadNode.Count > 0)
                                {
                                    if (weaponReloadNode[0].Attributes["rate"] != null)
                                        uiModelWeapon.ReloadRate = ParseToDouble(weaponReloadNode[0].Attributes["rate"].Value);
                                    if (weaponReloadNode[0].Attributes["time"] != null)
                                        uiModelWeapon.ReloadTime = ParseToDouble(weaponReloadNode[0].Attributes["time"].Value);

                                }
                                if (weaponDamageNode.Count > 0)
                                {
                                    if (weaponDamageNode[0].Attributes["value"] != null)
                                        uiModelWeapon.Damage = ParseToDouble(weaponDamageNode[0].Attributes["value"].Value);
                                    if (weaponDamageNode[0].Attributes["shield"] != null)
                                        uiModelWeapon.Shield = ParseToDouble(weaponDamageNode[0].Attributes["shield"].Value);
                                    if (weaponDamageNode[0].Attributes["repair"] != null)
                                        uiModelWeapon.Repair = ParseToDouble(weaponDamageNode[0].Attributes["repair"].Value);
                                }
                                uiModelWeapon.Changed = false;
                                this.UIModel.UIModelProjectiles.Add(uiModelWeapon);
                            }
                        }
                    }
                }
            }
            this.UIModel.UIModelProjectilesVanilla.Clear();
            foreach (var item in this.UIModel.UIModelProjectiles)
            {
                this.UIModel.UIModelProjectilesVanilla.Add(item.Copy());
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
                using (XmlReader reader = XmlReader.Create(item.FullName))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "macros")
                        {
                            XDocument doc = XDocument.Load(item.FullName);

                            var weapons = doc.Descendants("macro").Where(p => (string)p.Attribute("class") == "turret" || (string)p.Attribute("class") == "weapon");

                            foreach (var weapon in weapons)
                            {
                                XmlDocument xD = new XmlDocument();
                                xD.LoadXml(weapon.ToString());
                                XmlNode xN = XmlHelper.ToXmlNode(weapon);
                                XmlNodeList weaponComponentNode = xN.SelectNodes("//component");
                                XmlNodeList weaponMacroNode = xN.SelectNodes("//macro");
                                XmlNodeList weaponIdentificationNode = xN.SelectNodes("//properties/identification");
                                XmlNodeList weaponBulletNode = xN.SelectNodes("//properties/bullet");
                                XmlNodeList weaponRotationspeedNode = xN.SelectNodes("//properties/rotationspeed");
                                XmlNodeList weaponRotationAccelerationNode = xN.SelectNodes("//properties/rotationacceleration");
                                XmlNodeList weaponReloadNode = xN.SelectNodes("//properties/reload");
                                XmlNodeList weaponHullNode = xN.SelectNodes("//properties/hull");
                                XmlNodeList weaponHeatNode = xN.SelectNodes("//properties/heat");

                                UIModelWeapon uiModelWeapon = new UIModelWeapon()
                                {
                                    File = item.FullName,
                                    Name = weaponMacroNode[0].Attributes["name"].Value
                                };
                                if (weaponBulletNode.Count > 0)
                                {
                                    if (weaponBulletNode[0].Attributes["class"] != null)
                                        uiModelWeapon.Projectile = weaponBulletNode[0].Attributes["class"].Value;
                                }
                                if (weaponIdentificationNode.Count > 0)
                                {
                                    if (weaponIdentificationNode[0].Attributes["mk"] != null)
                                        uiModelWeapon.MK = weaponIdentificationNode[0].Attributes["mk"].Value;
                                }
                                if (weaponHeatNode.Count > 0)
                                {
                                    if (weaponHeatNode[0].Attributes["overheat"] != null)
                                        uiModelWeapon.Overheat = Convert.ToInt32(weaponHeatNode[0].Attributes["overheat"].Value);
                                    if (weaponHeatNode[0].Attributes["cooldelay"] != null)
                                        uiModelWeapon.CoolDelay = ParseToDouble(weaponHeatNode[0].Attributes["cooldelay"].Value);
                                    if (weaponHeatNode[0].Attributes["coolrate"] != null)
                                        uiModelWeapon.CoolRate = Convert.ToInt32(weaponHeatNode[0].Attributes["coolrate"].Value);
                                    if (weaponHeatNode[0].Attributes["reenable"] != null)
                                        uiModelWeapon.Reenable = Convert.ToInt32(weaponHeatNode[0].Attributes["reenable"].Value);

                                }
                                if (weaponRotationspeedNode.Count > 0)
                                {
                                    if (weaponRotationspeedNode[0].Attributes["max"] != null)
                                        uiModelWeapon.RotationSpeed = ParseToDouble(weaponRotationspeedNode[0].Attributes["max"].Value);
                                }
                                if (weaponRotationAccelerationNode.Count > 0)
                                {
                                    if (weaponRotationAccelerationNode[0].Attributes["max"] != null)
                                        uiModelWeapon.RotationAcceleration = ParseToDouble(weaponRotationAccelerationNode[0].Attributes["max"].Value);
                                }
                                if (weaponReloadNode.Count > 0)
                                {
                                    if (weaponReloadNode[0].Attributes["rate"] != null)
                                        uiModelWeapon.ReloadRate = ParseToDouble(weaponReloadNode[0].Attributes["rate"].Value);
                                    if (weaponReloadNode[0].Attributes["time"] != null)
                                        uiModelWeapon.ReloadTime = ParseToDouble(weaponReloadNode[0].Attributes["time"].Value);
                                }
                                if (weaponHullNode.Count > 0)
                                {
                                    if (weaponHullNode[0].Attributes["max"] != null)
                                        uiModelWeapon.HullMax = ParseToDouble(weaponHullNode[0].Attributes["max"].Value);
                                    if (weaponHullNode[0].Attributes["threshold"] != null)
                                        uiModelWeapon.HullThreshold = ParseToDouble(weaponHullNode[0].Attributes["threshold"].Value);
                                }
                                uiModelWeapon.Changed = false;
                                this.UIModel.UIModelWeapons.Add(uiModelWeapon);
                            }
                        }
                    }
                }
            }
            this.UIModel.UIModelWeaponsVanilla.Clear();
            foreach (var item in this.UIModel.UIModelWeapons)
            {
                this.UIModel.UIModelWeaponsVanilla.Add(item.Copy());
            }
        }

        private void ReadAllMissiles(List<FileInfo> xmlMissilesList)
        {
            foreach (var item in xmlMissilesList)
            {
                using (XmlReader reader = XmlReader.Create(item.FullName))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "macros")
                        {
                            XDocument doc = XDocument.Load(item.FullName);

                            var missiles = doc.Descendants("macro").Where(p => ((string)p.Attribute("class") == "missile" || (string)p.Attribute("class") == "torpedo"));

                            foreach (var missile in missiles)
                            {
                                XmlDocument xD = new XmlDocument();
                                xD.LoadXml(missile.ToString());
                                XmlNode xN = XmlHelper.ToXmlNode(missile);
                                XmlNodeList missileComponentNode = xN.SelectNodes("//component");
                                XmlNodeList missileMacroNode = xN.SelectNodes("//macro");
                                XmlNodeList missileAmmunitionNode = xN.SelectNodes("//properties/ammunition");
                                XmlNodeList missilenMissileNode = xN.SelectNodes("//properties/missile");
                                XmlNodeList missileHullNode = xN.SelectNodes("//properties/hull");
                                XmlNodeList missileReloadNode = xN.SelectNodes("//properties/reload");
                                XmlNodeList missilePhysicsNode = xN.SelectNodes("//properties/physics");
                                XmlNodeList missileInertiaNode = xN.SelectNodes("//properties/physics/inertia");
                                XmlNodeList missileDragNode = xN.SelectNodes("//properties/physics/drag");
                                XmlNodeList missileDamageNode = xN.SelectNodes("//properties/explosiondamage");

                                UIModelMissile uiModelMissile = new UIModelMissile()
                                {
                                    File = item.FullName,
                                    Name = missileMacroNode[0].Attributes["name"].Value
                                };
                                if (missileAmmunitionNode.Count > 0)
                                {
                                    if (missileAmmunitionNode[0].Attributes["value"] != null)
                                        uiModelMissile.Ammunition = Convert.ToInt32(missileAmmunitionNode[0].Attributes["value"].Value);
                                }
                                if (missilenMissileNode.Count > 0)
                                {
                                    if (missilenMissileNode[0].Attributes["amount"] != null)
                                        uiModelMissile.MissileAmount = Convert.ToInt32(missilenMissileNode[0].Attributes["amount"].Value);
                                    if (missilenMissileNode[0].Attributes["lifetime"] != null)
                                        uiModelMissile.Lifetime = ParseToDouble(missilenMissileNode[0].Attributes["lifetime"].Value);
                                    if (missilenMissileNode[0].Attributes["barrelamount"] != null)
                                        uiModelMissile.BarrelAmount = Convert.ToInt32(missilenMissileNode[0].Attributes["barrelamount"].Value);
                                    if (missilenMissileNode[0].Attributes["range"] != null)
                                        uiModelMissile.Range = Convert.ToInt32(missilenMissileNode[0].Attributes["range"].Value);
                                    if (missilenMissileNode[0].Attributes["guided"] != null)
                                        uiModelMissile.Guided = Convert.ToInt32(missilenMissileNode[0].Attributes["guided"].Value);
                                    if (missilenMissileNode[0].Attributes["swarm"] != null)
                                        uiModelMissile.Swarm = Convert.ToInt32(missilenMissileNode[0].Attributes["swarm"].Value);
                                    if (missilenMissileNode[0].Attributes["retarget"] != null)
                                        uiModelMissile.Retarget = Convert.ToInt32(missilenMissileNode[0].Attributes["retarget"].Value);
                                }
                                if (missileDamageNode.Count > 0)
                                {
                                    if (missileDamageNode[0].Attributes["value"] != null)
                                        uiModelMissile.Damage = Convert.ToInt32(missileDamageNode[0].Attributes["value"].Value);

                                }
                                if (missileReloadNode.Count > 0)
                                {
                                    if (missileReloadNode[0].Attributes["time"] != null)
                                        uiModelMissile.Reload = ParseToDouble(missileReloadNode[0].Attributes["time"].Value);

                                }
                                if (missileHullNode.Count > 0)
                                {
                                    if (missileHullNode[0].Attributes["max"] != null)
                                        uiModelMissile.Hull = Convert.ToInt32(missileHullNode[0].Attributes["max"].Value);
                                }
                                if (missilePhysicsNode.Count > 0)
                                {
                                    if (missilePhysicsNode[0].Attributes["mass"] != null)
                                        uiModelMissile.Mass = ParseToDouble(missilePhysicsNode[0].Attributes["mass"].Value);
                                }
                                if (missileInertiaNode.Count > 0)
                                {
                                    if (missileInertiaNode[0].Attributes["yaw"] != null)
                                        uiModelMissile.InertiaYaw = ParseToDouble(missileInertiaNode[0].Attributes["yaw"].Value);
                                    if (missileInertiaNode[0].Attributes["pitch"] != null)
                                        uiModelMissile.InertiaPitch = ParseToDouble(missileInertiaNode[0].Attributes["pitch"].Value);
                                    if (missileInertiaNode[0].Attributes["roll"] != null)
                                        uiModelMissile.InertiaRoll = ParseToDouble(missileInertiaNode[0].Attributes["roll"].Value);
                                }
                                if (missileDragNode.Count > 0)
                                {
                                    if (missileDragNode[0].Attributes["forward"] != null)
                                        uiModelMissile.Forward = ParseToDouble(missileDragNode[0].Attributes["forward"].Value);
                                    if (missileDragNode[0].Attributes["reverse"] != null)
                                        uiModelMissile.Reverse = ParseToDouble(missileDragNode[0].Attributes["reverse"].Value);
                                    if (missileDragNode[0].Attributes["horizontal"] != null)
                                        uiModelMissile.Horizontal = ParseToDouble(missileDragNode[0].Attributes["horizontal"].Value);
                                    if (missileDragNode[0].Attributes["vertical"] != null)
                                        uiModelMissile.Vertical = ParseToDouble(missileDragNode[0].Attributes["vertical"].Value);
                                    if (missileDragNode[0].Attributes["pitch"] != null)
                                        uiModelMissile.Pitch = ParseToDouble(missileDragNode[0].Attributes["pitch"].Value);
                                    if (missileDragNode[0].Attributes["roll"] != null)
                                        uiModelMissile.Roll = ParseToDouble(missileDragNode[0].Attributes["roll"].Value);
                                    if (missileDragNode[0].Attributes["yaw"] != null)
                                        uiModelMissile.Yaw = ParseToDouble(missileDragNode[0].Attributes["yaw"].Value);
                                }
                                uiModelMissile.Changed = false;
                                this.UIModel.UIModelMissiles.Add(uiModelMissile);
                            }
                        }
                    }
                }
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
                foreach (var item in xmlShipsList)
                {
                    this.ReadSingleShipFile(item);
                    //using (XmlReader reader = XmlReader.Create(item.FullName))
                    //{
                    //    while (reader.Read())
                    //    {
                    //        if (reader.NodeType == XmlNodeType.Element && reader.Name == "macros")
                    //        {
                    //            XDocument doc = XDocument.Load(item.FullName);

                    //            var ships = doc.Descendants("macro").Where(p => p.Attribute("class") != null);

                    //            foreach (var ship in ships)
                    //            {
                    //                XmlDocument xD = new XmlDocument();
                    //                xD.LoadXml(ship.ToString());
                    //                XmlNode xN = XmlHelper.ToXmlNode(ship);
                    //                XmlNodeList shipComponentNode = xN.SelectNodes("//component");
                    //                XmlNodeList shipMacroNode = xN.SelectNodes("/macro");
                    //                XmlNodeList shipExplosionNode = xN.SelectNodes("//properties/explosiondamage");
                    //                XmlNodeList shipStorageNode = xN.SelectNodes("//properties/storage");
                    //                XmlNodeList shipHullNode = xN.SelectNodes("//properties/hull");
                    //                XmlNodeList shipSecrecyNode = xN.SelectNodes("//properties/secrecy");
                    //                XmlNodeList shipGatherRateNode = xN.SelectNodes("//properties/gatherrate");
                    //                XmlNodeList shipPeopleNode = xN.SelectNodes("//properties/people");
                    //                XmlNodeList shipPhysicsNode = xN.SelectNodes("//properties/physics");
                    //                XmlNodeList shipInertiaNode = xN.SelectNodes("//properties/physics/inertia");
                    //                XmlNodeList shipDragNode = xN.SelectNodes("//properties/physics/drag");
                    //                XmlNodeList shipShipTypeNode = xN.SelectNodes("//properties/ship");
                    //                string CargoFile = item.FullName.Replace("ship", "storage");


                    //                UIModelShip uiModelShip = new UIModelShip()
                    //                {
                    //                    File = item.FullName,
                    //                    Name = shipMacroNode[0].Attributes["name"].Value,
                    //                    Class = shipMacroNode[0].Attributes["class"].Value

                    //                };

                    //                if (File.Exists(CargoFile))
                    //                {
                    //                    uiModelShip.Cargo = new UIModelShipCargo();
                    //                    XmlDocument Cargodoc = new XmlDocument();
                    //                    Cargodoc.Load(CargoFile);
                    //                    string xmlcontents = Cargodoc.InnerXml;
                    //                    Cargodoc.LoadXml(xmlcontents);

                    //                    XmlNodeList shipCargoNode = Cargodoc.SelectNodes("//properties/cargo");

                    //                    if (shipCargoNode.Count > 0)
                    //                    {
                    //                        uiModelShip.Cargo.File = CargoFile;
                    //                        uiModelShip.Cargo.CargoMax = Convert.ToInt32(shipCargoNode[0].Attributes["max"].Value);
                    //                        uiModelShip.Cargo.CargoTags = shipCargoNode[0].Attributes["tags"].Value;
                    //                    }
                    //                    uiModelShip.Cargo.Changed = false;
                    //                }

                    //                if (shipShipTypeNode.Count > 0)
                    //                {
                    //                    if (shipShipTypeNode[0].Attributes["type"] != null)
                    //                        uiModelShip.Type = shipShipTypeNode[0].Attributes["type"].Value;
                    //                }

                    //                if (uiModelShip.Type == null)
                    //                    continue;

                    //                if (shipExplosionNode.Count > 0)
                    //                {
                    //                    if (shipExplosionNode[0].Attributes["value"] != null)
                    //                        uiModelShip.ExplosionDamage = Convert.ToInt32(shipExplosionNode[0].Attributes["value"].Value);
                    //                }
                    //                if (shipStorageNode.Count > 0)
                    //                {
                    //                    if (shipStorageNode[0].Attributes["missile"] != null)
                    //                        uiModelShip.StorageMissiles = Convert.ToInt32(shipStorageNode[0].Attributes["missile"].Value);
                    //                    if (shipStorageNode[0].Attributes["unit"] != null)
                    //                        uiModelShip.StorageUnits = Convert.ToInt32(shipStorageNode[0].Attributes["unit"].Value);
                    //                }
                    //                if (shipHullNode.Count > 0)
                    //                {
                    //                    if (shipHullNode[0].Attributes["max"] != null)
                    //                        uiModelShip.HullMax = Convert.ToInt32(shipHullNode[0].Attributes["max"].Value);
                    //                }
                    //                if (shipGatherRateNode.Count > 0)
                    //                {
                    //                    if (shipGatherRateNode[0].Attributes["gas"] != null)
                    //                        uiModelShip.GatherRrate = ParseToDouble(shipGatherRateNode[0].Attributes["gas"].Value);
                    //                }
                    //                if (shipSecrecyNode.Count > 0)
                    //                {
                    //                    if (shipSecrecyNode[0].Attributes["level"] != null)
                    //                        uiModelShip.Secrecy = Convert.ToInt32(shipSecrecyNode[0].Attributes["level"].Value);
                    //                }
                    //                if (shipPeopleNode.Count > 0)
                    //                {
                    //                    if (shipPeopleNode[0].Attributes["capacity"] != null)
                    //                        uiModelShip.People = Convert.ToInt32(shipPeopleNode[0].Attributes["capacity"].Value);
                    //                }
                    //                if (shipPhysicsNode.Count > 0)
                    //                {
                    //                    if (shipPhysicsNode[0].Attributes["mass"] != null)
                    //                        uiModelShip.Mass = ParseToDouble(shipPhysicsNode[0].Attributes["mass"].Value);
                    //                }
                    //                if (shipInertiaNode.Count > 0)
                    //                {
                    //                    if (shipInertiaNode[0].Attributes["roll"] != null)
                    //                        uiModelShip.InertiaRoll = ParseToDouble(shipInertiaNode[0].Attributes["roll"].Value);
                    //                    if (shipInertiaNode[0].Attributes["yaw"] != null)
                    //                        uiModelShip.InertiaYaw = ParseToDouble(shipInertiaNode[0].Attributes["yaw"].Value);
                    //                    if (shipInertiaNode[0].Attributes["pitch"] != null)
                    //                        uiModelShip.InertiaPitch = ParseToDouble(shipInertiaNode[0].Attributes["pitch"].Value);
                    //                }
                    //                if (shipDragNode.Count > 0)
                    //                {
                    //                    if (shipDragNode[0].Attributes["forward"] != null)
                    //                        uiModelShip.Forward = ParseToDouble(shipDragNode[0].Attributes["forward"].Value);
                    //                    if (shipDragNode[0].Attributes["reverse"] != null)
                    //                        uiModelShip.Reverse = ParseToDouble(shipDragNode[0].Attributes["reverse"].Value);
                    //                    if (shipDragNode[0].Attributes["horizontal"] != null)
                    //                        uiModelShip.Horizontal = ParseToDouble(shipDragNode[0].Attributes["horizontal"].Value);
                    //                    if (shipDragNode[0].Attributes["vertical"] != null)
                    //                        uiModelShip.Vertical = ParseToDouble(shipDragNode[0].Attributes["vertical"].Value);
                    //                    if (shipDragNode[0].Attributes["pitch"] != null)
                    //                        uiModelShip.Pitch = ParseToDouble(shipDragNode[0].Attributes["pitch"].Value);
                    //                    if (shipDragNode[0].Attributes["yaw"] != null)
                    //                        uiModelShip.Yaw = ParseToDouble(shipDragNode[0].Attributes["yaw"].Value);
                    //                    if (shipDragNode[0].Attributes["roll"] != null)
                    //                        uiModelShip.Roll = ParseToDouble(shipDragNode[0].Attributes["roll"].Value);
                    //                }
                    //                uiModelShip.Changed = false;
                    //                this.UIModel.UIModelShips.Add(uiModelShip);
                    //            }
                    //        }
                    //    }
                    //}
                }
                this.UIModel.UIModelShipsVanilla.Clear();
                foreach (var item in this.UIModel.UIModelShips)
                {
                    this.UIModel.UIModelShipsVanilla.Add(item.Copy());
                }
            }
        }

        private void ReadSingleShipFile(FileInfo file)
        {
            using (XmlReader reader = XmlReader.Create(file.FullName))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "macros")
                    {
                        XDocument doc = XDocument.Load(file.FullName);

                        var ships = doc.Descendants("macro").Where(p => p.Attribute("class") != null);

                        foreach (var ship in ships)
                        {
                            XmlDocument xD = new XmlDocument();
                            xD.LoadXml(ship.ToString());
                            XmlNode xN = XmlHelper.ToXmlNode(ship);
                            XmlNodeList shipComponentNode = xN.SelectNodes("//component");
                            XmlNodeList shipMacroNode = xN.SelectNodes("/macro");
                            XmlNodeList shipExplosionNode = xN.SelectNodes("//properties/explosiondamage");
                            XmlNodeList shipStorageNode = xN.SelectNodes("//properties/storage");
                            XmlNodeList shipHullNode = xN.SelectNodes("//properties/hull");
                            XmlNodeList shipSecrecyNode = xN.SelectNodes("//properties/secrecy");
                            XmlNodeList shipGatherRateNode = xN.SelectNodes("//properties/gatherrate");
                            XmlNodeList shipPeopleNode = xN.SelectNodes("//properties/people");
                            XmlNodeList shipPhysicsNode = xN.SelectNodes("//properties/physics");
                            XmlNodeList shipInertiaNode = xN.SelectNodes("//properties/physics/inertia");
                            XmlNodeList shipDragNode = xN.SelectNodes("//properties/physics/drag");
                            XmlNodeList shipShipTypeNode = xN.SelectNodes("//properties/ship");
                            string CargoFile = file.FullName.Replace("ship", "storage");


                            UIModelShip uiModelShip = new UIModelShip()
                            {
                                File = file.FullName,
                                Name = shipMacroNode[0].Attributes["name"].Value,
                                Class = shipMacroNode[0].Attributes["class"].Value

                            };

                            if (File.Exists(CargoFile))
                            {
                                uiModelShip.Cargo = new UIModelShipCargo();
                                XmlDocument Cargodoc = new XmlDocument();
                                Cargodoc.Load(CargoFile);
                                string xmlcontents = Cargodoc.InnerXml;
                                Cargodoc.LoadXml(xmlcontents);

                                XmlNodeList shipCargoNode = Cargodoc.SelectNodes("//properties/cargo");

                                if (shipCargoNode.Count > 0)
                                {
                                    uiModelShip.Cargo.File = CargoFile;
                                    uiModelShip.Cargo.CargoMax = Convert.ToInt32(shipCargoNode[0].Attributes["max"].Value);
                                    uiModelShip.Cargo.CargoTags = shipCargoNode[0].Attributes["tags"].Value;
                                }
                                uiModelShip.Cargo.Changed = false;
                            }

                            if (shipShipTypeNode.Count > 0)
                            {
                                if (shipShipTypeNode[0].Attributes["type"] != null)
                                    uiModelShip.Type = shipShipTypeNode[0].Attributes["type"].Value;
                            }

                            if (uiModelShip.Type == null)
                                continue;

                            if (shipExplosionNode.Count > 0)
                            {
                                if (shipExplosionNode[0].Attributes["value"] != null)
                                    uiModelShip.ExplosionDamage = Convert.ToInt32(shipExplosionNode[0].Attributes["value"].Value);
                            }
                            if (shipStorageNode.Count > 0)
                            {
                                if (shipStorageNode[0].Attributes["missile"] != null)
                                    uiModelShip.StorageMissiles = Convert.ToInt32(shipStorageNode[0].Attributes["missile"].Value);
                                if (shipStorageNode[0].Attributes["unit"] != null)
                                    uiModelShip.StorageUnits = Convert.ToInt32(shipStorageNode[0].Attributes["unit"].Value);
                            }
                            if (shipHullNode.Count > 0)
                            {
                                if (shipHullNode[0].Attributes["max"] != null)
                                    uiModelShip.HullMax = Convert.ToInt32(shipHullNode[0].Attributes["max"].Value);
                            }
                            if (shipGatherRateNode.Count > 0)
                            {
                                if (shipGatherRateNode[0].Attributes["gas"] != null)
                                    uiModelShip.GatherRrate = ParseToDouble(shipGatherRateNode[0].Attributes["gas"].Value);
                            }
                            if (shipSecrecyNode.Count > 0)
                            {
                                if (shipSecrecyNode[0].Attributes["level"] != null)
                                    uiModelShip.Secrecy = Convert.ToInt32(shipSecrecyNode[0].Attributes["level"].Value);
                            }
                            if (shipPeopleNode.Count > 0)
                            {
                                if (shipPeopleNode[0].Attributes["capacity"] != null)
                                    uiModelShip.People = Convert.ToInt32(shipPeopleNode[0].Attributes["capacity"].Value);
                            }
                            if (shipPhysicsNode.Count > 0)
                            {
                                if (shipPhysicsNode[0].Attributes["mass"] != null)
                                    uiModelShip.Mass = ParseToDouble(shipPhysicsNode[0].Attributes["mass"].Value);
                            }
                            if (shipInertiaNode.Count > 0)
                            {
                                if (shipInertiaNode[0].Attributes["roll"] != null)
                                    uiModelShip.InertiaRoll = ParseToDouble(shipInertiaNode[0].Attributes["roll"].Value);
                                if (shipInertiaNode[0].Attributes["yaw"] != null)
                                    uiModelShip.InertiaYaw = ParseToDouble(shipInertiaNode[0].Attributes["yaw"].Value);
                                if (shipInertiaNode[0].Attributes["pitch"] != null)
                                    uiModelShip.InertiaPitch = ParseToDouble(shipInertiaNode[0].Attributes["pitch"].Value);
                            }
                            if (shipDragNode.Count > 0)
                            {
                                if (shipDragNode[0].Attributes["forward"] != null)
                                    uiModelShip.Forward = ParseToDouble(shipDragNode[0].Attributes["forward"].Value);
                                if (shipDragNode[0].Attributes["reverse"] != null)
                                    uiModelShip.Reverse = ParseToDouble(shipDragNode[0].Attributes["reverse"].Value);
                                if (shipDragNode[0].Attributes["horizontal"] != null)
                                    uiModelShip.Horizontal = ParseToDouble(shipDragNode[0].Attributes["horizontal"].Value);
                                if (shipDragNode[0].Attributes["vertical"] != null)
                                    uiModelShip.Vertical = ParseToDouble(shipDragNode[0].Attributes["vertical"].Value);
                                if (shipDragNode[0].Attributes["pitch"] != null)
                                    uiModelShip.Pitch = ParseToDouble(shipDragNode[0].Attributes["pitch"].Value);
                                if (shipDragNode[0].Attributes["yaw"] != null)
                                    uiModelShip.Yaw = ParseToDouble(shipDragNode[0].Attributes["yaw"].Value);
                                if (shipDragNode[0].Attributes["roll"] != null)
                                    uiModelShip.Roll = ParseToDouble(shipDragNode[0].Attributes["roll"].Value);
                            }
                            uiModelShip.Changed = false;

                            var ShipExistsAlready = this.UIModel.UIModelShips.Where(x => x.File == uiModelShip.File).ToList();
                            if (ShipExistsAlready.Count == 0)
                                this.UIModel.UIModelShips.Add(uiModelShip);
                        }
                    }
                }
            }
        }


        private double ParseToDouble(string input)
        {
            return double.Parse(input, new NumberFormatInfo() { NumberDecimalSeparator = "." });
        }

        private void ExecuteReadAllModFilesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!Directory.Exists(this.UIModel.ModPath))
            {
                MessageBox.Show("Enter a valid folder path for mod files", "No valid mod folder");
            }
            else
            {
                List<string> ModDirectories = Directory.GetDirectories(this.UIModel.ModPath, "*", SearchOption.AllDirectories).ToList();
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
                                UIModelShield shield = this.UIModel.UIModelModulesShields.Where(x => x.File.Contains(fileName)).FirstOrDefault();
                                if (shield != null)
                                {
                                    string line;
                                    while (!sr.EndOfStream)
                                    {
                                        line = sr.ReadLine();
                                        if (line.Contains("@max") && line.Contains("recharge"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            shield.Max = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@rate"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            shield.Rate = ParseToDouble(value);
                                        }
                                        if (line.Contains("@delay"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            shield.Delay = ParseToDouble(value);
                                        }
                                        if (line.Contains("@max") && line.Contains("hull"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            shield.MaxHull = ParseToDouble(value);
                                        }
                                        if (line.Contains("@threshold") && line.Contains("hull"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            shield.Threshold = ParseToDouble(value);
                                        }
                                    }
                                }
                                else
                                {
                                    this.ReadSingleShield(new FileInfo(file));
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
                                UIModelEngine engine = this.UIModel.UIModelModulesEngines.Where(x => x.File.Contains(fileName)).First();
                                string line;
                                while (!sr.EndOfStream)
                                {
                                    line = sr.ReadLine();
                                    if (line.Contains("@duration") && line.Contains("boost"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.BoostDuration = ParseToDouble(value);
                                    }
                                    if (line.Contains("@thrust") && line.Contains("boost"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.BoostThrust = ParseToDouble(value);
                                    }
                                    if (line.Contains("@attack") && line.Contains("boost"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.BoostAttack = ParseToDouble(value);
                                    }
                                    if (line.Contains("@release") && line.Contains("boost"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.BoostRelease = ParseToDouble(value);
                                    }
                                    if (line.Contains("@charge") && line.Contains("travel"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.TravelCharge = Convert.ToInt32(value);
                                    }
                                    if (line.Contains("@thrust") && line.Contains("travel"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.TravelThrust = ParseToDouble(value);
                                    }
                                    if (line.Contains("@attack") && line.Contains("travel"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.TravelAttack = ParseToDouble(value);
                                    }
                                    if (line.Contains("@release") && line.Contains("travel"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.TravelRelease = ParseToDouble(value);
                                    }
                                    if (line.Contains("@forward"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.Forward = ParseToDouble(value);
                                    }
                                    if (line.Contains("@reverse"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.Reverse = ParseToDouble(value);
                                    }
                                    if (line.Contains("@strafe") && line.Contains("thrust"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.Strafe = ParseToDouble(value);
                                    }
                                    if (line.Contains("@pitch") && line.Contains("thrust"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.Pitch = ParseToDouble(value);
                                    }
                                    if (line.Contains("@yaw") && line.Contains("thrust"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.Yaw = ParseToDouble(value);
                                    }
                                    if (line.Contains("@roll") && line.Contains("thrust"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.Roll = ParseToDouble(value);
                                    }
                                    if (line.Contains("@roll") && line.Contains("angular"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.AngularRoll = ParseToDouble(value);
                                    }
                                    if (line.Contains("@pitch") && line.Contains("angular"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.AngularPitch = ParseToDouble(value);
                                    }
                                    if (line.Contains("@max") && line.Contains("hull"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.MaxHull = ParseToDouble(value);
                                    }
                                    if (line.Contains("@threshold") && line.Contains("hull"))
                                    {
                                        string value = line.Split('>')[1].Split('<')[0];
                                        engine.Threshold = ParseToDouble(value);
                                    }
                                }
                            }
                        }
                    }
                    //weapons
                    if (dir.Contains("assets\\fx\\weaponfx\\macros"))
                    {
                        List<string> files = Directory.GetFiles(dir).ToList();

                        foreach (string file in files)
                        {
                            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            string[] fileDepth = fileStream.Name.Split('\\');
                            string fileName = fileDepth[fileDepth.Count() - 1];
                            using (StreamReader sr = new StreamReader(fileStream))
                            {
                                if (this.UIModel.UIModelProjectiles.Any(x => x.File.Contains(fileName)))
                                {
                                    UIModelProjectile weapon = this.UIModel.UIModelProjectiles.Where(x => x.File.Contains(fileName)).First();
                                    string line;
                                    while (!sr.EndOfStream)
                                    {
                                        line = sr.ReadLine();
                                        if (line.Contains("@value") && line.Contains("ammunition"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.Ammunition = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@reload") && line.Contains("ammunition"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.AmmunitionReload = ParseToDouble(value);
                                        }
                                        if (line.Contains("@speed") && line.Contains("bullet"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.Speed = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@lifetime") && line.Contains("bullet"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.Lifetime = ParseToDouble(value);
                                        }
                                        if (line.Contains("@amount") && line.Contains("bullet"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.Amount = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@barrelamount") && line.Contains("bullet"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.BarrelAmount = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@timediff") && line.Contains("bullet"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.TimeDiff = ParseToDouble(value);
                                        }
                                        if (line.Contains("@angle") && line.Contains("bullet"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.Angle = ParseToDouble(value);
                                        }
                                        if (line.Contains("@scale") && line.Contains("bullet"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.Scale = ParseToDouble(value);
                                        }
                                        if (line.Contains("@ricochet") && line.Contains("bullet"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.Ricochet = ParseToDouble(value);
                                        }
                                        if (line.Contains("@chargetime") && line.Contains("bullet"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.ChargeTime = ParseToDouble(value);
                                        }
                                        if (line.Contains("@value") && line.Contains("heat"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.HeatValue = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@initial") && line.Contains("heat"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.HeatInitial = Convert.ToInt32(value);
                                        }
                                        if (line.Contains("@rate") && line.Contains("reload"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.ReloadRate = ParseToDouble(value);
                                        }
                                        if (line.Contains("@time") && line.Contains("reload"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.ReloadTime = ParseToDouble(value);
                                        }
                                        if (line.Contains("@value") && line.Contains("damage"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.Damage = ParseToDouble(value);
                                        }
                                        if (line.Contains("@shield") && line.Contains("damage"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.Shield = ParseToDouble(value);
                                        }
                                        if (line.Contains("@repair") && line.Contains("damage"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            weapon.Repair = ParseToDouble(value);
                                        }
                                    }
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
                                    ship = this.UIModel.UIModelShips.Where(x => x.File.Contains(fileName.Replace("storage", "ship"))).FirstOrDefault();
                                }
                                else
                                    ship = this.UIModel.UIModelShips.Where(x => x.File.Contains(fileName)).FirstOrDefault();

                                if (ship != null)
                                {
                                    string line;
                                    while (!sr.EndOfStream)
                                    {
                                        line = sr.ReadLine();
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
                                            ship.GatherRrate = ParseToDouble(value);
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
                                            ship.InertiaPitch = ParseToDouble(value);
                                        }
                                        if (line.Contains("@yaw") && line.Contains("inertia"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.InertiaYaw = ParseToDouble(value);
                                        }
                                        if (line.Contains("@roll") && line.Contains("inertia"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.InertiaRoll = ParseToDouble(value);
                                        }
                                        if (line.Contains("@forward") && line.Contains("drag"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Forward = ParseToDouble(value);
                                        }
                                        if (line.Contains("@reverse") && line.Contains("drag"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Reverse = ParseToDouble(value);
                                        }
                                        if (line.Contains("@horizontal") && line.Contains("drag"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Horizontal = ParseToDouble(value);
                                        }
                                        if (line.Contains("@vertical") && line.Contains("drag"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Vertical = ParseToDouble(value);
                                        }
                                        if (line.Contains("@pitch") && line.Contains("drag"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Pitch = ParseToDouble(value);
                                        }
                                        if (line.Contains("@yaw") && line.Contains("drag"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Yaw = ParseToDouble(value);
                                        }
                                        if (line.Contains("@roll") && line.Contains("drag"))
                                        {
                                            string value = line.Split('>')[1].Split('<')[0];
                                            ship.Roll = ParseToDouble(value);
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
                                    this.ReadSingleShipFile(new FileInfo(file));
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
