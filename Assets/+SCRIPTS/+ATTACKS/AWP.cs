namespace __SCRIPTS
{
	public class AWP : PrimaryGun
	{
		protected override float Spread => 0;
		protected override int numberOfBulletsPerShot => 1;
		public override float reloadTime => 1;

		protected override bool simpleShoot => false;

	}
}
