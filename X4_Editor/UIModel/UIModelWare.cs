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
    public class UIModelWare : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string MK { get; set; }
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

        private int m_PriceMin;
        public int Min
        {
            get { return m_PriceMin; }
            set
            {
                m_PriceMin = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_PriceAvg;
        public int Avg
        {
            get { return m_PriceAvg; }
            set
            {
                m_PriceAvg = value;
                Changed = true;
                X4Commands.RecalculatePriceCommand.Execute(null, null);
                NotifyPropertyChanged();
                NotifyPropertyChanged("Margin");
            }
        }

        private int m_PriceMax;
        public int Max
        {
            get { return m_PriceMax; }
            set
            {
                m_PriceMax = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }

        private int m_CalculatedPrice;
        public int CalculatedPrice
        {
            get { return m_CalculatedPrice; }
            set
            {
                m_CalculatedPrice = value;
                Changed = true;
                NotifyPropertyChanged();
                NotifyPropertyChanged("Margin");
            }
        }
        public int Margin
        {
            get
            {
                if (CalculatedPrice != 0)
                {
                    return Convert.ToInt32((((decimal)Avg / (decimal)CalculatedPrice) - 1) * 100);
                }
                else return 0;
            }
        }

        private double m_Time;
        public double Time
        {
            get { return m_Time; }
            set
            {
                m_Time = value;
                Changed = true;
                NotifyPropertyChanged();
            }
        }
        private string m_Component;
        public string Component
        {
            get { return m_Component; }
            set
            {
                m_Component = value;
                Changed = true;
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
                X4Commands.RecalculatePriceCommand.Execute(null, null);
                NotifyPropertyChanged();
                NotifyPropertyChanged("Margin");
            }
        }

        private string m_Ware1;
        public string Ware1
        {
            get { return m_Ware1; }
            set
            {
                m_Ware1 = value;
                Changed = true;
                X4Commands.RecalculatePriceCommand.Execute(null, null);
                NotifyPropertyChanged();
            }
        }

        private int m_Amount1;
        public int Amount1
        {
            get { return m_Amount1; }
            set
            {
                m_Amount1 = value;
                Changed = true;
                X4Commands.RecalculatePriceCommand.Execute(null, null);
                NotifyPropertyChanged();
                NotifyPropertyChanged("Margin");
            }
        }

        private string m_Ware2;
        public string Ware2
        {
            get { return m_Ware2; }
            set
            {
                m_Ware2 = value;
                Changed = true;
                X4Commands.RecalculatePriceCommand.Execute(null, null);
                NotifyPropertyChanged();
            }
        }

        private int m_Amount2;
        public int Amount2
        {
            get { return m_Amount2; }
            set
            {
                m_Amount2 = value;
                Changed = true;
                X4Commands.RecalculatePriceCommand.Execute(null, null);
                NotifyPropertyChanged();
                NotifyPropertyChanged("Margin");
            }
        }

        private string m_Ware3;
        public string Ware3
        {
            get { return m_Ware3; }
            set
            {
                m_Ware3 = value;
                Changed = true;
                X4Commands.RecalculatePriceCommand.Execute(null, null);
                NotifyPropertyChanged();
            }
        }

        private int m_Amount3;
        public int Amount3
        {
            get { return m_Amount3; }
            set
            {
                m_Amount3 = value;
                Changed = true;
                X4Commands.RecalculatePriceCommand.Execute(null, null);
                NotifyPropertyChanged();
                NotifyPropertyChanged("Margin");
            }
        }

        private string m_Ware4;
        public string Ware4
        {
            get { return m_Ware4; }
            set
            {
                m_Ware4 = value;
                Changed = true;
                X4Commands.RecalculatePriceCommand.Execute(null, null);
                NotifyPropertyChanged();
            }
        }

        private int m_Amount4;
        public int Amount4
        {
            get { return m_Amount4; }
            set
            {
                m_Amount4 = value;
                Changed = true;
                X4Commands.RecalculatePriceCommand.Execute(null, null);
                NotifyPropertyChanged();
                NotifyPropertyChanged("Margin");
            }
        }

        private string m_Ware5;
        public string Ware5
        {
            get { return m_Ware5; }
            set
            {
                m_Ware5 = value;
                Changed = true;
                X4Commands.RecalculatePriceCommand.Execute(null, null);
                NotifyPropertyChanged();
            }
        }

        private int m_Amount5;
        public int Amount5
        {
            get { return m_Amount5; }
            set
            {
                m_Amount5 = value;
                Changed = true;
                X4Commands.RecalculatePriceCommand.Execute(null, null);
                NotifyPropertyChanged();
                NotifyPropertyChanged("Margin");
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

        public UIModelWare Copy()
        {
            return (UIModelWare)this.MemberwiseClone();
        }

        public void Update()
        {
            NotifyPropertyChanged("Margin");
            NotifyPropertyChanged("CalculatedPrice");
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
