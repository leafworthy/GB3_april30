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
			if (players.Count <= 0)
			{
				return null;
			}

			DefenceHandler closest = null;
			foreach (DefenceHandler player in players)
			{
				if (closest == null)
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

		public static void AddPlayer(DefenceHandler newPlayer)
		{
			Debug.Log("player added");
			players.Add(newPlayer);
		}

		public static int GetNumberOfLivingPlayers()
		{
			return players.Count(t => !t.IsDead());
		}
	}
}
