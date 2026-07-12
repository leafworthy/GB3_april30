using UnityEngine;

namespace __SCRIPTS
{
	public class AWP_FX : MonoBehaviour
	{
		AWP awp => _awp ??= GetComponent<AWP>();
		AWP _awp;

		Body body => _body ??= GetComponent<Body>();
		Body _body;

		void OnEnable()
		{
			awp.OnShoot += AwpOnShoot;
		}

		void OnDisable()
		{
			awp.OnShoot -= AwpOnShoot;
		}

		void AwpOnShoot(Vector2 direction)
		{
			var shotgunBlast = Services.objectMaker.Make(Services.assetManager.FX.shotgunBlastPrefab,
				(Vector2) body.AttackStartPoint.transform.position + new Vector2(0, 5));

			var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			shotgunBlast.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		}
	}
}