using System;
using GangstaBean.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS
{
	public class JumpAbility : SerializedMonoBehaviour, IActivity
	{
		private Body body;
		public bool isResting;
		private float verticalVelocity;


		public event Action<Vector2> OnLand;
		public event Action<Vector2> OnResting;
		public event Action<Vector2> OnJump;
		public event Action OnBounce;

		private bool isOverLandable;
		[FormerlySerializedAs("isInAir")] public bool IsJumping;
		private ThingWithHeight thing;
		private float currentLandableHeight;
		private bool isJumping;
		private bool initiated;
		private float minBounceVelocity = 1000;
		private float bounceVelocityDragFactor = .2f;
		private float landTimer;
		private float maxFlyTime = 2.5f;
		public string AbilityName => "Jump";


		public void Jump(float startingHeight = 0, float verticalSpeed = 2, float minBounce = 1)
		{
			landTimer = 0;
			minBounceVelocity = minBounce;
			IsJumping = true;
			StopResting();
			OnJump?.Invoke(transform.position+ new Vector3(0,startingHeight,0));

			Debug.Log("on jump");
			verticalVelocity = verticalSpeed;

			thing = GetComponent<ThingWithHeight>();
			thing.SetDistanceToGround(startingHeight);

			body = GetComponent<Body>();
			if (body == null) return;
			body.ChangeLayer(Body.BodyLayer.jumping);
		}

		private void Start()
		{
			Init();
		}

		private void Init()
		{
			if (initiated) return;
			initiated = true;
			thing = GetComponent<ThingWithHeight>();
			body = GetComponent<Body>();
		}

		private void OnEnable()
		{
			Init();

		}




		protected void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;
			if (isResting) return;
			if (!IsJumping) return;

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
			if ((thing.GetDistanceToGround() + verticalVelocity <= currentLandableHeight) && (verticalVelocity < 0))
			{
				Land();
			}
			else
			{
				thing.SetDistanceToGround(thing.GetDistanceToGround() + verticalVelocity);
			}
		}


		private void Land()
		{
			if (Mathf.Abs(verticalVelocity) > minBounceVelocity)
			{
				Bounce();
				return;
			}
			IsJumping = false;
			OnLand?.Invoke(transform.position);
			Debug.Log("land");
			thing.SetDistanceToGround(currentLandableHeight);

			if (body != null) body.ChangeLayer( Body.BodyLayer.grounded);
			StartResting();
			verticalVelocity = 0;
		}

		private void Bounce()
		{
			verticalVelocity *= -1;
			var velocity = bounceVelocityDragFactor;
			verticalVelocity *= velocity;
			OnBounce?.Invoke();
		}

		private void StartResting()
		{
			isResting = true;
			OnResting?.Invoke(transform.position);
			if (body == null) return;
			body.legs.Stop(this);
			body.arms.Stop(this);
		}

		private void StopResting()
		{
			isResting = false;
			Debug.Log("[Jump] Stopped resting - character can move again");
		}



	}
}
