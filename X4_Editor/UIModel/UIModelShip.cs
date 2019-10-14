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
    public class UIModelShip : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string File { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }
        public UIModelShipCargo Cargo {get; set;}

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
        public string Size
        {
            get
            {
                if (this.Class.Contains("_xs"))
                    return "XS";
                if (this.Class.Contains("_s"))
                    return "S";
                if (this.Class.Contains("_m"))
                    return "M";
                if (this.Class.Contains("_l"))
                    return "L";
                if (this.Class.Contains("_xl"))
                    return "XL";
                else
                    return "?";
            }
        }

        private string m_IGName;
        public string IGName
        {
            get { return m_IGName; }
            set
            {
                m_IGName = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_ExplosionDamage;
        public int ExplosionDamage
        {
            get { return m_ExplosionDamage; }
            set
            {
                m_ExplosionDamage = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_StorageMissiles;
        public int StorageMissiles
        {
            get { return m_StorageMissiles; }
            set
            {
                m_StorageMissiles = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_StorageUnits;
        public int StorageUnits
        {
            get { return m_StorageUnits; }
            set
            {
                m_StorageUnits = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_HullMax;
        public int HullMax
        {
            get { return m_HullMax; }
            set
            {
                m_HullMax = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_Secrecy;
        public int Secrecy
        {
            get { return m_Secrecy; }
            set
            {
                m_Secrecy = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_GatherRrate;
        public double GatherRrate
        {
            get { return m_GatherRrate; }
            set
            {
                m_GatherRrate = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_People;
        public int People
        {
            get { return m_People; }
            set
            {
                m_People = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Mass;
        public double Mass
        {
            get { return m_Mass; }
            set
            {
                m_Mass = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_InertiaPitch;
        public double InertiaPitch
        {
            get { return m_InertiaPitch; }
            set
            {
                m_InertiaPitch = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_InertiaYaw;
        public double InertiaYaw
        {
            get { return m_InertiaYaw; }
            set
            {
                m_InertiaYaw = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_InertiaRoll;
        public double InertiaRoll
        {
            get { return m_InertiaRoll; }
            set
            {
                m_InertiaRoll = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Forward;
        public double Forward
        {
            get { return m_Forward; }
            set
            {
                m_Forward = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Reverse;
        public double Reverse
        {
            get { return m_Reverse; }
            set
            {
                m_Reverse = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }
        private double m_Horizontal;
        public double Horizontal
        {
            get { return m_Horizontal; }
            set
            {
                m_Horizontal = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Vertical;
        public double Vertical
        {
            get { return m_Vertical; }
            set
            {
                m_Vertical = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Pitch;
        public double Pitch
        {
            get { return m_Pitch; }
            set
            {
                m_Pitch = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Yaw;
        public double Yaw
        {
            get { return m_Yaw; }
            set
            {
                m_Yaw = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Roll;
        public double Roll
        {
            get { return m_Roll; }
            set
            {
                m_Roll = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        public UIModelShip Copy()
        {
            UIModelShip clone = (UIModelShip)this.MemberwiseClone();
            if (this.Cargo != null)
                clone.Cargo = this.Cargo.Copy();
            else
                clone.Cargo = null;
            return clone;
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
