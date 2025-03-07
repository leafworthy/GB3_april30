using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : Singleton<PlayerManager>
{
    // Character persistence flag
    public static bool ShouldPersistCharacters { get; set; } = false;
    
    // Track which scene players died in to restart from there
    private static GameScene.Type lastDeathScene = GameScene.Type.InLevel;
    
    // Event to notify when the player manager is ready
    public static event Action OnPlayerManagerReady;
    
    // Track player characters and their hidden/active state
    private Dictionary<Player, GameObject> persistentCharacters = new Dictionary<Player, GameObject>();
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        
        // Subscribe to scene loading events
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Subscribe to player deaths to track restart level
        Players.OnAllJoinedPlayersDead += OnAllPlayersDead;
    }
    
    private void Start()
    {
        // Let other systems know we're ready
        OnPlayerManagerReady?.Invoke();
    }
    
    // Properly clean up event subscriptions
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Players.OnAllJoinedPlayersDead -= OnAllPlayersDead;
    }
    
    // When all players die, store current scene for restart
    private void OnAllPlayersDead()
    {
        // Store the current scene as the restart point
        GameScene currentGameScene = FindObjectOfType<GameScene>();
        if (currentGameScene != null)
        {
            lastDeathScene = currentGameScene.SceneType;
        }
    }
    
    // When a scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // First check if this is a level restart
        if (scene.name == GameScene.Type.RestartLevel.ToString())
        {
            // Instead of going to RestartLevel, go back to the last death scene
            StartCoroutine(DelayedLoadScene(lastDeathScene));
            return;
        }
        
        // Update character visibility based on scene type
        GameScene gameScene = FindObjectOfType<GameScene>();
        if (gameScene != null)
        {
            if (gameScene is LevelGameScene levelScene)
            {
                // This is a level, handle character persistence
                HandleLevelSceneLoaded(levelScene);
            }
            else
            {
                // For non-level scenes, hide all persistent characters
                HideAllPersistentCharacters();
            }
        }
    }
    
    // Coroutine to load a scene after a short delay
    private System.Collections.IEnumerator DelayedLoadScene(GameScene.Type sceneType)
    {
        yield return new WaitForSeconds(0.1f);
        SceneLoader.I.SetDestinationScene(sceneType, true);
    }
    
    // Handle level scene initialization
    private void HandleLevelSceneLoaded(LevelGameScene levelScene)
    {
        if (ShouldPersistCharacters)
        {
            // Activate existing character instances
            ActivatePersistentCharacters(levelScene);
            
            // Reset persistence flag for next transition
            ShouldPersistCharacters = false;
        }
        else
        {
            // Clean up any existing persistent characters
            CleanUpPersistentCharacters();
            
            // Let the level game scene create fresh characters
            // No need to interfere
        }
    }
    
    // Store a player's character for persistence between scenes
    public void RegisterPersistentCharacter(Player player, GameObject characterInstance)
    {
        if (player == null || characterInstance == null) return;
        
        // If character already registered, update it
        if (persistentCharacters.ContainsKey(player))
        {
            // Destroy old instance if exists
            if (persistentCharacters[player] != null)
            {
                Destroy(persistentCharacters[player]);
            }
        }
        
        // Store the character and mark DontDestroyOnLoad
        persistentCharacters[player] = characterInstance;
        DontDestroyOnLoad(characterInstance);
    }
    
    // Activate persistent characters in a level
    private void ActivatePersistentCharacters(LevelGameScene levelScene)
    {
        // Get players who have joined
        List<Player> activePlayers = Players.AllJoinedPlayers;
        
        // Get spawn positions for all players from LevelTransition
        List<Vector2> spawnPositions = new List<Vector2>();
        if (LevelTransition.I != null)
        {
            spawnPositions = LevelTransition.I.GetSpawnPositions(levelScene.SceneType, activePlayers.Count);
        }
        
        // If we didn't get spawn positions, use level's default ones
        bool useDefaultSpawnPoints = spawnPositions.Count == 0 || spawnPositions[0] == Vector2.zero;
        
        for (int i = 0; i < activePlayers.Count; i++)
        {
            Player player = activePlayers[i];
            
            if (persistentCharacters.TryGetValue(player, out GameObject characterGO) && characterGO != null)
            {
                // Position the character at the appropriate spawn point
                if (!useDefaultSpawnPoints && i < spawnPositions.Count)
                {
                    // Use configured spawn positions from transition data
                    characterGO.transform.position = spawnPositions[i];
                }
                else
                {
                    // Use level's default spawn points - levelScene will handle this
                    // Just activate the character here
                }
                
                // Activate the character
                characterGO.SetActive(true);
                
                // Update player reference to this character
                player.SpawnedPlayerGO = characterGO;
                player.state = Player.State.Alive;
                
                // Setup life component references
                player.spawnedPlayerDefence = characterGO.GetComponent<Life>();
                if (player.spawnedPlayerDefence != null)
                {
                    player.spawnedPlayerDefence.OnDead += player.OnPlayerDied;
                    player.spawnedPlayerDefence.SetPlayer(player);
                }
                
                // Add to camera
                levelScene.AddToCameraFollow(player);
                
                Debug.Log($"Activated persistent character for Player {player.playerIndex} at position {characterGO.transform.position}");
            }
            else
            {
                // Character not found or was destroyed, create a new one
                if (!useDefaultSpawnPoints && i < spawnPositions.Count)
                {
                    // Pass specific spawn position
                    levelScene.SpawnPlayer(player, true, spawnPositions[i]);
                }
                else
                {
                    // Use default spawn point
                    levelScene.SpawnPlayer(player, true);
                }
            }
        }
    }
    
    // Hide all persistent characters (for menus, etc)
    private void HideAllPersistentCharacters()
    {
        foreach (var characterGO in persistentCharacters.Values)
        {
            if (characterGO != null)
            {
                characterGO.SetActive(false);
            }
        }
    }
    
    // Clean up persistent characters (for new game or when persistence isn't needed)
    public void CleanUpPersistentCharacters()
    {
        foreach (var characterGO in persistentCharacters.Values)
        {
            if (characterGO != null)
            {
                Destroy(characterGO);
            }
        }
        
        persistentCharacters.Clear();
    }
    
    // Check if a player has a persistent character
    public bool HasPersistentCharacter(Player player)
    {
        return persistentCharacters.ContainsKey(player) && persistentCharacters[player] != null;
    }
}