using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _SCRIPTS
{
	public class PLAYERS : Singleton<PLAYERS>
	{
		private static List<DefenceHandler> players = new List<DefenceHandler>();

		public static DefenceHandler GetClosestPlayer(Vector3 position)
		{
			if (!PlayersHaveBeenFound()) return null;

			DefenceHandler closest = null;
			foreach (DefenceHandler player in players)
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
				var player = playerGO.GetComponent<DefenceHandler>();
				if (player is null)
				{
					continue;
				}

				players.Add(player);
			}
		}

		public static void AddPlayer(DefenceHandler newPlayer)
		{
			Debug.Log("player added");
			players.Add(newPlayer);
		}

		public static int GetNumberOfLivingPlayers()
		{
			if (!PlayersHaveBeenFound()) return 0;
			return players.Count(t => !t.IsDead());
		}
	}
}
