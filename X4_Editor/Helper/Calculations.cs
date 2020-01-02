using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace X4_Editor.Helper
{
    public class Calculations
    {
        private UIManager parent;
        public Calculations(UIManager uiManager)
        {
            parent = uiManager;
        }

        
        public Counter CalculateOverAll(DataGrid dg, int operation, Counter counter)
        {
            foreach (DataGridCellInfo dataGridCell in dg.SelectedCells)
            {
                var shield = dataGridCell.Item as UIModelShield;
                if (shield != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        shield.Max = (int)Calculate(operation, shield.Max, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 6)
                    {
                        shield.Rate = Calculate(operation, shield.Rate, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        shield.Delay = Calculate(operation, shield.Delay, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        shield.MaxHull = (int)Calculate(operation, shield.MaxHull, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 9)
                    {
                        shield.Threshold = Calculate(operation, shield.Threshold, ref counter);

                    }
                }

                var engine = dataGridCell.Item as UIModelEngine;
                if (engine != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        engine.Forward = Calculate(operation, engine.Forward, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 6)
                    {
                        engine.Reverse = Calculate(operation, engine.Reverse, ref counter);

                    }

                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        engine.BoostThrust = Calculate(operation, engine.BoostThrust, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        engine.BoostDuration = Calculate(operation, engine.BoostDuration, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 9)
                    {
                        engine.BoostAttack = Calculate(operation, engine.BoostAttack, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 10)
                    {
                        engine.BoostRelease = Calculate(operation, engine.BoostRelease, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 11)
                    {
                        engine.TravelThrust = Calculate(operation, engine.TravelThrust, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 12)
                    {
                        engine.TravelCharge = (int)Calculate(operation, engine.TravelCharge, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 13)
                    {
                        engine.TravelAttack = Calculate(operation, engine.TravelAttack, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        engine.TravelRelease = Calculate(operation, engine.TravelRelease, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 15)
                    {
                        engine.Strafe = Calculate(operation, engine.Strafe, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 16)
                    {
                        engine.Yaw = Calculate(operation, engine.Yaw, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 17)
                    {
                        engine.Pitch = Calculate(operation, engine.Pitch, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 18)
                    {
                        engine.Roll = Calculate(operation, engine.Roll, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 19)
                    {
                        engine.AngularPitch = Calculate(operation, engine.AngularPitch, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 20)
                    {
                        engine.AngularRoll = Calculate(operation, engine.AngularRoll, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 21)
                    {
                        engine.MaxHull = (int)Calculate(operation, engine.MaxHull, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 22)
                    {
                        engine.Threshold = (int)Calculate(operation, engine.Threshold, ref counter);

                    }
                }

                var projectile = dataGridCell.Item as UIModelProjectile;
                if (projectile != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        projectile.Damage = Calculate(operation, projectile.Damage, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 6)
                    {
                        projectile.Shield = Calculate(operation, projectile.Shield, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        projectile.Hull = Calculate(operation, projectile.Hull, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        projectile.ReloadRate = Calculate(operation, projectile.ReloadRate, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 9)
                    {
                        projectile.ReloadTime = Calculate(operation, projectile.ReloadTime, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 10)
                    {
                        projectile.Amount = (int)Calculate(operation, projectile.Amount, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 11)
                    {
                        projectile.BarrelAmount = (int)Calculate(operation, projectile.BarrelAmount, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 13)
                    {
                        projectile.Range = (int)Calculate(operation, projectile.Range, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        projectile.Speed = (int)Calculate(operation, projectile.Speed, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 15)
                    {
                        projectile.Lifetime = Calculate(operation, projectile.Lifetime, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 16)
                    {
                        projectile.ChargeTime = Calculate(operation, projectile.ChargeTime, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 17)
                    {
                        projectile.Ammunition = (int)Calculate(operation, projectile.Ammunition, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 18)
                    {
                        projectile.AmmunitionReload = Calculate(operation, projectile.AmmunitionReload, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 19)
                    {
                        projectile.Angle = Calculate(operation, projectile.Angle, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 20)
                    {
                        projectile.TimeDiff = Calculate(operation, projectile.TimeDiff, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 21)
                    {
                        projectile.MaxHits = (int)Calculate(operation, projectile.MaxHits, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 22)
                    {
                        projectile.Scale = Calculate(operation, projectile.Scale, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 23)
                    {
                        projectile.Ricochet = (int)Calculate(operation, projectile.Ricochet, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 24)
                    {
                        projectile.HeatValue = (int)Calculate(operation, projectile.HeatValue, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 25)
                    {
                        projectile.HeatInitial = (int)Calculate(operation, projectile.HeatInitial, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 26)
                    {
                        projectile.Repair = Calculate(operation, projectile.Repair, ref counter);

                    }
                }

                var weapon = dataGridCell.Item as UIModelWeapon;
                if (weapon != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        weapon.RotationSpeed = Calculate(operation, weapon.RotationSpeed, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 6)
                    {
                        weapon.RotationAcceleration = Calculate(operation, weapon.RotationAcceleration, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        weapon.ReloadRate = Calculate(operation, weapon.ReloadRate, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        weapon.ReloadTime = Calculate(operation, weapon.ReloadTime, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 9)
                    {
                        weapon.Overheat = (int)Calculate(operation, weapon.Overheat, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 10)
                    {
                        weapon.CoolDelay = Calculate(operation, weapon.CoolDelay, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 11)
                    {
                        weapon.CoolRate = (int)Calculate(operation, weapon.CoolRate, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 12)
                    {
                        weapon.Reenable = (int)Calculate(operation, weapon.Reenable, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 13)
                    {
                        weapon.HullMax = (int)Calculate(operation, weapon.HullMax, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        weapon.HullThreshold = Calculate(operation, weapon.HullThreshold, ref counter);

                    }

                }

                var missile = dataGridCell.Item as UIModelMissile;
                if (missile != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 2)
                    {
                        missile.Damage = (int)Calculate(operation, missile.Damage, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 3)
                    {
                        missile.Reload = Calculate(operation, missile.Reload, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        missile.Range = (int)Calculate(operation, missile.Range, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 6)
                    {
                        missile.Lifetime = Calculate(operation, missile.Lifetime, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        missile.Forward = Calculate(operation, missile.Forward, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        missile.Reverse = Calculate(operation, missile.Reverse, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 9)
                    {
                        missile.Guided = (int)Calculate(operation, missile.Guided, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 10)
                    {
                        missile.Swarm = (int)Calculate(operation, missile.Swarm, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 11)
                    {
                        missile.Retarget = (int)Calculate(operation, missile.Retarget, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 12)
                    {
                        missile.Hull = (int)Calculate(operation, missile.Hull, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 13)
                    {
                        missile.Ammunition = (int)Calculate(operation, missile.Ammunition, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        missile.MissileAmount = (int)Calculate(operation, missile.MissileAmount, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 15)
                    {
                        missile.Mass = Calculate(operation, missile.Mass, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 16)
                    {
                        missile.Horizontal = Calculate(operation, missile.Horizontal, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 17)
                    {
                        missile.Vertical = Calculate(operation, missile.Vertical, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 18)
                    {
                        missile.Pitch = Calculate(operation, missile.Pitch, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 19)
                    {
                        missile.Yaw = Calculate(operation, missile.Yaw, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 20)
                    {
                        missile.Roll = Calculate(operation, missile.Roll, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 21)
                    {
                        missile.InertiaPitch = Calculate(operation, missile.InertiaPitch, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 22)
                    {
                        missile.InertiaYaw = Calculate(operation, missile.InertiaYaw, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 23)
                    {
                        missile.InertiaRoll = Calculate(operation, missile.InertiaRoll, ref counter);

                    }
                }

                var ship = dataGridCell.Item as UIModelShip;
                if (ship != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 5)
                    {
                        ship.HullMax = (int)Calculate(operation, ship.HullMax, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 12)
                    {
                        ship.StorageMissiles = (int)Calculate(operation, ship.StorageMissiles, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 13)
                    {
                        ship.People = (int)Calculate(operation, ship.People, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        ship.ExplosionDamage = (int)Calculate(operation, ship.ExplosionDamage, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 15)
                    {
                        ship.Secrecy = (int)Calculate(operation, ship.Secrecy, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 16)
                    {
                        ship.GatherRrate = Calculate(operation, ship.GatherRrate, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 17)
                    {
                        ship.Mass = Calculate(operation, ship.Mass, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 18)
                    {
                        ship.Forward = Calculate(operation, ship.Forward, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 19)
                    {
                        ship.Reverse = Calculate(operation, ship.Reverse, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 20)
                    {
                        ship.Horizontal = Calculate(operation, ship.Horizontal, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 21)
                    {
                        ship.Vertical = Calculate(operation, ship.Vertical, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 22)
                    {
                        ship.Pitch = Calculate(operation, ship.Pitch, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 23)
                    {
                        ship.Yaw = Calculate(operation, ship.Yaw, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 24)
                    {
                        ship.Roll = Calculate(operation, ship.Roll, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 25)
                    {
                        ship.InertiaPitch = Calculate(operation, ship.InertiaPitch, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 26)
                    {
                        ship.InertiaYaw = Calculate(operation, ship.InertiaYaw, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 27)
                    {
                        ship.InertiaRoll = Calculate(operation, ship.InertiaRoll, ref counter);

                    }
                }

                var ware = dataGridCell.Item as UIModelWare;
                if (ware != null)
                {
                    if (dataGridCell.Column.DisplayIndex == 2)
                    {
                        ware.Min = (int)Calculate(operation, ware.Min, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 3)
                    {
                        ware.Avg = (int)Calculate(operation, ware.Avg, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 4)
                    {
                        ware.Max = (int)Calculate(operation, ware.Max, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 7)
                    {
                        ware.Amount = (int)Calculate(operation, ware.Amount, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 8)
                    {
                        ware.Time = (int)Calculate(operation, ware.Time, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 10)
                    {
                        ware.Amount1 = (int)Calculate(operation, ware.Amount1, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 12)
                    {
                        ware.Amount2 = (int)Calculate(operation, ware.Amount2, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 14)
                    {
                        ware.Amount3 = (int)Calculate(operation, ware.Amount3, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 16)
                    {
                        ware.Amount4 = (int)Calculate(operation, ware.Amount4, ref counter);

                    }
                    if (dataGridCell.Column.DisplayIndex == 18)
                    {
                        ware.Amount5 = (int)Calculate(operation, ware.Amount5, ref counter);

                    }
                }

            }

            if (counter.successcounter > 0 && dg.SelectedCells.Count == counter.successcounter)
                MessageBox.Show("All " + counter.successcounter + " values have been changed successfully.", "Calculation Success", MessageBoxButton.OK, MessageBoxImage.None);
            else if (counter.successcounter == 0)
                MessageBox.Show("No cells could be changed. \rBe sure to select only cells you can use calculations on.", "Calculation Failed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else if (counter.successcounter > 0 && dg.SelectedCells.Count != counter.successcounter)
            {
                MessageBox.Show("Only " + counter.successcounter + " of " + dg.SelectedCells.Count + " cells could be changed succesfully. \rBe sure to select only cells you can use calculations on.", "Calculation partially Success.", MessageBoxButton.OK ,MessageBoxImage.Warning);
            }

            return counter;
        }
        public double Calculate(int operation, double param1, ref Counter counter)
        {
            switch (operation)
            {
                case 1:
                    counter.successcounter++;
                    return param1 + parent.UIModel.MathParameter;
                case 2:
                    counter.successcounter++;
                    return param1 * parent.UIModel.MathParameter;
                case 3:
                    counter.successcounter++;
                    return param1 - parent.UIModel.MathParameter;
                case 4:
                    counter.successcounter++;
                    return parent.UIModel.MathParameter;
                case 5:
                    {
                        if (parent.UIModel.MathParameter > 0)
                        {
                            counter.successcounter++;
                            return param1 / parent.UIModel.MathParameter;
                        }
                        return param1;
                    }
                default:
                    return param1;
            }
        }
    }
}
