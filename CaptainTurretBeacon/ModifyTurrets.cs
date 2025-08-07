using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;

namespace CaptainTurretBeacon
{
    public class ModifyTurrets : MonoBehaviour
    {
        public CharacterMaster master;
        // public int beaconLimit;
        public List<int> turretIndicesToRemoveList = new();
        public List<CharacterMaster> turrets = new();

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

            // beaconLimit = master.GetDeployableSameSlotLimit(DeployableSlot.CaptainSupplyDrop);
            HandleRemovingExtraTurrets();
            HandleTurretBlacklistAndGodmode();
        }

        public void HandleRemovingExtraTurrets()
        {
            var maxBeacons = master.GetDeployableSameSlotLimit(DeployableSlot.CaptainSupplyDrop);

            if (turrets.Count >= maxBeacons)
            {
                turrets[0].minionOwnership.ownerMaster = null;
                turrets[0].TrueKill();
                turrets.RemoveAt(0);
            }
            turrets[0].minionOwnership.ownerMaster = null;
            turrets[0].TrueKill();
        }

        public void HandleTurretBlacklistAndGodmode()
        {
            var turretList = master.deployablesList.Where(x => x.slot == DeployableSlot.EngiTurret).ToList();
            var maxBeacons = master.GetDeployableSameSlotLimit(DeployableSlot.CaptainSupplyDrop);

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

                /*
                if (turretList.Count >= maxBeacons)
                {
                    for (int j = turretList.Count - maxBeacons; j >= 0; j--)
                    {
                        master.deployablesList.Remove(j);
                        deployable.ownerMaster = null;
                        deployable.onUndeploy.Invoke();
                        deployableMaster.TrueKill();
                        // possible error: Collection was modified; enumeration operation may not execute?
                    }
                }
                */

                var extraTurrets = turretList.Count - maxBeacons;
                var index = master.deployablesList.Count - 1;
                while (extraTurrets > 0 || index >= 0)
                {
                    // if (master.deployablesList[i] == turret) if deployable slot is equal to engiturret? or do I NEED a horrible gameobject reference because deployable is garbage and minionownership is garbage and captain spaghetti code is garbage
                    {
                        // deployableMaster.TrueKill();
                        deployable.ownerMaster = null;
                        deployable.onUndeploy.Invoke();
                        // turretIndicesToRemoveList.Add(index);
                        extraTurrets--;
                    }
                    index--;
                }
                /*
                for (int j = turretIndicesToRemoveList.Count - 1; j >= 0; j--)
                {
                    deployable.ownerMaster = null;
                    deployable.onUndeploy.Invoke();
                    turretIndicesToRemoveList.RemoveAt(j);
                }

                turretIndicesToRemoveList.Clear();
                */

                if (deployable.TryGetComponent<CharacterMaster>(out var deployableMaster))
                {
                    var deployableBody = deployableMaster.GetBody();
                    if (!deployableBody)
                    {
                        continue;
                    }

                    var inventory = deployableBody.inventory;

                }
            }
        }
    }
}
