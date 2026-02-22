using UnityEngine;

namespace __SCRIPTS
{
	[ExecuteAlways, RequireComponent(typeof(MoveAbility))]
	public class MoveJumpAndRotateAbility : JumpAndRotateAbility
	{
		private readonly float bounceSpeed = 5;
		private MoveAbility _moveAbility;
		public MoveAbility moveAbility => _moveAbility ??= GetComponent<MoveAbility>();

		#region Event Functions

		private void Update()
		{
			if (isResting) moveAbility.SetDragging(true);
		}

		#endregion

		protected override void Land()
		{
			base.Land();
			moveAbility.SetDragging(true);
		}

		public virtual void Fire(Vector2 shootAngle, float startingHeight, float verticalSpeed = 0, float pushSpeed = 40)
		{
			Jump(startingHeight, verticalSpeed, bounceSpeed);
			moveAbility.SetDragging(false);
			moveAbility.Push(shootAngle, Random.Range(0, pushSpeed));
		}
	}
}
