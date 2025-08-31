using GangstaBean.Core;

namespace __SCRIPTS
{
	public class DoableAttack : Ability
	{
		protected Life attacker => _attacker ?? GetComponent<Life>();
		private Life _attacker;
		protected override bool requiresArms() => false;

		protected override bool requiresLegs() => false;

		protected override void DoAbility()
		{
		}

	}
}
