using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class AnimationEvents : MonoBehaviour
	{
		public event Action OnRecovered;

		public event Action<int> OnAttackHit;

		public event Action OnHitStart;
		public event Action OnDieStart;
		public event Action<bool> OnDieStop;
		public event Action OnDash;
		public event Action OnReload;
		public event Action OnRoar;
		public event Action OnTeleport;
		public event Action<bool> OnInvincible;
		public event Action OnStep;

		public void MoveStart()
		{
		}

		public void MoveStop()
		{
		}

		public void Recovered()
		{
			OnRecovered?.Invoke();
		}

		public void AnimationComplete()
		{
		}

		public void AfterHit()
		{
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
		}

		public void Reload()
		{
			OnReload?.Invoke();
		}

		public void ReloadStop()
		{
		}

		public void StartUsingLegs()
		{
		}

		public void StopUsingLegs()
		{
		}

		public void DashStart()
		{
		}

		public void Dash()
		{
			OnDash?.Invoke();
		}

		public void DashStop()
		{
		}

		public void ThrowStart()
		{
		}

		public void Throw()
		{
		}

		public void ThrowStop()
		{
		}

		public void AirThrowStart()
		{
		}

		public void AirThrow()
		{
		}

		public void AirThrowStop()
		{
		}

		public void DieStart()
		{
			OnDieStart?.Invoke();
		}

		public void DieStop()
		{
			OnDieStop?.Invoke(false);
		}

		public void HitStart()
		{
			OnHitStart?.Invoke();
		}

		public void HitStop()
		{
		}

		public void AttackStart(int attackType = 1)
		{
		}

		public void AttackStop(int attackType = 1)
		{
		}

		public void AttackHit(int attackType = 1)
		{
			OnAttackHit?.Invoke(attackType);
		}

		public void LandingStop()
		{
		}

		public void LandingStart()
		{
		}

		public void InvincibleStart()
		{
			OnInvincible?.Invoke(true);
		}

		public void InvincibleStop()
		{
			OnInvincible?.Invoke(false);
		}

		public void FullyCharged()
		{
		}

	}
}
