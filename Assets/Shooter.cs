using UnityEngine;

public class Shooter : MonoBehaviour
{
	private bool isClicking;
	public GameObject start;
	public GameObject end;

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			if (isClicking) return;
			isClicking = true;
			Shoot();
		}
		else
		{
			isClicking = false;
		}
	}

	private void Shoot()
	{
		var newBullet = MAKER.Make(ASSETS.FX.BulletPrefab, start.transform.position);

		var bulletScript = newBullet.GetComponent<BulletFX>();
		//bulletScript.Fire(start.transform.position, end.transform.position);
	}
}
