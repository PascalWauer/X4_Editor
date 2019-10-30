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
    public class UIModelMissile : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string File { get; set; }
        public string Class { get; set; }

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
            get { return m_IGName; }
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

        public double DPS
        {
            get
            {
                return m_Damage / m_Reload * m_MissileAmount;
            }
        }

        private int m_Ammunition;
        public int Ammunition
        {
            get { return m_Ammunition; }
            set
            {
                m_Ammunition = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_BarrelAmount;
        public int BarrelAmount
        {
            get { return m_BarrelAmount; }
            set
            {
                m_BarrelAmount = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_MissileAmount;
        public int MissileAmount
        {
            get { return m_MissileAmount; }
            set
            {
                m_MissileAmount = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Lifetime;
        public double Lifetime
        {
            get { return m_Lifetime; }
            set
            {
                m_Lifetime = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_Range;
        public int Range
        {
            get { return m_Range; }
            set
            {
                m_Range = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }
        
        private int m_Swarm;
        public int Swarm
        {
            get { return m_Swarm; }
            set
            {
                m_Swarm = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_Guided;
        public int Guided
        {
            get { return m_Guided; }
            set
            {
                m_Guided = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_Retarget;
        public int Retarget
        {
            get { return m_Retarget; }
            set
            {
                m_Retarget = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_Damage;
        public int Damage
        {
            get { return m_Damage; }
            set
            {
                m_Damage = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Reload;
        public double Reload
        {
            get { return m_Reload; }
            set
            {
                m_Reload = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_Hull;
        public int Hull
        {
            get { return m_Hull; }
            set
            {
                m_Hull = value;
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

        //private double m_InertiaMass;
        //public double InertiaMass
        //{
        //    get { return m_InertiaMass; }
        //    set
        //    {
        //        m_InertiaMass = value;
        //        Changed = true;
        //        NotifyPropertyChanged();
        //    }
        //}

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

        public UIModelMissile Copy()
        {
            return (UIModelMissile)this.MemberwiseClone();
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
