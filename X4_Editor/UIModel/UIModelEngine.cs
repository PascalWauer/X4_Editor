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
    public class UIModelEngine : INotifyPropertyChanged
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

        private string m_IGName;
        public string IGName
        {
            get {
                if (string.IsNullOrEmpty(m_IGName))
                    return "";
                return m_IGName; }
            set
            {
                m_IGName = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_BoostDuration;
        public double BoostDuration
        {
            get { return m_BoostDuration; }
            set
            {
                m_BoostDuration = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_BoostThrust;
        public double BoostThrust
        {
            get { return m_BoostThrust; }
            set
            {
                m_BoostThrust = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_BoostAttack;
        public double BoostAttack
        {
            get { return m_BoostAttack; }
            set
            {
                m_BoostAttack = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_BoostRelease;
        public double BoostRelease
        {
            get { return m_BoostRelease; }
            set
            {
                m_BoostRelease = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_TravelCharge;
        public double TravelCharge
        {
            get { return m_TravelCharge; }
            set
            {
                m_TravelCharge = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_TravelThrust;
        public double TravelThrust
        {
            get { return m_TravelThrust; }
            set
            {
                m_TravelThrust = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }
        private double m_TravelAttack;
        public double TravelAttack
        {
            get { return m_TravelAttack; }
            set
            {
                m_TravelAttack = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }
        private double m_TravelRelease;
        public double TravelRelease
        {
            get { return m_TravelRelease; }
            set
            {
                m_TravelRelease = value;
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

        private double m_Strafe;
        public double Strafe
        {
            get { return m_Strafe; }
            set
            {
                m_Strafe = value;
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

        private double m_AngularRoll;
        public double AngularRoll
        {
            get { return m_AngularRoll; }
            set
            {
                m_AngularRoll = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_AngularPitch;
        public double AngularPitch
        {
            get { return m_AngularPitch; }
            set
            {
                m_AngularPitch = value;
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
                Changed = true;
                NotifyPropertyChanged();
            }
        }
        public UIModelEngine Copy()
        {
            return (UIModelEngine)this.MemberwiseClone();
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
