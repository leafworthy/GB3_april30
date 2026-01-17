using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	[ExecuteAlways]
	public class JumpAndRotateAbility : HeightAbility
	{
		public bool isResting;
		float verticalVelocity;
		float currentRotationRate;
		float maxRotationRate = 360;
		[SerializeField] bool freezeRotation;

		public event Action<Vector2> OnLand;
		public event Action<Vector2> OnResting;
		public event Action<Vector2> OnJump;

		bool isOverLandable;
		public bool IsJumping;
		Vector2 DistanceToGround;
		Body body => _body ??= GetComponent<Body>();
		Body _body;
		float currentLandableHeight;
		float minBounceVelocity = 1000;
		float bounceVelocityDragFactor = .2f;
		float landTimer;
		float maxFlyTime = 2.5f;

		public void RotateToDirection(Vector2 direction, GameObject rotationObject)
		{
			var rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

			if (rotationObject == null) return;
			rotationObject.transform.eulerAngles = new Vector3(0, 0, rotation);
		}

		public void SetRotationRate(float newRate)
		{
			maxRotationRate = newRate;
		}

		public void SetFreezeRotation(bool freeze)
		{
			freezeRotation = freeze;
			maxRotationRate = 0;
		}

		void Rotate(float rotationSpeed)
		{
			if (freezeRotation) return;
			HeightObject.transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.fixedDeltaTime * 10));
		}

		public void FreezeRotationAtIdentity()
		{
			freezeRotation = true;
			HeightObject.transform.rotation = Quaternion.identity;
		}

		public void Jump(float startingHeight = 0, float verticalSpeed = 2, float minBounce = 1)
		{
			currentRotationRate = Random.Range(0, maxRotationRate);
			landTimer = 0;
			minBounceVelocity = minBounce;
			IsJumping = true;
			StopResting();
			OnJump?.Invoke(transform.position + new Vector3(0, startingHeight, 0));

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

		void Fly()
		{
			landTimer += Time.fixedDeltaTime;
			if (landTimer > maxFlyTime)
			{
				Land();
				return;
			}

			currentLandableHeight = 0;
			verticalVelocity -= Services.assetManager.Vars.Gravity.y * Time.fixedDeltaTime;
			if (GetHeight() + verticalVelocity <= currentLandableHeight && verticalVelocity < 0)
				Land();
			else
				SetHeight(GetHeight() + verticalVelocity);
		}

		protected virtual void Land()
		{
			if (Mathf.Abs(verticalVelocity) > minBounceVelocity)
			{
				Bounce();
				return;
			}

			IsJumping = false;
			OnLand?.Invoke(transform.position);
			SetHeight(currentLandableHeight);

			if (body != null) body.ChangeLayer(Body.BodyLayer.grounded);
			StartResting();
			verticalVelocity = 0;
		}

		void Bounce()
		{
			verticalVelocity *= -1;
			var velocity = bounceVelocityDragFactor;
			verticalVelocity *= velocity;
		}

		protected virtual void StartResting()
		{
			isResting = true;
			OnResting?.Invoke(transform.position);
			if (freezeRotation) transform.rotation = Quaternion.identity;
		}

		void StopResting()
		{
			isResting = false;
		}

		protected void FreezeHeight()
		{
			verticalVelocity = 0;
			IsJumping = false;
			SetHeight(GetHeight());
		}
	}
}
