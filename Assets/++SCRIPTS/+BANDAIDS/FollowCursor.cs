using UnityEngine;

public class FollowCursor : MonoBehaviour
{
	private AimAbility aim;

	// Start is called before the first frame update
	public void Init(Player player)
	{
		aim = player.SpawnedPlayerGO.GetComponent<AimAbility>();
		if (aim == null) return;
		transform.position = aim.GetAimPoint();
	}

	private void Update()
	{
		transform.position = aim.GetAimPoint();
	}
}