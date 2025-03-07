using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[Serializable]
public class LevelTransitionData
{
    public string sceneName;
    public string locationTitle;
    public Sprite locationImage;
}

public class SceneLoader : Singleton<SceneLoader>
{
    private AsyncOperation loadingOperation;
    
    // Progress indicator references
    public Image progressBarImage; // This will be used for both regular progress and radial progress
    public TextMeshProUGUI percentLoadedText;
    
    // Scene transition UI elements
    public TextMeshProUGUI locationTitleText;
    public Image locationImage;
    public GameObject pressAnyButtonText;
    
    // Screen GameObjects
    public GameObject loadingScreen;
    public GameObject levelTransitionScreen;
    
    private bool isLoading;
    private bool loadingComplete;
    private bool waitingForInput;

    private GameScene.Type DestinationScene;
    [SerializeField] private GameScene.Type StartingScene;
    [SerializeField] private LevelTransitionData[] levelTransitions;

    public Animator faderAnimator;
    public static bool hasLoaded;
    private static readonly int IsFadedIn = Animator.StringToHash("IsFadedIn");

    // Dictionary to quickly look up level transition data
    private System.Collections.Generic.Dictionary<string, LevelTransitionData> transitionDataDict;
    
    // Reference to the SceneDefinitionManager
    private SceneDefinitionManager sceneDefinitionManager;
    
    // Input management
    private PlayerControls playerControls;
    private InputAction anyButtonAction;

    private void OnEnable()
    {
        // Initialize input
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            SetupInputActions();
        }
        
        playerControls.Enable();
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Disable();
        }
    }

    private void SetupInputActions()
    {
        // Create an action that will trigger on any button/key press
        anyButtonAction = new InputAction(binding: "/*/<button>", type: InputActionType.Button);
        anyButtonAction.performed += OnAnyButtonPressed;
        anyButtonAction.Enable();
    }

    private void OnAnyButtonPressed(InputAction.CallbackContext context)
    {
        if (waitingForInput && loadingComplete)
        {
            // Allow scene activation and complete the loading process
            loadingOperation.allowSceneActivation = true;
            waitingForInput = false;
            isLoading = false;
            FadeOut();
        }
    }

    private void Start()
    {
        // Initialize the dictionary
        transitionDataDict = new System.Collections.Generic.Dictionary<string, LevelTransitionData>();
        if (levelTransitions != null)
        {
            foreach (var transition in levelTransitions)
            {
                transitionDataDict[transition.sceneName] = transition;
            }
        }

        // Hide the "press any button" text initially
        if (pressAnyButtonText != null)
            pressAnyButtonText.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
    }

    public void FadeOutComplete()
    {
        loadingScreen.SetActive(false);
        if (levelTransitionScreen != null)
            levelTransitionScreen.SetActive(false);
    }

    public void FadeInComplete()
    {
        LoadScene(DestinationScene);
        Debug.Log("load scene");
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject); // Changed to use gameObject instead of this
        SceneManager.sceneLoaded += FirstFade;
        
        // Ensure SceneDefinitionManager is initialized
        if (SceneDefinitionManager.Instance == null)
        {
            var go = new GameObject("SceneDefinitionManager");
            go.transform.SetParent(transform);
            sceneDefinitionManager = go.AddComponent<SceneDefinitionManager>();
        }
        else
        {
            sceneDefinitionManager = SceneDefinitionManager.Instance;
        }
        
        // Don't load the scene if we're already in a scene other than GameManager
        SceneManager.LoadScene(StartingScene.ToString());
    }

    private void FirstFade(Scene arg0, LoadSceneMode arg1)
    {
        FadeOut();
        hasLoaded = true;
        SceneManager.sceneLoaded -= FirstFade;
    }

    private void StartFadingIn(bool isLevelTransition = false)
    {
        if (isLevelTransition && levelTransitionScreen != null)
        {
            // Setup level transition UI with location details
            levelTransitionScreen.SetActive(true);
            loadingScreen.SetActive(false);
            
            // First try to get data from SceneDefinitionManager
            SceneDefinition sceneDefinition = null;
            if (sceneDefinitionManager != null)
            {
                sceneDefinition = sceneDefinitionManager.GetDefinitionByType(DestinationScene);
            }
            
            if (sceneDefinition != null)
            {
                // Use scene definition data if available
                if (locationTitleText != null)
                    locationTitleText.text = sceneDefinition.displayName;
                
                if (locationImage != null && sceneDefinition.sceneImage != null)
                {
                    locationImage.sprite = sceneDefinition.sceneImage;
                    locationImage.gameObject.SetActive(true);
                }
                else if (locationImage != null)
                {
                    // Fall back to the old transition data if no image in definition
                    bool foundImage = false;
                    
                    if (transitionDataDict != null && transitionDataDict.TryGetValue(DestinationScene.ToString(), out LevelTransitionData data) && data.locationImage != null)
                    {
                        locationImage.sprite = data.locationImage;
                        locationImage.gameObject.SetActive(true);
                        foundImage = true;
                    }
                    
                    if (!foundImage)
                        locationImage.gameObject.SetActive(false);
                }
            }
            else
            {
                // Fall back to old transition data if no scene definition
                if (transitionDataDict != null && transitionDataDict.TryGetValue(DestinationScene.ToString(), out LevelTransitionData data))
                {
                    if (locationTitleText != null)
                        locationTitleText.text = data.locationTitle;
                    
                    if (locationImage != null && data.locationImage != null)
                    {
                        locationImage.sprite = data.locationImage;
                        locationImage.gameObject.SetActive(true);
                    }
                    else if (locationImage != null)
                    {
                        locationImage.gameObject.SetActive(false);
                    }
                }
                else
                {
                    // Default values if no specific data is found
                    if (locationTitleText != null)
                        locationTitleText.text = DestinationScene.ToString();
                    
                    if (locationImage != null)
                        locationImage.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            // Regular loading screen
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

    public void SetDestinationScene(GameScene.Type newScene, bool isLevelTransition = false)
    {
        DestinationScene = newScene;
        Debug.Log("destination scene set to " + DestinationScene);
        StartFadingIn(isLevelTransition);
    }

    private void LoadScene(GameScene.Type newScene)
    {
        loadingOperation = SceneManager.LoadSceneAsync(newScene.ToString());
        
        // Determine if this is a level transition or a menu transition
        bool isLevel = newScene == GameScene.Type.InLevel;
        bool isLevelToLevel = DestinationScene == GameScene.Type.InLevel && 
                             SceneManager.GetActiveScene().name.Contains("Level");
        
        // Only wait for input if going to or between levels
        if (isLevel || isLevelToLevel)
        {
            // Prevent automatic activation to allow "press any button to continue"
            loadingOperation.allowSceneActivation = false;
        }
        else
        {
            // For menu transitions, don't wait for user input
            loadingOperation.allowSceneActivation = true;
        }
        
        isLoading = true;
    }

    private void Update()
    {
        if (!isLoading) return;

        var progressValue = ShowLoadingProgress();
        
        // For scenes that don't require button press, we complete when loading is done
        if (loadingOperation.allowSceneActivation && progressValue >= 1.0f)
        {
            isLoading = false;
            FadeOut();
            return;
        }
        
        // For scenes that require button press
        if (!loadingComplete && !loadingOperation.allowSceneActivation)
        {
            // When progress reaches 0.9 (90%), loading is essentially complete
            // Unity keeps the last 10% for activation
            if (progressValue >= 0.9f)
            {
                loadingComplete = true;
                
                // Show "press any button to continue" message
                if (pressAnyButtonText != null)
                {
                    pressAnyButtonText.gameObject.SetActive(true);
                    
                    // Update the percentage to show 100% when ready
                    if (percentLoadedText != null)
                        percentLoadedText.text = "100";
                }
                
                waitingForInput = true;
            }
        }
        // Note: Input handling for "press any button" is done by the Input System
    }

    private float ShowLoadingProgress()
    {
        // Progress is clamped at 0.9 until allowSceneActivation is true
        var progressValue = Mathf.Clamp01(loadingOperation.progress / 0.9f);
        
        // Update the progress bar (works for both regular and radial progress bars)
        if (progressBarImage != null)
            progressBarImage.fillAmount = progressValue;
            
        // Update percentage text
        if (percentLoadedText != null)
            percentLoadedText.text = Mathf.Round(progressValue * 100).ToString();
            
        return progressValue;
    }

    private void FadeOut()
    {
        faderAnimator.SetBool(IsFadedIn, false);
        
        // Hide "press any button" message when fading out
        if (pressAnyButtonText != null)
            pressAnyButtonText.gameObject.SetActive(false);
    }
}