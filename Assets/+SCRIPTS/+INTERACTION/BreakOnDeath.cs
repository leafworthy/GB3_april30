using UnityEngine;

namespace __SCRIPTS
{
	public class BreakOnDeath : MonoBehaviour
	{
		private HideRevealObjects breakList;
		[SerializeField]private Life life;

		private void Start()
		{
			breakList = GetComponent<HideRevealObjects>();
			life.OnDying += Life_Dead;
			breakList.Set(0);
		}

		private void Life_Dead(Attack attack)
		{
			breakList.Set(1);
		}
	}
}
