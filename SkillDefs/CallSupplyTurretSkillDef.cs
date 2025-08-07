using System;
using R2API;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CaptainTurretBeacon
{
    public class CallSupplyTurretSkillDef
    {
        public static void Init()
        {
            ContentAddition.AddEntityState(typeof(CallSupplyTurretState), out _);
            ContentAddition.AddEntityState(typeof(TurretMainState), out _);
            ContentAddition.AddEntityState(typeof(TurretFireState), out _);

            var skillDef = ScriptableObject.CreateInstance<SkillDef>();

            skillDef.skillName = "CaptainSupplyDropTurret";
            skillDef.skillNameToken = Main.turretBeaconSkillNameToken;
            skillDef.skillDescriptionToken = "CAPTAIN_SUPPLY_TURRET_DESCRIPTION";

            skillDef.icon = Main.bundle.LoadAsset<Sprite>("texCaptainTurret.png");
            skillDef.activationStateMachineName = "Weapon";
            skillDef.activationState = new(typeof(CallSupplyTurretState));
            skillDef.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;
            skillDef.baseRechargeInterval = 0;
            skillDef.baseMaxStock = 1;
            skillDef.rechargeStock = 0;
            skillDef.requiredStock = 1;
            skillDef.stockToConsume = 0;
            skillDef.attackSpeedBuffsRestockSpeed = false;
            skillDef.attackSpeedBuffsRestockSpeed_Multiplier = 1;
            skillDef.resetCooldownTimerOnUse = false;
            skillDef.fullRestockOnAssign = true;
            skillDef.dontAllowPastMaxStocks = false;
            skillDef.beginSkillCooldownOnSkillEnd = true;
            skillDef.isCooldownBlockedUntilManuallyReset = false;
            skillDef.cancelSprintingOnActivation = true;
            skillDef.forceSprintDuringState = false;
            skillDef.canceledFromSprinting = true;
            skillDef.isCombatSkill = true;
            skillDef.mustKeyPress = true;
            skillDef.triggeredByPressRelease = false;
            skillDef.autoHandleLuminousShot = true;
            skillDef.hideStockCount = false;

            LanguageAPI.Add("CAPTAIN_SUPPLY_TURRET_NAME", "Beacon: Turret");
            LanguageAPI.Add("CAPTAIN_SUPPLY_TURRET_DESCRIPTION", "<style=cIsUtility>Request</style> a turret that <style=cIsUtility>inherits all your items</style> and attacks all nearby enemies for <style=cIsDamage>200% damage per second</style>.");
            LanguageAPI.Add("CAPTAIN_TURRET_NAME", "Captain Turret");
            ContentAddition.AddSkillDef(skillDef);

            var firstSlotMiscFamily = Addressables.LoadAssetAsync<SkillFamily>("d72f3891fdf9e744d93ed1df83c1098a").WaitForCompletion();
            var secondSlotMiscFamily = Addressables.LoadAssetAsync<SkillFamily>("6e49127cdcc369c4da9be6fb00913307").WaitForCompletion();

            Array.Resize(ref firstSlotMiscFamily.variants, firstSlotMiscFamily.variants.Length + 1);
            firstSlotMiscFamily.variants[firstSlotMiscFamily.variants.Length - 1].skillDef = skillDef;

            Array.Resize(ref secondSlotMiscFamily.variants, secondSlotMiscFamily.variants.Length + 1);
            secondSlotMiscFamily.variants[secondSlotMiscFamily.variants.Length - 1].skillDef = skillDef;
        }
    }
}