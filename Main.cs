using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using R2API;
using R2API.ContentManagement;
using UnityEngine;

namespace CaptainTurretBeacon
{
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(R2APIContentManager.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    [BepInDependency("Phys09.CaptainBeaconCooldown", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "HIFU";
        public const string PluginName = "CaptainTurretBeacon";
        public const string PluginVersion = "1.0.0";
        public static ManualLogSource ctbLogger;
        public static bool CaptainBeaconCooldownLoaded = false;
        public static AssetBundle bundle;
        public static Main Instance;

        public static ConfigEntry<bool> useCooldown { get; set; }
        public static ConfigEntry<float> beaconCooldown { get; set; }

        public void Awake()
        {
            Instance = this;

            ctbLogger = base.Logger;

            CaptainBeaconCooldownLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Phys09.CaptainBeaconCooldown");

            bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Instance.Info.Location), "captainturretbeacon"));

            useCooldown = Config.Bind("Behavior", "Use Cooldown?", false, "Makes the Beacon skill use a cooldown, unlike vanilla. Automatically enabled when Phys09s CaptainBeaconCooldown is installed.");
            beaconCooldown = Config.Bind("Behavior", "Cooldown Amount", 60f, "The cooldown that the Beacon skill has.");

            CallSupplyTurretSkillDef.Init();
            TurretFireSkillDef.Init();
            Prefabs.Init();

            if (CaptainBeaconCooldownLoaded || useCooldown.Value)
            {
                On.RoR2.CaptainSupplyDropController.FixedUpdate += CaptainSpaghettiCode;
            }
        }

        public float leftBeaconCooldown = 0f;
        public float rightBeaconCooldown = 0f;
        public float leftBeaconStopwatch = 0f;
        public float rightBeaconStopwatch = 0f;
        internal static string turretBeaconSkillNameToken = "CAPTAIN_SUPPLY_TURRET_NAME";

        private void CaptainSpaghettiCode(On.RoR2.CaptainSupplyDropController.orig_FixedUpdate orig, RoR2.CaptainSupplyDropController self)
        {
            self.supplyDrop1Skill.maxStock = 1 + self.supplyDrop1Skill.bonusStockFromBody;
            self.supplyDrop2Skill.maxStock = 1 + self.supplyDrop2Skill.bonusStockFromBody;

            if (self.supplyDrop1Skill.skillNameToken == turretBeaconSkillNameToken)
            {
                leftBeaconCooldown = beaconCooldown.Value;
            }

            if (self.supplyDrop1Skill.stock < self.supplyDrop1Skill.maxStock)
            {
                leftBeaconStopwatch += Time.fixedDeltaTime;
            }

            if (self.supplyDrop1Skill.stock < self.supplyDrop1Skill.maxStock)
            {
                leftBeaconStopwatch += Time.fixedDeltaTime;
            }
            if (self.supplyDrop2Skill.stock < 1)
            {
                rightBeaconStopwatch += Time.fixedDeltaTime;
            }
            if (self.supplyDrop2Skill.stock > 1)
            {
                self.supplyDrop2Skill.stock = 1;
            }
            if (leftBeaconStopwatch >= leftBeaconCooldown)
            {
                leftBeaconStopwatch = 0f;
                var supplyDrop1Skill = self.supplyDrop1Skill;
                supplyDrop1Skill.stock += 1;
            }
            if (rightBeaconStopwatch >= rightBeaconCooldown)
            {
                rightBeaconStopwatch = 0f;
                var supplyDrop2Skill = self.supplyDrop2Skill;
                supplyDrop2Skill.stock += 1;
            }
            orig(self);
        }
    }
}
