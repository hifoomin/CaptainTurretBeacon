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
        public override Interactability GetInteractability(Interactor activator)
        {
            return Interactability.Disabled;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            var genericOwnership = outer.GetComponent<GenericOwnership>();
            var finalPosition = interactionComponent.transform.position + new Vector3(0f, heightOffset, 0f);
            if (genericOwnership)
            {
                var owner = genericOwnership.ownerObject;
                if (owner)
                {
                    var ownerBody = owner.GetComponent<CharacterBody>();
                    if (ownerBody)
                    {
                        ownerBody.SendConstructTurret(ownerBody, finalPosition, interactionComponent.transform.rotation, MasterCatalog.FindMasterIndex(Prefabs.turretMasterPrefab));
                        enableRadiusIndicator = true;
                    }
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
}