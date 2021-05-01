using System;
using UnityEngine;

namespace _SCRIPTS
{
    public class AnimationEvents : MonoBehaviour
    {
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
    }
}
