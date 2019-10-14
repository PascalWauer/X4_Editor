﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using X4_Editor.Helper;

namespace X4_Editor
{
    public class XmlExtractor
    {
        private UIManager m_UIManager;


        public XmlExtractor(UIManager uiManager)
        {
            m_UIManager = uiManager;
        }

        public void ReadAllWares(string waresPath)
        {
            if (!File.Exists(waresPath))
            {
                MessageBox.Show("No valid wares found.", "No data found.");
            }
            else
            {
                using (XmlReader reader = XmlReader.Create(waresPath))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "wares")
                        {
                            XDocument doc = XDocument.Load(waresPath);

                            var wares = doc.Descendants("wares");

                            foreach (var ware in wares)
                            {
                                XmlDocument xD = new XmlDocument();
                                xD.LoadXml(ware.ToString());
                                XmlNode xN = XmlHelper.ToXmlNode(ware);
                                XmlNodeList wareNodes = xN.SelectNodes("//ware");

                                foreach (XmlNode item in wareNodes)
                                {

                                    if (item.Attributes["id"] != null)
                                    {

                                        XmlNode priceNode = null;
                                        foreach (XmlNode child in item.SelectNodes("price"))
                                        {
                                            priceNode = child;
                                        }

                                        UIModelWare uiModelWare = new UIModelWare()
                                        {
                                            File = waresPath,
                                            Name = item.Attributes["id"].Value,
                                            ID = item.Attributes["id"].Value,
                                            Max = Convert.ToInt32(priceNode.Attributes["max"].Value),
                                            Min = Convert.ToInt32(priceNode.Attributes["min"].Value),
                                            Avg = Convert.ToInt32(priceNode.Attributes["average"].Value),
                                        };

                                        if (item.SelectNodes("./production").Count == 1)
                                        {
                                            XmlNodeList productionNode = item.SelectNodes("./production");
                                            if (productionNode.Count > 0)
                                            {
                                                uiModelWare.Time = Convert.ToInt32(productionNode[0].Attributes["time"].Value);
                                                uiModelWare.Amount = Convert.ToInt32(productionNode[0].Attributes["amount"].Value);
                                            }
                                            for (int i = 0; i < item.SelectNodes("./production/primary/ware").Count; i++)
                                            {
                                                if (i == 0)
                                                {
                                                    uiModelWare.Ware1 = item.SelectNodes("./production/primary/ware")[i].Attributes["ware"].Value;
                                                    uiModelWare.Amount1 = Convert.ToInt32(item.SelectNodes("./production/primary/ware")[i].Attributes["amount"].Value);
                                                }
                                                if (i == 1)
                                                {
                                                    uiModelWare.Ware2 = item.SelectNodes("./production/primary/ware")[i].Attributes["ware"].Value;
                                                    uiModelWare.Amount2 = Convert.ToInt32(item.SelectNodes("./production/primary/ware")[i].Attributes["amount"].Value);
                                                }
                                                if (i == 2)
                                                {
                                                    uiModelWare.Ware3 = item.SelectNodes("./production/primary/ware")[i].Attributes["ware"].Value;
                                                    uiModelWare.Amount3 = Convert.ToInt32(item.SelectNodes("./production/primary/ware")[i].Attributes["amount"].Value);
                                                }
                                                if (i == 3)
                                                {
                                                    uiModelWare.Ware4 = item.SelectNodes("./production/primary/ware")[i].Attributes["ware"].Value;
                                                    uiModelWare.Amount4 = Convert.ToInt32(item.SelectNodes("./production/primary/ware")[i].Attributes["amount"].Value);
                                                }
                                                if (i == 4)
                                                {
                                                    uiModelWare.Ware5 = item.SelectNodes("./production/primary/ware")[i].Attributes["ware"].Value;
                                                    uiModelWare.Amount5 = Convert.ToInt32(item.SelectNodes("./production/primary/ware")[i].Attributes["amount"].Value);
                                                }
                                            }
                                        }
                                        else if (item.SelectNodes("./production").Count == 2)
                                        {
                                            int noXenon = 0;
                                            bool xenon = false;

                                            XmlNodeList productionNodes = item.SelectNodes("./production");
                                            if (item.SelectNodes("./production")[0].Attributes["method"].Value == "xenon" && item.SelectNodes("./production")[1].Attributes["method"].Value == "default")
                                            {
                                                noXenon = 1;
                                                xenon = true;
                                            }
                                            if (item.SelectNodes("./production")[1].Attributes["method"].Value == "xenon" && item.SelectNodes("./production")[0].Attributes["method"].Value == "default")
                                            {
                                                noXenon = 0;
                                                xenon = true;
                                            }
                                            // only if one of the two production ways is xenon and the other one is default, show default
                                            if (xenon)
                                            {
                                                productionNodes = item.SelectNodes("./production")[noXenon].ChildNodes[0].ChildNodes;

                                                if (productionNodes.Count > 0)
                                                {
                                                    uiModelWare.Time = Utility.ParseToDouble(item.SelectNodes("./production")[noXenon].Attributes["time"].Value);
                                                    uiModelWare.Amount = Convert.ToInt32(item.SelectNodes("./production")[noXenon].Attributes["amount"].Value);
                                                }
                                                for (int i = 0; i < item.SelectNodes("./production/primary/ware").Count; i++)
                                                {
                                                    if (i == 0 && productionNodes[i] != null)
                                                    {
                                                        uiModelWare.Ware1 = productionNodes[i].Attributes["ware"].Value;
                                                        uiModelWare.Amount1 = Convert.ToInt32(productionNodes[i].Attributes["amount"].Value);
                                                    }
                                                    if (i == 1 && productionNodes[i] != null)
                                                    {
                                                        uiModelWare.Ware2 = productionNodes[i].Attributes["ware"].Value;
                                                        uiModelWare.Amount2 = Convert.ToInt32(productionNodes[i].Attributes["amount"].Value);
                                                    }
                                                    if (i == 2 && productionNodes[i] != null)
                                                    {
                                                        uiModelWare.Ware3 = productionNodes[i].Attributes["ware"].Value;
                                                        uiModelWare.Amount3 = Convert.ToInt32(productionNodes[i].Attributes["amount"].Value);
                                                    }
                                                    if (i == 3 && productionNodes[i] != null)
                                                    {
                                                        uiModelWare.Ware4 = productionNodes[i].Attributes["ware"].Value;
                                                        uiModelWare.Amount4 = Convert.ToInt32(productionNodes[i].Attributes["amount"].Value);
                                                    }
                                                    if (i == 4 && productionNodes[i] != null)
                                                    {
                                                        uiModelWare.Ware5 = productionNodes[i].Attributes["ware"].Value;
                                                        uiModelWare.Amount5 = Convert.ToInt32(productionNodes[i].Attributes["amount"].Value);
                                                    }
                                                }
                                            }
                                        }
                                        uiModelWare.Changed = false;

                                        m_UIManager.UIModel.UIModelWares.Add(uiModelWare);
                                    }
                                }
                            }
                        }
                    }
                }
                m_UIManager.UIModel.AllWaresLoaded = true;
                m_UIManager.UIModel.CalculateWarePrices();

                m_UIManager.UIModel.UIModelWaresVanilla.Clear();
                foreach (var item in m_UIManager.UIModel.UIModelWares)
                {
                    m_UIManager.UIModel.UIModelWaresVanilla.Add(item.Copy());
                }
            }
        }

        public UIModelProjectile ReadSingleProjectile(FileInfo file)
        {
            UIModelProjectile uiModelProjectile = new UIModelProjectile()
            {
                File = "",
                Name = ""
            };

            using (XmlReader reader = XmlReader.Create(file.FullName))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "macros")
                    {
                        XDocument doc = XDocument.Load(file.FullName);

                        var weapons = doc.Descendants("macro").Where(p => (string)p.Attribute("class") == "bullet");

                        foreach (var weapon in weapons)
                        {
                            XmlDocument xD = new XmlDocument();
                            xD.LoadXml(weapon.ToString());
                            XmlNode xN = XmlHelper.ToXmlNode(weapon);
                            XmlNodeList weaponMacroNode = xN.SelectNodes("//macro");
                            XmlNodeList weaponComponentNode = xN.SelectNodes("//component");
                            XmlNodeList weaponAmmunitionNode = xN.SelectNodes("//properties/ammunition");
                            XmlNodeList weaponBulletNode = xN.SelectNodes("//properties/bullet");
                            XmlNodeList weaponHeatNode = xN.SelectNodes("//properties/heat");
                            XmlNodeList weaponReloadNode = xN.SelectNodes("//properties/reload");
                            XmlNodeList weaponDamageNode = xN.SelectNodes("//properties/damage");

                            uiModelProjectile.File = file.FullName;
                            uiModelProjectile.Name = weaponMacroNode[0].Attributes["name"].Value;

                            if (weaponAmmunitionNode.Count > 0)
                            {
                                if (weaponAmmunitionNode[0].Attributes["value"] != null)
                                    uiModelProjectile.Ammunition = Convert.ToInt32(weaponAmmunitionNode[0].Attributes["value"].Value);
                                if (weaponAmmunitionNode[0].Attributes["reload"] != null)
                                    uiModelProjectile.AmmunitionReload = Utility.ParseToDouble(weaponAmmunitionNode[0].Attributes["reload"].Value);
                            }
                            if (weaponBulletNode.Count > 0)
                            {
                                if (weaponBulletNode[0].Attributes["speed"] != null)
                                    uiModelProjectile.Speed = Convert.ToInt32(weaponBulletNode[0].Attributes["speed"].Value);
                                if (weaponBulletNode[0].Attributes["lifetime"] != null)
                                    uiModelProjectile.Lifetime = Utility.ParseToDouble(weaponBulletNode[0].Attributes["lifetime"].Value);
                                if (weaponBulletNode[0].Attributes["amount"] != null)
                                    uiModelProjectile.Amount = Convert.ToInt32(weaponBulletNode[0].Attributes["amount"].Value);
                                if (weaponBulletNode[0].Attributes["range"] != null)
                                    uiModelProjectile.Range = Convert.ToInt32(weaponBulletNode[0].Attributes["range"].Value);
                                if (weaponBulletNode[0].Attributes["maxhits"] != null)
                                    uiModelProjectile.MaxHits = Convert.ToInt32(weaponBulletNode[0].Attributes["maxhits"].Value);
                                if (weaponBulletNode[0].Attributes["ricochet"] != null)
                                    uiModelProjectile.Ricochet = Utility.ParseToDouble(weaponBulletNode[0].Attributes["ricochet"].Value);
                                if (weaponBulletNode[0].Attributes["scale"] != null)
                                    uiModelProjectile.Scale = Utility.ParseToDouble(weaponBulletNode[0].Attributes["scale"].Value);
                                if (weaponBulletNode[0].Attributes["chargetime"] != null)
                                    uiModelProjectile.ChargeTime = Utility.ParseToDouble(weaponBulletNode[0].Attributes["chargetime"].Value);
                                if (weaponBulletNode[0].Attributes["timediff"] != null)
                                    uiModelProjectile.TimeDiff = Utility.ParseToDouble(weaponBulletNode[0].Attributes["timediff"].Value);
                                if (weaponBulletNode[0].Attributes["angle"] != null)
                                    uiModelProjectile.Angle = Utility.ParseToDouble(weaponBulletNode[0].Attributes["angle"].Value);
                            }
                            if (weaponHeatNode.Count > 0)
                            {
                                if (weaponHeatNode[0].Attributes["initial"] != null)
                                    uiModelProjectile.HeatInitial = Convert.ToInt32(weaponHeatNode[0].Attributes["initial"].Value);
                                if (weaponHeatNode[0].Attributes["value"] != null)
                                    uiModelProjectile.HeatValue = Convert.ToInt32(weaponHeatNode[0].Attributes["value"].Value);

                            }
                            if (weaponReloadNode.Count > 0)
                            {
                                if (weaponReloadNode[0].Attributes["rate"] != null)
                                    uiModelProjectile.ReloadRate = Utility.ParseToDouble(weaponReloadNode[0].Attributes["rate"].Value);
                                if (weaponReloadNode[0].Attributes["time"] != null)
                                    uiModelProjectile.ReloadTime = Utility.ParseToDouble(weaponReloadNode[0].Attributes["time"].Value);

                            }
                            if (weaponDamageNode.Count > 0)
                            {
                                if (weaponDamageNode[0].Attributes["value"] != null)
                                    uiModelProjectile.Damage = Utility.ParseToDouble(weaponDamageNode[0].Attributes["value"].Value);
                                if (weaponDamageNode[0].Attributes["shield"] != null)
                                    uiModelProjectile.Shield = Utility.ParseToDouble(weaponDamageNode[0].Attributes["shield"].Value);
                                if (weaponDamageNode[0].Attributes["repair"] != null)
                                    uiModelProjectile.Repair = Utility.ParseToDouble(weaponDamageNode[0].Attributes["repair"].Value);
                            }
                            uiModelProjectile.Changed = false;
                            //m_UIManager.UIModel.UIModelProjectiles.Add(uiModelWeapon);
                        }
                    }
                }
            }
            return uiModelProjectile;
        }
        public UIModelMissile ReadSingleMissile(FileInfo file)
        {
            UIModelMissile uiModelMissile = new UIModelMissile()
            {
                File = "",
                Name = ""
            };

            using (XmlReader reader = XmlReader.Create(file.FullName))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "macros")
                    {
                        XDocument doc = XDocument.Load(file.FullName);

                        var missiles = doc.Descendants("macro").Where(p => ((string)p.Attribute("class") == "missile" || (string)p.Attribute("class") == "torpedo"));

                        foreach (var missile in missiles)
                        {
                            XmlDocument xD = new XmlDocument();
                            xD.LoadXml(missile.ToString());
                            XmlNode xN = XmlHelper.ToXmlNode(missile);
                            XmlNodeList missileComponentNode = xN.SelectNodes("//component");
                            XmlNodeList missileMacroNode = xN.SelectNodes("//macro");
                            XmlNodeList missileAmmunitionNode = xN.SelectNodes("//properties/ammunition");
                            XmlNodeList missilenMissileNode = xN.SelectNodes("//properties/missile");
                            XmlNodeList missileHullNode = xN.SelectNodes("//properties/hull");
                            XmlNodeList missileReloadNode = xN.SelectNodes("//properties/reload");
                            XmlNodeList missilePhysicsNode = xN.SelectNodes("//properties/physics");
                            XmlNodeList missileInertiaNode = xN.SelectNodes("//properties/physics/inertia");
                            XmlNodeList missileDragNode = xN.SelectNodes("//properties/physics/drag");
                            XmlNodeList missileDamageNode = xN.SelectNodes("//properties/explosiondamage");

                            uiModelMissile.File = file.FullName;
                            uiModelMissile.Name = missileMacroNode[0].Attributes["name"].Value;

                            if (missileAmmunitionNode.Count > 0)
                            {
                                if (missileAmmunitionNode[0].Attributes["value"] != null)
                                    uiModelMissile.Ammunition = Convert.ToInt32(missileAmmunitionNode[0].Attributes["value"].Value);
                            }
                            if (missilenMissileNode.Count > 0)
                            {
                                if (missilenMissileNode[0].Attributes["amount"] != null)
                                    uiModelMissile.MissileAmount = Convert.ToInt32(missilenMissileNode[0].Attributes["amount"].Value);
                                if (missilenMissileNode[0].Attributes["lifetime"] != null)
                                    uiModelMissile.Lifetime = Utility.ParseToDouble(missilenMissileNode[0].Attributes["lifetime"].Value);
                                if (missilenMissileNode[0].Attributes["barrelamount"] != null)
                                    uiModelMissile.BarrelAmount = Convert.ToInt32(missilenMissileNode[0].Attributes["barrelamount"].Value);
                                if (missilenMissileNode[0].Attributes["range"] != null)
                                    uiModelMissile.Range = Convert.ToInt32(missilenMissileNode[0].Attributes["range"].Value);
                                if (missilenMissileNode[0].Attributes["guided"] != null)
                                    uiModelMissile.Guided = Convert.ToInt32(missilenMissileNode[0].Attributes["guided"].Value);
                                if (missilenMissileNode[0].Attributes["swarm"] != null)
                                    uiModelMissile.Swarm = Convert.ToInt32(missilenMissileNode[0].Attributes["swarm"].Value);
                                if (missilenMissileNode[0].Attributes["retarget"] != null)
                                    uiModelMissile.Retarget = Convert.ToInt32(missilenMissileNode[0].Attributes["retarget"].Value);
                            }
                            if (missileDamageNode.Count > 0)
                            {
                                if (missileDamageNode[0].Attributes["value"] != null)
                                    uiModelMissile.Damage = Convert.ToInt32(missileDamageNode[0].Attributes["value"].Value);

                            }
                            if (missileReloadNode.Count > 0)
                            {
                                if (missileReloadNode[0].Attributes["time"] != null)
                                    uiModelMissile.Reload = Utility.ParseToDouble(missileReloadNode[0].Attributes["time"].Value);

                            }
                            if (missileHullNode.Count > 0)
                            {
                                if (missileHullNode[0].Attributes["max"] != null)
                                    uiModelMissile.Hull = Convert.ToInt32(missileHullNode[0].Attributes["max"].Value);
                            }
                            if (missilePhysicsNode.Count > 0)
                            {
                                if (missilePhysicsNode[0].Attributes["mass"] != null)
                                    uiModelMissile.Mass = Utility.ParseToDouble(missilePhysicsNode[0].Attributes["mass"].Value);
                            }
                            if (missileInertiaNode.Count > 0)
                            {
                                if (missileInertiaNode[0].Attributes["yaw"] != null)
                                    uiModelMissile.InertiaYaw = Utility.ParseToDouble(missileInertiaNode[0].Attributes["yaw"].Value);
                                if (missileInertiaNode[0].Attributes["pitch"] != null)
                                    uiModelMissile.InertiaPitch = Utility.ParseToDouble(missileInertiaNode[0].Attributes["pitch"].Value);
                                if (missileInertiaNode[0].Attributes["roll"] != null)
                                    uiModelMissile.InertiaRoll = Utility.ParseToDouble(missileInertiaNode[0].Attributes["roll"].Value);
                            }
                            if (missileDragNode.Count > 0)
                            {
                                if (missileDragNode[0].Attributes["forward"] != null)
                                    uiModelMissile.Forward = Utility.ParseToDouble(missileDragNode[0].Attributes["forward"].Value);
                                if (missileDragNode[0].Attributes["reverse"] != null)
                                    uiModelMissile.Reverse = Utility.ParseToDouble(missileDragNode[0].Attributes["reverse"].Value);
                                if (missileDragNode[0].Attributes["horizontal"] != null)
                                    uiModelMissile.Horizontal = Utility.ParseToDouble(missileDragNode[0].Attributes["horizontal"].Value);
                                if (missileDragNode[0].Attributes["vertical"] != null)
                                    uiModelMissile.Vertical = Utility.ParseToDouble(missileDragNode[0].Attributes["vertical"].Value);
                                if (missileDragNode[0].Attributes["pitch"] != null)
                                    uiModelMissile.Pitch = Utility.ParseToDouble(missileDragNode[0].Attributes["pitch"].Value);
                                if (missileDragNode[0].Attributes["roll"] != null)
                                    uiModelMissile.Roll = Utility.ParseToDouble(missileDragNode[0].Attributes["roll"].Value);
                                if (missileDragNode[0].Attributes["yaw"] != null)
                                    uiModelMissile.Yaw = Utility.ParseToDouble(missileDragNode[0].Attributes["yaw"].Value);
                            }
                            uiModelMissile.Changed = false;
                            m_UIManager.UIModel.UIModelMissiles.Add(uiModelMissile);
                        }
                    }
                }
            }
            return uiModelMissile;
        }
        public UIModelEngine ReadSingleEngineFile(FileInfo file)
        {
            UIModelEngine uiModelEngine = new UIModelEngine()
            {
                File = "",
                Name = "",
            };

            using (XmlReader reader = XmlReader.Create(file.FullName))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "macros")
                    {
                        XDocument doc = XDocument.Load(file.FullName);

                        var engines = doc.Descendants("macro")
                            .Where(p => (string)p.Attribute("class") == "engine");

                        foreach (var engine in engines)
                        {
                            XmlDocument xD = new XmlDocument();
                            xD.LoadXml(engine.ToString());
                            XmlNode xN = XmlHelper.ToXmlNode(engine);
                            XmlNodeList engineComponentNode = xN.SelectNodes("//component");
                            XmlNodeList engineMacroNode = xN.SelectNodes("//macro");
                            XmlNodeList engineIdentificationNode = xN.SelectNodes("//properties/identification");
                            XmlNodeList engineBoostNode = xN.SelectNodes("//properties/boost");
                            XmlNodeList engineTravelNode = xN.SelectNodes("//properties/travel");
                            XmlNodeList engineThrustlNode = xN.SelectNodes("//properties/thrust");
                            XmlNodeList engineAngularNode = xN.SelectNodes("//properties/angular");
                            XmlNodeList engineHullNode = xN.SelectNodes("//properties/hull");

                            uiModelEngine.File = file.FullName;
                            uiModelEngine.Name = engineMacroNode[0].Attributes["name"].Value;

                            if (engineIdentificationNode.Count > 0)
                            {
                                if (engineIdentificationNode[0].Attributes["makerrace"] != null)
                                    uiModelEngine.Faction = engineIdentificationNode[0].Attributes["makerrace"].Value;
                                if (engineIdentificationNode[0].Attributes["mk"] != null)
                                    uiModelEngine.MK = engineIdentificationNode[0].Attributes["mk"].Value;
                            }
                            if (engineBoostNode.Count > 0)
                            {
                                if (engineBoostNode[0].Attributes["attack"] != null)
                                    uiModelEngine.BoostAttack = Utility.ParseToDouble(engineBoostNode[0].Attributes["attack"].Value);
                                if (engineBoostNode[0].Attributes["thrust"] != null)
                                    uiModelEngine.BoostThrust = Utility.ParseToDouble(engineBoostNode[0].Attributes["thrust"].Value);
                                if (engineBoostNode[0].Attributes["release"] != null)
                                    uiModelEngine.BoostRelease = Utility.ParseToDouble(engineBoostNode[0].Attributes["release"].Value);
                                if (engineBoostNode[0].Attributes["duration"] != null)
                                    uiModelEngine.BoostDuration = Utility.ParseToDouble(engineBoostNode[0].Attributes["duration"].Value);
                            }
                            if (engineThrustlNode.Count > 0)
                            {
                                if (engineThrustlNode[0].Attributes["forward"] != null)
                                    uiModelEngine.Forward = Utility.ParseToDouble(engineThrustlNode[0].Attributes["forward"].Value);
                                if (engineThrustlNode[0].Attributes["reverse"] != null)
                                    uiModelEngine.Reverse = Utility.ParseToDouble(engineThrustlNode[0].Attributes["reverse"].Value);
                                if (engineThrustlNode[0].Attributes["strafe"] != null)
                                    uiModelEngine.Strafe = Utility.ParseToDouble(engineThrustlNode[0].Attributes["strafe"].Value);
                                if (engineThrustlNode[0].Attributes["pitch"] != null)
                                    uiModelEngine.Pitch = Utility.ParseToDouble(engineThrustlNode[0].Attributes["pitch"].Value);
                                if (engineThrustlNode[0].Attributes["yaw"] != null)
                                    uiModelEngine.Yaw = Utility.ParseToDouble(engineThrustlNode[0].Attributes["yaw"].Value);
                                if (engineThrustlNode[0].Attributes["roll"] != null)
                                    uiModelEngine.Roll = Utility.ParseToDouble(engineThrustlNode[0].Attributes["roll"].Value);
                            }
                            if (engineTravelNode.Count > 0)
                            {
                                if (engineTravelNode[0].Attributes["attack"] != null)
                                    uiModelEngine.TravelAttack = Utility.ParseToDouble(engineTravelNode[0].Attributes["attack"].Value);
                                if (engineTravelNode[0].Attributes["charge"] != null)
                                    uiModelEngine.TravelCharge = Utility.ParseToDouble(engineTravelNode[0].Attributes["charge"].Value);
                                if (engineTravelNode[0].Attributes["release"] != null)
                                    uiModelEngine.TravelRelease = Utility.ParseToDouble(engineTravelNode[0].Attributes["release"].Value);
                                if (engineTravelNode[0].Attributes["thrust"] != null)
                                    uiModelEngine.TravelThrust = Utility.ParseToDouble(engineTravelNode[0].Attributes["thrust"].Value);
                            }
                            if (engineAngularNode.Count > 0)
                            {
                                if (engineAngularNode[0].Attributes["roll"] != null)
                                    uiModelEngine.AngularPitch = Utility.ParseToDouble(engineAngularNode[0].Attributes["roll"].Value);
                                if (engineAngularNode[0].Attributes["pitch"] != null)
                                    uiModelEngine.AngularRoll = Utility.ParseToDouble(engineAngularNode[0].Attributes["pitch"].Value);
                            }
                            if (engineHullNode.Count > 0)
                            {
                                if (engineHullNode[0].Attributes["max"] != null)
                                    uiModelEngine.MaxHull = Utility.ParseToDouble(engineHullNode[0].Attributes["max"].Value);
                                if (engineHullNode[0].Attributes["threshold"] != null)
                                    uiModelEngine.Threshold = Utility.ParseToDouble(engineHullNode[0].Attributes["threshold"].Value);
                            }
                            uiModelEngine.Changed = false;

                            //m_UIManager.UIModel.UIModelModulesEngines.Add(uiModelEngine);
                        }
                    }
                }
            }
            return uiModelEngine;
        }
        public UIModelShield ReadSingleShield(FileInfo file)
        {
            UIModelShield uiModelShield = new UIModelShield()
            {
                File = "",
                Name = ""
            };

            using (XmlReader reader = XmlReader.Create(file.FullName))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "macros")
                    {
                        XDocument doc = XDocument.Load(file.FullName);

                        var shields = doc.Descendants("macro")
                            .Where(p => (string)p.Attribute("class") == "shieldgenerator");

                        foreach (var shield in shields)
                        {
                            XmlDocument xD = new XmlDocument();
                            xD.LoadXml(shield.ToString());
                            XmlNode xN = XmlHelper.ToXmlNode(shield);
                            XmlNodeList shielComponentNode = xN.SelectNodes("//component");
                            XmlNodeList shielMacroNode = xN.SelectNodes("//macro");
                            XmlNodeList shielIdentificationNode = xN.SelectNodes("//properties/identification");
                            XmlNodeList shielRechargedNode = xN.SelectNodes("//properties/recharge");
                            XmlNodeList shieldHullNode = xN.SelectNodes("//properties/hull");

                            uiModelShield.File = file.FullName;
                            uiModelShield.Name = shielMacroNode[0].Attributes["name"].Value;
                            //Faction = shielIdentificationNode[0].Attributes["makerrace"].Value,
                            //MK = shielIdentificationNode[0].Attributes["mk"].Value,
                            uiModelShield.Max = Convert.ToInt32(shielRechargedNode[0].Attributes["max"].Value);
                            uiModelShield.Rate = Utility.ParseToDouble(shielRechargedNode[0].Attributes["rate"].Value);
                            uiModelShield.Delay = Utility.ParseToDouble(shielRechargedNode[0].Attributes["delay"].Value);

                            // not neccessary
                            if (shielIdentificationNode.Count > 0)
                            {
                                uiModelShield.Faction = shielIdentificationNode[0].Attributes["makerrace"].Value;
                                uiModelShield.MK = shielIdentificationNode[0].Attributes["mk"].Value;
                            }
                            if (shieldHullNode.Count > 0)
                            {
                                if (shieldHullNode[0].Attributes["max"] != null)
                                    uiModelShield.MaxHull = Utility.ParseToDouble(shieldHullNode[0].Attributes["max"].Value);
                                if (shieldHullNode[0].Attributes["threshold"] != null)
                                    uiModelShield.Threshold = Utility.ParseToDouble(shieldHullNode[0].Attributes["threshold"].Value);
                            }
                            uiModelShield.Changed = false;
                            //m_UIManager.UIModel.UIModelModulesShields.Add(uiModelShield);
                        }
                    }
                }
            }
            return uiModelShield;
        }
        public UIModelWeapon ReadSingleWeapon(FileInfo xmlWeaponFileInfo)
        {
            UIModelWeapon uiModelWeapon = new UIModelWeapon() { Name = "", File = xmlWeaponFileInfo.FullName };

            using (XmlReader reader = XmlReader.Create(xmlWeaponFileInfo.FullName))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "macros")
                    {
                        XDocument doc = XDocument.Load(xmlWeaponFileInfo.FullName);

                        var weapons = doc.Descendants("macro").Where(p => (string)p.Attribute("class") == "turret" || (string)p.Attribute("class") == "weapon" || (string)p.Attribute("class") == "missileturret" || (string)p.Attribute("class") == "missilelauncher");

                        foreach (var weapon in weapons)
                        {
                            XmlDocument xD = new XmlDocument();
                            xD.LoadXml(weapon.ToString());
                            XmlNode xN = XmlHelper.ToXmlNode(weapon);
                            XmlNodeList weaponComponentNode = xN.SelectNodes("//component");
                            XmlNodeList weaponMacroNode = xN.SelectNodes("//macro");
                            XmlNodeList weaponIdentificationNode = xN.SelectNodes("//properties/identification");
                            XmlNodeList weaponBulletNode = xN.SelectNodes("//properties/bullet");
                            XmlNodeList weaponRotationspeedNode = xN.SelectNodes("//properties/rotationspeed");
                            XmlNodeList weaponRotationAccelerationNode = xN.SelectNodes("//properties/rotationacceleration");
                            XmlNodeList weaponReloadNode = xN.SelectNodes("//properties/reload");
                            XmlNodeList weaponHullNode = xN.SelectNodes("//properties/hull");
                            XmlNodeList weaponHeatNode = xN.SelectNodes("//properties/heat");

                            uiModelWeapon.File = xmlWeaponFileInfo.FullName;
                            uiModelWeapon.Name = weaponMacroNode[0].Attributes["name"].Value;
                            
                            if (weaponBulletNode.Count > 0)
                            {
                                if (weaponBulletNode[0].Attributes["class"] != null)
                                    uiModelWeapon.Projectile = weaponBulletNode[0].Attributes["class"].Value;
                            }
                            if (weaponIdentificationNode.Count > 0)
                            {
                                if (weaponIdentificationNode[0].Attributes["mk"] != null)
                                    uiModelWeapon.MK = weaponIdentificationNode[0].Attributes["mk"].Value;
                            }
                            if (weaponHeatNode.Count > 0)
                            {
                                if (weaponHeatNode[0].Attributes["overheat"] != null)
                                    uiModelWeapon.Overheat = Convert.ToInt32(weaponHeatNode[0].Attributes["overheat"].Value);
                                if (weaponHeatNode[0].Attributes["cooldelay"] != null)
                                    uiModelWeapon.CoolDelay = Utility.ParseToDouble(weaponHeatNode[0].Attributes["cooldelay"].Value);
                                if (weaponHeatNode[0].Attributes["coolrate"] != null)
                                    uiModelWeapon.CoolRate = Convert.ToInt32(weaponHeatNode[0].Attributes["coolrate"].Value);
                                if (weaponHeatNode[0].Attributes["reenable"] != null)
                                    uiModelWeapon.Reenable = Convert.ToInt32(weaponHeatNode[0].Attributes["reenable"].Value);

                            }
                            if (weaponRotationspeedNode.Count > 0)
                            {
                                if (weaponRotationspeedNode[0].Attributes["max"] != null)
                                    uiModelWeapon.RotationSpeed = Utility.ParseToDouble(weaponRotationspeedNode[0].Attributes["max"].Value);
                            }
                            if (weaponRotationAccelerationNode.Count > 0)
                            {
                                if (weaponRotationAccelerationNode[0].Attributes["max"] != null)
                                    uiModelWeapon.RotationAcceleration = Utility.ParseToDouble(weaponRotationAccelerationNode[0].Attributes["max"].Value);
                            }
                            if (weaponReloadNode.Count > 0)
                            {
                                if (weaponReloadNode[0].Attributes["rate"] != null)
                                    uiModelWeapon.ReloadRate = Utility.ParseToDouble(weaponReloadNode[0].Attributes["rate"].Value);
                                if (weaponReloadNode[0].Attributes["time"] != null)
                                    uiModelWeapon.ReloadTime = Utility.ParseToDouble(weaponReloadNode[0].Attributes["time"].Value);
                            }
                            if (weaponHullNode.Count > 0)
                            {
                                if (weaponHullNode[0].Attributes["max"] != null)
                                    uiModelWeapon.HullMax = Utility.ParseToDouble(weaponHullNode[0].Attributes["max"].Value);
                                if (weaponHullNode[0].Attributes["threshold"] != null)
                                    uiModelWeapon.HullThreshold = Utility.ParseToDouble(weaponHullNode[0].Attributes["threshold"].Value);
                            }
                            uiModelWeapon.Changed = false;
                            
                            //m_UIManager.UIModel.UIModelWeapons.Add(uiModelWeapon);
                        }
                    }
                }
            }
            return uiModelWeapon;
        }
        public UIModelShip ReadSingleShipFile(FileInfo file)
        {
            UIModelShip uiModelShip = new UIModelShip()
            {
                File = "",
                Name = "",
                Class = ""

            };

            using (XmlReader reader = XmlReader.Create(file.FullName))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "macros")
                    {
                        XDocument doc = XDocument.Load(file.FullName);

                        var ships = doc.Descendants("macro").Where(p => p.Attribute("class") != null);

                        foreach (var ship in ships)
                        {
                            XmlDocument xD = new XmlDocument();
                            xD.LoadXml(ship.ToString());
                            XmlNode xN = XmlHelper.ToXmlNode(ship);

                            XmlNodeList shipComponentNode = xN.SelectNodes("//component");
                            XmlNodeList shipMacroNode = xN.SelectNodes("/macro");
                            XmlNodeList shipIdentificationNode = xN.SelectNodes("//properties/identification");
                            XmlNodeList shipExplosionNode = xN.SelectNodes("//properties/explosiondamage");
                            XmlNodeList shipStorageNode = xN.SelectNodes("//properties/storage");
                            XmlNodeList shipHullNode = xN.SelectNodes("//properties/hull");
                            XmlNodeList shipSecrecyNode = xN.SelectNodes("//properties/secrecy");
                            XmlNodeList shipGatherRateNode = xN.SelectNodes("//properties/gatherrate");
                            XmlNodeList shipPeopleNode = xN.SelectNodes("//properties/people");
                            XmlNodeList shipPhysicsNode = xN.SelectNodes("//properties/physics");
                            XmlNodeList shipInertiaNode = xN.SelectNodes("//properties/physics/inertia");
                            XmlNodeList shipDragNode = xN.SelectNodes("//properties/physics/drag");
                            XmlNodeList shipShipTypeNode = xN.SelectNodes("//properties/ship");

                            #region region for cargo 
                            string CargoFile = file.FullName.Replace(m_UIManager.UIModel.ModPath1, m_UIManager.UIModel.Path).Replace(m_UIManager.UIModel.ModPath2, m_UIManager.UIModel.Path).Replace("ship", "storage");
                            string CargoModFile1 = CargoFile.Replace(m_UIManager.UIModel.Path, m_UIManager.UIModel.ModPath1).Replace("ship", "storage");
                            string CargoModFile2 = CargoFile.Replace(m_UIManager.UIModel.Path, m_UIManager.UIModel.ModPath2).Replace("ship", "storage");

                            uiModelShip.File = file.FullName;
                            uiModelShip.Name = shipMacroNode[0].Attributes["name"].Value;
                            uiModelShip.Class = shipMacroNode[0].Attributes["class"].Value;

                            string priorityCargoFile = "";
                            if (File.Exists(CargoModFile2))
                            {
                                priorityCargoFile = CargoModFile2;
                            }
                            else if (File.Exists(CargoModFile1))
                            {
                                priorityCargoFile = CargoModFile1;
                            }
                            else if (File.Exists(CargoFile))
                                priorityCargoFile = CargoFile;

                            if (!string.IsNullOrWhiteSpace(priorityCargoFile))
                            {
                                uiModelShip.Cargo = new UIModelShipCargo();
                                XmlDocument Cargodoc = new XmlDocument();
                                Cargodoc.Load(priorityCargoFile);
                                string xmlcontents = Cargodoc.InnerXml;
                                Cargodoc.LoadXml(xmlcontents);

                                XmlNodeList shipCargoNode = Cargodoc.SelectNodes("//properties/cargo");

                                if (shipCargoNode.Count > 0)
                                {
                                    uiModelShip.Cargo.File = priorityCargoFile;
                                    uiModelShip.Cargo.CargoMax = Convert.ToInt32(shipCargoNode[0].Attributes["max"].Value);
                                    uiModelShip.Cargo.CargoTags = shipCargoNode[0].Attributes["tags"].Value;
                                }
                                uiModelShip.Cargo.Changed = false;
                            }
                            #endregion 

                            if (shipShipTypeNode.Count > 0)
                            {
                                if (shipShipTypeNode[0].Attributes["type"] != null)
                                    uiModelShip.Type = shipShipTypeNode[0].Attributes["type"].Value;
                            }

                            if (uiModelShip.Type == null || uiModelShip.Type == "cockpit")
                            {
                                return null;
                            }
                                

                            if (shipIdentificationNode.Count > 0)
                            {
                                if (shipIdentificationNode[0].Attributes["basename"] != null)
                                    uiModelShip.IGName = this.GetIGName(shipIdentificationNode[0].Attributes["basename"].Value);
                                else
                                    uiModelShip.IGName = "'unknown'";
                            }

                            if (shipExplosionNode.Count > 0)
                            {
                                if (shipExplosionNode[0].Attributes["value"] != null)
                                    uiModelShip.ExplosionDamage = Convert.ToInt32(shipExplosionNode[0].Attributes["value"].Value);
                            }
                            if (shipStorageNode.Count > 0)
                            {
                                if (shipStorageNode[0].Attributes["missile"] != null)
                                    uiModelShip.StorageMissiles = Convert.ToInt32(shipStorageNode[0].Attributes["missile"].Value);
                                if (shipStorageNode[0].Attributes["unit"] != null)
                                    uiModelShip.StorageUnits = Convert.ToInt32(shipStorageNode[0].Attributes["unit"].Value);
                            }
                            if (shipHullNode.Count > 0)
                            {
                                if (shipHullNode[0].Attributes["max"] != null)
                                    uiModelShip.HullMax = Convert.ToInt32(shipHullNode[0].Attributes["max"].Value);
                            }
                            if (shipGatherRateNode.Count > 0)
                            {
                                if (shipGatherRateNode[0].Attributes["gas"] != null)
                                    uiModelShip.GatherRrate = Utility.ParseToDouble(shipGatherRateNode[0].Attributes["gas"].Value);
                            }
                            if (shipSecrecyNode.Count > 0)
                            {
                                if (shipSecrecyNode[0].Attributes["level"] != null)
                                    uiModelShip.Secrecy = Convert.ToInt32(shipSecrecyNode[0].Attributes["level"].Value);
                            }
                            if (shipPeopleNode.Count > 0)
                            {
                                if (shipPeopleNode[0].Attributes["capacity"] != null)
                                    uiModelShip.People = Convert.ToInt32(shipPeopleNode[0].Attributes["capacity"].Value);
                            }
                            if (shipPhysicsNode.Count > 0)
                            {
                                if (shipPhysicsNode[0].Attributes["mass"] != null)
                                    uiModelShip.Mass = Utility.ParseToDouble(shipPhysicsNode[0].Attributes["mass"].Value);
                            }
                            if (shipInertiaNode.Count > 0)
                            {
                                if (shipInertiaNode[0].Attributes["roll"] != null)
                                    uiModelShip.InertiaRoll = Utility.ParseToDouble(shipInertiaNode[0].Attributes["roll"].Value);
                                if (shipInertiaNode[0].Attributes["yaw"] != null)
                                    uiModelShip.InertiaYaw = Utility.ParseToDouble(shipInertiaNode[0].Attributes["yaw"].Value);
                                if (shipInertiaNode[0].Attributes["pitch"] != null)
                                    uiModelShip.InertiaPitch = Utility.ParseToDouble(shipInertiaNode[0].Attributes["pitch"].Value);
                            }
                            if (shipDragNode.Count > 0)
                            {
                                if (shipDragNode[0].Attributes["forward"] != null)
                                    uiModelShip.Forward = Utility.ParseToDouble(shipDragNode[0].Attributes["forward"].Value);
                                if (shipDragNode[0].Attributes["reverse"] != null)
                                    uiModelShip.Reverse = Utility.ParseToDouble(shipDragNode[0].Attributes["reverse"].Value);
                                if (shipDragNode[0].Attributes["horizontal"] != null)
                                    uiModelShip.Horizontal = Utility.ParseToDouble(shipDragNode[0].Attributes["horizontal"].Value);
                                if (shipDragNode[0].Attributes["vertical"] != null)
                                    uiModelShip.Vertical = Utility.ParseToDouble(shipDragNode[0].Attributes["vertical"].Value);
                                if (shipDragNode[0].Attributes["pitch"] != null)
                                    uiModelShip.Pitch = Utility.ParseToDouble(shipDragNode[0].Attributes["pitch"].Value);
                                if (shipDragNode[0].Attributes["yaw"] != null)
                                    uiModelShip.Yaw = Utility.ParseToDouble(shipDragNode[0].Attributes["yaw"].Value);
                                if (shipDragNode[0].Attributes["roll"] != null)
                                    uiModelShip.Roll = Utility.ParseToDouble(shipDragNode[0].Attributes["roll"].Value);
                            }
                            uiModelShip.Changed = false;

                            //var ShipExistsAlready = m_UIManager.UIModel.UIModelShips.Where(x => x.File == uiModelShip.File).ToList();
                            //if (ShipExistsAlready.Count == 0)
                            //{
                            //    //m_UIManager.UIModel.UIModelShipsVanilla.Add(uiModelShip.Copy());
                            //    m_UIManager.UIModel.UIModelShips.Add(uiModelShip);
                            //}
                        }
                    }
                }
            }
            return uiModelShip;
        }

        private string GetIGName(string id)
        {
            if (m_UIManager.TextDictionary.ContainsKey(id.Replace(" ", "")))
                return m_UIManager.TextDictionary[id.Replace(" ", "")];
            else
                return "no name";
        }

        public Dictionary<string, string> ReadTextXml(string path)
        {
            var dict = new Dictionary<string, string>();

            //path = path + m_UIManager.PathToTexts + @"\0001-l044.xml";
            //string modPath1 = m_UIManager.UIModel.ModPath1 + m_UIManager.PathToTexts + @"\0001.xml";
            //string modPath2 = m_UIManager.UIModel.ModPath2 + m_UIManager.PathToTexts + @"\0001.xml";

            if (File.Exists(path))
            {
                dict = ReadTextxml(m_UIManager.TextDictionary, path);
            }
            //if (File.Exists(modPath1))
            //{
            //    dict = ReadTextxml(m_UIManager.TextDictionary, modPath1);
            //}
            //if (File.Exists(modPath2))
            //{
            //    dict = ReadTextxml(m_UIManager.TextDictionary, modPath2);
            //}
            return dict;
        }
        private Dictionary<string, string> ReadTextxml(Dictionary<string, string> dictionary, string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlNodeList nodeListPage = doc.GetElementsByTagName("page");

            string page;
            string textId;
            string IGName;
            string id;
            foreach (XmlNode nodepage in nodeListPage)
            {
                XmlNodeList nodeListText = nodepage.SelectNodes("t");
                foreach (XmlNode nodetext in nodeListText)
                {
                    page = nodepage.Attributes["id"].Value.ToString();
                    textId = nodetext.Attributes["id"].Value.ToString();
                    id = "{" + page + "," + textId + "}";
                    IGName = nodetext.InnerText;
                    if (!dictionary.ContainsKey(id))
                        dictionary.Add(id, IGName);
                    else
                        dictionary[id] = IGName;
                }
            }
            return dictionary;
        }
    }
}
