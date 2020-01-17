using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace X4_Editor
{
    public class XmlWriter
    {
        private UIManager m_UIManager;
        public XmlWriter(UIManager uiManager)
        {
            m_UIManager = uiManager;
        }

        public void WriteAllChanges()
        {
            bool fileswritten = false;

            foreach (var item in m_UIManager.UIModel.UIModelShields)
            {
                var vanillaItem = m_UIManager.UIModel.UIModelShieldsVanilla.Where(x => x.Name == item.Name).FirstOrDefault();

                if (item.Changed)
                {
                    string extensionModPart = "";
                    if (item.File.Contains(m_UIManager.UIModel.ModPath1))
                    {
                        string[] folders = m_UIManager.UIModel.ModPath1.Split('\\');
                        string lastFolderName = folders.Last();
                        extensionModPart = @"\extensions\" + lastFolderName;
                    }

                    if (!Directory.Exists(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToShields))
                    {
                        Directory.CreateDirectory(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToShields);
                    }

                    string outputPath = m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToShields + item.File.Split(new[] { "macros" }, StringSplitOptions.None)[1];
                    using (StreamWriter sw = new StreamWriter(outputPath))
                    {
                        //sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        sw.WriteLine("<diff> ");
                        if (vanillaItem.Max != item.Max)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/recharge/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Max) + "</replace>");
                        if (vanillaItem.Rate != item.Rate)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/recharge/@rate\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Rate) + "</replace>");
                        if (vanillaItem.Delay != item.Delay)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/recharge/@delay\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Delay) + "</replace>");
                        if (vanillaItem.MaxHull != item.MaxHull)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.MaxHull) + "</replace>");
                        if (vanillaItem.Threshold != item.Threshold)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@threshold\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Threshold) + "</replace>");
                        sw.WriteLine("</diff> ");
                    }
                    fileswritten = true;
                }
            }

            foreach (var item in m_UIManager.UIModel.UIModelEngines)
            {
                var vanillaItem = m_UIManager.UIModel.UIModelEnginesVanilla.Where(x => x.Name == item.Name).FirstOrDefault();

                if (item.Changed)
                {
                    string extensionModPart = "";
                    if (item.File.Contains(m_UIManager.UIModel.ModPath1))
                    {
                        string[] folders = m_UIManager.UIModel.ModPath1.Split('\\');
                        string lastFolderName = folders.Last();
                        extensionModPart = @"\extensions\" + lastFolderName;
                    }

                    if (!Directory.Exists(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToEngines))
                    {
                        Directory.CreateDirectory(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToEngines);
                    }

                    string outputPath = m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToEngines + item.File.Split(new[] { "macros" }, StringSplitOptions.None)[1];
                    using (StreamWriter sw = new StreamWriter(outputPath))
                    {
                        //sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        sw.WriteLine("<diff> ");
                        if (vanillaItem.BoostDuration != item.BoostDuration)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/boost/@duration\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.BoostDuration) + "</replace>");
                        if (vanillaItem.BoostAttack != item.BoostAttack)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/boost/@attack\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.BoostAttack) + "</replace>");
                        if (vanillaItem.BoostThrust != item.BoostThrust)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/boost/@thrust\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.BoostThrust) + "</replace>");
                        if (vanillaItem.BoostRelease != item.BoostRelease)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/boost/@release\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.BoostRelease) + "</replace>");
                        if (vanillaItem.TravelAttack != item.TravelAttack)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/travel/@attack\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.TravelAttack) + "</replace>");
                        if (vanillaItem.TravelCharge != item.TravelCharge)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/travel/@charge\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.TravelCharge) + "</replace>");
                        if (vanillaItem.TravelRelease != item.TravelRelease)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/travel/@release\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.TravelRelease) + "</replace>");
                        if (vanillaItem.TravelThrust != item.TravelThrust)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/travel/@thrust\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.TravelThrust) + "</replace>");
                        if (vanillaItem.AngularPitch != item.AngularPitch)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/angular/@pitch\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.AngularPitch) + "</replace>");
                        if (vanillaItem.AngularRoll != item.AngularRoll)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/angular/@roll\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.AngularRoll) + "</replace>");
                        if (vanillaItem.Forward != item.Forward)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/thrust/@forward\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Forward) + "</replace>");
                        if (vanillaItem.Reverse != item.Reverse)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/thrust/@reverse\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Reverse) + "</replace>");
                        if (vanillaItem.Pitch != item.Pitch)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/thrust/@pitch\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Pitch) + "</replace>");
                        if (vanillaItem.Strafe != item.Strafe)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/thrust/@Strafe\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Strafe) + "</replace>");
                        if (vanillaItem.Yaw != item.Yaw)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/thrust/@yaw\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Yaw) + "</replace>");
                        if (vanillaItem.Roll != item.Roll)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/thrust/@roll\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Roll) + "</replace>");
                        if (vanillaItem.MaxHull != item.MaxHull)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.MaxHull) + "</replace>");
                        if (vanillaItem.Threshold != item.Threshold)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@threshold\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Threshold) + "</replace>");
                        sw.WriteLine("</diff> ");

                        fileswritten = true;
                    }
                }
            }

            foreach (var item in m_UIManager.UIModel.UIModelShips)
            {
                var vanillaItem = m_UIManager.UIModel.UIModelShipsVanilla.Where(x => x.Name == item.Name).FirstOrDefault();

                if (item.Changed)
                {
                    string shipClasssFolder = item.File.Split(new[] { "units" }, StringSplitOptions.None)[1].Split(new[] { "macros" }, StringSplitOptions.None)[0];

                    string extensionModPart = "";
                    if (item.File.Contains(m_UIManager.UIModel.ModPath1))
                    {
                        string[] folders = m_UIManager.UIModel.ModPath1.Split('\\');
                        string lastFolderName = folders.Last();
                        extensionModPart = @"\extensions\" + lastFolderName;
                    }

                    if (!Directory.Exists(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToShips + shipClasssFolder + @"\macros"))
                    {
                        Directory.CreateDirectory(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToShips + shipClasssFolder + @"\macros");
                    }

                    string outputPath = m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToShips + shipClasssFolder + "macros" + item.File.Split(new[] { "macros" }, StringSplitOptions.None)[1];

                    using (StreamWriter sw = new StreamWriter(outputPath))
                    {
                        //sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        sw.WriteLine("<diff> ");
                        if (vanillaItem == null || vanillaItem.HullMax != item.HullMax)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@max\">" + item.HullMax + "</replace>");
                        if (vanillaItem == null || vanillaItem.ExplosionDamage != item.ExplosionDamage)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/boost/@explosiondamage\">" + item.ExplosionDamage + "</replace>");
                        if (vanillaItem == null || vanillaItem.StorageMissiles != item.StorageMissiles)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/storage/@missile\">" + item.StorageMissiles + "</replace>");
                        if (vanillaItem == null || vanillaItem.StorageUnits != item.StorageUnits)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/storage/@unit\">" + item.StorageUnits + "</replace>");
                        if (vanillaItem == null || vanillaItem.Secrecy != item.Secrecy)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/secrecy/@level\">" + item.Secrecy + "</replace>");
                        if (vanillaItem == null || vanillaItem.People != item.People)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/people/@capacity\">" + item.People + "</replace>");
                        if (vanillaItem == null || vanillaItem.GatherRrate != item.GatherRrate)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/gatherrate/@gas\">" + item.GatherRrate + "</replace>");
                        if (vanillaItem == null || vanillaItem.Mass != item.Mass)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/@mass\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Mass) + "</replace>");
                        if (vanillaItem == null || vanillaItem.InertiaPitch != item.InertiaPitch)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@pitch\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaPitch) + "</replace>");
                        if (vanillaItem == null || vanillaItem.InertiaYaw != item.InertiaYaw)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@yaw\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaYaw) + "</replace>");
                        if (vanillaItem == null || vanillaItem.InertiaRoll != item.InertiaRoll)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@roll\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaRoll) + "</replace>");
                        if (vanillaItem == null || vanillaItem.Forward != item.Forward)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@forward\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Forward) + "</replace>");
                        if (vanillaItem == null || vanillaItem.Reverse != item.Reverse)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@reverse\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Reverse) + "</replace>");
                        if (vanillaItem == null || vanillaItem.Horizontal != item.Horizontal)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@horizontal\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Horizontal) + "</replace>");
                        if (vanillaItem == null || vanillaItem.Vertical != item.Vertical)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@vertical\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Vertical) + "</replace>");
                        if (vanillaItem == null || vanillaItem.Pitch != item.Pitch)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@pitch\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Pitch) + "</replace>");
                        if (vanillaItem == null || vanillaItem.Yaw != item.Yaw)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@yaw\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Yaw) + "</replace>");
                        if (vanillaItem == null || vanillaItem.Roll != item.Roll)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@roll\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Roll) + "</replace>");
                        sw.WriteLine("</diff> ");

                        fileswritten = true;
                    }

                    if (item.Cargo != null && item.Cargo.Changed)
                    {
                        using (StreamWriter sw = new StreamWriter(outputPath.Replace("ship", "storage")))
                        {
                            //sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                            sw.WriteLine("<diff> ");
                            if (vanillaItem == null || vanillaItem.Cargo.CargoMax != item.Cargo.CargoMax)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/cargo/@max\">" + item.Cargo.CargoMax + "</replace>");
                            if (vanillaItem == null || vanillaItem.Cargo.CargoTags != item.Cargo.CargoTags)
                                sw.WriteLine("\t<replace sel=\"//macros/macro/properties/cargo/@tags\">" + item.Cargo.CargoTags + "</replace>");
                            sw.WriteLine("</diff> ");

                            fileswritten = true;
                        }
                    }
                }
            }

            foreach (var item in m_UIManager.UIModel.UIModelProjectiles)
            {
                var vanillaItem = m_UIManager.UIModel.UIModelProjectilesVanilla.Where(x => x.Name == item.Name).FirstOrDefault();

                if (item.Changed)
                {
                    string extensionModPart = "";
                    if (item.File.Contains(m_UIManager.UIModel.ModPath1))
                    {
                        string[] folders = m_UIManager.UIModel.ModPath1.Split('\\');
                        string lastFolderName = folders.Last();
                        extensionModPart = @"\extensions\" + lastFolderName;
                    }

                    if (!Directory.Exists(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToProjectiles))
                    {
                        Directory.CreateDirectory(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToProjectiles);
                    }

                    string outputPath = m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToProjectiles + item.File.Split(new[] { "macros" }, StringSplitOptions.None)[1];
                    using (StreamWriter sw = new StreamWriter(outputPath))
                    {
                        //sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        sw.WriteLine("<diff> ");
                        if (vanillaItem.Speed != item.Speed)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@speed\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Speed) + "</replace>");
                        if (vanillaItem.Lifetime != item.Lifetime)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@lifetime\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Lifetime) + "</replace>");
                        if (vanillaItem.Range != item.Range)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@range\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Range) + "</replace>");
                        if (vanillaItem.Amount != item.Amount)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@amount\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Amount) + "</replace>");
                        if (vanillaItem.BarrelAmount != item.BarrelAmount)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@barrelamount\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.BarrelAmount) + "</replace>");
                        if (vanillaItem.TimeDiff != item.TimeDiff)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@timediff\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.TimeDiff) + "</replace>");
                        if (vanillaItem.Angle != item.Angle)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@angle\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.000}", item.Angle) + "</replace>");
                        if (vanillaItem.MaxHits != item.MaxHits)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@maxhits\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.MaxHits) + "</replace>");
                        if (vanillaItem.Ricochet != item.Ricochet)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@ricochet\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Ricochet) + "</replace>");
                        if (vanillaItem.Scale != item.Scale)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@scale\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Scale) + "</replace>");
                        if (vanillaItem.HeatInitial != item.HeatInitial)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/heat/@initial\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.HeatInitial) + "</replace>");
                        if (vanillaItem.HeatValue != item.HeatValue)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/heat/@value\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.HeatValue) + "</replace>");
                        if (vanillaItem.Damage != item.Damage)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/damage/@value\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Damage) + "</replace>");
                        if (vanillaItem.Repair != item.Repair)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/damage/@repair\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Repair) + "</replace>");
                        if (vanillaItem.Shield != item.Shield)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/damage/@shield\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Shield) + "</replace>");
                        if (vanillaItem.Hull != item.Hull)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/damage/@hull\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Hull) + "</replace>");
                        if (vanillaItem.ReloadRate != item.ReloadRate)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/reload/@rate\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.ReloadRate) + "</replace>");
                        if (vanillaItem.ReloadTime != item.ReloadTime)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/reload/@time\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.ReloadTime) + "</replace>");

                        sw.WriteLine("</diff> ");

                        fileswritten = true;
                    }
                }
            }

            foreach (var item in m_UIManager.UIModel.UIModelWeapons)
            {
                var vanillaItem = m_UIManager.UIModel.UIModelWeaponsVanilla.Where(x => x.Name == item.Name).FirstOrDefault();

                if (item.Changed)
                {
                    string extensionModPart = "";
                    if (item.File.Contains(m_UIManager.UIModel.ModPath1))
                    {
                        string[] folders = m_UIManager.UIModel.ModPath1.Split('\\');
                        string lastFolderName = folders.Last();
                        extensionModPart = @"\extensions\" + lastFolderName;
                    }

                    if (item.File.Contains(m_UIManager.PathToTurretsStandard) && !Directory.Exists(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToTurretsStandard))
                    {
                        Directory.CreateDirectory(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToTurretsStandard);
                    }
                    if (item.File.Contains(m_UIManager.PathToTurretsHeavy) && !Directory.Exists(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToTurretsHeavy))
                    {
                        Directory.CreateDirectory(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToTurretsHeavy);
                    }
                    if (item.File.Contains(m_UIManager.PathToTurretsGuided) && !Directory.Exists(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToTurretsGuided))
                    {
                        Directory.CreateDirectory(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToTurretsGuided);
                    }
                    if (item.File.Contains(m_UIManager.PathToTurretsEnergy) && !Directory.Exists(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToTurretsEnergy))
                    {
                        Directory.CreateDirectory(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToTurretsEnergy);
                    }
                    if (item.File.Contains(m_UIManager.PathToTurretsCapital) && !Directory.Exists(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToTurretsCapital))
                    {
                        Directory.CreateDirectory(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToTurretsCapital);
                    }
                    if (item.File.Contains(m_UIManager.PathToTurretsDumbfire) && !Directory.Exists(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToTurretsDumbfire))
                    {
                        Directory.CreateDirectory(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToTurretsDumbfire);
                    }

                    string[] splittedPath = item.File.Split(Path.DirectorySeparatorChar);
                    string lastThreeFoldersAndFileToWeaponPath = splittedPath[splittedPath.Length - 3] + Path.DirectorySeparatorChar + splittedPath[splittedPath.Length - 2] + Path.DirectorySeparatorChar +  splittedPath[splittedPath.Length - 1];


                    string outputPath = m_UIManager.UIModel.ExportPath + extensionModPart + @"\assets\props\WeaponSystems\" + lastThreeFoldersAndFileToWeaponPath;
                    //string outputPath = "";
                    //if (m_UIManager.UIModel.ModPath2.Length > 0)
                    //    outputPath = item.File.Replace(m_UIManager.UIModel.ModPath2, m_UIManager.UIModel.ExportPath);
                    //if (m_UIManager.UIModel.ModPath1.Length > 0)
                    //    outputPath = item.File.Replace(m_UIManager.UIModel.ModPath1, m_UIManager.UIModel.ExportPath);
                    //outputPath = item.File.Replace(m_UIManager.UIModel.Path, m_UIManager.UIModel.ExportPath);

                    using (StreamWriter sw = new StreamWriter(outputPath))
                    {
                        //sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        sw.WriteLine("<diff> ");
                        if (vanillaItem.Projectile != item.Projectile)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/bullet/@class\">" + item.Projectile + "</replace>");
                        if (vanillaItem.RotationSpeed != item.RotationSpeed)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/rotationspeed/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.RotationSpeed) + "</replace>");
                        if (vanillaItem.RotationAcceleration != item.RotationAcceleration)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/rotationacceleration/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.RotationAcceleration) + "</replace>");
                        if (vanillaItem.ReloadRate != item.ReloadRate)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/reload/@rate\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.ReloadRate) + "</replace>");
                        if (vanillaItem.ReloadTime != item.ReloadTime)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/reload/@time\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.ReloadTime) + "</replace>");
                        if (vanillaItem.HullMax != item.HullMax)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.HullMax) + "</replace>");
                        if (vanillaItem.HullThreshold != item.HullThreshold)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@threshold\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.HullThreshold) + "</replace>");
                        if (vanillaItem.Overheat != item.Overheat)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/heat/@overheat\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Overheat) + "</replace>");
                        if (vanillaItem.CoolDelay != item.CoolDelay)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/heat/@cooldelay\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.CoolDelay) + "</replace>");
                        if (vanillaItem.CoolRate != item.CoolRate)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/heat/@coolrate\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.CoolRate) + "</replace>");
                        if (vanillaItem.Reenable != item.Reenable)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/heat/@reenable\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Reenable) + "</replace>");
                        sw.WriteLine("</diff> ");

                        fileswritten = true;
                    }
                }
            }

            foreach (var item in m_UIManager.UIModel.UIModelMissiles)
            {
                var vanillaItem = m_UIManager.UIModel.UIModelMissilesVanilla.Where(x => x.Name == item.Name).FirstOrDefault();

                if (item.Changed)
                {
                    string extensionModPart = "";
                    if (item.File.Contains(m_UIManager.UIModel.ModPath1))
                    {
                        string[] folders = m_UIManager.UIModel.ModPath1.Split('\\');
                        string lastFolderName = folders.Last();
                        extensionModPart = @"\extensions\" + lastFolderName;
                    }

                    if (!Directory.Exists(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToMissiles))
                    {
                        Directory.CreateDirectory(m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToMissiles);
                    }

                    string outputPath = m_UIManager.UIModel.ExportPath + extensionModPart + m_UIManager.PathToMissiles + item.File.Split(new[] { "macros" }, StringSplitOptions.None)[1];
                    using (StreamWriter sw = new StreamWriter(outputPath))
                    {
                        //sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                        sw.WriteLine("<diff> ");
                        if (vanillaItem.Ammunition != item.Ammunition)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/ammunition/@value\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Ammunition) + "</replace>");
                        if (vanillaItem.MissileAmount != item.MissileAmount)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/missile/@amount\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.MissileAmount) + "</replace>");
                        if (vanillaItem.BarrelAmount != item.BarrelAmount)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/missile/@barrelamount\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.BarrelAmount) + "</replace>");
                        if (vanillaItem.Lifetime != item.Lifetime)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/missile/@lifetime\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Lifetime) + "</replace>");
                        if (vanillaItem.Range != item.Range)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/missile/@range\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Range) + "</replace>");
                        if (vanillaItem.Guided != item.Guided)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/missile/@guided\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Guided) + "</replace>");
                        if (vanillaItem.Swarm != item.Swarm)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/missile/@swarm\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Swarm) + "</replace>");
                        if (vanillaItem.Retarget != item.Retarget)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/missile/@retarget\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Retarget) + "</replace>");
                        if (vanillaItem.Damage != item.Damage)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/explosiondamage/@value\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Damage) + "</replace>");
                        if (vanillaItem.Reload != item.Reload)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/reload/@time\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Reload) + "</replace>");
                        if (vanillaItem.Hull != item.Hull)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/hull/@max\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Hull) + "</replace>");
                        if (vanillaItem.Forward != item.Forward)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@forward\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Forward) + "</replace>");
                        if (vanillaItem.Reverse != item.Reverse)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@reverse\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Reverse) + "</replace>");
                        if (vanillaItem.Horizontal != item.Horizontal)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@horizontal\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Horizontal) + "</replace>");
                        if (vanillaItem.Vertical != item.Vertical)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@vertical\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Vertical) + "</replace>");
                        if (vanillaItem.Pitch != item.Pitch)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@pitch\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Pitch) + "</replace>");
                        if (vanillaItem.Yaw != item.Yaw)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@yaw\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Yaw) + "</replace>");
                        if (vanillaItem.Roll != item.Roll)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/drag/@roll\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Roll) + "</replace>");

                        if (vanillaItem.Mass != item.Mass)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/@mass\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Mass) + "</replace>");
                        if (vanillaItem.InertiaPitch != item.InertiaPitch)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@pitch\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaPitch) + "</replace>");
                        if (vanillaItem.InertiaRoll != item.InertiaRoll)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@roll\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaRoll) + "</replace>");
                        if (vanillaItem.InertiaYaw != item.InertiaYaw)
                            sw.WriteLine("\t<replace sel=\"//macros/macro/properties/physics/inertia/@yaw\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.InertiaYaw) + "</replace>");
                        sw.WriteLine("</diff> ");

                        fileswritten = true;
                    }
                }
            }

            bool waresWritten = this.WriteWaresFile();

            if (fileswritten || waresWritten)
            {
                if (!waresWritten)
                    MessageBox.Show("Mod files have been created.");
                if (!File.Exists(m_UIManager.UIModel.ExportPath + @"\content.xml"))
                {
                    MessageBoxResult result = MessageBox.Show(
                        "No content.xml file detected.\r\r" +
                        "content.xml file is needed to use the mod. Shall a default content.xml file be written?", "Content.xml missing", MessageBoxButton.YesNo
                        );
                    if (result == MessageBoxResult.Yes)
                    {
                        using (var sw = new StreamWriter(m_UIManager.UIModel.ExportPath + @"\content.xml"))
                        {
                            string dateNow = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture).Replace("/", "-");
                            string[] modPath = m_UIManager.UIModel.ExportPath.Split('\\');
                            string modName = "ZZ_" + modPath[modPath.Length - 1];
                            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                            sw.WriteLine("<!-- This context.xml file has been created by the X4 Editor -->");
                            sw.WriteLine("<content id=\"" + modName + "\" name=\"" + modName + "\" description=\"This mod was created by the X4 Editor\" author=\"X4 Editor\" version=\"001\" date=\"" + dateNow + "\" save=\"0\" enabled=\"1\" sync=\"false\">");
                            sw.WriteLine("</content>");
                        }
                    }
                }
                if (waresWritten)
                {
                    MessageBox.Show
                        (
                        "Mod files have been created. \r\r" +
                        "Wares you have changed have been attached to wares.xml library. You need to check the outer xml nodes for correctness!\r" +
                        "Wares.xml will be opened...", "Mod files written", MessageBoxButton.OK, MessageBoxImage.Warning
                        );
                    string pathToWares = m_UIManager.UIModel.ExportPath + m_UIManager.PathToWares;
                    Process.Start(pathToWares);
                }
            }
            else
                MessageBox.Show("No changes detected - no files written.", "No Files written", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private bool WriteWaresFile()
        {
            string outputPath = m_UIManager.UIModel.ExportPath + m_UIManager.PathToWares;

            if (m_UIManager.UIModel.UIModelWares.Any(x => x.Changed))
            {
                if (!Directory.Exists(m_UIManager.UIModel.ExportPath + @"\libraries"))
                {
                    Directory.CreateDirectory(m_UIManager.UIModel.ExportPath + @"\libraries");
                }

                using (StreamWriter sw = new StreamWriter(outputPath, true))
                {
                    sw.WriteLine("<!-- DELETE THIS LINE IF IT IS THE FIRST LINE IN THIS FILE Code below has been added at " + DateTime.Now + " -->");
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    sw.WriteLine("<diff> ");

                    foreach (var item in m_UIManager.UIModel.UIModelWares)
                    {
                        var vanillaItem = m_UIManager.UIModel.UIModelWaresVanilla.Where(x => x.Name == item.Name).ToList()[0];

                        if (item.Changed)
                        {
                            if (vanillaItem.Max != item.Max || vanillaItem.Avg != item.Avg || vanillaItem.Min != item.Min)
                            {
                                sw.WriteLine("\t<replace sel=\"/wares/ware[@id='" + item.ID + "']/price\">");
                                sw.WriteLine("\t\t<price min=\"" + item.Min + "\" average=\"" + item.Avg + "\" max=\"" + item.Max + "\" /> ");
                                sw.WriteLine("\t</replace>");
                            }
                            if (vanillaItem.Amount != item.Amount)
                                sw.WriteLine("\t<replace sel=\"/wares/ware[@id='" + item.ID + "']/production/@amount\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Amount) + "</replace>");
                            if (vanillaItem.Time != item.Time)
                                sw.WriteLine("\t<replace sel=\"/wares/ware[@id='" + item.ID + "']/production/@time\">" + String.Format(CultureInfo.InvariantCulture, "{0:0}", item.Time) + "</replace>");

                            if (vanillaItem.Amount1 != item.Amount1 || vanillaItem.Amount2 != item.Amount2 || vanillaItem.Amount3 != item.Amount3 || vanillaItem.Amount4 != item.Amount4 || vanillaItem.Amount5 != item.Amount5)
                            {
                                sw.WriteLine("\t<replace sel=\"/wares/ware[@id='" + item.ID + "']/production/primary\">");
                                sw.WriteLine("\t\t<primary> ");
                                if (item.Amount1 > 0)
                                    sw.WriteLine("\t\t\t<ware ware=\"" + item.Ware1 + "\" amount=\"" + item.Amount1 + "\" />");
                                if (item.Amount2 > 0)
                                    sw.WriteLine("\t\t\t<ware ware=\"" + item.Ware2 + "\" amount=\"" + item.Amount2 + "\" />");
                                if (item.Amount3 > 0)
                                    sw.WriteLine("\t\t\t<ware ware=\"" + item.Ware3 + "\" amount=\"" + item.Amount3 + "\" />");
                                if (item.Amount4 > 0)
                                    sw.WriteLine("\t\t\t<ware ware=\"" + item.Ware4 + "\" amount=\"" + item.Amount4 + "\" />");
                                if (item.Amount5 > 0)
                                    sw.WriteLine("\t\t\t<ware ware=\"" + item.Ware5 + "\" amount=\"" + item.Amount5 + "\" />");
                                sw.WriteLine("\t\t</primary> ");
                                sw.WriteLine("\t</replace>");
                                sw.WriteLine("");
                            }
                            if (vanillaItem.Threshold != item.Threshold)
                            {
                                sw.WriteLine("\t<replace sel=\"/wares/ware[@id='" + item.ID + "']/use/@threshold\">" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.Threshold) + "</replace>");
                            }
                        }
                    }
                    sw.WriteLine("</diff> ");
                }
                return true;
            }
            return false;
        }
    }
}
