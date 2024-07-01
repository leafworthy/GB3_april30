using System.Collections.Generic;
using __SCRIPTS._ATTACKS;
using __SCRIPTS._CAMERA;
using __SCRIPTS._COMMON;
using UnityEngine;

namespace __SCRIPTS._ABILITIES
{
	public class GunAimAbility : AimAbility
	{
	
		private Animations anim;
		private bool isAttacking;

		public AnimationClip E;
		public AnimationClip EEN;
		public AnimationClip EN;
		public AnimationClip NE;
		public AnimationClip NW;
		public AnimationClip WN;
		public AnimationClip WWN;
		public AnimationClip W;
		public AnimationClip WWS;
		public AnimationClip WS;
		public AnimationClip SW;
		public AnimationClip SE;
		public AnimationClip ES;
		public AnimationClip EES;
		private List<AnimationClip> clipz = new();
		private GunAttacks gunAttacks;

		public override void Start()
		{
			base.Start();
			gunAttacks = GetComponent<GunAttacks>();
			gunAttacks.OnShoot += Gun_Shoot;
			anim = GetComponent<Animations>();
			anim.animEvents.OnAnimationComplete += Anim_OnComplete;
			clipz.AddMany(E, EEN, EN, NE, NW, WN, WWN, W, WWS, WS, SW, SE, ES, EES);
		

		}
		public override Vector3 GetAimDir()
		{
			if (owner.isUsingMouse)
			{
				return CursorManager.GetMousePosition() - body.AimCenter.transform.position;
			}
			return AimDir;
		}

		private void Anim_OnComplete()
		{
			isAttacking = false;
		}

		private void Gun_Shoot(Attack attack, Vector2 vector2)
		{
			if (attack.Owner != owner) return;
			isAttacking = true;
			anim.animator.speed = 1;
			anim.SetFloat(Animations.ShootSpeed, 1);
			anim.Play(GetAnimationClipNameFromDegrees(GetDegrees()), 1, .25f);
		}


		private float GetDegrees()
		{
			var degrees = Vector2.Angle(AimDir, Vector2.right);
			if (AimDir.y < 0) degrees -= 360;

			return degrees;
		}

	
		protected override void Update()
		{
			if(life.IsDead()) return;
			base.Update();
			if (GlobalManager.IsPaused) return;
			body.TopFaceDirection(GetClampedAimDir().x >= 0);
			if (owner.isUsingMouse)
			{
				AimDir = CursorManager.GetMousePosition() - body.AimCenter.transform.position;
			
				if (!isAttacking && !body.arms.isActive) AimInDirection(GetDegrees());
				return;
			}
			
			

		}

	

	

		private string GetAnimationClipNameFromDegrees(float degrees)
		{
			degrees = Mathf.Abs(degrees);
			degrees += 12.85f; // (-12.85 - 347.15)

			var angleDifference = 25.7f;
			var whichOne = Mathf.FloorToInt(Mathf.Abs(degrees / angleDifference));

			if (whichOne >= clipz.Count) whichOne = 0;


			return clipz[whichOne].name;
		}

		private void AimInDirection(float degrees)
		{
			degrees = Mathf.Abs(degrees);
			if (degrees > 90)
				degrees += 12.85f; // (-12.85 - 347.15)

			var angleDifference = 25.7f;
			var whichOne = Mathf.FloorToInt(Mathf.Abs(degrees / angleDifference));

			if (whichOne >= clipz.Count) whichOne = 0;

			if (clipz[whichOne] != null) anim.Play(clipz[whichOne].name, 1, 0);
			Debug.Log(clipz[whichOne].name + "clip" + " degrees:" + degrees);
			anim.SetFloat(Animations.ShootSpeed, 0);

		}





	

		private Vector2 GetClampedAimDir()
		{
			var clampedDir = AimDir.normalized;
			if (clampedDir.x <= .25f && clampedDir.x >= 0)
			{
				clampedDir.x = .25f;
				if (AimDir.y > 0)
					clampedDir.y = 1f;
				else
					clampedDir.y = -1;
			}

			if (clampedDir.x < 0 && clampedDir.x > -.25) clampedDir.x = -.25f;
			return clampedDir;
		}

	



	}
}

