
using UnityEngine;

namespace __SCRIPTS
{
	[RequireComponent(typeof(MoveAbility)), RequireComponent(typeof(JumpAbility))]
	public class FallToFloor : ThingWithHeight
	{
		public SpriteRenderer spriteRendererToTint;
		protected JumpAbility jumper;
		protected JumpController jumpController => GetComponent<JumpController>();
		protected MoveAbility mover;
		private bool componentsInitialized = false;
		protected float rotationRate = 100;
		private float RotationRate;
		private float PushSpeed = 40;
		private float bounceSpeed = .25f;
		private float jumpSpeed = .5f;
		private bool _freezeRotation;

		protected void InitializeComponents()
		{
			if (componentsInitialized) return;

			// Cache component references - these are required components
			jumper = GetComponent<JumpAbility>();
			mover = GetComponent<MoveAbility>();

			// Validate required components
			Debug.Assert(jumper != null, $"JumpAbility required on {gameObject.name}");
			Debug.Assert(mover != null, $"MoveAbility required on {gameObject.name}");

			componentsInitialized = true;
		}

		public void FireForDrops(Vector3 shootAngle, Color color, float height, bool freezeRotation = false)
		{
			// Ensure components are cached before use
			InitializeComponents();

			RotationRate = Random.Range(0, rotationRate);
			jumper.OnBounce += Jumper_OnBounce;
			jumper.OnResting += Jumper_OnResting;
			jumper.StartActivity();
			mover.SetDragging(false);
			mover.Push(shootAngle, Random.Range(0, PushSpeed));
			spriteRendererToTint.color = color;
			_freezeRotation = freezeRotation;
		}

		private void Jumper_OnResting(Vector2 obj)
		{
			if (_freezeRotation)
			{
				transform.rotation = Quaternion.identity;
			}
		}

		private void Jumper_OnBounce()
		{
			mover.SetDragging(true);
		}

		public void Fire(Attack attack, bool isFlipped = false)
		{
			RotationRate = Random.Range(0, rotationRate);
			jumper = GetComponent<JumpAbility>();
			jumper.OnBounce += Jumper_OnBounce;
			jumper.OnResting += Jumper_OnResting;
			mover = GetComponent<MoveAbility>();
			jumpController.Jump(attack.OriginHeight);
			mover.SetDragging(false);
			mover.Push(isFlipped ? attack.FlippedDirection : attack.Direction.normalized + new Vector2(Random.Range(-.4f, .4f), Random.Range(-.4f, .4f)),
				Random.Range(0, PushSpeed));
		}

		protected override void FixedUpdate()
		{
			// Ensure components are initialized (no GetComponent calls in FixedUpdate!)
			InitializeComponents();

			base.FixedUpdate();

			// Use cached references - components are guaranteed to exist due to RequireComponent
			if (jumper.IsJumping && !_freezeRotation)
				Rotate(RotationRate);
			else
				mover.SetDragging(true);
		}

		private void Rotate(float rotationSpeed)
		{
			JumpObject.transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.fixedDeltaTime * 10));
		}
	}
}
