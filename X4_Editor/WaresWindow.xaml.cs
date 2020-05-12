using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace X4_Editor
{
    /// <summary>
    /// Interaktionslogik für WaresWindow.xaml
    /// </summary>
    public partial class WaresWindow : Window
    {
        public WaresWindow()
        {
            InitializeComponent();
        }

        private void OnWaresWindowCellRightClick(object sender, MouseButtonEventArgs e)
        {
            X4Commands.OnWaresWindowCellRightClick.Execute(null, this);
        }
        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()).ToString();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this.Hide();
            e.Cancel = true;
        }
    }
}
