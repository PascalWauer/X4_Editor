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
    public class UIModelProjectile : INotifyPropertyChanged
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

        private double ShotsPerSecond
        {
            get
            {
                if (ReloadTime == 0)
                    return ReloadRate;
                return 1 / ReloadTime;
            }
        }
        private double BeamDamageMultiplier
        {
            get
            {
                if (Name.ToUpper().Contains("BEAM"))
                    return Lifetime;
                else
                    return 1;
            }
        }
        public double DPS
        {
            get
            {
                return (DPS_Shield + DPS_Hull) * 0.5;
                    //if (Ammunition > 0)
                    //    return (Ammunition * ( (Damage + Hull) * BeamDamageMultiplier * Amount * BarrelAmount) / ((AmmunitionReload + Ammunition) / ShotsPerSecond));
                    //return (((Damage + Hull) * BeamDamageMultiplier * Amount * BarrelAmount) / (AmmunitionReload + 1 / ShotsPerSecond));
            }
        }

        public double DPS_Shield
        {
            get
            {
                if (Ammunition > 0)
                    return (Ammunition * ((Damage + Shield) * BeamDamageMultiplier * Amount * BarrelAmount) / (AmmunitionReload + (Ammunition / ShotsPerSecond)));
                return (((Damage + Shield) * BeamDamageMultiplier * Amount * BarrelAmount) / (AmmunitionReload + (1 / ShotsPerSecond)));
            }
        }
        public double DPS_Hull
        {
            get
            {
                if (Ammunition > 0)
                    return (Ammunition * ((Damage + Hull) * BeamDamageMultiplier * Amount * BarrelAmount) / (AmmunitionReload + (Ammunition / ShotsPerSecond)));
                return (((Damage + Hull) * BeamDamageMultiplier * Amount * BarrelAmount) / (AmmunitionReload + (1 / ShotsPerSecond)));
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
                NotifyPropertyChanged("DPS");
                NotifyPropertyChanged("DPS_Hull");
                NotifyPropertyChanged("DPS_Shield");
            }
        }

        private double m_AmmunitionReload;
        public double AmmunitionReload
        {
            get { return m_AmmunitionReload; }
            set
            {
                m_AmmunitionReload = value;
                Changed = true;
                NotifyPropertyChanged();
                NotifyPropertyChanged("DPS");
                NotifyPropertyChanged("DPS_Hull");
                NotifyPropertyChanged("DPS_Shield");
            }
        }

        private int m_Speed;
        public int Speed
        {
            get { return m_Speed; }
            set
            {
                m_Speed = value;
                Changed = true;
                NotifyPropertyChanged();
                NotifyPropertyChanged("EffectiveRange");
            }
        }

        private double m_Range;
        public double Range
        {
            get { return m_Range; }
            set
            {
                m_Range = value;
                Changed = true;
                NotifyPropertyChanged();
                NotifyPropertyChanged("EffectiveRange");
            }
        }

        public double EffectiveRange
        {
            get 
            {
                double effectiveRange = Lifetime * Speed;
                if (Range == 0 || Range > effectiveRange)
                    return effectiveRange;
                else
                    return Range;
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
                NotifyPropertyChanged("EffectiveRange");
                NotifyPropertyChanged("DPSl");
                NotifyPropertyChanged("DPS_Shield");
                NotifyPropertyChanged("DPS_Hull");
                NotifyPropertyChanged("BeamDamageMultiplier");
            }
        }

        private int m_Amount;
        public int Amount
        {
            get { return m_Amount; }
            set
            {
                m_Amount = value;
                Changed = true;
                NotifyPropertyChanged();
                NotifyPropertyChanged("DPS");
                NotifyPropertyChanged("DPS_Hull");
                NotifyPropertyChanged("DPS_Shield");
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
                NotifyPropertyChanged("DPS");
                NotifyPropertyChanged("DPS_Hull");
                NotifyPropertyChanged("DPS_Shield");
            }
        }

        private double m_TimeDiff;
        public double TimeDiff
        {
            get { return m_TimeDiff; }
            set
            {
                m_TimeDiff = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Angle;
        public double Angle
        {
            get { return m_Angle; }
            set
            {
                m_Angle = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_MaxHits;
        public int MaxHits
        {
            get { return m_MaxHits; }
            set
            {
                m_MaxHits = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Scale;
        public double Scale
        {
            get { return m_Scale; }
            set
            {
                m_Scale = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_Ricochete;
        public double Ricochet
        {
            get { return m_Ricochete; }
            set
            {
                m_Ricochete = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private double m_ChargeTime;
        public double ChargeTime
        {
            get { return m_ChargeTime; }
            set
            {
                m_ChargeTime = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_HeatValue;
        public int HeatValue
        {
            get { return m_HeatValue; }
            set
            {
                m_HeatValue = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_HeatInitial;
        public int HeatInitial
        {
            get { return m_HeatInitial; }
            set
            {
                m_HeatInitial = value;
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
                NotifyPropertyChanged("DPS");
                NotifyPropertyChanged("DPS_Hull");
                NotifyPropertyChanged("DPS_Shield");
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
                NotifyPropertyChanged("DPS");
                NotifyPropertyChanged("DPS_Hull");
                NotifyPropertyChanged("DPS_Shield");
            }
        }

        private double m_Damage;
        public double Damage
        {
            get { return m_Damage; }
            set
            {
                m_Damage = value;
                Changed = true;
                NotifyPropertyChanged();
                NotifyPropertyChanged("DPS");
                NotifyPropertyChanged("DPS_Shield");
                NotifyPropertyChanged("DPS_Hull");
            }
        }

        private double m_Shield;
        public double Shield
        {
            get { return m_Shield; }
            set
            {
                m_Shield = value;
                Changed = true;
                NotifyPropertyChanged();
                NotifyPropertyChanged("DPS");
                NotifyPropertyChanged("DPS_Shield");
                NotifyPropertyChanged("DPS_Hull");
            }
        }

        private double m_Hull;
        public double Hull
        {
            get { return m_Hull; }
            set
            {
                m_Hull = value;
                Changed = true;
                NotifyPropertyChanged();
                NotifyPropertyChanged("DPS");
                NotifyPropertyChanged("DPS_Shield");
                NotifyPropertyChanged("DPS_Hull");
            }
        }

        private double m_Repair;
        public double Repair
        {
            get { return m_Repair; }
            set
            {
                m_Repair = value;
                Changed = true;
                NotifyPropertyChanged();
                NotifyPropertyChanged("DPS");
                NotifyPropertyChanged("DPS_Shield");
                NotifyPropertyChanged("DPS_Hull");
            }
        }

        public UIModelProjectile Copy()
        {
            return (UIModelProjectile)this.MemberwiseClone();
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
