using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteAlways, RequireComponent(typeof(MoveAbility)), RequireComponent(typeof(SimpleJumpAbility))]
	public class FallToFloor : MonoBehaviour, IDebree
	{
		protected SimpleJumpAbility simpleJumpAbility => _simpleJumpAbility ??= GetComponent<SimpleJumpAbility>();
		private SimpleJumpAbility _simpleJumpAbility;
		protected MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;

		private protected readonly float PushSpeed = 40;
		protected readonly float bounceSpeed = .25f;

		private Vector2 DistanceToGround;

		private void Update()
		{
			if(simpleJumpAbility.isResting) moveAbility.SetDragging(true);
		}

		public IDebree SetDistanceToGround(float height)
		{
			DistanceToGround.y = height;
			simpleJumpAbility.HeightObject.transform.localPosition = DistanceToGround;
			return this;
		}

		protected virtual void StartFiring(float originHeight, float verticalSpeed, float bounceSpeed = 0)
		{
			simpleJumpAbility.Jump(originHeight, verticalSpeed, bounceSpeed);
			simpleJumpAbility.OnLand += SimpleJumpAbility_OnLand;
			moveAbility.SetDragging(false);
		}

		private void SimpleJumpAbility_OnLand(Vector2 obj)
		{
			simpleJumpAbility.OnLand -= SimpleJumpAbility_OnLand;
			moveAbility.SetDragging(true);
		}

		public IDebree Explode(float explosionSize)
		{
			StartFiring(0, explosionSize);
			var randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
			moveAbility.Push(
				randomDirection + new Vector2(Random.Range(-.4f * explosionSize, .4f * explosionSize), Random.Range(-.4f * explosionSize, .4f * explosionSize)),
				Random.Range(0, PushSpeed));

			return this;
		}

		public IDebree TintSprite(Color debreeTint)
		{
			var spriteToTint = GetComponentInChildren<SpriteRenderer>();
			if (spriteToTint != null)
				spriteToTint.color = debreeTint;
			return this;
		}

		public virtual IDebree Fire(Vector2 angle, float height)
		{
			StartFiring(height, 0, bounceSpeed);
			moveAbility.Push(angle, Random.Range(0, PushSpeed));
			return this;
		}

		public virtual IDebree Fire(Attack attack)
		{
			StartFiring(attack.OriginHeight, 0);
			moveAbility.Push(attack.Direction.normalized + new Vector2(Random.Range(-.4f, .4f), Random.Range(-.4f, .4f)), Random.Range(0, PushSpeed));

			return this;
		}

		public virtual IDebree FireFlipped(Attack attack)
		{
			StartFiring(attack.OriginHeight, 0);
			moveAbility.Push(attack.FlippedDirection, Random.Range(0, PushSpeed));

			return this;
		}
	}
}
