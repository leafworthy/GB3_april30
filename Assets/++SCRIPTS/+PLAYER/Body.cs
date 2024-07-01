using System;
using System.Collections.Generic;
using __SCRIPTS._ABILITIES;
using __SCRIPTS._COMMON;
using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS._PLAYER
{
	[Serializable]
	public class Body : ThingWithHeight
	{
		public GameObject AttackStartPoint;
		public GameObject FootPoint;
		public List<GameObject> RotateWithAim;
		public GameObject AimCenter;
		public Arms arms = new Arms();
		public Legs legs = new Legs();
		[FormerlySerializedAs("AnimScaleObject")] public GameObject BottomScaleObject;
		public GameObject AirThrowPoint;
		public GameObject ThrowPoint;
		[FormerlySerializedAs("IsFacingRight")] public bool BottomIsFacingRight;
		public bool TopIsFacingRight;
		public GameObject TopScaleObject;


		public void ChangeLayer(BodyLayer bodyLayer)
		{
			//Debug.Log("layer change" + bodyLayer.ToString());
			int layerValue = 0;
			switch (bodyLayer)
			{
				case BodyLayer.jumping:
					layerValue = ASSETS.LevelAssets.JumpingLayer;
					break;
				case BodyLayer.landed:
					layerValue = ASSETS.LevelAssets.LandedLayer;
					break;
				case BodyLayer.grounded:
					layerValue = ASSETS.LevelAssets.GroundedLayer;
					break;
			}
			FootPoint.layer = (int) Mathf.Log(layerValue, 2);
		}

		public enum BodyLayer
		{
			jumping,
			landed,
			grounded
		}

		public void BottomFaceDirection(bool faceRight)
		{
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

	
	}
}
