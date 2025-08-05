using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	/// <summary>
	/// Defines whether a spawn point is an entry, exit or both
	/// </summary>
	public enum SpawnPointType
	{
		Entry,
		Exit,
		Both
	}



	public class TravelPoint : MonoBehaviour
	{

		public SpawnPointType pointType = SpawnPointType.Both;

		[Tooltip("Scene this spawn point leads to (for Exit points)"), SerializeField]
		public SceneDefinition destinationScene;
		public bool fallFromSky;

		/// <summary>
		/// Get spawn positions for multiple players
		/// </summary>
		public List<Vector2> GetSpawnPositionsForPlayers(int playerCount)
		{
			var positions = new List<Vector2>();
			Vector2 basePosition = transform.position;

			var radius = 1.0f; // Base radius for positioning

			for (var i = 0; i < playerCount; i++)
			{
				if (i == 0)
				{
					// First player goes at the exact position
					positions.Add(basePosition);
				}
				else
				{
					// Calculate positions in a circle
					var angle = i * (360f / playerCount) * Mathf.Deg2Rad;
					var offset = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
					positions.Add(basePosition + offset);
				}
			}

			return positions;
		}

	}
}
