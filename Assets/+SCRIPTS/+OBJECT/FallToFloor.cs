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

		private readonly float bounceSpeed = .25f;

		private Vector2 Height;

		private void Update()
		{
			if(simpleJumpAbility.isResting) moveAbility.SetDragging(true);
		}

		public IHaveHeight SetHeight(float height)
		{
			Height.y = height;
			simpleJumpAbility.HeightObject.transform.localPosition = Height;
			return this;
		}

		public float GetHeight() => Height.y;



		private void SimpleJumpAbility_OnLand(Vector2 obj)
		{
			simpleJumpAbility.OnLand -= SimpleJumpAbility_OnLand;
			moveAbility.SetDragging(true);
		}


		public virtual IDebree Fire(Vector2 shootAngle, float height, float verticalSpeed = 0, float pushSpeed = 40)
		{
			simpleJumpAbility.Jump(height, verticalSpeed, bounceSpeed);
			simpleJumpAbility.OnLand += SimpleJumpAbility_OnLand;
			moveAbility.SetDragging(false);
			moveAbility.Push(shootAngle, Random.Range(0, pushSpeed));
			return this;
		}


	}
}
