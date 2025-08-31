namespace __SCRIPTS
{
	public class Shotgun : PrimaryGun
	{
		public override float Spread => .25f;
		public override int numberOfBulletsPerShot => 4;
	}
}