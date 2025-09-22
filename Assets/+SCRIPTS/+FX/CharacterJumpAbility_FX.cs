using __SCRIPTS;
using UnityEngine;

[RequireComponent(typeof(JumpAbility)),DisallowMultipleComponent]
public class CharacterJumpAbility_FX : MonoBehaviour
{

	private JumpAbility jump;
	private Body body;

	private void OnEnable()
	{
		jump = GetComponent<JumpAbility>();
		if (jump == null) return;
		jump.OnJump += Jump_OnJump;
		jump.OnLand += Jump_OnLand;
		body = GetComponent<Body>();
	}

	private void OnDisable()
	{
		if (jump == null) return;
		jump.OnJump -= Jump_OnJump;
		jump.OnLand -= Jump_OnLand;

	}

	private void Jump_OnJump(Vector2 obj)
	{
		Services.objectMaker.Make(Services.assetManager.FX.dust2_jump, transform.position );
	}

	private void Jump_OnLand(Vector2 pos)
	{
		Services.objectMaker.Make(Services.assetManager.FX.dust1_ground, pos);
		var flipDust = Services.objectMaker.Make(Services.assetManager.FX.dust1_ground, pos);
		flipDust.transform.localScale =
			new Vector3(flipDust.transform.localScale.x * -1, flipDust.transform.localScale.y, 0);
	}
}
