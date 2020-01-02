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
    public class UIModelWeapon : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string Name { get; set; }
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

        private string m_MK;
        public string MK
        {
            get { return m_MK; }
            set
            {
                m_MK = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private string m_Projectile;
        public string Projectile
        {
            get 
            { 
                return m_Projectile; 
            }
            set
            {
                m_Projectile = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_Overheat;
        public int Overheat
        {
            get { return m_Overheat; }
            set
            {
                m_Overheat = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_CoolDelay;
        public double CoolDelay
        {
            get { return m_CoolDelay; }
            set
            {
                m_CoolDelay = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_CoolRate;
        public int CoolRate
        {
            get { return m_CoolRate; }
            set
            {
                m_CoolRate = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_Reenable;
        public int Reenable
        {
            get { return m_Reenable; }
            set
            {
                m_Reenable = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_RotationSpeed;
        public double RotationSpeed
        {
            get { return m_RotationSpeed; }
            set
            {
                m_RotationSpeed = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_RotationAcceleration;
        public double RotationAcceleration
        {
            get { return m_RotationAcceleration; }
            set
            {
                m_RotationAcceleration = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_ReloadRate;
        public double ReloadRate
        {
            get { return m_ReloadRate; }
            set
            {
                m_ReloadRate = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_ReloadTime;
        public double ReloadTime
        {
            get { return m_ReloadTime; }
            set
            {
                m_ReloadTime = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_HullMax;
        public double HullMax
        {
            get { return m_HullMax; }
            set
            {
                m_HullMax = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_HullThreshold;
        public double HullThreshold
        {
            get { return m_HullThreshold; }
            set
            {
                m_HullThreshold = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }
        
        public UIModelWeapon Copy()
        {
            return (UIModelWeapon)this.MemberwiseClone();
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
