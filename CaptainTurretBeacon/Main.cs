using System.Runtime.CompilerServices;
using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using MonoMod.RuntimeDetour;
using R2API;
using R2API.ContentManagement;
using UnityEngine;
using HarmonyLib;
using System.ComponentModel;

namespace CaptainTurretBeacon
{
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(R2APIContentManager.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
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

        public static ConfigEntry<float> beaconCooldown { get; set; }
        public Hook hook;
        public bool hooked = false;

        public void Awake()
        {
            Instance = this;

            ctbLogger = Logger;

            bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Instance.Info.Location), "captainturretbeacon"));

            beaconCooldown = Config.Bind("Behavior", "Cooldown Amount", 60f, "Enabled when Phys09s Captain Beacon Cooldown and Tweaks is installed. The cooldown that the Beacon skill has.");

            CallSupplyTurretSkillDef.Init();
            TurretFireSkillDef.Init();
            Prefabs.Init();

            AddCooldownToBeacon();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public void AddCooldownToBeacon()
        {
            try
            {
                hook = new Hook(
                typeof(CaptainBeaconCooldown.CaptainBeaconCooldown).GetMethod(nameof(CaptainBeaconCooldown.CaptainBeaconCooldown.Awake), ~BindingFlags.Default),
                typeof(Main).GetMethod(nameof(AddCooldownsHook), ~BindingFlags.Default));
            }
            catch { }
        }

        private static void AddCooldownsHook(Action<CaptainBeaconCooldown.CaptainBeaconCooldown> orig, CaptainBeaconCooldown.CaptainBeaconCooldown self)
        {
            orig(self);

            // ctbLogger.LogError("beacon name tokens array length before changes: " + self.beaconNameTokens);
            Array.Resize(ref self.beaconNameTokens, self.beaconNameTokens.Length + 1);
            self.beaconNameTokens[self.beaconNameTokens.Length - 1] = "CAPTAIN_SUPPLY_TURRET_NAME";
            // ctbLogger.LogError("beacon name tokens array length AFTERRR changes: " + self.beaconNameTokens);
            // ctbLogger.LogError("beacon name tokens array last slot: " + self.beaconNameTokens[self.beaconNameTokens.Length - 1]);

            // ctbLogger.LogError("beacon COOLDOWNS array length before changes: " + self.beaconCooldowns);
            Array.Resize(ref self.beaconCooldowns, self.beaconCooldowns.Length + 1);
            self.beaconCooldowns[self.beaconCooldowns.Length - 1] = beaconCooldown.Value;
            // ctbLogger.LogError("beacon COOLDOWNS array length ADFFETTERRR changes: " + self.beaconCooldowns);
            // ctbLogger.LogError("beacon COOLDOWNS tokens array last slot: " + self.beaconCooldowns[self.beaconCooldowns.Length - 1]);
        }
    }
}
