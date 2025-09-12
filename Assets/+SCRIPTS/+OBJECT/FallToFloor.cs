
using UnityEngine;

namespace __SCRIPTS
{
	[RequireComponent(typeof(MoveAbility)), RequireComponent(typeof(SimpleJumpAbility))]
	public class FallToFloor : ThingWithHeight
	{
		public SpriteRenderer spriteRendererToTint;
		private SimpleJumpAbility SimpleJumpAbility => simpleJumpAbility ??= GetComponent<SimpleJumpAbility>();
		private SimpleJumpAbility simpleJumpAbility;
		protected MoveAbility moveAbility  => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;

		protected float rotationRate = 100;
		private float RotationRate;
		private float PushSpeed = 40;
		private float bounceSpeed = .25f;
		private float jumpSpeed = .5f;
		private bool freezeRotation;


		public void FireForDrops(Vector3 shootAngle, Color color, float height, bool _freezeRotation = false)
		{
			RotationRate = Random.Range(0, rotationRate);
			SimpleJumpAbility.OnBounce += SimpleJumpAbilityOnBounce;
			SimpleJumpAbility.OnResting += SimpleJumpAbilityOnResting;
			SimpleJumpAbility.Jump(height, jumpSpeed, bounceSpeed);
			moveAbility.SetDragging(false);
			moveAbility.Push(shootAngle, Random.Range(0, PushSpeed));
			spriteRendererToTint.color = color;
			freezeRotation = _freezeRotation;
		}

		private void SimpleJumpAbilityOnResting(Vector2 obj)
		{
			if (freezeRotation)
			{
				transform.rotation = Quaternion.identity;
			}
		}

		private void SimpleJumpAbilityOnBounce()
		{
			moveAbility.SetDragging(true);
		}

		public void Fire(Attack attack, bool isFlipped = false)
		{
			RotationRate = Random.Range(0, rotationRate);
			SimpleJumpAbility.OnBounce += SimpleJumpAbilityOnBounce;
			SimpleJumpAbility.OnResting += SimpleJumpAbilityOnResting;
			var verticalSpeed = Random.Range(0, bounceSpeed);
			SimpleJumpAbility.Jump(attack.OriginHeight, 0);
			moveAbility.SetDragging(false);
			moveAbility.Push(isFlipped ? attack.FlippedDirection : attack.Direction.normalized + new Vector2(Random.Range(-.4f, .4f), Random.Range(-.4f, .4f)),
				Random.Range(0, PushSpeed));
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			if (SimpleJumpAbility.IsJumping && !freezeRotation)
				Rotate(RotationRate);
			else
				moveAbility.SetDragging(true);
		}

		private void Rotate(float rotationSpeed)
		{
			JumpObject.transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.fixedDeltaTime * 10));
		}
	}
}
