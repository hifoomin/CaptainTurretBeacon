using EntityStates;
using EntityStates.CaptainSupplyDrop;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace CaptainTurretBeacon
{
    public class TurretMainState : BaseMainState
    {
        public static GameObject turretPrefab = Prefabs.turretMasterPrefab;
        public GameObject turretPrefabInstance;

        public GameObject indicatorInstance;
        public CharacterBody ownerBody;
        public float heightOffset = 3.5f;
        public HopooGames hopooGames;
        public override Interactability GetInteractability(Interactor activator)
        {
            return Interactability.Disabled;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            hopooGames = outer.GetComponent<HopooGames>();
            var genericOwnership = outer.GetComponent<GenericOwnership>();
            var finalPosition = interactionComponent.transform.position + interactionComponent.transform.up * heightOffset;
            if (genericOwnership)
            {
                var owner = genericOwnership.ownerObject;
                if (owner)
                {
                    if (hopooGames && isAuthority)
                    {
                        hopooGames.CmdSpawnTurret(owner, finalPosition, interactionComponent.transform.rotation);
                    }

                    enableRadiusIndicator = true;
                }
            }
        }

        public bool enableRadiusIndicator
        {
            get
            {
                return indicatorInstance;
            }
            set
            {
                if (enableRadiusIndicator != value)
                {
                    if (value)
                    {
                        indicatorInstance = Object.Instantiate(Prefabs.indicatorPrefab, transform.position, Quaternion.identity);
                        indicatorInstance.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(gameObject, null);
                    }
                    else
                    {
                        Object.Destroy(indicatorInstance);
                        indicatorInstance = null;
                    }
                }
            }
        }

        public override void OnExit()
        {
            enableRadiusIndicator = false;
            base.OnExit();
        }
    }
    /*
    public class StupidFuck : MonoBehaviour
    {
        public Deployable beaconDeployable;
        public CharacterBody captainBody;
        public HopooGames hopooGamesTurret;
        public void Start()
        {
            beaconDeployable = GetComponent<Deployable>();
            if (!beaconDeployable)
            {
                return;
            }

            var captainMaster = beaconDeployable.ownerMaster;
            if (!captainMaster)
            {
                return;
            }

            captainBody = captainMaster.GetBody();
            if (!captainBody)
            {
                return;
            }

            hopooGamesTurret = captainBody.GetComponent<HopooGames>();
        }

        public void OnDisable()
        {
            Fuck();
        }

        public void OnDestroy()
        {
            Fuck();
        }

        public void Fuck()
        {
            if (hopooGamesTurret && hopooGamesTurret.turretMaster)
            {
                var turretBody = hopooGamesTurret.turretMaster.GetBody();
                if (turretBody)
                {
                    var healthComponent = turretBody.healthComponent;
                    if (healthComponent)
                    {
                        healthComponent.godMode = false;
                        healthComponent.isDefaultGodMode = false;
                    }
                }

                hopooGamesTurret.turretMaster.TrueKill();
            }
        }
    }
    */
}
