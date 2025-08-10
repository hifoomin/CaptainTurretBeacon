using EntityStates;
using RoR2;
using UnityEngine;

namespace CaptainTurretBeacon
{
    public class TurretFireState : BaseState
    {
        public static GameObject effectPrefab = Prefabs.muzzleFlashPrefab;

        public static GameObject hitEffectPrefab = Prefabs.impactPrefab;

        public static GameObject tracerEffectPrefab = Prefabs.tracerPrefab;

        public static string attackSoundString = "Play_engi_R_turret_shot";

        public static float damageCoefficient = 0.7f;

        public static float force = 200f;

        public static float minSpread = 0f;

        public static float maxSpread = 1.5f;

        public static float baseDuration = 0.35f;

        private float duration;

        private static int stateHash = Animator.StringToHash("FireGauss");

        private static int paramHash = Animator.StringToHash("FireGauss.playbackRate");

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / attackSpeedStat;

            Util.PlaySound(attackSoundString, gameObject);

            Ray aimRay = GetAimRay();
            StartAimMode(aimRay);

            PlayAnimation("Gesture", stateHash, paramHash, duration);

            string muzzleName = "Muzzle";

            EffectManager.SimpleMuzzleFlash(effectPrefab, gameObject, muzzleName, transmit: false);

            if (isAuthority)
            {
                BulletAttack bulletAttack = new()
                {
                    owner = gameObject,
                    weapon = gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = minSpread,
                    maxSpread = maxSpread,
                    bulletCount = 1u,
                    damage = damageCoefficient * damageStat,
                    force = force,
                    tracerEffectPrefab = tracerEffectPrefab,
                    muzzleName = muzzleName,
                    hitEffectPrefab = hitEffectPrefab,
                    isCrit = Util.CheckRoll(critStat, characterBody.master),
                    HitEffectNormal = false,
                    radius = 0.15f
                };
                bulletAttack.damageType.damageSource = DamageSource.Primary;
                bulletAttack.Fire();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}