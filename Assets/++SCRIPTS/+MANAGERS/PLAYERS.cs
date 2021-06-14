using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _SCRIPTS
{
	public class PLAYERS : Singleton<PLAYERS>
	{
		private static List<Player> players = new List<Player>();
		public static event Action OnAllPlayersDead;
		public static event Action<Player> OnPlayerDead;

		public static Player GetClosestPlayer(Vector3 position)
		{
			//if (!PlayersHaveBeenFound()) return null;

			Player closest = null;
			foreach (Player player in players)
			{
				if (closest is null)
				{
					closest = player;
				}
				else
				{
					var distance = Vector3.Distance(position, player.transform.position);
					var closestDistance = Vector3.Distance(position, closest.transform.position);
					if(distance < closestDistance){
						closest = player;
					}
				}
			}

			return closest;
		}

		private static bool PlayersHaveBeenFound()
		{
			if (players.Count <= 0)
			{
				FindPlayers();
			}

			if (players.Count <= 0)
			{
				Debug.Log("NOPLAYERSINTHESCENE");
				return false;
			}

			return true;
		}

		private static void FindPlayers()
		{
			foreach (GameObject playerGO in GameObject.FindGameObjectsWithTag("Player"))
			{
				var player = playerGO.GetComponent<Player>();
				if (player is null)
				{
					continue;
				}

				players.Add(player);
			}
		}

		public static void AddPlayer(Player newPlayer)
		{
			Debug.Log("player added");
			players.Add(newPlayer);
			newPlayer.OnDead += PlayerDies;
		}

		private static void PlayerDies(Player deadPlayer)
		{
			OnPlayerDead?.Invoke(deadPlayer);
			if (GetNumberOfLivingPlayers() <= 0)
			{
				OnAllPlayersDead?.Invoke();

			}
		}

		public static int GetNumberOfLivingPlayers()
		{
			if (!PlayersHaveBeenFound()) return 0;
			return players.Count(t => !t.IsDead());
		}

	}
}
