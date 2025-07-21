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
	public class SceneLoader : ServiceUser, IService
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
		private static readonly int IsFadedIn = Animator.StringToHash("IsFadedIn");

		// Scene transition state tracking
		private SceneDefinition currentlyLoadedScene;
		private SceneDefinition loadingScene;


		public event Action<SceneDefinition> OnSceneReadyToStartLevel;

		#region Lifecycle Methods



		public void QuitGame()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		public void StartService()
		{
			SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
			pressAnyButtonText.gameObject.SetActive(false);
			StartFadingIn();
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
		}

		private void Update()
		{
			if (!isLoading) return;
			var progressValue = UpdateLoadingProgress();
			if (!loadingOperation.allowSceneActivation || !(progressValue >= 1.0f)) return;
			Debug.Log(  "Scene load complete, fading out now: " + loadingScene?.sceneName);
			isLoading = false;
			FadeOut();
		}



		#endregion

		#region Public Scene Loading Methods


		public void GoToScene(SceneDefinition sceneDefinition)
		{
			Debug.Log("go to scene" + sceneDefinition?.sceneName);
			//FROM LEVEL MANAGER
			if (sceneDefinition == null || !sceneDefinition.IsValid())
			{
				Debug.Log("Invalid scene definition provided, cannot load scene: " + sceneDefinition?.sceneName);
				return;
			}

			// Store the destination
			Debug.Log("loading scene, fade in begins" + sceneDefinition.sceneName);
			loadingScene = sceneDefinition;

			// Start the transition
			StartFadingIn();
		}

		#endregion


		private void SetCurrentSceneReady(SceneDefinition scene)
		{
			if (scene == null) return;

			currentlyLoadedScene = scene;

			OnSceneReadyToStartLevel?.Invoke(currentlyLoadedScene);
			loadingScene = null;
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
			if (scene.name == assets.Scenes.gameManager)
			{
				Debug.Log("game manager scene loaded");
				return;
			}

			Debug.Log("Scene loaded: " + scene.name);
			isLoading = false;

			SetCurrentSceneReady(assets.Scenes.GetByName(scene.name));
		}


		private void StartFadingIn()
		{
			faderAnimator.SetBool(IsFadedIn, true);
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

			if (percentLoadedText == null) return;
			percentLoadedText.text = "0";
			percentLoadedText.gameObject.SetActive(true);
		}



		private void StartLoadingSceneAsync()
		{
			isLoading = true;
			Debug.Log(loadingScene?.sceneName + " is loading async");
			loadingOperation = SceneManager.LoadSceneAsync(loadingScene.SceneName);
			Debug.Log("Starting async load for scene: " + loadingScene?.SceneName);
		}

		private float UpdateLoadingProgress()
		{
			// Progress is clamped at 0.9 until allowSceneActivation is true
			var progressValue = Mathf.Clamp01(loadingOperation.progress / 0.9f);

			if (progressBarImage != null)
				progressBarImage.fillAmount = progressValue;

			if (percentLoadedText != null)
				percentLoadedText.text = Mathf.Round(progressValue * 100).ToString();

			return progressValue;
		}


		private void FadeOut()
		{
			faderAnimator.SetBool(IsFadedIn, false);

			if (pressAnyButtonText != null)
				pressAnyButtonText.gameObject.SetActive(false);
		}

		#endregion


	}
}
