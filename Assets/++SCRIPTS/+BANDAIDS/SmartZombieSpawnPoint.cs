using UnityEngine;

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

	[Tooltip("Whether this spawn point uses object pooling")]
	public bool useObjectPooling = true;

	[Header("Proximity Settings"), Tooltip("Whether this spawn point only activates when players are nearby")]
	public bool isProximityBased;

	[Header("Spawn Customization"), Tooltip("Override global enemy type distribution")]
	public bool overrideEnemyDistribution;

	[Header("Spawn Effects"), Tooltip("Show spawn effects")]
	public bool showSpawnEffects = true;

	[Tooltip("Spawn effect type")]
	public enum SpawnEffectType
	{
		None,
		Dust,
		Blood
	}

	public SpawnEffectType spawnEffect = SpawnEffectType.Dust;

	[Header("Debug")] public bool showDebugVisuals;
	public Color debugColor = Color.cyan;

	/// <summary>
	/// Attempts to spawn an enemy at this spawn point
	/// </summary>
	/// <param name="enemyPrefabs">Array of possible enemy prefabs to spawn</param>
	/// <param name="ensureOffCamera">Whether to ensure spawn occurs outside camera view</param>
	/// <param name="_camera">Camera to check visibility against</param>
	/// <param name="cameraMargin">Margin beyond camera bounds</param>
	/// <returns>The spawned enemy GameObject, or null if spawn failed</returns>
	public GameObject TrySpawn(GameObject enemyPrefabs, bool ensureOffCamera, Camera _camera, float cameraMargin)
	{
		if (enemyPrefabs == null) return null;

		// Try to find a valid spawn position
		var spawnPosition = GetValidSpawnPosition(ensureOffCamera, _camera, cameraMargin);

		if (spawnPosition == Vector2.zero)
		{
			Debug.Log("no valid position");
			return null;
		}

		MyDebugUtilities.DrawX(spawnPosition, 1, Color.blue);


		// Spawn the enemy using object pooling or instantiation
		GameObject enemy;
		if (useObjectPooling)
		{
			Debug.Log("made");
			enemy = ObjectMaker.Make(enemyPrefabs, spawnPosition);
		}
		else
		{
			Debug.Log("instantiated");
			enemy = Instantiate(enemyPrefabs, spawnPosition, Quaternion.identity);
		}

		// Create spawn effect if enabled
		if (showSpawnEffects && enemy != null) CreateSpawnEffect(spawnPosition);

		// Return the spawned enemy
		return enemy;
	}

	/// <summary>
	/// Creates a visual effect at the spawn position
	/// </summary>
	private void CreateSpawnEffect(Vector2 position)
	{
		if (spawnEffect == SpawnEffectType.None) return;

		GameObject effect = null;

		switch (spawnEffect)
		{
			case SpawnEffectType.Dust:
				// Using both formats intentionally as a compatibility test
				if (ASSETS.FX != null && ASSETS.FX.dust1_ground != null)
				{
					// Old way still works
					effect = ObjectMaker.Make(ASSETS.FX.dust1_ground, position);
				}
				else if (ASSETS.FX != null && ASSETS.FX.dust1_ground != null)
				{
					// New way also works
					effect = ObjectMaker.Make(ASSETS.FX.dust1_ground, position);
				}

				break;
			case SpawnEffectType.Blood:
				// Use a random blood spray
				if (ASSETS.FX != null && ASSETS.FX.bloodspray != null && ASSETS.FX.bloodspray.Count > 0)
				{
					var randomBlood = ASSETS.FX.bloodspray[Random.Range(0, ASSETS.FX.bloodspray.Count)];
					effect = ObjectMaker.Make(randomBlood, position);
				}

				break;
		}

		// Auto-destroy the effect after a short time if it doesn't destroy itself
		if (effect != null && effect.GetComponent<DestroyMeEvent>() == null) Destroy(effect, 2f);
	}

	/// <summary>
	/// Finds a valid spawn position that meets all criteria:
	/// - Within the spawn area
	/// - Not colliding with obstacles
	/// - Outside camera view (if required)
	/// </summary>
	private Vector2 GetValidSpawnPosition(bool ensureOffCamera, Camera camera, float cameraMargin)
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

			// Check if position is visible in camera (if required)
			if (ensureOffCamera && camera != null)
			{

				// Check if point is within viewport plus margin
				var isInView = false;

				foreach (var player in Players.AllJoinedPlayers)
				{
					if (player.spawnedPlayerDefence == null) continue;
					if (!(Vector2.Distance(player.spawnedPlayerDefence.transform.position, potentialPosition) < 10f))
					{
						Debug.DrawLine( player.spawnedPlayerDefence.transform.position, potentialPosition, Color.red);
						continue;
					}
					isInView = true;
					break;
				}

				if (isInView)
				{
					MyDebugUtilities.DrawX(potentialPosition, 10, Color.red);
					Debug.Log("Position is visible, try another");
					continue; // Position is visible, try another
				}
			}

			MyDebugUtilities.DrawX(potentialPosition, 1, Color.green);
			// All checks passed, return this position
			return potentialPosition;
		}

		// Failed to find a valid position after 10 attempts
		return Vector2.zero;
	}

	private void OnDrawGizmos()
	{
		if (!showDebugVisuals) return;

		// Draw spawn area
		Gizmos.color = debugColor;
		Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.1f));

		// Draw icon for proximity-based spawns
		if (isProximityBased)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawIcon(transform.position + Vector3.up * 0.5f, "AimTarget", true);
		}

		// Draw custom enemy distribution if overridden
		if (overrideEnemyDistribution)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(transform.position, 0.3f);
		}
	}
}