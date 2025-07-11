using __SCRIPTS;
using UnityEngine;

[RequireComponent(typeof(DoableJumpAbility)),DisallowMultipleComponent]
public class DoableJumpAbility_FX : ServiceUser
{
	private DoableJumpAbility jump;
	private Body body;

	private void OnEnable()
	{
		jump = GetComponent<DoableJumpAbility>();
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
		objectMaker.Make( assets.FX.dust2_jump, transform.position + new Vector3(0, body.GetCurrentLandableHeight()));
	}

	private void Jump_OnLand(Vector2 pos)
	{
		objectMaker.Make( assets.FX.dust1_ground, pos);
		var flipDust = objectMaker.Make( assets.FX.dust1_ground, pos);
		flipDust.transform.localScale =
			new Vector3(flipDust.transform.localScale.x * -1, flipDust.transform.localScale.y, 0);
	}
}
