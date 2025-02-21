using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
	public Player owner;
	public AimAbility aimAbility => owner.SpawnedPlayerGO?.GetComponent<AimAbility>();
}