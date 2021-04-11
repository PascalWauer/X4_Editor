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
    public class UIModelShield : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string MK { get; set; }
        public string Faction { get; set; }
        public string File { get; set; }

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

        private string m_IGName;
        public string IGName
        {
            get 
            {
                if (string.IsNullOrEmpty(m_IGName))
                    return "";
                return m_IGName; 
            }
            set
            {
                m_IGName = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }
        public string Size
        {
            get
            {
                if (this.Name.Contains("_xs_"))
                    return "XS";
                if (this.Name.Contains("_s_"))
                    return "S";
                if (this.Name.Contains("_m_"))
                    return "M";
                if (this.Name.Contains("_l_"))
                    return "L";
                if (this.Name.Contains("_xl_"))
                    return "XL";
                else
                    return "?";
            }
        }

        private int m_Max;
        public int Max
        {
            get { return m_Max; }
            set
            {
                m_Max = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Rate;
        public double Rate
        {
            get { return m_Rate; }
            set
            {
                m_Rate = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Delay;
        public double Delay
        {
            get { return m_Delay; }
            set
            {
                m_Delay = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_MaxHull;
        public double MaxHull
        {
            get { return m_MaxHull; }
            set
            {
                m_MaxHull = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Threshold;
        public double Threshold
        {
            get { return m_Threshold; }
            set
            {
                m_Threshold = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_Price;
        public int Price
        {
            get { return m_Price; }
            set
            {
                m_Price = value;
                NotifyPropertyChanged();
            }
        }

        public UIModelShield Copy()
        {
            return (UIModelShield)this.MemberwiseClone();
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
