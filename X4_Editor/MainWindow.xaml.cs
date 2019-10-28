using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.IO;

namespace X4_Editor
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        public DataGrid DataGridShields
        {
            get
            {
                return DG_Shields;
            }
        }
        public DataGrid DataGridEngines
        {
            get
            {
                return DG_Engines;
            }
        }
        public DataGrid DataGridProjectiles
        {
            get
            {
                return DG_Projectiles;
            }
        }
        public DataGrid DataGridWeapons
        {
            get
            {
                return DG_Weapons;
            }
        }
        public DataGrid DataGridShips
        {
            get
            {
                return DG_Ships;
            }
        }
        public DataGrid DataGridMissiles
        {
            get
            {
                return DG_Missiles;
            }
        }

        public TextBox TextBoxMathParameter
        {
            get
            {
                return tb_MathParameter;
            }
        }

        private void OnButtonReadDataClick(object sender, RoutedEventArgs e)
        {
            X4Commands.ReadAllVanillaFilesCommand.Execute(null, this);

            string folderPath = @"E:\Privat\Rev31\rev31\Weapons";
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.tdf"))
            {

            }
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()).ToString();
        }


        private void OnInformationClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Info\r" +
                "\r" +
                "This tool is designed to read and manipulate X4 Foundation files.\r" +
                "Simply select the UNPACKED X4 folder containing the unpacked .cat files. \r" +
                "\r" +
                "You can read up to 2 different mod files. Keep in mind, that the order in which you read the mod files is important. \r" +
                "You can open a file by right clicking into the row. \r"+
                "\r" +
                "Always make a backup of your mod before writing into a mod.\r" +
                "\r" +
                "\r" +
                "\r" +
                "\r" +
                "The author of this program is Pascal Wauer. Using this program is under your own responsibility." +
                "\r"
                , "X4 Editor"
                );
        }

        private void OnShowEcoClick(object sender, RoutedEventArgs e)
        {
            //this.DataGridFBI.SelectedCells.Clear();
        }

        private void OnSelectionChangedFBI(object sender, SelectedCellsChangedEventArgs e)
        {
            //this.DataGridTDF.SelectedCells.Clear();
        }

        private void OnMainWindowCellRightClick(object sender, MouseButtonEventArgs e)
        {
            X4Commands.OnMainWindowCellRightClick.Execute(null, this);
        }

        private void OnWaresSelected(object sender, SelectedCellsChangedEventArgs e)
        {
            this.DataGridEngines.SelectedCells.Clear();
            this.DataGridShields.SelectedCells.Clear();
            this.DataGridShips.SelectedCells.Clear();
            this.DataGridProjectiles.SelectedCells.Clear();
            this.DataGridMissiles.SelectedCells.Clear();
            this.DataGridWeapons.SelectedCells.Clear();
        }

        private void OnShipsSelected(object sender, SelectedCellsChangedEventArgs e)
        {
            this.DataGridEngines.SelectedCells.Clear();
            this.DataGridShields.SelectedCells.Clear();
            this.DataGridMissiles.SelectedCells.Clear();
            this.DataGridProjectiles.SelectedCells.Clear();
            this.DataGridWeapons.SelectedCells.Clear();
        }

        private void OnWeaponsSelected(object sender, SelectedCellsChangedEventArgs e)
        {
            this.DataGridEngines.SelectedCells.Clear();
            this.DataGridShields.SelectedCells.Clear();
            this.DataGridShips.SelectedCells.Clear();
            this.DataGridMissiles.SelectedCells.Clear();
            this.DataGridProjectiles.SelectedCells.Clear();
        }

        private void OnProjectilesSelected(object sender, SelectedCellsChangedEventArgs e)
        {
            this.DataGridEngines.SelectedCells.Clear();
            this.DataGridShields.SelectedCells.Clear();
            this.DataGridShips.SelectedCells.Clear();
            this.DataGridMissiles.SelectedCells.Clear();
            this.DataGridWeapons.SelectedCells.Clear();
        }

        private void OnEnginesSelected(object sender, SelectedCellsChangedEventArgs e)
        {
            this.DataGridMissiles.SelectedCells.Clear();
            this.DataGridShields.SelectedCells.Clear();
            this.DataGridShips.SelectedCells.Clear();
            this.DataGridProjectiles.SelectedCells.Clear();
            this.DataGridWeapons.SelectedCells.Clear();
        }

        private void OnShieldsSelected(object sender, SelectedCellsChangedEventArgs e)
        {
            this.DataGridEngines.SelectedCells.Clear();
            this.DataGridMissiles.SelectedCells.Clear();
            this.DataGridShips.SelectedCells.Clear();
            this.DataGridProjectiles.SelectedCells.Clear();
            this.DataGridWeapons.SelectedCells.Clear();
        }

        private void OnMissilesSelected(object sender, SelectedCellsChangedEventArgs e)
        {
            this.DataGridEngines.SelectedCells.Clear();
            this.DataGridShields.SelectedCells.Clear();
            this.DataGridShips.SelectedCells.Clear();
            this.DataGridProjectiles.SelectedCells.Clear();
            this.DataGridWeapons.SelectedCells.Clear();
        }

        private void OnWeaponDoubleClick(object sender, MouseButtonEventArgs e)
        {
            X4Commands.OnWeaponDoubleClick.Execute(null, null);
        }

        private void OnProjectileDoubleClick(object sender, MouseButtonEventArgs e)
        {
            X4Commands.OnProjectileDoubleClick.Execute(null, null);
        }
    }
}
