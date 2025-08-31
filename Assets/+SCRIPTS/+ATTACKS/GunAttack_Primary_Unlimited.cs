using System;
using UnityEngine;

namespace __SCRIPTS
{



	public interface IAimableGunAttack
	{
		event Action<Attack, Vector2> OnShotHitTarget;
		event Action<Attack, Vector2> OnShotMissed;
		bool IsUsingPrimaryGun { get;  }
		Vector2 AimDir { get; }
	}
	[Serializable]
	public class GunAttack_Primary_Unlimited : Ability, IAimableGunAttack
	{
		public event Action<Attack, Vector2> OnShotHitTarget;
		public event Action<Attack, Vector2> OnShotMissed;
		public event Action OnNeedReload;
		public event Action OnEmpty;
		public bool IsUsingPrimaryGun
		{
			get => isUsingPrimaryGun;
			set => isUsingPrimaryGun = value;
		}
		private bool isUsingPrimaryGun = true;
		public bool isPressingShoot;

		private Gun currentGun => IsUsingPrimaryGun ? primaryGun : unlimitedGun;

		private float currentCooldownTime;
		private PrimaryGun primaryGun => _primaryGun ??= GetComponent<PrimaryGun>();
		private PrimaryGun _primaryGun;
		private UnlimitedGun unlimitedGun => _unlimitedGun ??= GetComponent<UnlimitedGun>();
		private UnlimitedGun _unlimitedGun;
		private IAimAbility gunAimAbility => _gunAimAbility ??= GetComponent<IAimAbility>();
		private IAimAbility _gunAimAbility;
		public override string VerbName => "Shooting";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		private float attackRate => currentGun.AttackRate;

		private float damage => currentGun.Damage;
		public Vector2 AimDir => gunAimAbility.AimDir;

		public Ammo GetCorrectAmmoType() => currentGun.Ammo;



		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			primaryGun.OnEmpty += Gun_OnEmpty;
			 primaryGun.OnNeedReload += Gun_NeedReload;
			 primaryGun.OnShotHitTarget += Gun_ShotHitTarget;
			 primaryGun.OnShotMissed += Gun_ShotMissed;
			unlimitedGun.OnEmpty += Gun_OnEmpty;
			unlimitedGun.OnNeedReload += () => OnNeedReload?.Invoke();
			unlimitedGun.OnShotHitTarget += Gun_ShotHitTarget;

			ListenToPlayer();
		}

		private void Gun_ShotMissed(Attack attack, Vector2 hitPoint)
		{ OnShotMissed?.Invoke(attack, hitPoint);
		}

		private void Gun_ShotHitTarget(Attack attack, Vector2 hitPoint)
		{
			OnShotHitTarget?.Invoke(attack, hitPoint);
		}

		private void Gun_NeedReload()
		{
			OnNeedReload?.Invoke();
		}

		private void Gun_OnEmpty()
		{
			 OnEmpty?.Invoke();
		}

		private void OnDisable()
		{
			StopListeningToPlayer();
		}

		public override bool canDo() => base.canDo() && currentGun.CanShoot();

		protected override void DoAbility()
		{
			Debug.Log("shoot with cooldown");
			ShootWithCooldown(gunAimAbility.AimDir);
		}

		private void OnDestroy()
		{
			StopListeningToPlayer();
		}

		private void ListenToPlayer()
		{
			player.Controller.Attack1RightTrigger.OnPress += PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease += PlayerControllerShootRelease;
		}

		private void StopListeningToPlayer()
		{
			player.Controller.Attack1RightTrigger.OnPress -= PlayerControllerShootPress;
			player.Controller.Attack1RightTrigger.OnRelease -= PlayerControllerShootRelease;
		}

		private void FixedUpdate()
		{
			if (isPressingShoot) Do();
		}

		private void PlayerControllerShootPress(NewControlButton newControlButton)
		{
			if (canDo()) isPressingShoot = true;
			Do();
		}

		private void PlayerControllerShootRelease(NewControlButton newControlButton)
		{
			isPressingShoot = false;
		}

		private void ShootTarget(Vector3 targetPosition)
		{
			Debug.Log("shoot in ability");
			currentCooldownTime = Time.time + attackRate;
			currentGun.Shoot(targetPosition);
		}



		private void ShootWithCooldown(Vector3 targetDir)
		{
			if (!(Time.time >= currentCooldownTime)) return;

			if (!GetCorrectAmmoType().hasAmmoInReserveOrClip())
			{
				Debug.Log("[GUN] empty");
				Stop();
				OnEmpty?.Invoke();
				return;
			}

			if (!GetCorrectAmmoType().hasAmmoInClip())
			{
				Debug.Log("[GUN] need reload");
				Stop();
				OnNeedReload?.Invoke();
				return;
			}

			GetCorrectAmmoType().UseAmmo(1);
			Debug.Log("[GUN] shoot target");
			ShootTarget(targetDir);
			PlayAnimationClip(null, 1, .25f);//TODO adjust speed based on attack rate
		}
	}
}
