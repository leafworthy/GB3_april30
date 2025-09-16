using UnityEngine;

namespace __SCRIPTS
{
	public class MoveAbility_FX : MonoBehaviour
	{
		private Body body;
		private AnimationEvents animEvents;
		private UnitAnimations anim;
		private MoveAbility mover;

		private void OnEnable()
		{
			mover = GetComponent<MoveAbility>();
			anim = GetComponent<UnitAnimations>();
			animEvents = anim.animEvents;
			animEvents.OnStep += Anim_OnStep;
			body = GetComponent<Body>();
		}

		private void OnDisable()
		{
			animEvents.OnStep -= Anim_OnStep;
		}


		private void Anim_OnStep()
		{
			var dust = Services.objectMaker.Make(Services.assetManager.FX.dust1_ground, body.FootPoint.transform.position);
			if (mover.GetMoveDir().x > 0)
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
}
