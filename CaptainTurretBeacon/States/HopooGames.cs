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
        public GameObject turretInstance;

        [Command]
        public void CmdSpawnTurret(GameObject builder, Vector3 position, Quaternion rotation)
        {
            if (!builder)
            {
                return;
            }

            var builderBody = builder.GetComponent<CharacterBody>();
            if (!builderBody)
            {
                return;
            }

            var turretMasterPrefab = GetMasterPrefab(FindMasterIndex(Prefabs.turretMasterPrefab));
            if (!turretMasterPrefab)
            {
                return;
            }

            var builderCharacterMaster = builderBody.master;
            if (builderCharacterMaster)
            {
                KillTurretInstance();

                var turretSummon = new MasterSummon
                {
                    masterPrefab = turretMasterPrefab,
                    position = position,
                    rotation = rotation,
                    summonerBodyObject = builder,
                    ignoreTeamMemberLimit = true,
                    inventoryToCopy = builderCharacterMaster.inventory
                }.Perform();

                var turretSummonInventory = turretSummon.inventory;
                if (turretSummonInventory)
                {
                    turretSummonInventory.RemoveItem(RoR2Content.Items.Mushroom, turretSummonInventory.GetItemCount(RoR2Content.Items.Mushroom));
                }

                var turretSummonBody = turretSummon.GetBody();

                if (turretSummonBody)
                {
                    var turretSummonHealthComponent = turretSummonBody.healthComponent;
                    if (turretSummonHealthComponent)
                    {
                        turretSummonHealthComponent.godMode = true;
                        turretSummonHealthComponent.isDefaultGodMode = true;
                    }
                }
                turretInstance = turretSummon.gameObject;
            }

        }

        public void OnDisable()
        {
            KillTurretInstance();
        }

        public void OnDestroy()
        {
            KillTurretInstance();
        }

        public void KillTurretInstance()
        {
            if (turretInstance)
            {
                var turretInstanceMaster = turretInstance.GetComponent<CharacterMaster>();
                var turretInstanceBody = turretInstanceMaster.GetBody();

                if (turretInstanceBody)
                {
                    var turretInstanceHealthComponent = turretInstanceBody.healthComponent;
                    if (turretInstanceHealthComponent)
                    {
                        turretInstanceHealthComponent.godMode = false;
                        turretInstanceHealthComponent.isDefaultGodMode = false;
                    }
                }
                turretInstanceMaster.TrueKill();
            }
        }
    }
}