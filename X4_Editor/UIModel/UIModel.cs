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

    public class UIModel : INotifyPropertyChanged
    {
        private ICollectionView m_ProjectilesDataView { get; set; }
        private ICollectionView m_MissilesDataView { get; set; }
        private ICollectionView m_ShieldsDataView { get; set; }
        private ICollectionView m_EnginesDataView { get; set; }
        private ICollectionView m_WaresDataView { get; set; }
        private ICollectionView m_ShipsDataView { get; set; }
        private ICollectionView m_WeaponsDataView { get; set; }
        private ObservableCollection<UIModelProjectile> m_ProjectilesData { get; set; }
        private ObservableCollection<UIModelMissile> m_MissilesData { get; set; }
        private ObservableCollection<UIModelShield> m_ShieldsData { get; set; }
        private ObservableCollection<UIModelEngine> m_EnginesData { get; set; }
        private ObservableCollection<UIModelWare> m_WaresData { get; set; }
        private ObservableCollection<UIModelShip> m_ShipsData { get; set; }
        private ObservableCollection<UIModelWeapon> m_WeaponsData { get; set; }
        private string m_SpecificWeaponFilter;

        public bool FilterWeaponsForWords { get; set; }
        public string DoubleClickedWeapon;

        #region filters
        // Filters
        private bool m_Shields;
        private bool m_Engines;
        private bool m_Weapons;
        private bool m_Ships;
        private bool m_Wares;
        private bool m_Size_S;
        private bool m_Size_M;
        private bool m_Size_L;
        private bool m_Size_XL;
        private bool m_Size_Other;

        public bool Shields
        {
            get { return m_Shields; }
            set
            {
                if (m_Shields == value) return;
                m_Shields = value;
                this.NotifyPropertyChanged();
            }
        }

        public bool Engines
        {
            get { return m_Engines; }
            set
            {
                if (m_Engines == value) return;
                m_Engines = value;
                this.NotifyPropertyChanged();
            }
        }

        public bool Weapons
        {
            get { return m_Weapons; }
            set
            {
                if (m_Weapons == value) return;
                m_Weapons = value;
                this.NotifyPropertyChanged();
            }
        }

        public bool Ships
        {
            get { return m_Ships; }
            set
            {
                if (m_Ships == value) return;
                m_Ships = value;
                this.NotifyPropertyChanged();
            }
        }

        public bool Wares
        {
            get { return m_Wares; }
            set
            {
                if (m_Wares == value) return;
                m_Wares = value;
                this.NotifyPropertyChanged();
            }
        }

        public bool Size_S
        {
            get { return m_Size_S; }
            set
            {
                if (m_Size_S == value) return;
                m_Size_S = value;
                this.NotifyPropertyChanged();
            }
        }

        public bool Size_M
        {
            get { return m_Size_M; }
            set
            {
                if (m_Size_M == value) return;
                m_Size_M = value;
                this.NotifyPropertyChanged();
            }
        }

        public bool Size_L
        {
            get { return m_Size_L; }
            set
            {
                if (m_Size_L == value) return;
                m_Size_L = value;
                this.NotifyPropertyChanged();
            }
        }

        public bool Size_XL
        {
            get { return m_Size_XL; }
            set
            {
                if (m_Size_XL == value) return;
                m_Size_XL = value;
                this.NotifyPropertyChanged();
            }
        }

        public bool Size_Other
        {
            get { return m_Size_Other; }
            set
            {
                if (m_Size_Other == value) return;
                m_Size_Other = value;
                this.NotifyPropertyChanged();
            }
        }
        #endregion

        #region Column filters

        public ObservableCollection<UIModelProjectile> UIModelProjectilesVanilla
        {
            get; set;
        }
        public ObservableCollection<UIModelWeapon> UIModelWeaponsVanilla
        {
            get; set;
        }
        public ObservableCollection<UIModelShield> UIModelShieldsVanilla
        {
            get; set;
        }
        public ObservableCollection<UIModelEngine> UIModelEnginesVanilla
        {
            get; set;
        }
        public ObservableCollection<UIModelMissile> UIModelMissilesVanilla
        {
            get; set;
        }
        public ObservableCollection<UIModelShip> UIModelShipsVanilla
        {
            get; set;
        }
        public ObservableCollection<UIModelWare> UIModelWaresVanilla
        {
            get; set;
        }

        public ObservableCollection<UIModelProjectile> UIModelProjectiles
        {
            get
            {
                if (m_ProjectilesData == null)
                {
                    m_ProjectilesDataView = CollectionViewSource.GetDefaultView(m_ProjectilesData);
                    m_ProjectilesDataView.Filter = FilterProjectilesData;
                }
                return m_ProjectilesData;
            }
            set
            {
                m_ProjectilesData = value;
                m_ProjectilesDataView = CollectionViewSource.GetDefaultView(m_ProjectilesData);
                m_ProjectilesDataView.Filter = FilterProjectilesData;
            }
        }

        public ObservableCollection<UIModelMissile> UIModelMissiles
        {
            get
            {
                if (m_MissilesData == null)
                {
                    m_MissilesDataView = CollectionViewSource.GetDefaultView(m_MissilesData);
                    m_MissilesDataView.Filter = FilterMissilesData;
                }
                return m_MissilesData;
            }
            set
            {
                m_MissilesData = value;
                m_MissilesDataView = CollectionViewSource.GetDefaultView(m_MissilesData);
                m_MissilesDataView.Filter = FilterMissilesData;
            }
        }

        public ObservableCollection<UIModelWeapon> UIModelWeapons
        {
            get
            {
                if (m_WeaponsData == null)
                {
                    m_WeaponsDataView = CollectionViewSource.GetDefaultView(m_WeaponsData);
                    m_WeaponsDataView.Filter = FilterTurretsData;
                }
                return m_WeaponsData;
            }
            set
            {
                m_WeaponsData = value;
                m_WeaponsDataView = CollectionViewSource.GetDefaultView(m_WeaponsData);
                m_WeaponsDataView.Filter = FilterTurretsData;
            }
        }

        public ObservableCollection<UIModelShield> UIModelShields
        {
            get
            {
                if (m_ShieldsData == null)
                {
                    m_ShieldsDataView = CollectionViewSource.GetDefaultView(m_ShieldsData);
                    m_ShieldsDataView.Filter = FilterShieldsData;
                }
                return m_ShieldsData;
            }
            set
            {
                m_ShieldsData = value;
                m_ShieldsDataView = CollectionViewSource.GetDefaultView(m_ShieldsData);
                m_ShieldsDataView.Filter = FilterShieldsData;
            }
        }

        public ObservableCollection<UIModelEngine> UIModelEngines
        {
            get
            {
                if (m_EnginesData == null)
                {
                    m_EnginesDataView = CollectionViewSource.GetDefaultView(m_EnginesData);
                    m_EnginesDataView.Filter = FilterEnginesData;
                }
                return m_EnginesData;
            }
            set
            {
                m_EnginesData = value;
                m_EnginesDataView = CollectionViewSource.GetDefaultView(m_EnginesData);
                m_EnginesDataView.Filter = FilterEnginesData;
            }
        }

        public ObservableCollection<UIModelWare> UIModelWares
        {
            get
            {
                if (m_WaresData == null)
                {
                    m_WaresDataView = CollectionViewSource.GetDefaultView(m_WaresData);
                    m_WaresDataView.Filter = FilterWaresData;
                }
                return m_WaresData;
            }
            set
            {
                m_WaresData = value;
                m_WaresDataView = CollectionViewSource.GetDefaultView(m_WaresData);
                m_WaresDataView.Filter = FilterWaresData;
            }
        }

        public ObservableCollection<UIModelShip> UIModelShips
        {
            get
            {
                if (m_ShipsData == null)
                {
                    m_ShipsDataView = CollectionViewSource.GetDefaultView(m_ShipsData);
                    m_ShipsDataView.Filter = FilterShipsData;
                }
                return m_ShipsData;
            }
            set
            {
                m_ShipsData = value;
                m_ShipsDataView = CollectionViewSource.GetDefaultView(m_ShipsData);
                m_ShipsDataView.Filter = FilterShipsData;
            }
        }

        private string m_Path;
        public string Path
        {
            get {
                if (!Directory.Exists(m_Path))
                    return "enter vanilla X4 folder path here...";
                return m_Path;
            }
            set
            {
                if (!Directory.Exists(value) && !string.IsNullOrEmpty(value))
                    return;
                m_Path = value;
                NotifyPropertyChanged();
            }
        }

        private string m_ModPath1;
        public string ModPath1
        {
            get
            {
                if (!Directory.Exists(m_ModPath1))
                    return "enter valid base mod path here...";
                return m_ModPath1;
            }
            set
            {
                if (!Directory.Exists(value) && !string.IsNullOrEmpty(value))
                    return;
                m_ModPath1 = value;
                NotifyPropertyChanged();
            }
        }

        private string m_ModPath2;
        public string ModPath2
        {
            get
            {
                if (!Directory.Exists(m_ModPath2))
                    return "enter active mod folder path here...";
                return m_ModPath2;
            }
            set
            {
                if (!Directory.Exists(value) && !string.IsNullOrEmpty(value))
                    return;
                m_ModPath2 = value;
                NotifyPropertyChanged();
            }
        }

        private string m_ExportPath;
        public string ExportPath
        {
            get
            {
                if (!Directory.Exists(m_ExportPath))
                    return "enter export folder path here...";
                return m_ExportPath;
            }
            set
            {
                if (!Directory.Exists(value) && !string.IsNullOrEmpty(value))
                    return;
                if (value == Path || value == ModPath1)
                {
                    MessageBox.Show("Export Path must not be Vanilla Path or Base Mod Path. It must be Active Mod path or a new one!");
                    m_ExportPath = "enter export folder path here...";
                    NotifyPropertyChanged();
                    return;
                }
                m_ExportPath = value;
                NotifyPropertyChanged();
            }
        }

        private string m_SearchText;
        public string SearchText
        {
            get { return m_SearchText; }
            set
            {
                if (m_SearchText == value)
                    return;
                m_SearchText = value;
                this.NotifyPropertyChanged();
            }
        }

        private double m_MathParameter;
        public double MathParameter
        {
            get 
            { 
                return m_MathParameter; 
            }
            set
            {
                if (m_MathParameter == value)
                    return;
                m_MathParameter = value;
                this.NotifyPropertyChanged();
            }
        }

        private bool m_UseFilters;
        public bool UseFilters
        {
            get { return m_UseFilters; }
            set
            {
                if (m_UseFilters == value)
                    return;
                m_UseFilters = value;
                this.NotifyPropertyChanged();
            }
        }

        private bool m_AllWaresLoaded;
        public bool AllWaresLoaded
        {
            get { return m_AllWaresLoaded; }
            set
            {
                if (m_AllWaresLoaded == value)
                    return;
                m_AllWaresLoaded = value;
                this.NotifyPropertyChanged();
            }
        }

        public UIModel()
        {
            UIModelProjectiles = new ObservableCollection<UIModelProjectile>();
            UIModelWeapons = new ObservableCollection<UIModelWeapon>();
            UIModelMissiles = new ObservableCollection<UIModelMissile>();
            UIModelShields = new ObservableCollection<UIModelShield>();
            UIModelEngines = new ObservableCollection<UIModelEngine>();
            UIModelWares = new ObservableCollection<UIModelWare>();
            UIModelShips = new ObservableCollection<UIModelShip>();
            this.UIModelMissilesVanilla = new ObservableCollection<UIModelMissile>();
            this.UIModelProjectilesVanilla = new ObservableCollection<UIModelProjectile>();
            this.UIModelWeaponsVanilla = new ObservableCollection<UIModelWeapon>();
            this.UIModelShieldsVanilla = new ObservableCollection<UIModelShield>();
            this.UIModelShipsVanilla = new ObservableCollection<UIModelShip>();
            this.UIModelWaresVanilla = new ObservableCollection<UIModelWare>();
            this.UIModelEnginesVanilla = new ObservableCollection<UIModelEngine>();
            UseFilters = true;
            Shields = true;
            Engines = true;
            Weapons = true;
            Ships = true;
            Wares = true;
            Size_S = true;
            Size_M = true;
            Size_L = true;
            Size_XL = true;
            Size_Other = true;
            AllWaresLoaded = false;
        }

        public void SetProjectileFilter(string projectile)
        {
            m_SpecificWeaponFilter = projectile;
            m_ProjectilesDataView.Filter = this.FilterSpecificProjectile;
        }

        public void SetWeaponsFilter(string projectile)
        {
            m_SpecificWeaponFilter = projectile;
            m_WeaponsDataView.Filter = this.FilterSpecificWeapon;
        }

        public void SetFilters()
        {
                m_ShieldsDataView.Filter = this.FilterShieldsData;
                m_EnginesDataView.Filter = this.FilterEnginesData;
                m_ProjectilesDataView.Filter = this.FilterProjectilesData;
                m_MissilesDataView.Filter = this.FilterMissilesData;
                m_ShipsDataView.Filter = this.FilterShipsData;
                m_WeaponsDataView.Filter = this.FilterTurretsData;
                m_WaresDataView.Filter = this.FilterWaresData;
        }
        private bool FilterShieldsData(object item)
        {
            if (!Shields)
                return false;

            UIModelShield shield = item as UIModelShield;
            List<string> searchArray = new List<string>();

            if ((Size_S && shield.Size == "S") || (m_Size_M && shield.Size == "M") || (m_Size_L && shield.Size == "L") || (m_Size_XL && shield.Size == "XL") || (m_Size_Other && (shield.Size != "S" && shield.Size != "M" && shield.Size != "L" && shield.Size != "XL" )))
            {
                if (this.SearchText != null && this.SearchText.Length > 0)
                {
                    searchArray = this.SearchText.Split(' ').ToList();
                }
                // filter entered
                if (searchArray.Count > 0)
                {
                    bool found = false;
                    foreach (string searchString in searchArray)
                    {
                        if (searchString.Length > 0)
                        {

                            if (shield.Name.ToUpper().Contains(searchString.ToUpper()) || shield.IGName.ToUpper().Contains(searchString.ToUpper()))
                            {
                                found = true;
                            }
                        }
                    }
                    return found;
                }
                return true;
            }
            return false;
        }

        private bool FilterSpecificProjectile(object item)
        {
            UIModelProjectile projectile = item as UIModelProjectile;
            List<string> searchArray = new List<string>();

            bool found = false;

            if (m_SpecificWeaponFilter.Length > 0)
            {
                if (projectile.Name.ToUpper().Contains(m_SpecificWeaponFilter.ToUpper()))
                {
                    DoubleClickedWeapon = "";
                    found = true;
                }
                DoubleClickedWeapon = "";
            }
            else 
                return true;
            return found;
        }

        private bool FilterSpecificWeapon(object item)
        {
            UIModelWeapon weapon = item as UIModelWeapon;
            List<string> searchArray = new List<string>();

            bool found = false;

            if (m_SpecificWeaponFilter.Length > 0)
            {
                if (weapon.Projectile.ToUpper().Contains(m_SpecificWeaponFilter.ToUpper()))
                {
                    found = true;
                }
            }
            return found;
        }

        private bool FilterTurretsData(object item)
        {
            if (!Weapons)
                return false;

            UIModelWeapon weapon = item as UIModelWeapon;
            List<string> searchArray = new List<string>();

            if ((Size_S && weapon.Size == "S") || (m_Size_M && weapon.Size == "M") || (m_Size_L && weapon.Size == "L") || (m_Size_XL && weapon.Size == "XL") || (m_Size_Other && (weapon.Size != "S" && weapon.Size != "M" && weapon.Size != "L" && weapon.Size != "XL")))
            {
                if (this.SearchText != null && this.SearchText.Length > 0)
                {
                    searchArray = this.SearchText.Split(' ').ToList();
                }
                // filter entered
                if (searchArray.Count > 0)
                {
                    bool found = false;
                    foreach (string searchString in searchArray)
                    {
                        if (searchString.Length > 0)
                        {
                            if (weapon.Name.ToUpper().Contains(searchString.ToUpper()) || weapon.Projectile.ToUpper().Contains(searchString.ToUpper()) || weapon.IGName.ToUpper().Contains(searchString.ToUpper()) )
                            {
                                found = true;
                            }
                        }
                    }
                    return found;
                }
                return true;
            }
            return false;
        }

        private bool FilterEnginesData(object item)
        {
            if (!Engines)
                return false;

            UIModelEngine engine = item as UIModelEngine;

            if ((Size_S && engine.Size == "S") || (m_Size_M && engine.Size == "M") || (m_Size_L && engine.Size == "L") || (m_Size_XL && engine.Size == "XL") || (m_Size_Other && (engine.Size != "S" && engine.Size != "M" && engine.Size != "L" && engine.Size != "XL")))
            {
                List<string> searchArray = new List<string>();
                if (this.SearchText != null && this.SearchText.Length > 0)
                {
                    searchArray = this.SearchText.Split(' ').ToList();
                }
                // filter entered
                if (searchArray.Count > 0)
                {
                    bool found = false;
                    foreach (string searchString in searchArray)
                    {
                        if (searchString.Length > 0)
                        {
                            if (engine.Name.ToUpper().Contains(searchString.ToUpper()) || engine.IGName.ToUpper().Contains(searchString.ToUpper()))
                            {
                                found = true;
                            }
                        }
                    }
                    return found;
                }
                return true;
            }
            return false;
        }
        private bool FilterProjectilesData(object item)
        {
            if (!Weapons)
                return false;

            UIModelProjectile projectile = item as UIModelProjectile;

            if ((Size_S && projectile.Size == "S") || (m_Size_M && projectile.Size == "M") || (m_Size_L && projectile.Size == "L") || (m_Size_XL && projectile.Size == "XL") || (m_Size_Other && (projectile.Size != "S" && projectile.Size != "M" && projectile.Size != "L" && projectile.Size != "XL")))
            {
                List<string> searchArray = new List<string>();
                if (this.SearchText != null && this.SearchText.Length > 0)
                {
                    searchArray = this.SearchText.Split(' ').ToList();
                }
                // filter entered
                if (searchArray.Count > 0)
                {
                    bool found = false;
                    foreach (string searchString in searchArray)
                    {
                        if (searchString.Length > 0)
                        {

                            if (projectile.Name.ToUpper().Contains(searchString.ToUpper()))
                            {
                                found = true;
                            }
                        }
                    }
                    return found;
                }
                return true;
            }
            return false;
        }

        private bool FilterMissilesData(object item)
        {
            if (!Weapons)
                return false;

            UIModelMissile missile = item as UIModelMissile;
            List<string> searchArray = new List<string>();

            if (this.SearchText != null && this.SearchText.Length > 0)
            {
                searchArray = this.SearchText.Split(' ').ToList();
            }
            // filter entered
            if (searchArray.Count > 0)
            {
                bool found = false;
                foreach (string searchString in searchArray)
                {
                    if (searchString.Length > 0)
                    {
                        if (missile.Name.ToUpper().Contains(searchString.ToUpper()) || missile.IGName.ToUpper().Contains(searchString.ToUpper()))
                        {
                            found = true;
                        }
                    }
                }
                return found;
            }
            return true;
        }

        private bool FilterShipsData(object item)
        {
            if (!Ships)
                return false;

            UIModelShip ship = item as UIModelShip;

            if ((Size_S && ship.Size == "S") || (m_Size_M && ship.Size == "M") || (m_Size_L && ship.Size == "L") || (m_Size_XL && ship.Size == "XL") || (m_Size_Other && (ship.Size != "S" && ship.Size != "M" && ship.Size != "L" && ship.Size != "XL")))
            {
                List<string> searchArray = new List<string>();
                if (this.SearchText != null && this.SearchText.Length > 0)
                {
                    searchArray = this.SearchText.Split(' ').ToList();
                }
                // filter entered
                if (searchArray.Count > 0)
                {
                    bool found = false;
                    foreach (string searchString in searchArray)
                    {
                        if (searchString.Length > 0)
                        {
                            if (ship.Name.ToUpper().Contains(searchString.ToUpper()) || ship.IGName.ToUpper().Contains(searchString.ToUpper()))
                            {
                                found = true;
                            }
                        }
                    }
                    return found;
                }
                return true;
            }
            return false;
        }
        private bool FilterWaresData(object item)
        {
            UIModelWare ware = item as UIModelWare;

            if (Wares)
            {
                if ((Size_S && ware.Size == "S") || (m_Size_M && ware.Size == "M") || (m_Size_L && ware.Size == "L") || (m_Size_XL && ware.Size == "XL") || (m_Size_Other && (ware.Size != "S" && ware.Size != "M" && ware.Size != "L" && ware.Size != "XL")))
                {
                    List<string> searchArray = new List<string>();
                    if (this.SearchText != null && this.SearchText.Length > 0)
                    {
                        searchArray = this.SearchText.Split(' ').ToList();
                    }
                    // filter entered
                    if (searchArray.Count > 0)
                    {
                        bool found = false;
                        foreach (string searchString in searchArray)
                        {
                            if (searchString.Length > 0)
                            {
                                if (ware.Name.ToUpper().Contains(searchString.ToUpper()))
                                {
                                    found = true;
                                }
                            }
                        }
                        return found;
                    }
                    return true;
                }
                return false;
            }
            else
            {
                bool found = false;

                foreach (var s in m_ShipsDataView)
                {
                    UIModelShip ship = s as UIModelShip;
                    if (ship != null && ship.Name.Contains(ware.Name))
                        return true;
                }
                foreach (var e in m_EnginesDataView)
                {
                    UIModelEngine engine = e as UIModelEngine;
                    if (engine != null && engine.Name.Contains(ware.Name))
                        return true;
                }
                foreach (var s in m_ShieldsDataView)
                {
                    UIModelShield shield = s as UIModelShield;
                    if (shield != null && shield.Name.Contains(ware.Name))
                        return true;
                }
                foreach (var p in m_ProjectilesDataView)
                {
                    UIModelProjectile weapon = p as UIModelProjectile;
                    if (weapon != null &&weapon.Name.Contains(ware.Name))
                        return true;
                }
                foreach (var w in m_WeaponsDataView)
                {
                    UIModelWeapon weapon = w as UIModelWeapon;
                    if (weapon != null && weapon.Name.Contains(ware.Name))
                        return true;
                }
                foreach (var w in m_ProjectilesDataView)
                {
                    UIModelWeapon weapon = w as UIModelWeapon;
                    if (weapon != null && weapon.Name.Contains(ware.Name))
                        return true;
                }
                foreach (var m in m_MissilesDataView)
                {
                    UIModelMissile missile = m as UIModelMissile;
                    if (missile != null && missile.Name.Contains(ware.Name))
                        return true;
                }
                return found;
            }
        }

        public void CalculateWarePrices()
        {
            if (AllWaresLoaded)
            {
                foreach (var item in this.UIModelWares)
                {
                    item.CalculatedPrice = 0;
                    if (item.Ware1 != null)
                    {
                        var match = this.UIModelWares.FirstOrDefault(X => X.Name == item.Ware1);
                        item.CalculatedPrice = item.CalculatedPrice + match.Avg * item.Amount1;
                    }
                    if (item.Ware2 != null)
                    {
                        var match = this.UIModelWares.FirstOrDefault(X => X.Name == item.Ware2);
                        item.CalculatedPrice = item.CalculatedPrice + match.Avg * item.Amount2;
                    }
                    if (item.Ware3 != null)
                    {
                        var match = this.UIModelWares.FirstOrDefault(X => X.Name.Equals(item.Ware3));
                        item.CalculatedPrice = item.CalculatedPrice + match.Avg * item.Amount3;
                    }
                    if (item.Ware4 != null)
                    {
                        var match = this.UIModelWares.FirstOrDefault(X => X.Name == item.Ware4);
                        item.CalculatedPrice = item.CalculatedPrice + match.Avg * item.Amount4;
                    }
                    if (item.Ware5 != null)
                    {
                        var match = this.UIModelWares.FirstOrDefault(X => X.Name == item.Ware5);
                        item.CalculatedPrice = item.CalculatedPrice + match.Avg * item.Amount5;
                    }
                    item.Update();
                }
            }
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
#endregion