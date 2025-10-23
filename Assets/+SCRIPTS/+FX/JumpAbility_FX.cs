using UnityEngine;

namespace __SCRIPTS
{
	public class JumpAbility_FX : MonoBehaviour
	{
		private JumpAndRotateAbility jumpAndRotate;
		private Body body;

		private void OnEnable()
		{
			jumpAndRotate = GetComponent<JumpAndRotateAbility>();
			jumpAndRotate.OnJump += JumpAndRotateOnJumpAndRotate;
			jumpAndRotate.OnLand += JumpAndRotateOnLand;
			body = GetComponent<Body>();
		}

		private void OnDisable()
		{
			jumpAndRotate.OnJump -= JumpAndRotateOnJumpAndRotate;
			jumpAndRotate.OnLand -= JumpAndRotateOnLand;

		}

		private void JumpAndRotateOnJumpAndRotate(Vector2 obj)
		{
			Services.objectMaker.Make(Services.assetManager.FX.dust2_jump, transform.position);
		}

		private  void JumpAndRotateOnLand(Vector2 pos)
		{
			Services.objectMaker.Make(Services.assetManager.FX.dust1_ground, pos);
			var flipDust = Services.objectMaker.Make(Services.assetManager.FX.dust1_ground, pos);
			flipDust.transform.localScale = new Vector3(flipDust.transform.localScale.x * -1, flipDust.transform.localScale.y, 0);
		}
	}
}
