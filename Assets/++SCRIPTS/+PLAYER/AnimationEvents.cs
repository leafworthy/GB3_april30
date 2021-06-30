using System;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public event Action OnAfterHit;
    public event Action<int> OnAttackStart;
    public event Action<int> OnAttackStop;
    public event Action OnLandingStart;
    public event Action OnLandingStop;
    public event Action<int> OnAttackHit;
    public event Action OnHitStart;
    public event Action OnHitStop;
    public event Action OnDieStart;
    public event Action OnDieStop;
    public event Action OnDashStart;
    public event Action OnDash;
    public event Action OnDashStop;
    public event Action OnThrowStart;
    public event Action OnThrow;
    public event Action OnThrowStop;
    public event Action OnAirThrowStart;
    public event Action OnAirThrow;
    public event Action OnAirThrowStop;
    public event Action OnMoveStart;
    public event Action OnMoveStop;
    public event Action OnReloadStart;
    public event Action OnReload;
    public event Action OnReloadStop;
    public event Action OnStep;
    public event Action OnShoot;
    public event Action OnRoar;
    public event Action OnTeleport;
    public event Action<bool> OnInvincible;

    public void AfterHit()
    {
        OnAfterHit?.Invoke();
    }
    public void Teleport()
    {
        OnTeleport?.Invoke();
    }
    public void Roar()
    {
        OnRoar?.Invoke();
    }
    public void Step()
    {
        OnStep?.Invoke();
    }
    public void ReloadStart()
    {
        OnReloadStart?.Invoke();
    }

    public void Reload()
    {
        OnReload?.Invoke();
    }

    public void ReloadStop()
    {
        OnReloadStop?.Invoke();
    }


    public void MoveStart()
    {
        OnMoveStart?.Invoke();
    }

    public void MoveStop()
    {
        OnMoveStop?.Invoke();
    }
    public void DashStart()
    {
        OnDashStart?.Invoke();
    }

    public void Dash()
    {
        OnDash?.Invoke();
    }

    public void DashStop()
    {

        OnDashStop?.Invoke();
    }

    public void ThrowStart()
    {
        OnThrowStart?.Invoke();
    }

    public void Throw()
    {
        OnThrow?.Invoke();
    }

    public void ThrowStop()
    {

        OnThrowStop?.Invoke();
    }

    public void AirThrowStart()
    {
        OnAirThrowStart?.Invoke();
    }

    public void AirThrow()
    {
        OnAirThrow?.Invoke();
    }

    public void AirThrowStop()
    {

        OnAirThrowStop?.Invoke();
    }

    public void DieStart()
    {
        OnDieStart?.Invoke();
    }

    public void DieStop()
    {
        OnDieStop?.Invoke();
    }

    public void HitStart()
    {
        OnHitStart?.Invoke();
    }

    public void HitStop()
    {
        OnHitStop?.Invoke();
    }

    public void AttackStart(int attackType = 1)
    {
        OnAttackStart?.Invoke(attackType);
    }
    public void AttackStop(int attackType = 1)
    {
        OnAttackStop?.Invoke(attackType);
    }

    public void AttackHit(int attackType = 1)
    {
        OnAttackHit?.Invoke(attackType);
    }

    public void LandingStop()
    {
        OnLandingStop?.Invoke();
    }

    public void LandingStart()
    {
        OnLandingStart?.Invoke();
    }

    public void InvincibleStart()
    {
        OnInvincible?.Invoke(true);
    }

    public void InvincibleStop()
    {
        OnInvincible?.Invoke(false);
    }

}
