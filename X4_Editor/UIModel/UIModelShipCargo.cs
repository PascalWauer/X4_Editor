using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace X4_Editor
{
    public class UIModelShipCargo : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string File { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }

        private bool m_Changed;
        public bool Changed
        {
            get { return m_Changed; }
            set
            {
                if (value == m_Changed)
                    return;
                m_Changed = value;
                this.NotifyPropertyChanged();
            }
        }
      
        private int m_CargoMax;
        public int CargoMax
        {
            get { return m_CargoMax; }
            set
            {
                m_CargoMax = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private string m_CargoTags;
        public string CargoTags
        {
            get { return m_CargoTags; }
            set
            {
                m_CargoTags = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        public UIModelShipCargo Copy()
        {
            return (UIModelShipCargo)this.MemberwiseClone();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
