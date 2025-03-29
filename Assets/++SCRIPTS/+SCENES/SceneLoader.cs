using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace __SCRIPTS
{
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
		//[SerializeField] private Image locationImage;
		[SerializeField] private GameObject pressAnyButtonText;

		// State tracking
		private AsyncOperation loadingOperation;
		private bool isLoading;
		private bool loadingComplete;
		private bool waitingForInput;
		private static readonly int IsFadedIn = Animator.StringToHash("IsFadedIn");

		// Scene transition state tracking
		private SceneDefinition currentlyLoadedScene;
		private SceneDefinition loadingScene;

		// Static properties
		public bool hasLoadedForTheFirstTime; // Made public for SceneStarter

		// Input handling
		private PlayerControls playerControls;
		private InputAction anyButtonAction;

		public static event Action<SceneDefinition> OnSceneReadyToStartLevel;

		#region Lifecycle Methods

		protected void Start()
		{
			currentlyLoadedScene = ASSETS.Scenes.mainMenu;
			SceneManager.LoadScene(currentlyLoadedScene.sceneName);
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
				Debug.Log(loadingScene.displayName +"scene loaded, time to fade out");
				isLoading = false;
				FadeOut();
				return;
			}

			// For scenes that require "press any button"
			if (!loadingComplete && !loadingOperation.allowSceneActivation && progressValue >= 0.95f) LoadingComplete();
		}

		private void LoadingComplete()
		{
			Debug.Log("loading complete for scene: " + loadingScene.DisplayName);
			loadingComplete = true;
			if (percentLoadedText != null) percentLoadedText.text = "100";
			pressAnyButtonText.gameObject.SetActive(loadingScene.requiresButtonPressToLoad);

			if (loadingScene.requiresButtonPressToLoad)
			{
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
			return ASSETS.Scenes.GetByName(sceneName);
		}

		#region Public Scene Loading Methods

		/// <summary>
		/// Set the destination scene using a SceneDefinition
		/// </summary>
		public void GoToScene(SceneDefinition sceneDefinition)
		{
			//FROM LEVEL MANAGER
			if (sceneDefinition == null || !sceneDefinition.IsValid())
			{
				Debug.LogError("Invalid scene definition");
				return;
			}

			// Store the destination
			loadingScene = sceneDefinition;
			Debug.Log($"Loading scene: {sceneDefinition.DisplayName}");

			// Start the transition
			StartFadingIn();
		}

		#endregion

		/// <summary>
		/// Update the current scene reference (called when a new scene is loaded)
		/// </summary>
		private void SetCurrentScene(SceneDefinition scene)
		{
			if (scene == null)
			{
				Debug.Log("scene is null");
				return;
			}

			currentlyLoadedScene = scene;
			Debug.Log($"Current scene set to {currentlyLoadedScene.DisplayName} ({currentlyLoadedScene.SceneName})");
			OnSceneReadyToStartLevel?.Invoke(currentlyLoadedScene);
			loadingScene = null;
			isLoading = false;
		}

		#region Animation Callbacks

		/// <summary>
		/// Called by the fade-in animation when complete
		/// </summary>
		public void FadeInComplete()
		{
			StartLoadingSceneAsync();
		}

		public void FadeOutComplete()
		{
			loadingScreen.SetActive(false);
			if (levelTransitionScreen != null)
				levelTransitionScreen.SetActive(false);
		}

		#endregion

		#region Private Methods

		private void SceneManager_OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			// First scene load needs a fade out
			if (HandleFirstLoad()) return;
			isLoading = false;
			Debug.Log($"Scene manager load finished: {scene.name}");
			// Update internal tracking 
			SetCurrentScene(ASSETS.Scenes.GetByName(scene.name));
			//START LEVEL
		}

		private bool HandleFirstLoad()
		{
			if (!hasLoadedForTheFirstTime)
			{
				FadeOut();
				hasLoadedForTheFirstTime = true;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Start the transition with fading animation
		/// </summary>
		private void StartFadingIn()
		{
			Debug.Log("start fading in");
			// Setup the appropriate screen
			HandleTransitionScreen();

			// Start fade animation
			faderAnimator.SetBool(IsFadedIn, true);

			// Reset states
			loadingComplete = false;
			waitingForInput = false;

			if (pressAnyButtonText != null)
				pressAnyButtonText.gameObject.SetActive(false);

			ResetProgressIndicators();
		}

		private void ResetProgressIndicators()
		{
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

		private void HandleTransitionScreen()
		{
			if (loadingScene.requiresButtonPressToLoad && levelTransitionScreen != null)
				SetupLevelTransitionScreen();
			else
			{
				// Use regular loading screen
				loadingScreen.SetActive(true);
				if (levelTransitionScreen != null)
					levelTransitionScreen.SetActive(false);
			}
		}

		private void SetupLevelTransitionScreen()
		{
			levelTransitionScreen.SetActive(true);
			loadingScreen.SetActive(false);

			// Set title
			if (locationTitleText != null)
				locationTitleText.text = loadingScene.DisplayName;

			/*
			if (locationImage == null) return;
			if (loadingScene.sceneImage != null)
			{
				locationImage.sprite = loadingScene.sceneImage;
				locationImage.gameObject.SetActive(true);
			}
			else
				locationImage.gameObject.SetActive(false);
				*/
		}

		private void StartLoadingSceneAsync()
		{
			isLoading = true;
			loadingOperation = SceneManager.LoadSceneAsync(loadingScene.SceneName);
			loadingOperation.allowSceneActivation = !loadingScene.requiresButtonPressToLoad;
			Debug.Log("loading scene async starting: " + loadingScene.SceneName + " it requires button press: " + loadingScene.requiresButtonPressToLoad);
		}

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
		
			waitingForInput = false;
			loadingOperation.allowSceneActivation = true;
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
}