using System;
using GangstaBean.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace __SCRIPTS
{
	[ExecuteAlways]
	public class SimpleJumpAbility : HeightAbility, IRotate
	{
		public bool isResting;
		private float verticalVelocity;
		private float currentRotationRate;
		private float maxRotationRate = 360;
		private bool freezeRotation;

		public event Action<Vector2> OnLand;
		public event Action<Vector2> OnResting;
		public event Action<Vector2> OnJump;

		private bool isOverLandable;
		public bool IsJumping;
		private Vector2 DistanceToGround;
		private Body body => _body ??= GetComponent<Body>();
		private Body _body;
		private float currentLandableHeight;
		private bool isJumping;
		private bool initiated;
		private float minBounceVelocity = 1000;
		private float bounceVelocityDragFactor = .2f;
		private float landTimer;
		private float maxFlyTime = 2.5f;

		public IRotate RotateToDirection(Vector2 direction, GameObject rotationObject)
		{
			var rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

			if (rotationObject == null) return this;
			rotationObject.transform.eulerAngles = new Vector3(0, 0, rotation);
			return this;
		}

		public IRotate SetRotationRate(float newRate)
		{
			maxRotationRate = newRate;
			return this;
		}



		public IRotate SetFreezeRotation(bool freeze = true)
		{
			freezeRotation = freeze;
			maxRotationRate = 0;
			return this;
		}

		private void Rotate(float rotationSpeed)
		{
			if (freezeRotation) return;
			HeightObject.transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.fixedDeltaTime * 10));
		}

		public void Jump(float startingHeight = 0, float verticalSpeed = 2, float minBounce = 1)
		{
			currentRotationRate = Random.Range(0, maxRotationRate);
			landTimer = 0;
			minBounceVelocity = minBounce;
			IsJumping = true;
			StopResting();
			OnJump?.Invoke(transform.position+ new Vector3(0,startingHeight,0));

			verticalVelocity = verticalSpeed;

			SetHeight(startingHeight);

			if (body == null) return;
			body?.ChangeLayer(Body.BodyLayer.jumping);
		}




		protected void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;
			if (isResting) return;
			if (!IsJumping) return;
			 Rotate(currentRotationRate);
			Fly();

		}



		private void Fly()
		{
			landTimer += Time.fixedDeltaTime;
			if(landTimer> maxFlyTime)
			{
				Land();
				return;
			}
			currentLandableHeight = 0;
			verticalVelocity -= (Services.assetManager.Vars.Gravity.y) * Time.fixedDeltaTime;
			if ((GetHeight() + verticalVelocity <= currentLandableHeight) && (verticalVelocity < 0))
			{
				Land();
			}
			else
			{
				SetHeight(GetHeight() + verticalVelocity);
			}
		}


		private void Land()
		{
			Debug.Log("landed", this);
			if (Mathf.Abs(verticalVelocity) > minBounceVelocity)
			{
				Bounce();
				return;
			}

			IsJumping = false;
			OnLand?.Invoke(transform.position);
			SetHeight(currentLandableHeight);

			if (body != null) body.ChangeLayer( Body.BodyLayer.grounded);
			StartResting();
			verticalVelocity = 0;
		}

		private void Bounce()
		{
			verticalVelocity *= -1;
			var velocity = bounceVelocityDragFactor;
			verticalVelocity *= velocity;
		}

		private void StartResting()
		{
			isResting = true;
			OnResting?.Invoke(transform.position);
			if (freezeRotation) transform.rotation = Quaternion.identity;
		}

		private void StopResting()
		{
			isResting = false;
		}

		public void FreezeRotationAtIdentity()
		{
			freezeRotation = true;
			HeightObject.transform.rotation = Quaternion.identity;
		}
	}
}
