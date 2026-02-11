using UnityEngine;

namespace __SCRIPTS
{
	public class Shotgun_FX : MonoBehaviour
	{
		Shotgun shotgun => _shotgun ??= GetComponent<Shotgun>();
		Shotgun _shotgun;

		Body body => _body ??= GetComponent<Body>();
		Body _body;

		void OnEnable()
		{
			shotgun.OnShoot += Shotgun_OnShoot;
		}

		void OnDisable()
		{
			shotgun.OnShoot -= Shotgun_OnShoot;
		}

		void Shotgun_OnShoot(Vector2 direction)
		{
			var shotgunBlast = Services.objectMaker.Make(Services.assetManager.FX.shotgunBlastPrefab, (Vector2)body.AttackStartPoint.transform.position + new Vector2(0,5));

			var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			shotgunBlast.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		}
	}
}
