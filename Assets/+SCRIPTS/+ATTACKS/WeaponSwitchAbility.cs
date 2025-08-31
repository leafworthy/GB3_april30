namespace __SCRIPTS
{
	public class WeaponSwitchAbility : Ability
	{
		public bool IsGlocking => isGlocking;
		private bool isGlocking;
		public bool IsGlockingOnPurpose => isGlockingOnPurpose;
		private bool isGlockingOnPurpose;
		public override string VerbName => "Swapping Weapon";
		protected override bool requiresArms() => true;

		protected override bool requiresLegs() => false;

		protected override void DoAbility()
		{
			StartSwapping();
		}

		public override void SetPlayer(Player _player)
		{
			base.SetPlayer(_player);
			ListenToPlayer();
		}

		private void ListenToPlayer()
		{
			player.Controller.SwapWeaponSquare.OnPress += Player_SwapWeapon;
		}

		private void StartSwapping()
		{
			isGlocking = !isGlocking;
			isGlockingOnPurpose = isGlocking;
		}

		private void Player_SwapWeapon(NewControlButton obj)
		{
			Do();
		}
	}
}
