using System;
using System.Collections.Generic;
using System.Globalization;
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

        public void ReadAllWares(string waresPath, bool activeMod = false)
        {
            if (File.Exists(waresPath))
            {
                string additionalErrorText = "";
                try
                {
                    m_UIManager.UIModel.AllWaresLoaded = false;
                    using (XmlReader reader = XmlReader.Create(waresPath))
                    {
                        {
                            XDocument doc = XDocument.Load(waresPath);
                            bool addedWaresByMod = false;
                            if (doc.Descendants().Where(x => x.Name.LocalName == "add").ToList().Count > 0)
                                addedWaresByMod = true;
                            if (doc.Descendants().Where(x => x.Name.LocalName == "wares").ToList().Count > 0 || addedWaresByMod)
                            {
                                ReadNewWares(waresPath, doc, addedWaresByMod);
                            }

                            if (doc.Descendants().Where(x => x.Name.LocalName == "diff").ToList().Count > 0)
                            {
                                // this part is for ware properties replaced by a mod
                                var diffNodes = doc.Descendants().Where(x => x.Name.LocalName == "diff").ToList();

                                if (diffNodes.Descendants().Where(x => x.Name.LocalName == "add").ToList().Count > 0)
                                    addedWaresByMod = true;

                                if (doc.Descendants().Where(x => x.Name.LocalName == "wares").ToList().Count > 0 || addedWaresByMod)
                                {
                                    ReadNewWares(waresPath, doc, addedWaresByMod);
                                }

                                // if ware values have been altered these have to be applied also
                                {
                                    foreach (var diff in diffNodes)
                                    {
                                        //if (diff.Descendants().Where(x => x.Name.LocalName == "replace").ToList().Count > 0)
                                        //{
                                        //    var replacedWares = diff.Descendants().Where(x => x.Name.LocalName == "replace").ToList();


                                        //    foreach (var item in replacedWares)
                                        //    {
                                        XmlDocument xD = new XmlDocument();
                                        xD.LoadXml(diff.ToString());
                                        XmlNode xN = XmlHelper.ToXmlNode(diff);
                                        XmlNodeList wareNodes = xN.SelectNodes("//replace");


                                        foreach (XmlNode wareNode in wareNodes)
                                        {
                                            XmlDocument xDD = new XmlDocument();
                                            xDD.LoadXml(wareNode.OuterXml);
                                            XmlNode item = xDD.FirstChild;

                                            //XmlNode item = XmlHelper.ToXmlNode(wareNode.OuterXml.ToXElement());

                                            if (item.Attributes["sel"] != null)
                                            {
                                                string wareSel = item.Attributes["sel"].Value;
                                                string wareID = wareSel.Split('\'')[1];
                                                additionalErrorText = wareID;
                                                var wareToChange = this.m_UIManager.UIModel.UIModelWares.Where(x => x.Name == wareID).ToList();

                                                //check for direct changes "@property"
                                                if (wareSel.Contains("@threshold"))
                                                {
                                                    wareToChange.FirstOrDefault().Threshold = Utility.ParseToDouble(item.InnerText);
                                                }
                                                if (wareSel.Contains("@amount"))
                                                {
                                                    wareToChange.FirstOrDefault().Amount = Convert.ToInt32(item.InnerText);
                                                }
                                                if (wareSel.Contains("@time"))
                                                {
                                                    wareToChange.FirstOrDefault().Time = Convert.ToInt32(item.InnerText);
                                                }
 

                                                if (wareToChange != null && item.FirstChild != null)
                                                {
                                                    wareToChange.FirstOrDefault().Changed = true;
                                                    if (item.FirstChild.LocalName == "price")
                                                    {
                                                        wareToChange.FirstOrDefault().Min = Convert.ToInt32(item.FirstChild.Attributes["min"].Value);
                                                        wareToChange.FirstOrDefault().Max = Convert.ToInt32(item.FirstChild.Attributes["max"].Value);
                                                        wareToChange.FirstOrDefault().Avg = Convert.ToInt32(item.FirstChild.Attributes["average"].Value);
                                                    }
                                                    if (item.FirstChild.LocalName == "primary")
                                                    {
                                                        for (int i = 0; i < item.FirstChild.ChildNodes.Count; i++)
                                                        {
                                                            if (i == 0)
                                                            {
                                                                wareToChange.FirstOrDefault().Ware1 = item.FirstChild.ChildNodes[i].Attributes["ware"].Value;
                                                                wareToChange.FirstOrDefault().Amount1 = Convert.ToInt32(item.FirstChild.ChildNodes[i].Attributes["amount"].Value);
                                                            }
                                                            if (i == 1)
                                                            {
                                                                wareToChange.FirstOrDefault().Ware2 = item.FirstChild.ChildNodes[i].Attributes["ware"].Value;
                                                                wareToChange.FirstOrDefault().Amount2 = Convert.ToInt32(item.FirstChild.ChildNodes[i].Attributes["amount"].Value);
                                                            }
                                                            if (i == 2)
                                                            {
                                                                wareToChange.FirstOrDefault().Ware3 = item.FirstChild.ChildNodes[i].Attributes["ware"].Value;
                                                                wareToChange.FirstOrDefault().Amount3 = Convert.ToInt32(item.FirstChild.ChildNodes[i].Attributes["amount"].Value);
                                                            }
                                                            if (i == 3)
                                                            {
                                                                wareToChange.FirstOrDefault().Ware4 = item.FirstChild.ChildNodes[i].Attributes["ware"].Value;
                                                                wareToChange.FirstOrDefault().Amount4 = Convert.ToInt32(item.FirstChild.ChildNodes[i].Attributes["amount"].Value);
                                                            }
                                                            if (i == 4)
                                                            {
                                                                wareToChange.FirstOrDefault().Ware5 = item.FirstChild.ChildNodes[i].Attributes["ware"].Value;
                                                                wareToChange.FirstOrDefault().Amount5 = Convert.ToInt32(item.FirstChild.ChildNodes[i].Attributes["amount"].Value);
                                                            }
                                                        }
                                                    }
                                                    wareToChange.FirstOrDefault().Changed = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    m_UIManager.UIModel.AllWaresLoaded = true;
                    m_UIManager.UIModel.CalculateWarePrices();
                    foreach (var item in m_UIManager.UIModel.UIModelWares)
                    {
                        item.Changed = false;
                    }
                    //TODO: this needs to be done only in case of vanilla and not mods
                    //m_UIManager.UIModel.UIModelWaresVanilla.Clear();
                    if (!activeMod)
                    {
                        foreach (var item in m_UIManager.UIModel.UIModelWares)
                        {
                            m_UIManager.UIModel.UIModelWaresVanilla.Add(item.Copy());
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("\rErrer reading wares.xml: " + ex.Message + " " + additionalErrorText);
                }
            }
        }

        private void ReadNewWares(string waresPath, XDocument doc, bool addedWaresByMod)
        {
            var wares = doc.Descendants("wares");
            if (addedWaresByMod)
                wares = doc.Descendants("add");

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
                        XmlNode useNode = item.SelectSingleNode("use");
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
                        if (useNode != null)
                        {
                            if (useNode.Attributes["threshold"] != null)
                                uiModelWare.Threshold = Utility.ParseToDouble(useNode.Attributes["threshold"].Value);
                        }
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

                        else if (item.SelectNodes("./production").Count >= 2)
                        {
                            XmlNodeList productionNodes = item.SelectNodes("./production");

                            int defaultIndex = 0;
                            for (int i = 0; i < productionNodes.Count; i++ )
                            {
                                if (item.SelectNodes("./production")[i].Attributes["method"].Value == "default")
                                    defaultIndex = i;
                            }
                            
                            // always take default production values

                            productionNodes = item.SelectNodes("./production")[defaultIndex].ChildNodes[0].ChildNodes;

                            if (productionNodes.Count > 0)
                            {
                                uiModelWare.Time = Utility.ParseToDouble(item.SelectNodes("./production")[defaultIndex].Attributes["time"].Value);
                                uiModelWare.Amount = Convert.ToInt32(item.SelectNodes("./production")[defaultIndex].Attributes["amount"].Value);
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
                        uiModelWare.Changed = false;

                        var wareWithSameId = m_UIManager.UIModel.UIModelWares.Where(x => x.ID.Equals(uiModelWare.ID)).ToList();
                        if (wareWithSameId.Count == 0)
                            m_UIManager.UIModel.UIModelWares.Add(uiModelWare);
                    }
                }
            }
        }

        public UIModelProjectile ReadSingleProjectile(FileInfo file)
        {
            if (file.FullName.EndsWith(".sig"))
                    return null;
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
                                    uiModelProjectile.Amount = Convert.ToInt16(weaponBulletNode[0].Attributes["amount"].Value);
                                if (weaponBulletNode[0].Attributes["barrelamount"] != null)
                                    uiModelProjectile.BarrelAmount = Convert.ToInt16(weaponBulletNode[0].Attributes["barrelamount"].Value);
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
                                if (weaponDamageNode[0].Attributes["hull"] != null)
                                    uiModelProjectile.Hull = Utility.ParseToDouble(weaponDamageNode[0].Attributes["hull"].Value);
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
            if (file.FullName.EndsWith(".sig"))
                return null;
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
                            XmlNodeList missileIdentificationNode = xN.SelectNodes("//properties/identification");
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

                            if (missileIdentificationNode.Count > 0)
                            {
                                if (missileIdentificationNode[0].Attributes["name"] != null)
                                    uiModelMissile.IGName = this.GetIGName(missileIdentificationNode[0].Attributes["name"].Value);
                                else
                                    uiModelMissile.IGName = "'unknown'";
                            }

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
                            //m_UIManager.UIModel.UIModelMissiles.Add(uiModelMissile);
                        }
                    }
                }
            }
            return uiModelMissile;
        }
        public UIModelEngine ReadSingleEngineFile(FileInfo file)
        {
            if (file.FullName.EndsWith(".sig"))
                return null;
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
                                if (engineIdentificationNode[0].Attributes["name"] != null)
                                    uiModelEngine.IGName = this.GetIGName(engineIdentificationNode[0].Attributes["name"].Value);
                                else
                                    uiModelEngine.IGName = "'unknown'";
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
            if (file.FullName.EndsWith(".sig"))
                return null;
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
                                if (shielIdentificationNode[0].Attributes["makerrace"] != null)
                                    uiModelShield.Faction = shielIdentificationNode[0].Attributes["makerrace"].Value;
                                uiModelShield.MK = shielIdentificationNode[0].Attributes["mk"].Value;
                                if (shielIdentificationNode[0].Attributes["name"] != null)
                                    uiModelShield.IGName = this.GetIGName(shielIdentificationNode[0].Attributes["name"].Value);
                                else
                                    uiModelShield.IGName = "'unknown'";
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
            if (xmlWeaponFileInfo.FullName.EndsWith(".sig"))
                return null;
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
                                if (weaponIdentificationNode[0].Attributes["name"] != null)
                                    uiModelWeapon.IGName = this.GetIGName(weaponIdentificationNode[0].Attributes["name"].Value);
                                else
                                    uiModelWeapon.IGName = "'unknown'";
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
            if (file.FullName.EndsWith(".sig"))
                return null;
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
                    try
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

                                if (shipMacroNode[0].Attributes["class"] != null && shipMacroNode[0].Attributes["class"].Value != "storage")
                                {
                                    #region MyRegion for components
                                    if (shipShipTypeNode.Count > 0)
                                    {
                                        Tuple<string, string, string> components = this.ReadShipComponents(file);
                                        uiModelShip.Shields = components.Item1;
                                        uiModelShip.Turrets = components.Item2;
                                        uiModelShip.Weapons = components.Item3;
                                    }
                                    #endregion

                                    #region region for cargo 
                                    string CargoFile = file.FullName.Replace(m_UIManager.UIModel.ModPath1, m_UIManager.UIModel.Path).Replace(m_UIManager.UIModel.ModPath2, m_UIManager.UIModel.Path).Replace("ship_", "storage_");
                                    string CargoModFile1 = "";
                                    string CargoModFile2 = "";
                                    // different cases: 1. storage file is in the same folder
                                    //                  2. storage file is in a mod folder
                                    // storage file can be the same name with ships_ replaced by storage or units_ replaced by storage

                                    if (file.Name.Contains("ship_"))
                                    {
                                        if (File.Exists(file.FullName.Replace(m_UIManager.UIModel.Path, m_UIManager.UIModel.ModPath1).Replace("ship_", "storage_")))
                                        {
                                            CargoModFile1 = file.FullName.Replace(m_UIManager.UIModel.Path, m_UIManager.UIModel.ModPath1).Replace("ship_", "storage_");
                                        }
                                        if (File.Exists(file.FullName.Replace(m_UIManager.UIModel.Path, m_UIManager.UIModel.ModPath2).Replace("ship_", "storage_")))
                                        {
                                            CargoModFile2 = file.FullName.Replace(m_UIManager.UIModel.Path, m_UIManager.UIModel.ModPath2).Replace("ship_", "storage_");
                                        }
                                    }
                                    if (file.Name.Contains("units_"))
                                    {
                                        if (File.Exists(file.FullName.Replace(m_UIManager.UIModel.Path, m_UIManager.UIModel.ModPath1).Replace("units_", "storage_units_")))
                                        {
                                            CargoModFile1 = file.FullName.Replace(m_UIManager.UIModel.Path, m_UIManager.UIModel.ModPath1).Replace("units_", "storage_units_");
                                        }
                                        if (File.Exists(file.FullName.Replace(m_UIManager.UIModel.Path, m_UIManager.UIModel.ModPath2).Replace("units_", "storage_units_")))
                                        {
                                            CargoModFile2 = file.FullName.Replace(m_UIManager.UIModel.Path, m_UIManager.UIModel.ModPath2).Replace("units_", "storage_units_");
                                        }
                                    }

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
                                        if (shipIdentificationNode[0].Attributes["name"] != null)
                                            uiModelShip.IGName = this.GetIGName(shipIdentificationNode[0].Attributes["name"].Value);
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

                                }
                                //var ShipExistsAlready = m_UIManager.UIModel.UIModelShips.Where(x => x.File == uiModelShip.File).ToList();
                                //if (ShipExistsAlready.Count == 0)
                                //{
                                //    //m_UIManager.UIModel.UIModelShipsVanilla.Add(uiModelShip.Copy());
                                //    m_UIManager.UIModel.UIModelShips.Add(uiModelShip);
                                //}
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(file.FullName + " could not be loaded.");
                    }
                }
            }
            return uiModelShip;
        }

        private Tuple<string, string, string> ReadShipComponents(FileInfo file)
        {
            if (!Directory.Exists(file.DirectoryName))
            {
                throw new DirectoryNotFoundException(file.DirectoryName + " not found");
            }
            string ComponentPath = Directory.GetParent(file.DirectoryName).ToString() + @"\" + file.Name.Replace("_macro", "");
            string VanillaPath = "";
            string shields = "";
            string turrets = "";
            string weapons = "";
            int shields_S = 0;
            int shields_M = 0;
            int shields_L = 0;
            int shields_XL = 0;
            int turrets_M = 0;
            int turrets_L = 0;
            int weapons_S = 0;
            int weapons_M = 0;
            int weapons_L = 0;
            if (!File.Exists(ComponentPath))
            {
                string newComponentPath = "";
                string[] path = ComponentPath.Split('_');
                for (int i = 0; i <= path.Length - 2; i++)
                {
                    newComponentPath = newComponentPath + path[i] + "_";
                }
                ComponentPath = newComponentPath.Substring(0, newComponentPath.Length - 1) + ".xml";
            }
            // if file does not exists it maybe exist in vanilla folder
            if (File.Exists(ComponentPath))
            {
                //because modpath1+2 are unique this should work
                VanillaPath = ComponentPath.Replace(this.m_UIManager.UIModel.ModPath1, m_UIManager.UIModel.Path);
                VanillaPath = ComponentPath.Replace(this.m_UIManager.UIModel.ModPath2, m_UIManager.UIModel.Path);
            }
            if (File.Exists(VanillaPath))
            {
                try
                {
                    using (XmlReader reader = XmlReader.Create(VanillaPath))
                    {
                        XDocument doc = XDocument.Load(VanillaPath);
                        if (doc.Descendants().Where(x => x.Name.LocalName == "components").ToList().Count > 0)
                        {
                            List<XElement> Connections = doc.Descendants().Where(x => x.Name.LocalName == "connection").ToList();
                            List<string> Names = new List<string>();
                            string NoUniqueNameIds = "";

                            this.CalculateModules(file, ref shields_S, ref shields_M, ref shields_L, ref shields_XL, ref turrets_M, ref turrets_L, ref weapons_S, ref weapons_M, ref weapons_L, Connections, Names, ref NoUniqueNameIds);

                            if (NoUniqueNameIds.Length > 0)
                                MessageBox.Show(NoUniqueNameIds);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            // excpect only addition of new components - no removal or alteration of components!
            if (File.Exists(ComponentPath) && !VanillaPath.Equals(ComponentPath))
            {
                // if file exists the vanilla components need to be checked first as base
                try
                {
                    //using (XmlReader reader = XmlReader.Create(ComponentPath))
                    {
                        XDocument doc = XDocument.Load(ComponentPath);
                        if (doc.Descendants().Where(x => x.Name.LocalName == "components").ToList().Count > 0)
                        {
                            List<XElement> components = doc.Descendants().Where(x => x.Name.LocalName == "components").ToList();
                            List<XElement> Connections = components.Descendants().Where(x => x.Name.LocalName == "connection").ToList();
                            List<string> Names = new List<string>();
                            string NoUniqueNameIds = "";

                            this.CalculateModules(file, ref shields_S, ref shields_M, ref shields_L, ref shields_XL, ref turrets_M, ref turrets_L, ref weapons_S, ref weapons_M, ref weapons_L, Connections, Names, ref NoUniqueNameIds);

                            if (NoUniqueNameIds.Length > 0)
                                MessageBox.Show(NoUniqueNameIds);
                        }
                        else if (doc.Descendants().Where(x => x.Name.LocalName == "add").ToList().Count > 0)
                        {
                            List<XElement> additions = doc.Descendants().Where(x => x.Name.LocalName == "add").ToList();
                            foreach (var addition in additions)
                            {
                                List<XElement> Connections = addition.Descendants().Where(x => x.Name.LocalName == "connection").ToList();
                                List<string> Names = new List<string>();
                                string NoUniqueNameIds = "";
                                this.CalculateModules(file, ref shields_S, ref shields_M, ref shields_L, ref shields_XL, ref turrets_M, ref turrets_L, ref weapons_S, ref weapons_M, ref weapons_L, Connections, Names, ref NoUniqueNameIds);
                                if (NoUniqueNameIds.Length > 0)
                                    MessageBox.Show(NoUniqueNameIds);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            shields = AddIfNotNull(shields_S, "S ") + AddIfNotNull(shields_M, "M ") + AddIfNotNull(shields_L, "L ") + AddIfNotNull(shields_XL, "XL");
            turrets = AddIfNotNull(turrets_M, "M ") + AddIfNotNull(turrets_L, "L ");
            weapons = AddIfNotNull(weapons_S, "S ") + AddIfNotNull(weapons_M, "M ") + AddIfNotNull(weapons_L, "L ");
            return new Tuple<string, string, string>(shields, turrets, weapons);
        }

        private void CalculateModules(FileInfo file, ref int shields_S, ref int shields_M, ref int shields_L, ref int shields_XL, ref int turrets_M, ref int turrets_L, ref int weapons_S, ref int weapons_M, ref int weapons_L, List<XElement> Connections, List<string> Names, ref string NoUniqueNameIds)
        {
            foreach (var connection in Connections)
            {
                //check for douplicates
                if (!Names.Contains(connection.Attribute("name").Value))
                {
                    Names.Add(connection.Attribute("name").Value);
                }
                else
                {
                    NoUniqueNameIds = NoUniqueNameIds + "File " + file.Name + " has no unique name ID: " + connection.Attribute("name").Value + "\r";
                }
                // end check

                if (connection.Attribute("tags") != null)
                {
                    string tags = connection.Attribute("tags").Value;
                    if (tags.Contains("turret") && tags.Contains("medium"))
                    {
                        turrets_M++;
                    }
                    if (tags.Contains("turret") && tags.Contains("large") && !tags.Contains("extralarge"))
                    {
                        turrets_L++;
                    }
                    if (tags.Contains("weapon") && tags.Contains("small"))
                    {
                        weapons_S++;
                    }
                    if (tags.Contains("weapon") && tags.Contains("medium"))
                    {
                        weapons_M++;
                    }
                    if (tags.Contains("weapon") && tags.Contains("large") && !tags.Contains("extralarge"))
                    {
                        weapons_L++;
                    }
                }

                if (connection.Attribute("tags") != null && connection.Attribute("group") == null)
                {
                    string tags = connection.Attribute("tags").Value;
                    if (tags.Contains("shield") && tags.Contains("small"))
                    {
                        shields_S++;
                    }
                    if (tags.Contains("shield") && tags.Contains("medium"))
                    {
                        shields_M++;
                    }
                    if (tags.Contains("shield") && tags.Contains("large") && !tags.Contains("extralarge"))
                    {
                        shields_L++;
                    }
                    if (tags.Contains("shield") && tags.Contains("extralarge"))
                    {
                        shields_XL++;
                    }
                }
            }
        }

        private static void NewMethod(FileInfo file, ref int shields_S, ref int shields_M, ref int shields_L, ref int shields_XL, ref int turrets_M, ref int turrets_L, ref int weapons_S, ref int weapons_M, ref int weapons_L, List<XElement> additions)
        {

        }

        private string AddIfNotNull(int inputAmount, string inputType)
        {
            if (inputAmount == 0)
                return String.Empty;
            return inputAmount + inputType;
        }
        private string GetIGName(string id)
        {
            if (m_UIManager.TextDictionary.ContainsKey(id.Replace(" ", "")))
                return m_UIManager.TextDictionary[id.Replace(" ", "")];
            else
                return "no name";
        }

        public Dictionary<string, string> ReadTextXml(string path, Dictionary<string, string> dictionary)
        {
            var dict = new Dictionary<string, string>();

            //path = path + m_UIManager.PathToTexts + @"\0001-l044.xml";
            //string modPath1 = m_UIManager.UIModel.ModPath1 + m_UIManager.PathToTexts + @"\0001.xml";
            //string modPath2 = m_UIManager.UIModel.ModPath2 + m_UIManager.PathToTexts + @"\0001.xml";

            if (File.Exists(path))
            {
                dict = ReadTextxml(dictionary, path);
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
                    IGName = nodetext.InnerText.Split('{')[0].Replace("(", "").Replace(")", "").Replace(@"\", "");
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

