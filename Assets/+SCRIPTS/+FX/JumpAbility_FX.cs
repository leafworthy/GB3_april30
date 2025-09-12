using UnityEngine;

namespace __SCRIPTS
{
	public class JumpAbility_FX : MonoBehaviour
	{
		private SimpleJumpAbility simpleJump;
		private Body body;

		private void OnEnable()
		{
			simpleJump = GetComponent<SimpleJumpAbility>();
			simpleJump.OnJump += SimpleJumpOnSimpleJump;
			simpleJump.OnLand += SimpleJumpOnLand;
			body = GetComponent<Body>();
		}

		private void OnDisable()
		{
			simpleJump.OnJump -= SimpleJumpOnSimpleJump;
			simpleJump.OnLand -= SimpleJumpOnLand;

		}

		private void SimpleJumpOnSimpleJump(Vector2 obj)
		{
			Services.objectMaker.Make(Services.assetManager.FX.dust2_jump, transform.position);
		}

		private  void SimpleJumpOnLand(Vector2 pos)
		{
			Services.objectMaker.Make(Services.assetManager.FX.dust1_ground, pos);
			var flipDust = Services.objectMaker.Make(Services.assetManager.FX.dust1_ground, pos);
			flipDust.transform.localScale = new Vector3(flipDust.transform.localScale.x * -1, flipDust.transform.localScale.y, 0);
		}
	}
}
