using R2API;
using RoR2.Skills;
using UnityEngine;

namespace CaptainTurretBeacon
{
    public class TurretFireSkillDef
    {
        public static SkillDef skillDef;
        public static void Init()
        {
            skillDef = ScriptableObject.CreateInstance<SkillDef>();
            skillDef.skillName = "CaptainTurretFire";
            skillDef.skillNameToken = "CAPTAIN_SUPPLY_TURRET_FIRE_NAME";
            skillDef.skillDescriptionToken = "CAPTAIN_SUPPLY_TURRET_FIRE_DESCRIPTION";

            skillDef.icon = null;
            skillDef.activationStateMachineName = "Weapon";
            skillDef.activationState = new(typeof(TurretFireState));
            skillDef.interruptPriority = EntityStates.InterruptPriority.Any;
            skillDef.baseRechargeInterval = 0;
            skillDef.baseMaxStock = 1;
            skillDef.rechargeStock = 1;
            skillDef.requiredStock = 1;
            skillDef.stockToConsume = 1;
            skillDef.attackSpeedBuffsRestockSpeed = false;
            skillDef.attackSpeedBuffsRestockSpeed_Multiplier = 1;
            skillDef.resetCooldownTimerOnUse = false;
            skillDef.fullRestockOnAssign = true;
            skillDef.dontAllowPastMaxStocks = false;
            skillDef.beginSkillCooldownOnSkillEnd = false;
            skillDef.isCooldownBlockedUntilManuallyReset = false;
            skillDef.cancelSprintingOnActivation = true;
            skillDef.forceSprintDuringState = false;
            skillDef.canceledFromSprinting = false;
            skillDef.isCombatSkill = true;
            skillDef.mustKeyPress = false;
            skillDef.triggeredByPressRelease = false;
            skillDef.autoHandleLuminousShot = true;
            skillDef.hideStockCount = false;

            ContentAddition.AddSkillDef(skillDef);
        }
    }
}