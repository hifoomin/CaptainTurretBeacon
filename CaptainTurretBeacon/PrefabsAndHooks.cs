using RoR2.Skills;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using On.EntityStates.Bandit2.Weapon;
using System;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace CaptainTurretBeacon
{
    public static class Prefabs
    {
        public static GameObject beaconPrefab;
        public static GameObject turretBodyPrefab;
        public static GameObject turretMasterPrefab;

        public static GameObject indicatorPrefab;
        public static GameObject tracerPrefab;
        public static GameObject muzzleFlashPrefab;
        public static GameObject impactPrefab;

        public static BodyIndex captainBodyIndex;
        public static void Init()
        {
            var captainMasterPrefab = Addressables.LoadAssetAsync<GameObject>("2e38a50898e5bc249b2c13f15d9825ca").WaitForCompletion();
            captainMasterPrefab.AddComponent<HopooGames>();
            // guid is captain body

            beaconPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("7eaa9cf9fa18c374ab0c0acc09db93a2").WaitForCompletion(), "Captain Turret Beacon", true);
            // guid is captain supply drop, healing
            var genericDisplayNameProvider = beaconPrefab.GetComponent<GenericDisplayNameProvider>();
            genericDisplayNameProvider.displayToken = "CAPTAIN_SUPPLY_TURRET_NAME";

            var entityStateMachine = beaconPrefab.GetComponent<EntityStateMachine>();
            entityStateMachine.mainStateType = new(typeof(TurretMainState));

            var beaconMr = beaconPrefab.GetComponent<ModelLocator>()._modelTransform.Find("CaptainSupplyDropMesh").GetComponent<SkinnedMeshRenderer>();

            var newBeaconMaterial = new Material(Addressables.LoadAssetAsync<Material>("5be601672a643584a8bf78a2ca56c12b").WaitForCompletion());
            newBeaconMaterial.SetTexture("_MainTex", Main.bundle.LoadAsset<Texture2D>("texTurretBeaconDiffuse.png"));
            newBeaconMaterial.SetColor("_EmColor", Color.white);
            newBeaconMaterial.SetFloat("_EmPower", 5.5f);
            // guid is mat captain supply drop healing

            beaconMr.material = newBeaconMaterial;

            // beaconPrefab.AddComponent<StupidFuck>();

            PrefabAPI.RegisterNetworkPrefab(beaconPrefab);
            ContentAddition.AddNetworkedObject(beaconPrefab);

            turretBodyPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("04508f474b420d546b5d55a9a18a9698").WaitForCompletion(), "Captain Turret Body", true);
            // guid is stationary engi turret body

            var characterBody = turretBodyPrefab.GetComponent<CharacterBody>();
            characterBody.baseNameToken = "CAPTAIN_TURRET_NAME";
            characterBody.baseDamage = 8f; // base is 16
            characterBody.levelDamage = 1.6f; // base is 3.2

            var healthComponent = turretBodyPrefab.GetComponent<HealthComponent>();
            healthComponent.isDefaultGodMode = true;
            healthComponent.godMode = true;

            var genericSkill = turretBodyPrefab.GetComponent<GenericSkill>();

            var newPrimarySkillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newPrimarySkillFamily.variants = new SkillFamily.Variant[1];
            newPrimarySkillFamily.defaultVariantIndex = 0;
            newPrimarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = TurretFireSkillDef.skillDef,
                unlockableDef = null
            };

            ContentAddition.AddSkillFamily(newPrimarySkillFamily);

            genericSkill._skillFamily = newPrimarySkillFamily;

            characterBody.portraitIcon = Main.bundle.LoadAsset<Texture2D>("texCaptainTurretPortait.png");

            var redRamp = Addressables.LoadAssetAsync<Texture2D>("d67c887632cd1704ebe3a19486dbe843").WaitForCompletion();
            // guid is 
            var newTurretMaterial = new Material(Addressables.LoadAssetAsync<Material>("a9b91ae3f60a5d14a97ec97e5991dc57").WaitForCompletion());
            newTurretMaterial.SetTexture("_MainTex", Main.bundle.LoadAsset<Texture2D>("texCaptainTurretDiffuse.png"));
            newTurretMaterial.SetTexture("_PrintRamp", redRamp);
            newTurretMaterial.SetColor("_EmColor", Color.white);
            newTurretMaterial.SetFloat("_EmPower", 5.5f);

            // guid is mat engi turret

            var modelTransform = turretBodyPrefab.GetComponent<ModelLocator>()._modelTransform;

            // var engiTurretMesh = Addressables.LoadAssetAsync<Mesh>("RoR2/Base/Engi/mdlEngiTurret.fbx").WaitForCompletion();
            // var engiTurretMesh = Addressables.LoadAssetAsync<Mesh>("RoR2/Base/Engi/EngiTurret/EngiTurretMesh.asset").WaitForCompletion();
            var engiTurretMesh = Addressables.LoadAssetAsync<Mesh>("bc52bb41b17826042a15ba20e4447f32").WaitForCompletion();
            // guid is mdl engi turret

            var smr = modelTransform.Find("EngiTurretMesh").GetComponent<SkinnedMeshRenderer>();
            smr.material = newTurretMaterial;
            // smr.sharedMesh = engiTurretMesh;

            var newSkinDefParams = ScriptableObject.CreateInstance<SkinDefParams>();
            newSkinDefParams.rendererInfos = new CharacterModel.RendererInfo[1];
            newSkinDefParams.rendererInfos[0] = new CharacterModel.RendererInfo
            {
                renderer = smr,
                defaultMaterial = newTurretMaterial,
                defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                hideOnDeath = false,
                ignoreOverlays = false,
                ignoresMaterialOverrides = false
            };
            newSkinDefParams.meshReplacements = new SkinDefParams.MeshReplacement[1];
            newSkinDefParams.meshReplacements[0] = new SkinDefParams.MeshReplacement
            {
                mesh = engiTurretMesh,
                renderer = smr
            };

            var newSkinDef = ScriptableObject.CreateInstance<SkinDef>();
            newSkinDef.rootObject = modelTransform.gameObject;
            newSkinDef.skinDefParams = newSkinDefParams;

            var modelSkinController = modelTransform.GetComponent<ModelSkinController>();
            modelSkinController.skins[0] = newSkinDef;

            ContentAddition.AddBody(turretBodyPrefab);

            turretMasterPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("ab6d27c997dd03546bcb6f3176059eba").WaitForCompletion(), "Captain Turret Master", true);
            // guid is stationary engi turret master

            var characterMaster = turretMasterPrefab.GetComponent<CharacterMaster>();
            characterMaster.bodyPrefab = turretBodyPrefab;

            ContentAddition.AddMaster(turretMasterPrefab);

            SetUpVFX();

            On.RoR2.BodyCatalog.Init += OnBodyCatalogInit;
            // CharacterBody.onBodyStartGlobal += OnBodyStart;
            // CharacterMaster.onStartGlobal += OnMasterStart;
        }

        private static IEnumerator OnBodyCatalogInit(On.RoR2.BodyCatalog.orig_Init orig)
        {
            yield return orig();
            captainBodyIndex = BodyCatalog.FindBodyIndex("CaptainBody(Clone)");
            // Main.ctbLogger.LogError("captain body index is " + captainBodyIndex);
        }

        public static void SetUpVFX()
        {
            indicatorPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("5ba295c0a3919a544939e6efe1ff17b3").WaitForCompletion(), "Captain Turret Range Indicator VFX", true);
            // guid is nearby damage bonus indicator
            var indicatorTransform = indicatorPrefab.transform.Find("Radius, Spherical");
            indicatorTransform.localScale = Vector3.one * 60f * 2f;

            var newIndicatorMaterial = new Material(Addressables.LoadAssetAsync<Material>("efcdb7ab1fe128a4eb2d79a8024c25bd").WaitForCompletion());
            // guid is mat nearby damage bonus range indicator
            var cloudTexture = Addressables.LoadAssetAsync<Texture2D>("cd8abd51143aa5140aded186614aa040").WaitForCompletion();
            // guid is perlin noise
            newIndicatorMaterial.SetTexture("_MainTex", cloudTexture);
            newIndicatorMaterial.SetTexture("_Cloud1Tex", cloudTexture);
            newIndicatorMaterial.SetColor("_TintColor", new Color32(255, 0, 0, 128));

            indicatorTransform.GetComponent<MeshRenderer>().material = newIndicatorMaterial;

            PrefabAPI.RegisterNetworkPrefab(indicatorPrefab);

            tracerPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("561b80f2f7f29ba4cb234df1444df815").WaitForCompletion(), "Captain Turret Tracer VFX", false);

            var lineRenderer = tracerPrefab.GetComponent<LineRenderer>();

            var lineGradient = new Gradient();

            var lineColors = new GradientColorKey[2];
            lineColors[0] = new GradientColorKey(Color.red, 0f);
            lineColors[1] = new GradientColorKey(Color.red, 1f);

            var lineAlphas = new GradientAlphaKey[2];
            lineAlphas[0] = new GradientAlphaKey(1f, 0f);
            lineAlphas[1] = new GradientAlphaKey(1f, 1f);

            lineGradient.SetKeys(lineColors, lineAlphas);

            lineRenderer.colorGradient = lineGradient;

            var redRamp = Addressables.LoadAssetAsync<Texture2D>("d67c887632cd1704ebe3a19486dbe843").WaitForCompletion();
            // guid is tex ramp golem

            var newTracerMaterial = new Material(Addressables.LoadAssetAsync<Material>("7fa66dfdd452dab48af0fbe1993f04d6").WaitForCompletion());
            newTracerMaterial.SetTexture("_RemapTex", redRamp);
            lineRenderer.material = newTracerMaterial;
            // guid is mat engi tracer

            ContentAddition.AddEffect(tracerPrefab);
            // guid is tracer engi turret

            muzzleFlashPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("1a8f4ee7d7e360d42ba0dca68ff828c6").WaitForCompletion(), "Captain Turret Muzzle Flash VFX", false);

            var muzzleFlashTransform = muzzleFlashPrefab.transform;

            var muzzleFlashRing = muzzleFlashTransform.Find("Ring").GetComponent<ParticleSystemRenderer>();

            var newRingMaterial = new Material(Addressables.LoadAssetAsync<Material>("67a2cf616ad4b42479cd8bdb2b0b0ca2").WaitForCompletion());
            newRingMaterial.SetTexture("_RemapTex", redRamp);
            // guid is mat engi trail

            muzzleFlashRing.material = newRingMaterial;

            var muzzleFlashFlames = muzzleFlashTransform.Find("Flames").GetComponent<ParticleSystem>().colorOverLifetime;

            var flamesGradient = new Gradient();

            var flameColors = new GradientColorKey[3];
            flameColors[0] = new GradientColorKey(new Color32(255, 103, 100, 255), 0.053f);
            flameColors[1] = new GradientColorKey(new Color32(233, 0, 13, 255), 0.309f);
            flameColors[2] = new GradientColorKey(new Color32(233, 0, 26, 255), 1f);

            var flameAlphas = new GradientAlphaKey[3];
            flameAlphas[0] = new GradientAlphaKey(0f, 0f);
            flameAlphas[0] = new GradientAlphaKey(1f, 0.141f);
            flameAlphas[0] = new GradientAlphaKey(0f, 1f);

            flamesGradient.SetKeys(flameColors, flameAlphas);

            muzzleFlashFlames.color = lineGradient;

            var muzzleFlashFlash = muzzleFlashTransform.Find("Flash").GetComponent<ParticleSystem>().colorOverLifetime;

            var flashGradient = new Gradient();

            var flashAlphas = new GradientAlphaKey[2];
            flashAlphas[0] = new GradientAlphaKey(0.271f, 0f);
            flashAlphas[1] = new GradientAlphaKey(0f, 1f);

            flashGradient.SetKeys(lineColors, flashAlphas);

            muzzleFlashFlash.color = flashGradient;

            var pointLight = muzzleFlashTransform.Find("Point light").GetComponent<Light>();
            pointLight.color = Color.red;

            ContentAddition.AddEffect(muzzleFlashPrefab);

            impactPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("5a6d604670c05594a83b3727683e711d").WaitForCompletion(), "Captain Turret Impact VFX", false);

            var impactTransform = impactPrefab.transform;

            var impactFlash = impactTransform.Find("Flash").GetComponent<ParticleSystem>().colorOverLifetime;
            impactFlash.color = flashGradient;

            var impactFlames = impactTransform.Find("Flames").GetComponent<ParticleSystem>().colorOverLifetime;
            impactFlames.color = flamesGradient;

            var impactRing = impactTransform.Find("Ring");

            var impactRingPSR = impactRing.GetComponent<ParticleSystemRenderer>();
            impactRingPSR.material = newRingMaterial;

            var impactRingPS = impactRing.GetComponent<ParticleSystem>().colorOverLifetime;

            var impactRingGradient = new Gradient();

            var impactRingColors = new GradientColorKey[2];
            impactRingColors[0] = new GradientColorKey(new Color32(202, 0, 35, 255), 0f);
            impactRingColors[0] = new GradientColorKey(Color.red, 0.585f);

            var impactRingAlphas = new GradientAlphaKey[3];
            impactRingAlphas[0] = new GradientAlphaKey(1f, 0f);
            impactRingAlphas[1] = new GradientAlphaKey(1f, 0.479f);
            impactRingAlphas[1] = new GradientAlphaKey(0.082f, 1);

            impactRingGradient.SetKeys(impactRingColors, impactRingAlphas);

            impactRingPS.color = impactRingGradient;

            // guid is impact engi turret
            ContentAddition.AddEffect(impactPrefab);
        }
    }
}