using ToolBox.Pools;
using UnityEngine;

public class DustFX : MonoBehaviour
{
	private Body body;
	private AnimationEvents animEvents;
	private Animations anim;
	private MoveAbility mover;
	private RecycleGameObject recycle;

	private void Awake()
	{
		
		Activate();
	}

	private void Activate()
	{
		mover = GetComponent<MoveAbility>();
		anim = GetComponent<Animations>();
		animEvents = anim.animEvents;
		animEvents.OnStep += Anim_OnStep;
		body = GetComponent<Body>();
	}

	private void CleanUp()
	{
		animEvents.OnStep -= Anim_OnStep;
	}


	private void Anim_OnStep()
	{
		var dust = ASSETS.FX.dust1_ground.Reuse(body.FootPoint.transform.position, Quaternion.identity);
		if (mover.moveDir.x > 0)
		{
			dust.transform.localScale = new Vector3(-Mathf.Abs(dust.transform.localScale.x), dust.transform.localScale.y,
				dust.transform.localScale.z);
		}
		else
		{
			dust.transform.localScale = new Vector3(Mathf.Abs(dust.transform.localScale.x),
				dust.transform.localScale.y,
				dust.transform.localScale.z);
		}
	}
}
