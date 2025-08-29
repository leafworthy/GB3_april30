using System;
using System.Collections.Generic;
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
	public class Body : ThingWithHeight
	{
		//Manages Layer and Direction
		public GameObject AttackStartPoint;
		public GameObject FootPoint;
		public List<GameObject> RotateWithAim;
		public GameObject AimCenter;
		public GameObject ThrowPoint;
		public GameObject TopScaleObject;
		public GameObject BottomScaleObject;

		public Arms arms = new();
		public Legs legs = new();
		[HideInInspector] public bool BottomIsFacingRight = true;
		[HideInInspector] public bool TopIsFacingRight = true;
		public DoableArms doableArms = new();
		public DoableArms doableLegs = new();
		private bool canMove;
		public event Action<bool> OnCanMove;
		private ScreenDebugText screenDebugText;
		private ScreenDebugText debug => screenDebugText ??= GetComponent<ScreenDebugText>();


		public bool CanMove
		{
			get => canMove;
			private set => canMove = value;
		}

		public void SetCanMove(bool _canMove)
		{
			canMove = _canMove;
			OnCanMove?.Invoke(_canMove);
		}



		public void ChangeLayer(BodyLayer bodyLayer)
		{
			var layerValue = 0;
			switch (bodyLayer)
			{
				case BodyLayer.jumping:
					layerValue = assets.LevelAssets.JumpingLayer;
					break;
				case BodyLayer.landed:
					layerValue = assets.LevelAssets.LandedLayer;
					break;
				case BodyLayer.grounded:
					layerValue = assets.LevelAssets.GroundedLayer;
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
	}
}
