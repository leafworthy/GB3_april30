using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Data for spawn points that can be used for level transitions
/// </summary>
[Serializable]
public class SpawnPointData
{
    public string id;
    public string sourceSceneName;
    public string destinationSceneName;
    public string connectedId; // ID of connected spawn point in destination scene
    public Vector2 position;
    public SpawnPointType pointType = SpawnPointType.Both;
    public int capacity = 4;

    // Helper to get the source scene definition
    public SceneDefinition GetSourceSceneDefinition() =>
        ASSETS.GetSceneByName(sourceSceneName);

    // Helper to get the destination scene definition
    public SceneDefinition GetDestinationSceneDefinition() =>
        ASSETS.GetSceneByName(destinationSceneName);
}

/// <summary>
/// Integrated scene transition system that handles both scene loading and player spawn positions
/// </summary>
public class SceneLoader : Singleton<SceneLoader>
{
    // UI References
    [Header("UI References")]
    [SerializeField] private Animator faderAnimator;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject levelTransitionScreen;
    [SerializeField] private Image progressBarImage;
    [SerializeField] private TextMeshProUGUI percentLoadedText;
    [SerializeField] private TextMeshProUGUI locationTitleText;
    [SerializeField] private Image locationImage;
    [SerializeField] private GameObject pressAnyButtonText;
    
    // Configuration
    [Header("Configuration")]
    [SerializeField] private SceneDefinition startingScene;
    
    // Spawn point system (merged from LevelTransition)
    [Header("Spawn Points")]
    [SerializeField] private List<SpawnPointData> spawnPoints = new List<SpawnPointData>();
    
    // State tracking
    private AsyncOperation loadingOperation;
    private bool isLoading;
    private bool loadingComplete;
    private bool waitingForInput;
    private static readonly int IsFadedIn = Animator.StringToHash("IsFadedIn");
    
    // Scene transition state tracking
    private SceneDefinition currentScene;
    private SceneDefinition destinationScene;
    private bool isLevelTransition;
    private string lastConnectedId;
    private bool isFirstLevelLoad = true;
    
    // Dictionaries for spawn point lookups
    private Dictionary<string, SpawnPointData> spawnPointsById = new Dictionary<string, SpawnPointData>();
    private Dictionary<string, List<SpawnPointData>> spawnPointsByScene = new Dictionary<string, List<SpawnPointData>>();
    
    // Static properties
    public static bool hasLoaded; // Made public for SceneStarter
    
    // Input handling
    private PlayerControls playerControls;
    private InputAction anyButtonAction;
    
    #region Lifecycle Methods
    
    protected override void Awake()
    {
        base.Awake();
        
        // Only once when in play mode
        if (Application.isPlaying)
        {
            // Set up as a persistent manager
            DontDestroyOnLoad(gameObject);
            
            // Initialize dictionaries for spawn points
            InitializeSpawnPointDictionaries();
            
            // Subscribe to scene loaded events
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // Load the starting scene
            if (startingScene != null && startingScene.IsValid())
            {
                SceneManager.LoadScene(startingScene.SceneName);
                
                // Set as current scene
                currentScene = startingScene;
            }
            else if (ASSETS.Scenes != null && ASSETS.Scenes.mainMenu != null)
            {
                // Fallback to main menu
                SceneManager.LoadScene(ASSETS.Scenes.mainMenu.SceneName);
                
                // Set as current scene
                currentScene = ASSETS.Scenes.mainMenu;
            }
        }
    }
    
    private void Start()
    {
        // Hide the press any button text initially
        if (pressAnyButtonText != null)
            pressAnyButtonText.gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        // Set up input for button press detection
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            anyButtonAction = new InputAction(binding: "/*/<button>", type: InputActionType.Button);
            anyButtonAction.performed += OnAnyButtonPressed;
            anyButtonAction.Enable();
        }
        
        playerControls.Enable();
    }
    
    private void OnDisable()
    {
        // Clean up input
        if (playerControls != null)
            playerControls.Disable();
            
        if (anyButtonAction != null)
            anyButtonAction.Disable();
            
        // Unsubscribe from events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void Update()
    {
        if (!isLoading) return;

        var progressValue = UpdateLoadingProgress();
        
        // For scenes that don't require button press
        if (loadingOperation.allowSceneActivation && progressValue >= 1.0f)
        {
            isLoading = false;
            FadeOut();
            return;
        }
        
        // For scenes that require "press any button"
        if (!loadingComplete && !loadingOperation.allowSceneActivation && progressValue >= 0.9f)
        {
            loadingComplete = true;
            if (percentLoadedText != null)
                percentLoadedText.text = "100";
            // Show press any button message
            if (pressAnyButtonText != null)
            {
                pressAnyButtonText.gameObject.SetActive(true);
                waitingForInput = true;
            }
            else
            {
                waitingForInput = false;
                StopWaitingForInput();
            }

          
           
        }
    }
    
    #endregion
    
    #region Public Scene Loading Methods
    
    /// <summary>
    /// Set the destination scene using a SceneDefinition
    /// </summary>
    public void GoToScene(SceneDefinition sceneDefinition, bool useLevelTransition = false)
    {
        if (sceneDefinition == null || !sceneDefinition.IsValid())
        {
            Debug.LogError("Invalid scene definition");
            return;
        }
        
        // Store the destination
        destinationScene = sceneDefinition;
        isLevelTransition = useLevelTransition;
        
        Debug.Log($"Loading scene: {sceneDefinition.DisplayName}");
        
        // Update internal tracking 
        SetCurrentScene(sceneDefinition);
        
        // Start the transition
        StartFadingIn();
    }
    
   
    
    
    #endregion
    
    #region Spawn Point System (from LevelTransition)
    
    /// <summary>
    /// Update the current scene reference (called when a new scene is loaded)
    /// </summary>
    public void SetCurrentScene(SceneDefinition scene)
    {
        if (scene != null)
        {
            currentScene = scene;
            Debug.Log($"Current scene set to {scene.DisplayName} ({scene.SceneName})");
        }
    }
    
    /// <summary>
    /// Set a transition ID when moving between levels via spawn points
    /// </summary>
    public void SetTransitionId(string transitionId)
    {
         isFirstLevelLoad = false;

        // Store the connected ID for the destination scene
        if (spawnPointsById.TryGetValue(transitionId, out var spawnData))
        {
            lastConnectedId = spawnData.connectedId;

            // Get and log destination info
            var destinationName = spawnData.destinationSceneName;
            Debug.Log($"Scene Transition: From {transitionId} to {spawnData.connectedId} in {destinationName}");
        }
    }
    
    /// <summary>
    /// Get spawn positions for multiple players in the current scene
    /// </summary>
    public List<Vector2> GetSpawnPositions(SceneDefinition currentScene, int playerCount)
    {
        if (currentScene != null && !string.IsNullOrEmpty(currentScene.SceneName))
            return GetSpawnPositions(currentScene.SceneName, playerCount);

        // Fallback to empty positions
        var defaultPositions = new List<Vector2>();
        for (var i = 0; i < playerCount; i++) defaultPositions.Add(Vector2.zero);
        return defaultPositions;
    }
    
    /// <summary>
    /// Get spawn positions using the current scene name
    /// </summary>
    public List<Vector2> GetSpawnPositions(string sceneName, int playerCount)
    {
        // Default positions in case we don't find suitable spawn points
        var positions = new List<Vector2>();
        for (var i = 0; i < playerCount; i++) positions.Add(Vector2.zero);

        // If this is the first level load, use default spawn points
        if (isFirstLevelLoad) return positions;

        // Try to find the connected spawn point in the destination scene
        if (!string.IsNullOrEmpty(lastConnectedId))
        {
            SpawnPointData destinationPoint = null;

            // Look for exact spawn point by ID
            foreach (var point in spawnPoints)
            {
                if (point.id == lastConnectedId && (point.sourceSceneName == sceneName) &&
                    (point.pointType == SpawnPointType.Entry || point.pointType == SpawnPointType.Both))
                {
                    destinationPoint = point;
                    break;
                }
            }

            if (destinationPoint != null)
            {
                // Calculate spawn positions around this point
                return CalculatePositionsAroundPoint(destinationPoint.position, playerCount, destinationPoint.capacity);
            }
        }

        // If connected ID not found, look for any entry points in this scene
        if (spawnPointsByScene.TryGetValue(sceneName, out var scenePoints))
        {
            var entryPoints = scenePoints.Where(p => p.pointType == SpawnPointType.Entry || p.pointType == SpawnPointType.Both).ToList();

            if (entryPoints.Count > 0)
            {
                // Use the first available entry point
                return CalculatePositionsAroundPoint(entryPoints[0].position, playerCount, entryPoints[0].capacity);
            }
        }
        
        // Return default positions if no suitable spawn points found
        return positions;
    }
    
    /// <summary>
    /// Register a new spawn point
    /// </summary>
    public void RegisterSpawnPoint(SpawnPointData newSpawnPoint)
    {
        if (string.IsNullOrEmpty(newSpawnPoint.id))
        {
            Debug.LogError("Spawn point ID cannot be empty");
            return;
        }
        
        // Check if ID already exists
        for (var i = 0; i < spawnPoints.Count; i++)
        {
            if (spawnPoints[i].id == newSpawnPoint.id && 
                (spawnPoints[i].sourceSceneName == newSpawnPoint.sourceSceneName))
            {
                // Update existing spawn point
                spawnPoints[i] = newSpawnPoint;
                spawnPointsById[newSpawnPoint.id] = newSpawnPoint;

                // Update string-based dictionary
                if (!string.IsNullOrEmpty(newSpawnPoint.sourceSceneName) &&
                    spawnPointsByScene.TryGetValue(newSpawnPoint.sourceSceneName, out var scenePoints))
                {
                    for (var j = 0; j < scenePoints.Count; j++)
                    {
                        if (scenePoints[j].id == newSpawnPoint.id)
                        {
                            scenePoints[j] = newSpawnPoint;
                            break;
                        }
                    }
                }
                
                return;
            }
        }

        // Add new spawn point
        spawnPoints.Add(newSpawnPoint);
        spawnPointsById[newSpawnPoint.id] = newSpawnPoint;

        // Add to scene dictionaries
        if (!string.IsNullOrEmpty(newSpawnPoint.sourceSceneName))
        {
            if (!spawnPointsByScene.ContainsKey(newSpawnPoint.sourceSceneName))
                spawnPointsByScene[newSpawnPoint.sourceSceneName] = new List<SpawnPointData>();
                
            spawnPointsByScene[newSpawnPoint.sourceSceneName].Add(newSpawnPoint);
        }
    }
    
    /// <summary>
    /// Check if this is a new game or continuing between levels
    /// </summary>
    public bool IsFirstLoad() => isFirstLevelLoad;
    

    
    /// <summary>
    /// Get the last connected spawn point ID
    /// </summary>
    public string GetLastConnectedId() => lastConnectedId;
    
    /// <summary>
    /// Find all entry points in a scene
    /// </summary>
    public List<SpawnPointData> GetEntryPoints(SceneDefinition scene)
    {
        if (scene != null && !string.IsNullOrEmpty(scene.SceneName) && 
            spawnPointsByScene.TryGetValue(scene.SceneName, out var scenePoints))
        {
            return scenePoints.Where(p => p.pointType == SpawnPointType.Entry || 
                                          p.pointType == SpawnPointType.Both).ToList();
        }

        return new List<SpawnPointData>();
    }
    
    /// <summary>
    /// Find all exit points in a scene
    /// </summary>
    public List<SpawnPointData> GetExitPoints(SceneDefinition scene)
    {
        if (scene != null && !string.IsNullOrEmpty(scene.SceneName) && 
            spawnPointsByScene.TryGetValue(scene.SceneName, out var scenePoints))
        {
            return scenePoints.Where(p => p.pointType == SpawnPointType.Exit || 
                                          p.pointType == SpawnPointType.Both).ToList();
        }

        return new List<SpawnPointData>();
    }
    
    #endregion
    
    #region Animation Callbacks
    
    /// <summary>
    /// Called by the fade-in animation when complete
    /// </summary>
    public void FadeInComplete()
    {
        LoadScene();
    }
    
    /// <summary>
    /// Called by the fade-out animation when complete
    /// </summary>
    public void FadeOutComplete()
    {
        loadingScreen.SetActive(false);
        if (levelTransitionScreen != null)
            levelTransitionScreen.SetActive(false);
    }
    
    #endregion
    
    #region Private Methods
    
    /// <summary>
    /// Initialize dictionaries for spawn point lookups
    /// </summary>
    private void InitializeSpawnPointDictionaries()
    {
        spawnPointsById.Clear();
        spawnPointsByScene.Clear();
        
        // Initialize dictionaries
        foreach (var spawnPoint in spawnPoints)
        {
            if (!string.IsNullOrEmpty(spawnPoint.id))
            {
                // Store by ID for direct lookup
                spawnPointsById[spawnPoint.id] = spawnPoint;

                // Store by scene name
                if (!string.IsNullOrEmpty(spawnPoint.sourceSceneName))
                {
                    if (!spawnPointsByScene.ContainsKey(spawnPoint.sourceSceneName))
                        spawnPointsByScene[spawnPoint.sourceSceneName] = new List<SpawnPointData>();
                        
                    spawnPointsByScene[spawnPoint.sourceSceneName].Add(spawnPoint);
                }
            }
        }
    }
    
    /// <summary>
    /// Called when a scene is loaded
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // First scene load needs a fade out
        if (!hasLoaded)
        {
            FadeOut();
            hasLoaded = true;
            return;
        }
        
        currentScene = ASSETS.GetSceneByName(scene.name);
        
        // Notify any listeners that need to know the scene changed
        Debug.Log($"Scene loaded: {scene.name}");
    }
    
    /// <summary>
    /// Start the transition with fading animation
    /// </summary>
    private void StartFadingIn()
    {
        Debug.Log("start fading in");
        // Setup the appropriate screen
        if (isLevelTransition && levelTransitionScreen != null)
        {
            SetupLevelTransitionScreen();
        }
        else
        {
            // Use regular loading screen
            loadingScreen.SetActive(true);
            if (levelTransitionScreen != null)
                levelTransitionScreen.SetActive(false);
        }
        
        // Start fade animation
        faderAnimator.SetBool(IsFadedIn, true);
        
        // Reset states
        loadingComplete = false;
        waitingForInput = false;
        
        if (pressAnyButtonText != null)
            pressAnyButtonText.gameObject.SetActive(false);
            
        // Reset progress indicators
        if (progressBarImage != null)
        {
            progressBarImage.fillAmount = 0f;
            progressBarImage.gameObject.SetActive(true);
        }
            
        if (percentLoadedText != null)
        {
            percentLoadedText.text = "0";
            percentLoadedText.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// Set up the level transition screen
    /// </summary>
    private void SetupLevelTransitionScreen()
    {
        levelTransitionScreen.SetActive(true);
        loadingScreen.SetActive(false);
        
        // Set title
        if (locationTitleText != null)
            locationTitleText.text = destinationScene.DisplayName;
        
        // Set image
        if (locationImage != null)
        {
            if (destinationScene.sceneImage != null)
            {
                locationImage.sprite = destinationScene.sceneImage;
                locationImage.gameObject.SetActive(true);
            }
            else
            {
                locationImage.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Load the scene asynchronously
    /// </summary>
    private void LoadScene()
    {
        Debug.Log("loading scene");
        // Get scene name from the destination
        string sceneName = destinationScene.SceneName;
        
        // Start async loading
        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        
        // Check if this scene requires player input before continuing
        // Use explicit setting from SceneDefinition first
        bool requiresButtonPress = destinationScene.requiresButtonPressToLoad;
        
        // If not explicitly set, determine based on gameplay level status 
        if (!requiresButtonPress)
        {
            // Check if this is a level-to-level transition which should pause
            bool isCurrentLevelGameplay = currentScene != null && currentScene.isGameplayLevel;
            
            // Require button press for level-to-level transitions
            if (destinationScene.isGameplayLevel && isCurrentLevelGameplay)
            {
                requiresButtonPress = true;
            }
        }
        
        // Set whether to wait for user input
        loadingOperation.allowSceneActivation = !requiresButtonPress;
        isLoading = true;
    }
    
    /// <summary>
    /// Update the loading progress indicators
    /// </summary>
    private float UpdateLoadingProgress()
    {
        // Progress is clamped at 0.9 until allowSceneActivation is true
        var progressValue = Mathf.Clamp01(loadingOperation.progress / 0.9f);
        
        // Update UI elements
        if (progressBarImage != null)
            progressBarImage.fillAmount = progressValue;
            
        if (percentLoadedText != null)
            percentLoadedText.text = Mathf.Round(progressValue * 100).ToString();
            
        return progressValue;
    }
    
    /// <summary>
    /// Handle button press during loading
    /// </summary>
    private void OnAnyButtonPressed(InputAction.CallbackContext context)
    {
        if (waitingForInput && loadingComplete)
        {
            StopWaitingForInput();
        }
    }

    private void StopWaitingForInput()
    {
        loadingOperation.allowSceneActivation = true;
        waitingForInput = false;
        isLoading = false;
        FadeOut();
    }

    /// <summary>
    /// Start the fade out animation
    /// </summary>
    private void FadeOut()
    {
        faderAnimator.SetBool(IsFadedIn, false);
        
        if (pressAnyButtonText != null)
            pressAnyButtonText.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Calculate spawn positions around a central point
    /// </summary>
    private List<Vector2> CalculatePositionsAroundPoint(Vector2 center, int count, int capacity)
    {
        var positions = new List<Vector2>();

        // Calculate positions in a small circle/cluster around this point
        var actualCount = Mathf.Min(count, capacity);
        var radius = 1.0f; // Base radius for positioning

        for (var i = 0; i < actualCount; i++)
        {
            if (i == 0)
            {
                // First player goes at the exact position
                positions.Add(center);
            }
            else
            {
                // Calculate positions in a circle
                var angle = i * (360f / actualCount) * Mathf.Deg2Rad;
                var offset = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
                positions.Add(center + offset);
            }
        }

        // Fill remaining positions with center position if more players than capacity
        for (var i = positions.Count; i < count; i++) 
            positions.Add(center);

        return positions;
    }
    
    #endregion
}