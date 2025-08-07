using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;

namespace CaptainTurretBeacon
{
    public class ModifyTurrets : MonoBehaviour
    {
        public CharacterMaster master;
        public int beaconLimit;
        public List<GameObject> turrets = new();

        public void Start()
        {
            master = GetComponent<CharacterMaster>();
        }

        // fixedupdate for compat with other mods that change turret inventory all the time and change engi/captain max count all the time
        public void FixedUpdate()
        {
            if (master == null)
            {
                return;
            }

            beaconLimit = master.GetDeployableSameSlotLimit(DeployableSlot.CaptainSupplyDrop);
            HandleTurretBlacklistAndGodmode();
        }

        public void HandleTurretBlacklistAndGodmode()
        {
            if (master.deployablesList == null)
            {
                return;
            }

            for (int i = 0; i < master.deployablesList.Count; i++)
            {
                var deployableInfo = master.deployablesList[i];
                if (deployableInfo.slot != DeployableSlot.EngiTurret)
                {
                    continue;
                }

                var deployable = deployableInfo.deployable;
                if (!deployable)
                {
                    continue;
                }

                if (!turrets.Contains(deployable.gameObject))
                {
                    turrets.Add(deployable.gameObject);
                }

                if (deployable.TryGetComponent<CharacterMaster>(out var deployableMaster))
                {
                    var deployableBody = deployableMaster.GetBody();
                    if (!deployableBody)
                    {
                        continue;
                    }

                    var inventory = deployableBody.inventory;
                    if (inventory)
                    {
                        inventory.RemoveItem(RoR2Content.Items.Mushroom, inventory.GetItemCount(RoR2Content.Items.Mushroom));
                    }

                    var healthComponent = deployableBody.healthComponent;
                    if (!healthComponent)
                    {
                        continue;
                    }

                    healthComponent.godMode = true;
                    healthComponent.isDefaultGodMode = true;

                }
            }
        }
    }
}
