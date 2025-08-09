using System.Collections.Generic;
using RoR2;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using static RoR2.MasterCatalog;

namespace CaptainTurretBeacon
{
    public class HopooGames : NetworkBehaviour
    {
        [SyncVar]
        public List<CharacterMaster> turretMasters;

        [Command]
        public void HandleCreateTurret(CharacterBody builderBody, Vector3 position, Quaternion rotation, MasterIndex turretMasterIndex)
        {
            if (!builderBody)
            {
                return;
            }
            var builderCharacterMaster = builderBody.master;
            if (builderCharacterMaster)
            {
                var turretSummon = new MasterSummon
                {
                    masterPrefab = MasterCatalog.GetMasterPrefab(turretMasterIndex),
                    position = position,
                    rotation = rotation,
                    summonerBodyObject = builderBody.gameObject,
                    ignoreTeamMemberLimit = true,
                    inventoryToCopy = builderCharacterMaster.inventory
                }.Perform();

                var inventory = turretSummon.inventory;
                if (inventory)
                {
                    inventory.RemoveItem(RoR2Content.Items.Mushroom, inventory.GetItemCount(RoR2Content.Items.Mushroom));
                }

                turretMasters.Add(turretSummon);

                var turretBody = turretSummon.GetBody();

                if (turretBody)
                {
                    var healthComponent = turretBody.healthComponent;
                    if (healthComponent)
                    {
                        healthComponent.godMode = true;
                        healthComponent.isDefaultGodMode = true;
                    }
                }

                if (turretMasters.Count > builderCharacterMaster.GetDeployableSameSlotLimit(DeployableSlot.CaptainSupplyDrop))
                {
                    var oldestTurretBody = turretMasters[0].GetBody();
                    if (oldestTurretBody)
                    {
                        var healthComponent = oldestTurretBody.healthComponent;
                        if (healthComponent)
                        {
                            healthComponent.godMode = false;
                            healthComponent.isDefaultGodMode = false;
                        }
                    }
                    turretMasters[0].TrueKill();
                    turretMasters.RemoveAt(0);
                }
            }
        }
    }
}