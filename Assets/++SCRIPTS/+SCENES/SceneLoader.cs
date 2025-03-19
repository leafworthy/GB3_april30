using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Integrated scene transition system that handles both scene loading and player spawn positions
/// </summary>
public class SceneLoader : Singleton<SceneLoader>
{
	// UI References
	[Header("UI References"), SerializeField]
	private Animator faderAnimator;
	[SerializeField] private GameObject loadingScreen;
	[SerializeField] private GameObject levelTransitionScreen;
	[SerializeField] private Image progressBarImage;
	[SerializeField] private TextMeshProUGUI percentLoadedText;
	[SerializeField] private TextMeshProUGUI locationTitleText;
	[SerializeField] private Image locationImage;
	[SerializeField] private GameObject pressAnyButtonText;

	// State tracking
	private AsyncOperation loadingOperation;
	private bool isLoading;
	private bool loadingComplete;
	private bool waitingForInput;
	private static readonly int IsFadedIn = Animator.StringToHash("IsFadedIn");

	// Scene transition state tracking
	private SceneDefinition previoustScene;
	private SceneDefinition currentScene;
	private SceneDefinition destinationScene;

	private bool showImageAndTitleTransition;
	private string lastConnectedId;

	// Static properties
	public bool hasLoaded; // Made public for SceneStarter
	public bool gameHasStarted;
	// Input handling
	private PlayerControls playerControls;
	private InputAction anyButtonAction;

	public static event Action<SceneDefinition> OnSceneLoaded;

	#region Lifecycle Methods

	protected void Start()
	{
		SetScene(ASSETS.Scenes.mainMenu);
	}

	private void SetScene(SceneDefinition sceneDefinition)
	{
		currentScene = sceneDefinition;
		SceneManager.LoadScene(currentScene.sceneName);
		if (sceneDefinition.isGameplayLevel)
			pressAnyButtonText.gameObject.SetActive(false);
	}

	public void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
	}

	private void OnEnable()
	{
		DontDestroyOnLoad(gameObject);
		gameHasStarted = true;
		Debug.Log("dont destroy");
		// Set up input for button press detection
		if (playerControls == null)
		{
			playerControls = new PlayerControls();
			anyButtonAction = new InputAction(binding: "/*/<button>", type: InputActionType.Button);
			anyButtonAction.performed += OnAnyButtonPressed;
			anyButtonAction.Enable();
		}

		SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
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
		SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
	}

	private void Update()
	{
		if (!isLoading) return;

		var progressValue = UpdateLoadingProgress();

		// For scenes that don't require button press
		if (loadingOperation.allowSceneActivation && progressValue >= 1.0f)
		{
			Debug.Log("scene loaded, time to fade out");
			isLoading = false;
			FadeOut();
			return;
		}

		// For scenes that require "press any button"
		if (!loadingComplete && !loadingOperation.allowSceneActivation && progressValue >= 0.9f) LoadingComplete();
	}

	private void LoadingComplete()
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

	#endregion

	public SceneDefinition GetCurrentSceneDefinition()
	{
		var sceneName = SceneManager.GetActiveScene().name;
		return ASSETS.GetSceneByName(sceneName);
	}

	#region Public Scene Loading Methods

	/// <summary>
	/// Set the destination scene using a SceneDefinition
	/// </summary>
	public void GoToScene(SceneDefinition sceneDefinition)
	{
		if (sceneDefinition == null || !sceneDefinition.IsValid())
		{
			Debug.LogError("Invalid scene definition");
			return;
		}

		// Store the destination
		destinationScene = sceneDefinition;
		showImageAndTitleTransition = sceneDefinition.isGameplayLevel;

		Debug.Log($"Loading scene: {sceneDefinition.DisplayName}");

		// Update internal tracking 
		SetCurrentScene(sceneDefinition);

		// Start the transition
		StartFadingIn();
	}

	#endregion

	/// <summary>
	/// Update the current scene reference (called when a new scene is loaded)
	/// </summary>
	private void SetCurrentScene(SceneDefinition scene)
	{
		if (scene == null) return;
		currentScene = scene;
		Debug.Log($"Current scene set to {scene.DisplayName} ({scene.SceneName})");
	}

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
	/// Called when a scene is loaded
	/// </summary>
	private void SceneManager_OnSceneLoaded(Scene scene, LoadSceneMode mode)
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
		OnSceneLoaded?.Invoke(currentScene);
	}

	/// <summary>
	/// Start the transition with fading animation
	/// </summary>
	private void StartFadingIn()
	{
		Debug.Log("start fading in");
		// Setup the appropriate screen
		if (showImageAndTitleTransition && levelTransitionScreen != null)
			SetupLevelTransitionScreen();
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
				locationImage.gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Load the scene asynchronously
	/// </summary>
	private void LoadScene()
	{
		Debug.Log("loading scene");
		// Get scene name from the destination
		var sceneName = destinationScene.SceneName;

		// Start async loading
		loadingOperation = SceneManager.LoadSceneAsync(sceneName);

		// Check if this scene requires player input before continuing
		// Use explicit setting from SceneDefinition first
		var requiresButtonPress = destinationScene.requiresButtonPressToLoad;

		// If not explicitly set, determine based on gameplay level status 
		if (!requiresButtonPress)
		{
			// Check if this is a level-to-level transition which should pause
			var isCurrentLevelGameplay = currentScene != null && currentScene.isGameplayLevel;

			// Require button press for level-to-level transitions
			if (destinationScene.isGameplayLevel && isCurrentLevelGameplay) requiresButtonPress = true;
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
		if (waitingForInput && loadingComplete) StopWaitingForInput();
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
		Debug.Log("fade out here");
		faderAnimator.SetBool(IsFadedIn, false);

		if (pressAnyButtonText != null)
			pressAnyButtonText.gameObject.SetActive(false);
	}

	#endregion
}