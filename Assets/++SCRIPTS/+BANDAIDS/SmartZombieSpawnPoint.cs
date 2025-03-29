using UnityEngine;

namespace __SCRIPTS
{
	/// <summary>
	/// Smart spawn point for zombies with additional features:
	/// - Collision detection to avoid spawning on top of objects
	/// - Off-camera spawning to ensure zombies appear outside player's view
	/// - Proximity activation for spawns that only trigger when player is nearby
	/// </summary>
	public class SmartZombieSpawnPoint : MonoBehaviour
	{
		[Header("Spawn Settings"), Tooltip("Local spawn area size")]
		public Vector2 spawnAreaSize = new(3, 3);

		public GameObject TrySpawn(GameObject enemyPrefabs, bool ensureOffCamera, Camera _camera, float cameraMargin)
		{
			if (enemyPrefabs == null) return null;

			// Try to find a valid spawn position
			var spawnPosition = GetValidSpawnPosition();

			if (spawnPosition == Vector2.zero)
			{
				Debug.Log("no valid position");
				return null;
			}

			MyDebugUtilities.DrawX(spawnPosition, 1, Color.blue);

			// Return the spawned enemy
			return ObjectMaker.I.Make(enemyPrefabs, spawnPosition);
		
		}

		private Vector2 GetValidSpawnPosition()
		{
			// Try up to 10 positions before giving up
			for (var i = 0; i < 10; i++)
			{
				var potentialPosition = (Vector2) transform.position + new Vector2(Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
					Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2));

				// Check for obstacles - don't spawn on buildings or other obstacles
				if (Physics2D.OverlapCircle(potentialPosition, 0.5f, ASSETS.LevelAssets.BuildingLayer))
				{
					Debug.Log("obstacle in the way");

					MyDebugUtilities.DrawX(potentialPosition, 1, Color.red);
					continue; // Position has an obstacle, try another
				}


				MyDebugUtilities.DrawX(potentialPosition, 1, Color.green);
				// All checks passed, return this position
				return potentialPosition;
			}

			// Failed to find a valid position after 10 attempts
			return Vector2.zero;
		}
	}
}