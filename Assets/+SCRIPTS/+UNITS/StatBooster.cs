using __SCRIPTS;
using UnityEngine;

public class StatBooster : MonoBehaviour
{
	Life life => _life ??= GetComponent<Life>();
	Life _life;
	public float healthBoostAmount = 20;
	public bool isBasedOnJoinedPlayers;

	void Awake()
	{
		life.OnInitialize += LifeOnOnInitialize;
	}

	void LifeOnOnInitialize()
	{
		Debug.Log("caught initialize");
		if (isBasedOnJoinedPlayers)
		{
			var playerCount = Services.playerManager.AllJoinedPlayers.Count;
			var totalHealthBoost = healthBoostAmount * playerCount;
			Debug.Log( $"boosting health by {totalHealthBoost} based on {playerCount} players");
			life.Stats.ExtraHealthFactor = totalHealthBoost;
		}
		else
			life.Stats.ExtraHealthFactor = healthBoostAmount;
	}
}
