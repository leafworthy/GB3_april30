namespace __SCRIPTS
{
	public class Shotgun : PrimaryGun
	{
		protected override float Spread => .25f;
		protected override int numberOfBulletsPerShot => 4;
	}
}