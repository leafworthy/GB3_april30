using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace __SCRIPTS
{
	/// <summary>
	/// Integrated scene transition system that handles both scene loading and player spawn positions
	/// </summary>
	public class SceneLoader : MonoBehaviour, IService
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

		// Scene transition weaponState tracking
		private SceneDefinition currentlyLoadedScene;
		private SceneDefinition loadingScene;

		public event Action<SceneDefinition> OnSceneReadyToStartLevel;

		#region Lifecycle Methods

		public void StartService()
		{
			SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
			pressAnyButtonText.gameObject.SetActive(false);
			StartFadingIn();
		}

		private void OnDestroy()
		{
			//SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
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
			isLoading = false;
			FadeOut();
		}

		#endregion

		#region Public Scene Loading Methods

		public void GoToScene(SceneDefinition newScene)
		{
			loadingScene = newScene;
			StartFadingIn();
		}

		#endregion

		private void SetCurrentSceneReady()
		{
			currentlyLoadedScene = loadingScene;
			loadingScene = null;
			OnSceneReadyToStartLevel?.Invoke(currentlyLoadedScene);
		}


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


		#region Private Methods

		private void SceneManager_OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (scene.name == Services.assetManager.Scenes.gameManager)
			{
				return;
			}

			isLoading = false;
			SetCurrentSceneReady();
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
			loadingOperation = SceneManager.LoadSceneAsync(loadingScene.SceneName);
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
