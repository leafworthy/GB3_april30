using UnityEngine;

public class JumpAbility_FX : MonoBehaviour
{
	private JumpAbility jump;
	private Body body;

	private void OnEnable() 
	{
		jump = GetComponent<JumpAbility>();
		jump.OnJump += Jump_OnJump;
		jump.OnLand += Jump_OnLand;
		body = GetComponent<Body>();
	}

	private void OnDisable()
	{
		jump.OnJump -= Jump_OnJump;
		jump.OnLand -= Jump_OnLand;
	
	}

	private void Jump_OnJump(Vector2 obj)
	{
		ObjectMaker.Make(FX.Assets.dust2_jump, transform.position + new Vector3(0, body.GetCurrentLandableHeight()));
	}

	private  void Jump_OnLand(Vector2 pos)
	{
		ObjectMaker.Make(FX.Assets.dust1_ground, pos);
		var flipDust = ObjectMaker.Make(FX.Assets.dust1_ground, pos);
		flipDust.transform.localScale = new Vector3(flipDust.transform.localScale.x * -1, flipDust.transform.localScale.y, 0);
	}
}