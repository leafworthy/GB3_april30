using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Players : Singleton<Players>
{
    [SerializeField] private PlayerData EnemyPlayerData;
    [SerializeField] public List<PlayerData> playerPresets = new();
    
    // Character persistence flag
    public static bool ShouldPersistCharacters { get; set; } = false;
    
    // Track which scene players died in to restart from there
    private static SceneDefinition lastDeathScene;
    
    private static PlayerInputManager _inputManager;
    private Player _enemyPlayer;
    
    public static Player EnemyPlayer => I._enemyPlayer;
    public static readonly List<Player> AllJoinedPlayers = new();
    
    // Action maps
    public static string UIActionMap = "UI";
    public static string PlayerActionMap = "PlayerMovement";
    
    // Events
    public static event Action<Player> OnPlayerGetUpgrades;
    public static event Action OnAllJoinedPlayersDead;
    public static event Action<Player> OnPlayerJoins;
    public static event Action OnPlayersReady;
    
    // Track player characters and their hidden/active state
    private Dictionary<Player, GameObject> persistentCharacters = new Dictionary<Player, GameObject>();
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        
        // Subscribe to scene loading events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void Start()
    {
        _inputManager = GetComponent<PlayerInputManager>();
        _inputManager.onPlayerJoined += Input_OnPlayerJoins;
        
        Player.OnPlayerDies += Player_PlayerDies;
        ZombieWaveManager.OnWaveEnd += PlayersGetUpgrades;
        
        // Create enemy player
        var enemy = new GameObject("EnemyPlayer");
        _enemyPlayer = enemy.AddComponent<Player>();
        _enemyPlayer.Join(null, EnemyPlayerData, 5);
        SetActionMaps(UIActionMap);
        
        // Notify any systems waiting for player initialization
        OnPlayersReady?.Invoke();
    }
    
    // Properly clean up event subscriptions
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Player.OnPlayerDies -= Player_PlayerDies;
        
        if (_inputManager != null)
        {
            _inputManager.onPlayerJoined -= Input_OnPlayerJoins;
        }
        
        ZombieWaveManager.OnWaveEnd -= PlayersGetUpgrades;
    }
    
    // Handle upgrades at the end of waves
    private void PlayersGetUpgrades()
    {
        foreach (var player in AllJoinedPlayers)
        {
            OnPlayerGetUpgrades?.Invoke(player);
            Debug.Log("players get upgrades!");
        }
    }
    
    // Clear joined players (typically used when starting a new game)
    public static void ClearAllJoinedPlayers()
    {
        Debug.Log("cleared all players");
        foreach (var player in AllJoinedPlayers)
        {
            player.gameObject.SetActive(false);
        }
        
        AllJoinedPlayers.Clear();
        
        // Also clean up any persistent characters
        I.CleanUpPersistentCharacters();
    }
    
    // Handle a new player joining
    private void Input_OnPlayerJoins(PlayerInput newPlayerInput)
    {
        var joiningPlayer = newPlayerInput.GetComponent<Player>();
        JoinPlayer(newPlayerInput, joiningPlayer);
    }
    
    // Join a player to the game
    private void JoinPlayer(PlayerInput newPlayerInput, Player joiningPlayer)
    {
        if (AllJoinedPlayers.Contains(joiningPlayer)) return;
        AllJoinedPlayers.Add(joiningPlayer);
        joiningPlayer.Join(newPlayerInput, playerPresets[newPlayerInput.playerIndex], newPlayerInput.playerIndex);
        OnPlayerJoins?.Invoke(joiningPlayer);
        //Debug.Log("PLAYER" + newPlayerInput.name + newPlayerInput.playerIndex + " JOINS FROM INPUT MANAGER");
    }
    
    // Handle player death
    private static void Player_PlayerDies(Player deadPlayer)
    {
        //Debug.Log("PLAYER" + deadPlayer.name + deadPlayer.playerIndex+" has died");
        
        if (AllJoinedPlayersAreDead())
        {
            // Store the current scene as the restart point
            GameScene currentGameScene = FindFirstObjectByType<GameScene>();
            if (currentGameScene != null)
            {
                lastDeathScene = currentGameScene.sceneDefinition;
            }
            
            OnAllJoinedPlayersDead?.Invoke();
        }
    }
    
    // Check if all players are dead
    private static bool AllJoinedPlayersAreDead()
    {
        var playersAlive = AllJoinedPlayers.Where(t => t.state == Player.State.Alive).ToList();
        if (playersAlive.Count > 0)
        {
            //Debug.Log("players still alive: " + playersAlive);
            return false;
        }
        
        //Debug.Log("all players are dead");
        return true;
    }
    
    // Get players who have selected a character
    public static List<Player> GetPlayersWhoSelectedACharacter()
    {
        return AllJoinedPlayers.Where(t => t.state == Player.State.Selected).ToList();
    }
    
    // Set action maps for all players
    public static void SetActionMaps(string actionMap)
    {
        foreach (var player in AllJoinedPlayers)
        {
            SetActionMap(player, actionMap);
        }
    }
    
    // Set action map for a specific player
    public static void SetActionMap(Player player, string actionMap)
    {
        player.input.SwitchCurrentActionMap(actionMap);
    }
    
    // Get a list of all players (for external systems)
    public List<Player> GetPlayers()
    {
        return AllJoinedPlayers;
    }
    
    #region Character Persistence - From PlayerManager
    
    // When a scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
       
        // Update character visibility based on scene type
        GameScene gameScene = FindFirstObjectByType<GameScene>();
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
        List<Player> activePlayers = AllJoinedPlayers;
        
        // Get spawn positions for all players from SceneLoader
        List<Vector2> spawnPositions = new List<Vector2>();
        if (SceneLoader.I != null)
        {
            spawnPositions = SceneLoader.I.GetSpawnPositions(levelScene.sceneDefinition, activePlayers.Count);
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
    
    #endregion
}