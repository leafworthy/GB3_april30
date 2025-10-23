using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteAlways, RequireComponent(typeof(MoveAbility))]
	public class MoveJumpAndRotateAbility : JumpAndRotateAbility
	{
		public MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();
		private MoveAbility _moveAbility;

		private readonly float bounceSpeed = 5;

		private Vector2 Height;

		private void Update()
		{
			if(isResting) moveAbility.SetDragging(true);
		}

		protected override void Land()
		{
			base.Land();
			moveAbility.SetDragging(true);
		}



		public virtual void Fire(Vector2 shootAngle, float height, float verticalSpeed = 0, float pushSpeed = 40)
		{
			Jump(height, verticalSpeed, bounceSpeed);
			moveAbility.SetDragging(false);
			moveAbility.Push(shootAngle, Random.Range(0, pushSpeed));
		}


	}
}
