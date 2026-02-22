namespace __SCRIPTS
{
	public class Chainsaw : UnlimitedGun
	{
		protected override float Spread => .25f;
		protected override int numberOfBulletsPerShot => 4;
		public override float reloadTime => 1;

		protected override bool simpleShoot => true;
	}
}