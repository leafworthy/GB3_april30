using __SCRIPTS._ABILITIES;
using __SCRIPTS._UNITS;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __SCRIPTS
{
	[ExecuteInEditMode]
	public class HittableThing : ThingWithHeight
	{
		private Life life;
		private JumpAbility jumper;
		private MoveAbility mover;
		private float minVelocity = 1;
		private float maxVelocity = 3;
		public float heightSet;

		[Button()]
		public void SetHeight()
		{
			SetDistanceToGround(heightSet);
		}
		protected override void Start()
		{
			mover = GetComponent<MoveAbility>();
			jumper = GetComponent<JumpAbility>();
			life = GetComponentInChildren<Life>();
			life.OnDamaged += (s)=> GetHit(s.Direction.normalized);
		}

		private void GetHit(Vector3 shootAngle)
		{
			jumper.Jump();
			var speed = Random.Range(minVelocity, maxVelocity);
			mover.Push(shootAngle, speed);
		}
	}
}