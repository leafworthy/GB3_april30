using System;
using UnityEngine;

public class GameWinner : MonoBehaviour
{
	public static event Action<Player> OnPlayerWinsGame;
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject == gameObject) return;
		var player = col.GetComponent<Car2dController>();
		if (player == null) return;
		if (!player.owner) return;
		
		OnPlayerWinsGame?.Invoke(player.owner);
	}
}