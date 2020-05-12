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
    /// Interaktionslogik für ModManager.xaml
    /// </summary>
    public partial class ModManager : Window
    {
        public ModManager()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            X4Commands.CloseModPathManager.Execute(this, null);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this.Hide();
            e.Cancel = true;
        }

    }
}
