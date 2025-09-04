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

		private void Life_Dead(Player player, Life life1)
		{
			breakList.Set(1);
		}
	}
}
