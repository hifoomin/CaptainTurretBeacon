namespace CaptainTurretBeacon
{
    public class TurretFireState : EntityStates.EngiTurret.EngiTurretWeapon.FireGauss
    {
        public override void OnEnter()
        {
            tracerEffectPrefab = Prefabs.tracerPrefab;
            hitEffectPrefab = Prefabs.impactPrefab;
            effectPrefab = Prefabs.muzzleFlashPrefab;
            base.OnEnter();
            // keep stats the same actually
        }
    }
}