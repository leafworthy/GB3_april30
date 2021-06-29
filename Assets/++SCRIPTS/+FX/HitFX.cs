using UnityEngine;


public class HitFX : MonoBehaviour {

    private Color hurtTintColor = Color.red;
    private Color poisonTintColor = Color.green;
    private DefenceHandler defence;
    private TintHandler tintHandler;

    private void Start()
    {
        tintHandler = GetComponent<TintHandler>();
        defence = GetComponent<DefenceHandler>();
        defence.OnDamaged += Damaged;
    }

    private void Damaged(Attack attack)
    {
        tintHandler.StartTint(attack.IsPoison ? poisonTintColor : hurtTintColor);
        SHAKER.ShakeCamera(transform.position, GetShakeIntensityFromDamage(attack));
        HITSTUN.StartStun(attack.Stunlength);
    }

    private static SHAKER.ShakeIntensityType GetShakeIntensityFromDamage(Attack attack)
    {
        var shakeIntensityType = SHAKER.ShakeIntensityType.low;
        if (attack.DamageAmount > 10)
        {
            shakeIntensityType = SHAKER.ShakeIntensityType.normal;
        }

        if (attack.DamageAmount > 30)
        {
            shakeIntensityType = SHAKER.ShakeIntensityType.high;
        }

        return shakeIntensityType;
    }
}
