using UnityEngine;

namespace __SCRIPTS.Cursor
{
	public class PlayerCursor : MonoBehaviour
	{
		public Player owner;
		public IAimAbility aimAbility => owner.SpawnedPlayerGO?.GetComponent<IAimAbility>();
	}
}
