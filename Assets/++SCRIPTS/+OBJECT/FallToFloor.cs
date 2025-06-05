
using UnityEngine;

namespace __SCRIPTS
{
	[RequireComponent(typeof(MoveAbility)), RequireComponent(typeof(JumpAbility))]
	public class FallToFloor : ThingWithHeight
	{
		public SpriteRenderer spriteRendererToTint;
		protected JumpAbility jumper;
		protected MoveAbility mover;
		protected float rotationRate = 100;
		private float RotationRate;
		private float PushSpeed = 40;
		private float bounceSpeed = .25f;
		private float jumpSpeed = .5f;
		private bool _freezeRotation;

		public void FireForDrops(Vector3 shootAngle, Color color, float height, bool freezeRotation = false)
		{
			RotationRate = Random.Range(0, rotationRate);
			jumper = GetComponent<JumpAbility>();
			jumper.OnBounce += Jumper_OnBounce;
			jumper.OnResting += Jumper_OnResting;
			mover = GetComponent<MoveAbility>();
			jumper.Jump(height, jumpSpeed, bounceSpeed);
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
			var verticalSpeed = Random.Range(0, bounceSpeed);
			jumper.Jump(attack.OriginHeight, 0);
			mover.SetDragging(false);
			mover.Push(isFlipped ? attack.FlippedDirection : attack.Direction.normalized + new Vector2(Random.Range(-.4f, .4f), Random.Range(-.4f, .4f)),
				Random.Range(0, PushSpeed));
		}

		protected override void FixedUpdate()
		{
			if (mover == null) mover = GetComponent<MoveAbility>();
			if (jumper == null) jumper = GetComponent<JumpAbility>();
			base.FixedUpdate();
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
