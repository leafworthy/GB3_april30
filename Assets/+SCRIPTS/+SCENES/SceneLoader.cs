using System;
using __SCRIPTS;
using __SCRIPTS.Cursor;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace __SCRIPTS
{
}

public class SceneLoader : MonoBehaviour, IService
{
	// UI References
	[Header("UI References"), SerializeField]
	Animator faderAnimator;
	[SerializeField] GameObject loadingScreen;
	[SerializeField] Image progressBarImage;
	[SerializeField] TextMeshProUGUI percentLoadedText;
	[SerializeField] TextMeshProUGUI locationTitleText;
	public VideoPlayer videoPlayer;
	// State tracking
	AsyncOperation loadingOperation;
	bool isLoading;
	bool isFading;
	static readonly int IsFadedIn = Animator.StringToHash("IsFadedIn");

	// Scene transition weaponState tracking
	SceneDefinition currentlyLoadedScene;
	SceneDefinition loadingScene;
	public SceneAnimationActions sceneAnimationActions;

	public event Action<SceneDefinition> OnSceneReadyToStartLevel;
	public event Action OnVideoComplete;
	public event Action OnFadeInComplete;

	Action ActionOnFadeInComplete;
	public event Action OnSceneAboutToChange;

	#region Lifecycle Methods

	public void StartFadeInAndPlayVideo()
	{
		if (isFading) return;
		Debug.Log("start fading here");
		videoPlayer.targetCamera = CursorManager.GetCamera();
		isFading = true;
		StartFadingIn(SceneAnimationActions_OnFadeInComplete);
	}

	void SceneAnimationActions_OnFadeInComplete()
	{
		OnFadeInComplete?.Invoke();
		videoPlayer.Play();
		videoPlayer.loopPointReached += VideoComplete;
	}

	void VideoComplete(VideoPlayer source)
	{
		OnVideoComplete?.Invoke();
		faderAnimator.SetBool(IsFadedIn, true);
	}

	public void StartService()
	{
		SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
		sceneAnimationActions.OnFadeInComplete += FadeInComplete;
		sceneAnimationActions.OnFadeOutComplete += FadeOutComplete;
		StartFadingOut();
	}

	void StartFadingOut()
	{
		faderAnimator.SetBool(IsFadedIn, false);
	}

	void OnDestroy()
	{
		SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
	}

	void Update()
	{
		if (!isLoading) return;
		var progressValue = UpdateLoadingProgress();
		if (!loadingOperation.allowSceneActivation || !(progressValue >= 1.0f)) return;
		isLoading = false;
		FadeOut();
	}

	#endregion

	#region Public Scene Loading Methods

	void StartLoading()
	{
		StartLoadingSceneAsync();
		ResetProgressIndicators();
	}

	public void GoToScene(SceneDefinition newScene)
	{
		loadingScene = newScene;
		StartFadingIn(StartLoading);
		OnSceneAboutToChange?.Invoke();
	}

	#endregion

	void SetCurrentSceneReady()
	{
		currentlyLoadedScene = loadingScene;
		loadingScene = null;
		OnSceneReadyToStartLevel?.Invoke(currentlyLoadedScene);
	}

	public void FadeInComplete()
	{
		Debug.Log("fade in complete");
		ActionOnFadeInComplete();
	}

	public void FadeOutComplete()
	{
		loadingScreen.SetActive(false);
	}

	#region Private Methods

	void SceneManager_OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == Services.assetManager.Scenes.gameManager) return;

		isLoading = false;
		SetCurrentSceneReady();
	}

	public void StartFadingIn([CanBeNull] Action onComplete)
	{
		Debug.Log("Start fading in here");
		faderAnimator.SetBool(IsFadedIn, true);
		ActionOnFadeInComplete = onComplete;
	}


	void ResetProgressIndicators()
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

	void StartLoadingSceneAsync()
	{
		isLoading = true;
		loadingOperation = SceneManager.LoadSceneAsync(loadingScene.SceneName);
	}

	float UpdateLoadingProgress()
	{
		// Progress is clamped at 0.9 until allowSceneActivation is true
		var progressValue = Mathf.Clamp01(loadingOperation.progress / 0.9f);

		if (progressBarImage != null)
			progressBarImage.fillAmount = progressValue;

		if (percentLoadedText != null)
			percentLoadedText.text = Mathf.Round(progressValue * 100).ToString();

		return progressValue;
	}

	public void FadeOut()
	{
		faderAnimator.SetBool(IsFadedIn, false);
	}
}
#endregion
