using UnityEngine;

namespace __SCRIPTS
{
	public class TopFaceAimDirection : MonoBehaviour
	{
		IAimAbility aimAbility => _aimAbility ??= GetComponent<IAimAbility>();
		IAimAbility _aimAbility;
		Body body  => _body ??= GetComponent<Body>();
		Body _body;
		public Vector2 AimDir => aimAbility.AimDir;
		void FixedUpdate()
		{
			body.TopFaceDirection(AimDir.x >= 0);
		}

	}
}