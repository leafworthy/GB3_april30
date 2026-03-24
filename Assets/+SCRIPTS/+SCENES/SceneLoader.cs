using System;
using __SCRIPTS;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = System.Object;

public class SceneLoader : MonoBehaviour, IService
{
	[SerializeField] Animator faderAnimator;
	[SerializeField] GameObject loadingScreen;
	[SerializeField] Image progressBarImage;
	[SerializeField] TextMeshProUGUI percentLoadedText;
	[SerializeField] TextMeshProUGUI locationTitleText;

	bool isLoading;
	bool isFading;
	AsyncOperation loadingOperation;
	SceneDefinition currentlyLoadedScene;
	SceneDefinition loadingScene;
	static readonly int IsFadedIn = Animator.StringToHash("IsFadedIn");
	public SceneAnimationActions sceneAnimationActions;

	public event Action<SceneDefinition> OnSceneReadyToStartLevel;

	Action ActionOnFadeInComplete;
	public event Action OnSceneAboutToChange;

	#region Lifecycle Methods

	public void StartService()
	{
		Debug.Log("[SceneLoader] start service");
		ListenToEvents();
		StartFadingOut();
	}

	void ListenToEvents()
	{
		SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
		sceneAnimationActions.OnFadeInComplete += FadeInComplete;
		sceneAnimationActions.OnFadeOutComplete += FadeOutComplete;
	}

	void StopListeningToEvents()
	{
		SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
		if (sceneAnimationActions == null) return;
		sceneAnimationActions.OnFadeInComplete -= FadeInComplete;
		sceneAnimationActions.OnFadeOutComplete -= FadeOutComplete;
	}

	void StartFadingOut()
	{
		faderAnimator.SetBool(IsFadedIn, false);
	}

	void OnDestroy()
	{
		StopListeningToEvents();
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
		Debug.Log("going to scene " + newScene.SceneName, this);
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
		if (scene.name == Services.assetManager.Scenes.gameManager.SceneName) return;

		isLoading = false;
		var GameLevel = FindFirstObjectByType<GameLevel>();
		if(GameLevel != null) SetCurrentSceneReady();
	}

	public void StartFadingIn([CanBeNull] Action onComplete)
	{
		Debug.Log("Start fading in here");
		faderAnimator.SetBool(IsFadedIn, true);
		ActionOnFadeInComplete = onComplete;
	}

	void ResetProgressIndicators()
	{
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

	void FadeOut()
	{
		faderAnimator.SetBool(IsFadedIn, false);
	}
}

#endregion
