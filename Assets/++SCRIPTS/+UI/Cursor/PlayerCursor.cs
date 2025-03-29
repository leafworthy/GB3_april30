using UnityEngine;

namespace __SCRIPTS.Cursor
{
	public class PlayerCursor : MonoBehaviour
	{
		public Player owner;
		public AimAbility aimAbility => owner.SpawnedPlayerGO?.GetComponent<AimAbility>();
	}
}