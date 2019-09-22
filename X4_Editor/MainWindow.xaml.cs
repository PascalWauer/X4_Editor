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
                "This tool is designed to manipulate Total Annihilation data files.\r" +
                "Before you can read any data you need to extract the .ufo, .hpi or .gp3 files you want to manipulate (HPIviewer can help here). \r" +
                "\r" +
                "When you have extracted all the files, you need to specify the folder path where you have extracted the files. \r" +
                "This folder must have a folder containing the word 'unit' including .fbi files. It must have a folder containg the word 'weapon' including .tdf files if you want to edit weapon files. \r"+
                "It is suggested to make a backup of all WEAPONS and UNITS folders before starting to edit the files.\r" +
                "\r" +
                "Click on the specific button to read unit or weapon files. Weapons are always corresponding to the shown units. So be sure to read some units first. \r" +
                "\r" +
                "Edit the values in the specific cells manually or use the calculation functions on the selected cells.\r" +
                "After you are finished you need to SAVE the edited data. This will replace all changed files in the original folder with the values in the table. \r" +
                "The last step to do is to pack the folder back to its original file (HPIpacker can help here) and place it into your TA folder. \r" +
                "\r" +
                "The author of this program is Pascal Wauer. Using this program is under your own responsibility." +
                "\r"
                , "Total Annihilation Units and Weapons Editor V1.10"
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
