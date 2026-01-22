using System;
using System.Collections.Generic;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class DoableArms : DoableBodyPart
	{

	}

	public class DoableLegs : DoableBodyPart
	{
	}

	[Serializable]
	public class Body : HeightAbility
	{
		public GameObject AttackStartPoint;
		public GameObject FootPoint;
		public List<GameObject> RotateWithAim;
		public GameObject AimCenter;
		public GameObject ThrowPoint;
		public GameObject TopScaleObject;
		public GameObject BottomScaleObject;

		[HideInInspector] public bool BottomIsFacingRight = true;
		[HideInInspector] public bool TopIsFacingRight = true;
		public DoableArms doableArms = new();
		public DoableLegs doableLegs = new();
		public bool isGrounded  { get; private set; }

		#region LayerChanging

		public void ChangeLayer(BodyLayer bodyLayer)
		{
			var layerValue = 0;
			switch (bodyLayer)
			{
				case BodyLayer.jumping:
					layerValue = Services.assetManager.LevelAssets.JumpingLayer;
					break;
				case BodyLayer.landed:
					layerValue = Services.assetManager.LevelAssets.LandedLayer;
					break;
				case BodyLayer.grounded:
					layerValue = Services.assetManager.LevelAssets.GroundedLayer;
					break;
				case BodyLayer.enemy:
					layerValue = Services.assetManager.LevelAssets.EnemyLayer;
					break;
			}

			FootPoint.layer = (int) Mathf.Log(layerValue, 2);
		}
		public void SetGrounded(bool grounded)
		{
			isGrounded = grounded;
			SetHeight(0);
			ChangeLayer(grounded ? BodyLayer.grounded : Body.BodyLayer.jumping);
		}

		public enum BodyLayer
		{
			jumping,
			landed,
			grounded,
			enemy
		}

		#endregion

		#region FaceDirection

		public void BottomFaceDirection(bool faceRight)
		{
			if (this == null || gameObject == null) return;
			if (BottomIsFacingRight == faceRight) return;
			BottomIsFacingRight = faceRight;
			FlipBottom(faceRight);
		}

		public void TopFaceDirection(bool faceRight)
		{
			if (TopIsFacingRight == faceRight) return;
			if (TopScaleObject == null) return;
			TopIsFacingRight = faceRight;
			FlipTop(faceRight);
		}

		private void FlipBottom(bool toTheRight)
		{

			var localScale = BottomScaleObject.transform.localScale;
			var currentScale = localScale;

			currentScale.x = toTheRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
			localScale = currentScale;
			BottomScaleObject.transform.localScale = localScale;
		}

		private void FlipTop(bool toTheRight)
		{
			var localScale = TopScaleObject.transform.localScale;
			var currentScale = localScale;

			currentScale.x = toTheRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
			localScale = currentScale;
			TopScaleObject.transform.localScale = localScale;
		}

		#endregion

		public bool CanDoAbility(Ability abilityToDo)
		{
			if (abilityToDo.requiresArms() && !doableArms.CanDoActivity(abilityToDo)) return false;
			if (abilityToDo.requiresLegs() && !doableLegs.CanDoActivity(abilityToDo)) return false;
			return true;
		}

		public void DoAbility(Ability ability)
		{
			if (ability.requiresArms()) doableArms.DoAbility(ability);
			if (ability.requiresLegs()) doableLegs.DoAbility(ability);
		}
	}
}
