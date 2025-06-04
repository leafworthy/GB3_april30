using UnityEngine;

namespace GangstaBean.UI.Cursor
{
	public class PlayerCursor : MonoBehaviour
	{
		public Player owner;
		public AimAbility aimAbility => owner.SpawnedPlayerGO?.GetComponent<AimAbility>();
	}
}