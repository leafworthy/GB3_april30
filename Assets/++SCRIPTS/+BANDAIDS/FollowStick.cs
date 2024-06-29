using System;
using UnityEngine;

public class FollowStick : MonoBehaviour
{
	private GunAimer aim;

	// Start is called before the first frame update
	public void Init(Player player)
	{
		aim = player.SpawnedPlayerGO.GetComponent<GunAimer>();
		if (aim == null) return;
		transform.position = aim.GetAimPoint();
	}

	private void Update()
	{
		transform.position = aim.GetAimPoint();
	}
}