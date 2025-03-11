using UnityEngine;

public class LevelLoadInteraction : TimedInteraction
{
	[Header("Destination Settings"), SerializeField]
    private SceneDefinition _destinationScene;
    
    [SerializeField] private bool useLevelTransition = true;
    [SerializeField] private float interactionTime = 1.5f;

	[Header("Display Text"), Tooltip("Custom display name for the destination (overrides SceneDefinition)"), SerializeField]
	private string customDestinationName = "";

	[Tooltip("Display format used when prompting player (e.g. 'Go to {0}')"), SerializeField]
	private string promptFormat = "Go to {0}";

	[Header("Spawn Point"), Tooltip("ID of the exit spawn point in this scene"), SerializeField]
	private string exitPointId;

	[Tooltip("ID of the entry spawn point in the destination scene"), SerializeField]
	private string entryPointId;

	[Header("Persistent Characters"), Tooltip("Keep character instances between scenes instead of respawning"), SerializeField]
	private bool persistCharacters = true;

	// Cache of spawn points and destination name
	private SpawnPoint exitPoint;
	private string destinationDisplayName;

	// Cached player reference for saying lines
	private Player interactingPlayer;

	// Reference to interaction title component
	private InteractionTitle interactionTitle;
	
	// Flag to track if we've already initialized
	private bool isInitialized = false;

	private void Awake()
	{
		// Find our configured exit point (if any) in this scene
		if (!string.IsNullOrEmpty(exitPointId))
		{
			var spawnPoints = FindObjectsOfType<SpawnPoint>(true);

			foreach (var spawnPoint in spawnPoints)
			{
				if (spawnPoint.id == exitPointId)
				{
					exitPoint = spawnPoint;

					// Auto-configure if exit point is valid
					if (exitPoint != null)
					{
						// Set destination to match exit point if not already set
						if (_destinationScene == null)
						{
						    _destinationScene = exitPoint.DestinationScene;
						}

						// Set entry point ID if not already set
						if (string.IsNullOrEmpty(entryPointId)) entryPointId = exitPoint.connectedSpawnPointId;
					}

					break;
				}
			}
		}

		// Get the destination display name
		UpdateDestinationDisplayName();
	}

	private void UpdateDestinationDisplayName()
	{
		// This method should only be called in play mode
		if (!Application.isPlaying)
		{
#if UNITY_EDITOR
			// In edit mode, use the edit-mode specific method instead
			UpdateDisplayNameEditMode();
#endif
			return;
		}

		// If a custom name is provided, use that
		if (!string.IsNullOrEmpty(customDestinationName))
		{
			destinationDisplayName = customDestinationName;
		}
		else if (_destinationScene != null)
		{
			// Use the SceneDefinition's display name
			destinationDisplayName = _destinationScene.DisplayName;
		}

		// Update the interaction title if available
		if (interactionTitle != null) interactionTitle.SetTitle(string.Format(promptFormat, destinationDisplayName));
	}

	// Convert CamelCase to spaced words (e.g., "MainMenu" to "Main Menu")
	private string FormatSceneTypeName(string typeName)
	{
		var result = "";
		for (var i = 0; i < typeName.Length; i++)
		{
			if (i > 0 && char.IsUpper(typeName[i])) result += " ";
			result += typeName[i];
		}

		return result;
	}

	protected override void Start()
	{
		// Set the total time required to complete the interaction
		totalTime = interactionTime;

		// Try to get or add the InteractionTitle component (only in play mode)
		interactionTitle = GetComponent<InteractionTitle>();
		if (interactionTitle == null && Application.isPlaying) interactionTitle = gameObject.AddComponent<InteractionTitle>();

		// Update the destination display name if needed
		if (string.IsNullOrEmpty(destinationDisplayName)) UpdateDestinationDisplayName();

		// Check if ASSETS is ready before calling base.Start which uses ASSETS.FX
		if (ASSETS.I == null || ASSETS.FX == null)
		{
			// Delay initialization
			Invoke(nameof(DelayedInit), 0.1f);
			return;
		}
		
		CompleteStart();
	}
	
	private void DelayedInit()
	{
		if (isInitialized) return;
		
		// Check if ASSETS is ready now
		if (ASSETS.I == null || ASSETS.FX == null)
		{
			// Still not ready, try again later
			Invoke(nameof(DelayedInit), 0.1f);
			return;
		}
		
		CompleteStart();
	}
	
	private void CompleteStart()
	{
		if (isInitialized) return;
		isInitialized = true;
		
		// Call base.Start() which will initialize the loading bar
		base.Start();

		// Subscribe to the completion event
		OnTimeComplete += OnLevelLoadComplete;

		// Subscribe to action press/release events to handle player saying
		OnActionPress += OnPlayerBeginInteraction;
		OnActionRelease += OnPlayerCancelInteraction;
	}

	private void OnPlayerBeginInteraction(Player player)
	{
		interactingPlayer = player;

		// Make the player say where they're going
		if (player != null && player.SpawnedPlayerGO != null)
		{
			// Only say line if we have a player sayer component
			var sayer = player.GetComponentInChildren<PlayerSayer>();
			if (sayer != null) sayer.Say(string.Format(promptFormat, destinationDisplayName));
		}
	}

	private void OnPlayerCancelInteraction(Player player)
	{
		interactingPlayer = null;
	}

	private void OnLevelLoadComplete(Player player)
	{
		// Verify exit point is valid
		if (exitPoint == null && !string.IsNullOrEmpty(exitPointId))
			Debug.LogWarning($"LevelLoadInteraction: Exit point {exitPointId} not found in scene", this);

		// Register the transition with SceneLoader (only in play mode)
		if (Application.isPlaying && SceneLoader.I != null)
		{
			// Use the spawn point ID (which is the exit point from this scene)
			if (!string.IsNullOrEmpty(exitPointId))
			{
				// Configure the transition with connected points
				SceneLoader.I.SetTransitionId(exitPointId);
				string destName = _destinationScene.SceneName;
				Debug.Log($"LevelLoadInteraction: Transitioning from {exitPointId} to {entryPointId} in {destName}");
			}
		}

		// Set character persistence flag
		if (persistCharacters)
		{
			// Flag to keep character instances
			Players.ShouldPersistCharacters = true;
		}

		// Load the destination scene
		if (_destinationScene != null)
		{
			// Use SceneDefinition
			_destinationScene.Load(useLevelTransition);
		}
	}

	// Clean up event subscriptions when destroyed
	private void OnDestroy()
	{
		OnTimeComplete -= OnLevelLoadComplete;
		OnActionPress -= OnPlayerBeginInteraction;
		OnActionRelease -= OnPlayerCancelInteraction;
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		// Update display name in editor after inspector changes
		UpdateDisplayNameEditMode();
	}

	private void UpdateDisplayNameEditMode()
	{
		// Only call in edit mode
		if (Application.isPlaying)
			return;
		
		// In edit mode, we can't access ASSETS.Scenes or SceneDefinitionManager,
		// so we build a basic preview based on available information
		
		// If a custom name is provided, use that
		if (!string.IsNullOrEmpty(customDestinationName))
		{
			destinationDisplayName = customDestinationName;
		}
		else if (_destinationScene != null)
		{
			// Use the SceneDefinition's display name or name
			destinationDisplayName = !string.IsNullOrEmpty(_destinationScene.displayName) ? 
				_destinationScene.displayName : _destinationScene.name;
		}
		else
		{
			// Fallback to formatted scene type name
			destinationDisplayName = _destinationScene.DisplayName;
		}
	}
#endif
}