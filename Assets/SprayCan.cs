using __SCRIPTS;
using UnityEngine;

public class SprayCan : SecondaryGun
{
	protected override float Spread => 0;
	protected override int numberOfBulletsPerShot => 1;
	public override float reloadTime => 1;

	protected override bool simpleShoot => false;
}
