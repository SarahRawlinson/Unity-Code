using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Mirror;
using MultiplayerRTS.Stats;
using MultiplayerRTS.Spawning;
using MultiplayerRTS.UnitControl;
using MultiplayerRTS.Resources;
using MultiplayerRTS.Combat;
using MultiplayerRTS.Networking;
using UnityEngine.Serialization;

namespace MultiplayerRTS.Control
{
    public class ControlItem : NetworkBehaviour
    {
        [FormerlySerializedAs("nameOfIteam")] [SerializeField] string nameOfItem = "";
        [SerializeField] internal GameObject buildPrefab = null;
        [SerializeField] internal Health_RTS health = null;
        [SerializeField] internal Sprite icon = null;
        [SerializeField] internal int id = -1;
        [SerializeField] internal int price = 100;
        [SyncVar] private int connectionID;
        public GameObject BuildPrefab { get => buildPrefab; }
        public Health_RTS Health { get => health; }
        public Sprite Icon { get => icon; }
        public int ID { get => id; }
        public int Price { get => price; }
        public string NameOfItem
        {
            get => nameOfItem;
        }
        private string _description = "";

        public string GetDescription()
        {
            if (_description == "") _description = BuildDescription();
            return _description;
        }

        public override void OnStartServer()
        {
            connectionID = connectionToClient.connectionId;
        }

        private string BuildDescription()
        {
            var dataString = SpawnerDataString;

            dataString = ResourceModifierDataString(dataString);

            dataString = FighterDataString(dataString);

            dataString = BaseObjectDataString(dataString);
            
            dataString = MedicObjectDataString(dataString);

            return dataString;
        }

        private string MedicObjectDataString(string dataString)
        {
            if (TryGetComponent(out Medic medic))
            {
                dataString +=
                    $"Medic can heal {medic.HealthQtyOverTime.ToString(CultureInfo.CurrentCulture)} " +
                    $"every {medic.TimePeriod.ToString(CultureInfo.CurrentCulture)} seconds within radius " +
                              $"{medic.HealthRadius.ToString(CultureInfo.CurrentCulture)} of this object\n";
            }

            return dataString;
        }

        private string BaseObjectDataString(string dataString)
        {
            if (TryGetComponent(out BaseObject baseObject))
            {
                dataString += $"is a Base Object can build within radius " +
                              $"{baseObject.BaseRange.ToString(CultureInfo.CurrentCulture)} of this object\n";
            }

            return dataString;
        }

        private string FighterDataString(string dataString)
        {
            if (TryGetComponent(out CombatFiring fighter))
            {
                float maxRange = 0;
                int maxDamage = 0;
                List<CombatProjectile> projectiles = new List<CombatProjectile>();
                foreach (Weapon weapon in fighter.Weapons)
                {
                    if (weapon.ProjectilePrefab.TryGetComponent(out CombatProjectile projectile))
                    {
                        if (!projectiles.Contains(projectile))
                        {
                            projectiles.Add(projectile);
                        }
                    }
                    if (maxRange < weapon.FireRange) maxRange = weapon.FireRange;
                    if (maxDamage < weapon.Damage) maxDamage = weapon.Damage;
                }
                string projectileString = "";
                foreach (CombatProjectile projectile in projectiles)
                {
                    projectileString += $"{projectile.Name}, ";
                }
                if (projectileString != "") projectileString.Remove(projectileString.Length - 2, 2);
                dataString += $"Has fighter, has {fighter.Weapons.Length.ToString()} weapons max damage " +
                              $"{maxDamage.ToString()}, max range {maxRange.ToString(CultureInfo.CurrentCulture)}\n" +
                              $"Projectile types {projectileString}\n";
            }

            return dataString;
        }

        private string ResourceModifierDataString(string dataString)
        {
            if (TryGetComponent(out ResourceModifier resource))
            {
                if (resource.ResourcesPerInterval > 0)
                    dataString +=
                        $"Has Resource Generator, {resource.ResourcesPerInterval.ToString()} " +
                        $"every {resource.Interval.ToString(CultureInfo.CurrentCulture)} seconds\n";
                else
                    dataString += $"Requires Resources, {resource.ResourcesPerInterval.ToString()} " +
                                  $"every {resource.Interval.ToString(CultureInfo.CurrentCulture)} seconds\n" +
                                  $"Warning! If you run out of resources for this item it will explode\n";
            }

            return dataString;
        }

        private string SpawnerDataString
        {
            get
            {
                string dataString = $"{nameOfItem}\n";
                if (TryGetComponent(out RTSUnitSpawner spawner))
                {
                    string units = "";
                    foreach (Unit unit in spawner.UnitPrefabs) units += $"{unit.nameOfItem}, ";
                    if (units != "") units = units.Remove(units.Length - 2, 2);
                    dataString += $"Has Spawner, Units {units}\n";
                }

                return dataString;
            }
        }

        public RTSNetworkPlayer GetPlayer()
        {
            foreach (RTSNetworkPlayer player in FindObjectsOfType<RTSNetworkPlayer>())
            {
                if (connectionID == player.connectionID)
                {
                    return player;
                }
            }
            return null;
        }
        
    }
}
