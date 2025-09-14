using UnityEngine;

namespace __SCRIPTS
{
	public class Shotgun_FX:MonoBehaviour
	{
		private Shotgun shotgun => _shotgun ??= GetComponent<Shotgun>();
		private Shotgun _shotgun;

		private Body body => _body ??= GetComponent<Body>();
		private Body _body;

		private void OnEnable()
		{
			shotgun.OnShoot += Shotgun_OnShoot;
		}

		private void OnDisable()
		{
			shotgun.OnShoot -= Shotgun_OnShoot;
		}

		private void Shotgun_OnShoot(Vector2 direction)
		{
			var shotgunBlast = Services.objectMaker.Make(Services.assetManager.FX.shotgunBlastPrefab, body.AttackStartPoint.transform.position);

			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			shotgunBlast.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		}

	}
}