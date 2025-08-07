using EntityStates.Captain.Weapon;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace CaptainTurretBeacon
{
    public class CallSupplyTurretState : CallSupplyDropBase
    {
        public override void OnEnter()
        {
            supplyDropPrefab = Prefabs.beaconPrefab;
            base.OnEnter();
        }
    }
}