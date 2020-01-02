using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using X4_Editor.Helper;

namespace X4_Editor
{
    public class ModFilesReader
    {
        private UIManager parent;
        private XmlExtractor m_XmlExtractor;

        public ModFilesReader(UIManager Parent, XmlExtractor XmlExtractor)
        {
            parent = Parent;
            m_XmlExtractor = XmlExtractor;
        }
        public void ReadAllModFilesFromFolder(string modPath)
        {
            try
            {
                if (!Directory.Exists(modPath))
                {
                    MessageBox.Show("Enter a valid folder path for mod files", "No valid mod folder");
                }
                else
                {
                    List<string> ModDirectories = Directory.GetDirectories(modPath, "*", SearchOption.AllDirectories).ToList();

                    Dictionary<string, string> ModTexts = new Dictionary<string, string>();
                    ModTexts = m_XmlExtractor.ReadTextXml(modPath + parent.PathToTexts + @"\0001.xml", ModTexts);

                    while (ModTexts.Count > 0)
                    {
                        if (parent.TextDictionary.ContainsKey(ModTexts.First().Key))
                            parent.TextDictionary[ModTexts.First().Key] = ModTexts.First().Value;
                        else
                            parent.TextDictionary.Add(ModTexts.First().Key, ModTexts.First().Value);

                        ModTexts.Remove(ModTexts.First().Key);
                    }

                    // read wares of mod
                    m_XmlExtractor.ReadAllWares(modPath + @"\libraries\wares.xml");

                    foreach (string dir in ModDirectories)
                    {
                        //shields
                        if (dir.Contains("assets\\props\\SurfaceElements\\macros"))
                        {
                            List<string> files = Directory.GetFiles(dir).ToList();

                            foreach (string file in files)
                            {
                                try
                                {
                                    var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                    string[] fileDepth = fileStream.Name.Split('\\');
                                    string fileName = fileDepth[fileDepth.Count() - 1];
                                    using (StreamReader sr = new StreamReader(fileStream))
                                    {
                                        var shield = parent.UIModel.UIModelShields.FirstOrDefault(x => x.File.Contains(fileName));
                                        var shieldVanilla = parent.UIModel.UIModelShieldsVanilla.FirstOrDefault(x => x.File.Contains(fileName));
                                        if (shield != null)
                                        {
                                            int index = parent.UIModel.UIModelShields.IndexOf(shield);
                                            string line;

                                            while (!sr.EndOfStream)
                                            {
                                                line = sr.ReadLine();

                                                if (line.Contains(@"<replace") && line.Contains("sel") && line.Contains(@"/macros") && !line.Contains("@"))
                                                {
                                                    parent.UIModel.UIModelShields[index] = m_XmlExtractor.ReadSingleShield(new FileInfo(file));
                                                    break;
                                                }

                                                if (line.Contains("@max") && line.Contains("recharge"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    shield.Max = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("@rate"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    shield.Rate = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@delay"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    shield.Delay = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@max") && line.Contains("hull"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    shield.MaxHull = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@threshold") && line.Contains("hull"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    shield.Threshold = Utility.ParseToDouble(value);
                                                }
                                            }
                                            shieldVanilla.Delay = shield.Delay;
                                            shieldVanilla.Faction = shield.Faction;
                                            shieldVanilla.Max = shield.Max;
                                            shieldVanilla.MaxHull = shield.MaxHull;
                                            shieldVanilla.Rate = shield.Rate;
                                            shieldVanilla.Threshold = shield.Threshold;
                                            shieldVanilla.Changed = false;
                                            shield.Changed = false;
                                        }
                                        else
                                        {
                                            UIModelShield extractedShield = m_XmlExtractor.ReadSingleShield(new FileInfo(file));
                                            if (extractedShield.Name.Length > 1)
                                            {
                                                parent.UIModel.UIModelShields.Add(extractedShield);
                                                parent.UIModel.UIModelShieldsVanilla.Add(extractedShield);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(file + " could not be read.");
                                }
                            }
                        }
                        //engines
                        if (dir.Contains("assets\\props\\Engines\\macros"))
                        {
                            List<string> files = Directory.GetFiles(dir).ToList();

                            foreach (string file in files)
                            {
                                try
                                {
                                    var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                    string[] fileDepth = fileStream.Name.Split('\\');
                                    string fileName = fileDepth[fileDepth.Count() - 1];
                                    using (StreamReader sr = new StreamReader(fileStream))
                                    {
                                        var engine = parent.UIModel.UIModelEngines.FirstOrDefault(x => x.File.Contains(fileName));
                                        var engineVanilla = parent.UIModel.UIModelEnginesVanilla.FirstOrDefault(x => x.File.Contains(fileName));
                                        if (engine != null)
                                        {
                                            int index = parent.UIModel.UIModelEngines.IndexOf(engine);
                                            string line;

                                            while (!sr.EndOfStream)
                                            {
                                                line = sr.ReadLine();

                                                if (line.Contains(@"<replace") && line.Contains("sel") && line.Contains(@"/macros") && !line.Contains("@"))
                                                {
                                                    parent.UIModel.UIModelEngines[index] = m_XmlExtractor.ReadSingleEngineFile(new FileInfo(file));
                                                    break;
                                                }

                                                if (line.Contains("@duration") && line.Contains("boost"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.BoostDuration = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@thrust") && line.Contains("boost"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.BoostThrust = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@attack") && line.Contains("boost"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.BoostAttack = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@release") && line.Contains("boost"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.BoostRelease = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@charge") && line.Contains("travel"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.TravelCharge = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("@thrust") && line.Contains("travel"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.TravelThrust = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@attack") && line.Contains("travel"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.TravelAttack = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@release") && line.Contains("travel"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.TravelRelease = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@forward"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.Forward = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@reverse"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.Reverse = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@strafe") && line.Contains("thrust"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.Strafe = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@pitch") && line.Contains("thrust"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.Pitch = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@yaw") && line.Contains("thrust"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.Yaw = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@roll") && line.Contains("thrust"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.Roll = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@roll") && line.Contains("angular"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.AngularRoll = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@pitch") && line.Contains("angular"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.AngularPitch = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@max") && line.Contains("hull"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.MaxHull = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@threshold") && line.Contains("hull"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    engine.Threshold = Utility.ParseToDouble(value);
                                                }
                                            }
                                            engineVanilla.AngularPitch = engine.AngularPitch;
                                            engineVanilla.AngularRoll = engine.AngularRoll;
                                            engineVanilla.BoostAttack = engine.BoostAttack;
                                            engineVanilla.BoostDuration = engine.BoostDuration;
                                            engineVanilla.BoostRelease = engine.BoostRelease;
                                            engineVanilla.BoostThrust = engine.BoostThrust;
                                            engineVanilla.Forward = engine.Forward;
                                            engineVanilla.MaxHull = engine.MaxHull;
                                            engineVanilla.Pitch = engine.Pitch;
                                            engineVanilla.Reverse = engine.Reverse;
                                            engineVanilla.Roll = engine.Roll;
                                            engineVanilla.Strafe = engine.Strafe;
                                            engineVanilla.Threshold = engine.Threshold;
                                            engineVanilla.TravelAttack = engine.TravelAttack;
                                            engineVanilla.TravelCharge = engine.TravelCharge;
                                            engineVanilla.TravelRelease = engine.TravelRelease;
                                            engineVanilla.TravelThrust = engine.TravelThrust;
                                            engineVanilla.Yaw = engine.Yaw;
                                            engineVanilla.Changed = false;
                                            engine.Changed = false;
                                        }
                                        else
                                        {
                                            UIModelEngine extractedEngine = m_XmlExtractor.ReadSingleEngineFile(new FileInfo(file));
                                            if (extractedEngine.Name.Length > 1)
                                            {
                                                parent.UIModel.UIModelEngines.Add(extractedEngine);
                                                parent.UIModel.UIModelEnginesVanilla.Add(extractedEngine);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(file + " could not be read.");
                                }
                            }
                        }
                        //projectiles 
                        if (dir.ToUpper().Contains("ASSETS\\FX\\WEAPONFX\\MACROS"))
                        {
                            List<string> files = Directory.GetFiles(dir).ToList();

                            foreach (string file in files)
                            {
                                try
                                {
                                    var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                    string[] fileDepth = fileStream.Name.Split('\\');
                                    string fileName = fileDepth[fileDepth.Count() - 1];
                                    using (StreamReader sr = new StreamReader(fileStream))
                                    {
                                        // Projectiles
                                        if (parent.UIModel.UIModelProjectiles.Any(x => x.File.Contains(fileName)))
                                        {
                                            var weaponProjectile = parent.UIModel.UIModelProjectiles.FirstOrDefault(x => x.File.Contains(fileName));
                                            var weaponProjectileVanilla = parent.UIModel.UIModelProjectilesVanilla.FirstOrDefault(x => x.File.Contains(fileName));

                                            if (weaponProjectile != null)
                                            {
                                                int index = parent.UIModel.UIModelProjectiles.IndexOf(weaponProjectile);

                                                string line;
                                                while (!sr.EndOfStream)
                                                {
                                                    line = sr.ReadLine();

                                                    if (line.Contains(@"<replace") && line.Contains("sel") && line.Contains("/macros") && !line.Contains("@"))
                                                    {
                                                        parent.UIModel.UIModelProjectiles[index] = m_XmlExtractor.ReadSingleProjectile(new FileInfo(file));
                                                        break;
                                                    }

                                                    if (line.Contains("@value") && line.Contains("ammunition"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.Ammunition = Convert.ToInt32(value);
                                                    }
                                                    if (line.Contains("@reload") && line.Contains("ammunition"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.AmmunitionReload = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("@speed") && line.Contains("bullet"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.Speed = Convert.ToInt32(value);
                                                    }
                                                    if (line.Contains("@lifetime") && line.Contains("bullet"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.Lifetime = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("@amount") && line.Contains("bullet"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.Amount = Convert.ToInt32(value);
                                                    }
                                                    if (line.Contains("@barrelamount") && line.Contains("bullet"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.BarrelAmount = Convert.ToInt32(value);
                                                    }
                                                    if (line.Contains("@timediff") && line.Contains("bullet"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.TimeDiff = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("@angle") && line.Contains("bullet"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.Angle = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("@scale") && line.Contains("bullet"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.Scale = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("@ricochet") && line.Contains("bullet"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.Ricochet = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("@chargetime") && line.Contains("bullet"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.ChargeTime = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("@value") && line.Contains("heat"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.HeatValue = Convert.ToInt32(value);
                                                    }
                                                    if (line.Contains("@initial") && line.Contains("heat"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.HeatInitial = Convert.ToInt32(value);
                                                    }
                                                    if (line.Contains("@rate") && line.Contains("reload"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.ReloadRate = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("@time") && line.Contains("reload"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.ReloadTime = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("@value") && line.Contains("damage"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.Damage = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("@shield") && line.Contains("damage"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.Shield = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("@repair") && line.Contains("damage"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weaponProjectile.Repair = Utility.ParseToDouble(value);
                                                    }
                                                }
                                            }
                                            weaponProjectileVanilla.Ammunition = weaponProjectile.Ammunition;
                                            weaponProjectileVanilla.AmmunitionReload = weaponProjectile.AmmunitionReload;
                                            weaponProjectileVanilla.Amount = weaponProjectile.Amount;
                                            weaponProjectileVanilla.Angle = weaponProjectile.Angle;
                                            weaponProjectileVanilla.BarrelAmount = weaponProjectile.BarrelAmount;
                                            weaponProjectileVanilla.ChargeTime = weaponProjectile.ChargeTime;
                                            weaponProjectileVanilla.Damage = weaponProjectile.Damage;
                                            weaponProjectileVanilla.HeatInitial = weaponProjectile.HeatInitial;
                                            weaponProjectileVanilla.HeatValue = weaponProjectile.HeatValue;
                                            weaponProjectileVanilla.Lifetime = weaponProjectile.Lifetime;
                                            weaponProjectileVanilla.MaxHits = weaponProjectile.MaxHits;
                                            weaponProjectileVanilla.Range = weaponProjectile.Range;
                                            weaponProjectileVanilla.ReloadRate = weaponProjectile.ReloadRate;
                                            weaponProjectileVanilla.ReloadTime = weaponProjectile.ReloadTime;
                                            weaponProjectileVanilla.Repair = weaponProjectile.Repair;
                                            weaponProjectileVanilla.Ricochet = weaponProjectile.Ricochet;
                                            weaponProjectileVanilla.Shield = weaponProjectile.Shield;
                                            weaponProjectileVanilla.Speed = weaponProjectile.Speed;
                                            weaponProjectileVanilla.TimeDiff = weaponProjectile.TimeDiff;
                                            weaponProjectileVanilla.Changed = false;
                                            weaponProjectile.Changed = false;
                                        }
                                        else
                                        {
                                            UIModelProjectile extractedProjectile = m_XmlExtractor.ReadSingleProjectile(new FileInfo(file));
                                            if (extractedProjectile.Name.Length > 1)
                                            {
                                                parent.UIModel.UIModelProjectiles.Add(extractedProjectile);
                                                parent.UIModel.UIModelProjectilesVanilla.Add(extractedProjectile);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(file + " could not be read.");
                                }
                            }
                        }
                        // weapons
                        if (dir.Contains("assets\\props\\WeaponSystems"))
                        {
                            List<string> files = new List<string>();

                            if (Directory.Exists(modPath + parent.PathToTurretsStandard))
                                files.AddRange(Directory.GetFiles(modPath + parent.PathToTurretsStandard).ToList());
                            if (Directory.Exists(modPath + parent.PathToTurretsEnergy))
                                files.AddRange(Directory.GetFiles(modPath + parent.PathToTurretsEnergy).ToList());
                            if (Directory.Exists(modPath + parent.PathToTurretsCapital))
                                files.AddRange(Directory.GetFiles(modPath + parent.PathToTurretsCapital).ToList());
                            if (Directory.Exists(modPath + parent.PathToTurretsHeavy))
                                files.AddRange(Directory.GetFiles(modPath + parent.PathToTurretsHeavy).ToList());
                            if (Directory.Exists(modPath + parent.PathToTurretsGuided))
                                files.AddRange(Directory.GetFiles(modPath + parent.PathToTurretsGuided).ToList());
                            if (Directory.Exists(modPath + parent.PathToTurretsDumbfire))
                                files.AddRange(Directory.GetFiles(modPath + parent.PathToTurretsDumbfire).ToList());

                            foreach (string file in files)
                            {
                                try
                                {
                                    var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                    string[] fileDepth = fileStream.Name.Split('\\');
                                    string fileName = fileDepth[fileDepth.Count() - 1];
                                    using (StreamReader sr = new StreamReader(fileStream))
                                    {
                                        if (parent.UIModel.UIModelWeapons.Any(x => x.File.Contains(fileName)))
                                        {
                                            var weapon = parent.UIModel.UIModelWeapons.FirstOrDefault(x => x.File.Contains(fileName));
                                            var weaponVanilla = parent.UIModel.UIModelWeaponsVanilla.FirstOrDefault(x => x.File.Contains(fileName));

                                            if (weapon != null)
                                            {
                                                int index = parent.UIModel.UIModelWeapons.IndexOf(weapon);
                                                string line;
                                                while (!sr.EndOfStream)
                                                {
                                                    line = sr.ReadLine();

                                                    if (line.Contains(@"<replace") && line.Contains("sel") && line.Contains(@"/macros") && !line.Contains("@"))
                                                    {
                                                        parent.UIModel.UIModelWeapons[index] = m_XmlExtractor.ReadSingleWeapon(new FileInfo(file));
                                                        break;
                                                    }

                                                    if (line.Contains("class") && line.Contains("bullet"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weapon.Projectile = value;
                                                    }
                                                    if (line.Contains("rotationspeed") && line.Contains("@max"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weapon.RotationSpeed = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("rotationacceleration") && line.Contains("@max"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weapon.RotationAcceleration = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("reload") && line.Contains("@rate"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weapon.ReloadRate = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("reload") && line.Contains("@time"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weapon.ReloadTime = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("hull") && line.Contains("@max"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weapon.HullMax = Convert.ToInt32(value);
                                                    }
                                                    if (line.Contains("reload") && line.Contains("@threshold"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weapon.HullThreshold = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("heat") && line.Contains("@overheat"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weapon.Overheat = Convert.ToInt32(value);
                                                    }
                                                    if (line.Contains("heat") && line.Contains("@coolDelay"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weapon.CoolDelay = Utility.ParseToDouble(value);
                                                    }
                                                    if (line.Contains("heat") && line.Contains("@coolrate"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weapon.CoolRate = Convert.ToInt32(value);
                                                    }
                                                    if (line.Contains("heat") && line.Contains("@reenable"))
                                                    {
                                                        string value = line.Split('>')[1].Split('<')[0];
                                                        weapon.Reenable = Convert.ToInt32(value);
                                                    }
                                                }
                                                weaponVanilla.CoolDelay = weapon.CoolDelay;
                                                weaponVanilla.CoolRate = weapon.CoolRate;
                                                weaponVanilla.HullMax = weapon.HullMax;
                                                weaponVanilla.HullThreshold = weapon.HullThreshold;
                                                weaponVanilla.Overheat = weapon.Overheat;
                                                weaponVanilla.Reenable = weapon.Reenable;
                                                weaponVanilla.ReloadRate = weapon.ReloadRate;
                                                weaponVanilla.ReloadTime = weapon.ReloadTime;
                                                weaponVanilla.RotationAcceleration = weapon.RotationAcceleration;
                                                weaponVanilla.RotationSpeed = weapon.RotationSpeed;
                                                weaponVanilla.Changed = false;
                                                weapon.Changed = false;
                                            }
                                            else
                                            {
                                                parent.UIModel.UIModelWeapons.Add(m_XmlExtractor.ReadSingleWeapon(new FileInfo(file)));
                                                parent.UIModel.UIModelWeaponsVanilla.Add(m_XmlExtractor.ReadSingleWeapon(new FileInfo(file)));
                                            }
                                        }

                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(file + " could not be read.");
                                }
                            }
                        }
                        // missiles
                        if (dir.Contains(@"assets\props\WeaponSystems\missile\macros"))
                        {
                            List<string> files = Directory.GetFiles(dir).ToList();

                            foreach (string file in files)
                            {
                                try
                                {
                                    var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                    string[] fileDepth = fileStream.Name.Split('\\');
                                    string fileName = fileDepth[fileDepth.Count() - 1];
                                    using (StreamReader sr = new StreamReader(fileStream))
                                    {
                                        var weapon = parent.UIModel.UIModelMissiles.FirstOrDefault(x => x.File.Contains(fileName));
                                        var weaponVanilla = parent.UIModel.UIModelMissilesVanilla.FirstOrDefault(x => x.File.Contains(fileName));
                                        if (weapon != null)
                                        {
                                            int index = parent.UIModel.UIModelMissiles.IndexOf(weapon);
                                            string line;
                                            while (!sr.EndOfStream)
                                            {
                                                line = sr.ReadLine();

                                                if (line.Contains(@"<replace") && line.Contains("sel") && line.Contains(@"/macros") && !line.Contains("@"))
                                                {
                                                    parent.UIModel.UIModelMissiles[index] = m_XmlExtractor.ReadSingleMissile(new FileInfo(file));
                                                    break;
                                                }

                                                if (line.Contains("ammunition") && line.Contains("@value"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Ammunition = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("missile") && line.Contains("@amount"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.MissileAmount = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("missile") && line.Contains("@barrelamount"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.BarrelAmount = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("missile") && line.Contains("@lifetime"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Lifetime = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("missile") && line.Contains("@range"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Range = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("missile") && line.Contains("@guided"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Guided = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("missile") && line.Contains("@swarm"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Swarm = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("missile") && line.Contains("@retarget"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Retarget = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("explosiondamage") && line.Contains("@value"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Damage = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("reload") && line.Contains("@time"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Reload = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("hull") && line.Contains("@max"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Hull = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("drag") && line.Contains("@forward"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Forward = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("drag") && line.Contains("@forward"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Forward = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("drag") && line.Contains("@reverse"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Reverse = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("drag") && line.Contains("@horizontal"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Horizontal = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("drag") && line.Contains("@vertical"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Vertical = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("drag") && line.Contains("@pitch"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Pitch = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("drag") && line.Contains("@yaw"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Yaw = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("drag") && line.Contains("@roll"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Roll = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("physics") && line.Contains("@mass"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.Mass = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("inertia") && line.Contains("@pitch"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.InertiaPitch = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("inertia") && line.Contains("@roll"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.InertiaRoll = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("inertia") && line.Contains("@yaw"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    weapon.InertiaYaw = Utility.ParseToDouble(value);
                                                }
                                            }
                                            weaponVanilla.Ammunition = weapon.Ammunition;
                                            weaponVanilla.BarrelAmount = weapon.BarrelAmount;
                                            weaponVanilla.Damage = weapon.Damage;
                                            weaponVanilla.Forward = weapon.Forward;
                                            weaponVanilla.Guided = weapon.Guided;
                                            weaponVanilla.Horizontal = weapon.Horizontal;
                                            weaponVanilla.Hull = weapon.Hull;
                                            weaponVanilla.InertiaPitch = weapon.InertiaPitch;
                                            weaponVanilla.InertiaRoll = weapon.InertiaRoll;
                                            weaponVanilla.InertiaYaw = weapon.InertiaYaw;
                                            weaponVanilla.Lifetime = weapon.Lifetime;
                                            weaponVanilla.Mass = weapon.Mass;
                                            weaponVanilla.MissileAmount = weapon.MissileAmount;
                                            weaponVanilla.Pitch = weapon.Pitch;
                                            weaponVanilla.Range = weapon.Range;
                                            weaponVanilla.Reload = weapon.Reload;
                                            weaponVanilla.Retarget = weapon.Retarget;
                                            weaponVanilla.Reverse = weapon.Reverse;
                                            weaponVanilla.Roll = weapon.Roll;
                                            weaponVanilla.Swarm = weapon.Swarm;
                                            weaponVanilla.Vertical = weapon.Vertical;
                                            weaponVanilla.Yaw = weapon.Yaw;
                                            weaponVanilla.Changed = false;
                                            weapon.Changed = false;
                                        }
                                        else
                                        {
                                            UIModelMissile extractedMissile = m_XmlExtractor.ReadSingleMissile(new FileInfo(file));
                                            if (extractedMissile.Name.Length > 1)
                                            {
                                                parent.UIModel.UIModelMissiles.Add(extractedMissile);
                                                parent.UIModel.UIModelMissilesVanilla.Add(extractedMissile);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(file + " could not be read.");
                                }
                            }
                        }
                        //ships
                        if (dir.Contains("assets\\units\\size"))
                        {
                            List<string> files = Directory.GetFiles(dir, "*.xml", SearchOption.AllDirectories).ToList();

                            foreach (string file in files)
                            {
                                try
                                {
                                    if (!file.Contains(@"\macros\"))
                                        continue;
                                    var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                    string[] fileDepth = fileStream.Name.Split('\\');
                                    string fileName = fileDepth[fileDepth.Count() - 1];
                                    using (StreamReader sr = new StreamReader(fileStream))
                                    {
                                        UIModelShip ship = null;
                                        if (fileName.Contains("storage"))
                                        {
                                            ship = parent.UIModel.UIModelShips.FirstOrDefault(x => x.File.Contains(fileName.Replace("storage", "ship")));
                                        }
                                        else
                                        {
                                            ship = parent.UIModel.UIModelShips.FirstOrDefault(x => x.File.Contains(fileName));
                                        }
                                        if (ship != null)
                                        {
                                            var shipVanilla = parent.UIModel.UIModelShipsVanilla.FirstOrDefault(x => x.File.Contains(fileName));
                                            int index = parent.UIModel.UIModelShips.IndexOf(ship);
                                            string line;
                                            while (!sr.EndOfStream)
                                            {
                                                line = sr.ReadLine();
                                                if (line.Contains(@"<replace") && line.Contains("sel") && line.Contains(@"/macros") && line.Contains("macro']\">"))
                                                {
                                                    parent.UIModel.UIModelShips[index] = m_XmlExtractor.ReadSingleShipFile(new FileInfo(file));
                                                    break;
                                                }

                                                if (line.Contains("@max") && line.Contains("hull"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.HullMax = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("@value") && line.Contains("explosiondamage"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.ExplosionDamage = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("@missile") && line.Contains("storage"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.StorageMissiles = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("@unit") && line.Contains("storage"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.StorageUnits = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("@level") && line.Contains("secrecy"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.Secrecy = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("@gas") && line.Contains("gatherrate"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.GatherRrate = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@capacity") && line.Contains("people"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.People = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("@mass") && line.Contains("physics"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.Mass = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("@pitch") && line.Contains("inertia"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.InertiaPitch = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@yaw") && line.Contains("inertia"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.InertiaYaw = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@roll") && line.Contains("inertia"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.InertiaRoll = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@forward") && line.Contains("drag"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.Forward = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@reverse") && line.Contains("drag"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.Reverse = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@horizontal") && line.Contains("drag"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.Horizontal = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@vertical") && line.Contains("drag"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.Vertical = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@pitch") && line.Contains("drag"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.Pitch = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@yaw") && line.Contains("drag"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.Yaw = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@roll") && line.Contains("drag"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.Roll = Utility.ParseToDouble(value);
                                                }
                                                if (line.Contains("@max") && line.Contains("cargo"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.Cargo.CargoMax = Convert.ToInt32(value);
                                                }
                                                if (line.Contains("@tags") && line.Contains("cargo"))
                                                {
                                                    string value = line.Split('>')[1].Split('<')[0];
                                                    ship.Cargo.CargoTags = value;
                                                }
                                            }
                                            //shipVanilla.Cargo = ship.Ammunition;
                                            shipVanilla.ExplosionDamage = ship.ExplosionDamage;
                                            shipVanilla.Forward = ship.Forward;
                                            shipVanilla.GatherRrate = ship.GatherRrate;
                                            shipVanilla.Horizontal = ship.Horizontal;
                                            shipVanilla.HullMax = ship.HullMax;
                                            shipVanilla.InertiaPitch = ship.InertiaPitch;
                                            shipVanilla.InertiaRoll = ship.InertiaRoll;
                                            shipVanilla.InertiaYaw = ship.InertiaYaw;
                                            shipVanilla.Mass = ship.Mass;
                                            shipVanilla.People = ship.People;
                                            shipVanilla.Pitch = ship.Pitch;
                                            shipVanilla.Reverse = ship.Reverse;
                                            shipVanilla.Roll = ship.Roll;
                                            shipVanilla.Secrecy = ship.Secrecy;
                                            shipVanilla.StorageMissiles = ship.StorageMissiles;
                                            shipVanilla.StorageUnits = ship.StorageUnits;
                                            shipVanilla.Vertical = ship.Vertical;
                                            shipVanilla.Yaw = ship.Yaw;
                                            shipVanilla.Changed = false;
                                            ship.Changed = false;
                                        }
                                        else
                                        {
                                            UIModelShip extractedShip = m_XmlExtractor.ReadSingleShipFile(new FileInfo(file));
                                            if (extractedShip != null && extractedShip.Name.Length > 1 && extractedShip.Class != "storage")
                                            {
                                                parent.UIModel.UIModelShips.Add(extractedShip);
                                                parent.UIModel.UIModelShipsVanilla.Add(extractedShip);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(file + " could not be read.");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to read " + modPath);
            }
        }

    }
}
