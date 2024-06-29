using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
	private AsyncOperation loadingOperation;
	public UnityEngine.UI.Slider progressBar;
	public TextMeshProUGUI percentLoadedText;
	public GameObject loadingScreen;
	private bool isLoading;
	
	private Scene.Type DestinationScene;
	[SerializeField]private Scene.Type StartingScene;
	
	public Animator faderAnimator;
	private static readonly int FadingOut = Animator.StringToHash("FadingOut");
	private static readonly int FadeOutTrigger = Animator.StringToHash("FadeOutTrigger");
	private static readonly int FadeInTrigger = Animator.StringToHash("FadeInTrigger");
	private bool isFadingIn;
	public static event Action<Scene.Type> OnSceneComplete;
	public bool autoLoadScene;



	
	public void FadeOutComplete()
	{
		//animation event
		loadingScreen.SetActive(false);
		Debug.Log("fade out complete");
	}

	public void FadeInComplete()
	{ 
		//animation event
		LoadScene(DestinationScene);
	}

	private void Start()
	{
		Debug.Log("Start");
		DontDestroyOnLoad(gameObject);
		if(autoLoadScene)
		{
			SetDestinationScene(StartingScene);
		}
		else
		{
			 FadeOut();
		}
	}

	private void StartFadingIn()
	{
		if(isFadingIn) return;
		isFadingIn = true;
		loadingScreen.SetActive(true);
		faderAnimator.SetTrigger(FadeInTrigger);
		Debug.Log("start fading in animation");
	}

	public void SetDestinationScene(Scene.Type newScene)
	{
		//if (newScene == CurrentScene) return;
		DestinationScene = newScene;
		Debug.Log("Destination Scene set to " + newScene);
		StartFadingIn();
	}

	private void LoadScene(Scene.Type newScene)
	{
		loadingOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(newScene.ToString());
		isLoading = true;
		Debug.Log("load scene called");

	}

	private void Update()
	{
		if (!isLoading) return;
		var progressValue = ShowLoadingProgress();
		if (progressValue < 1) return;
		isLoading = false;
		isFadingIn = false;
		FadeOut();
	}

	private float ShowLoadingProgress()
	{
		var progressValue = Mathf.Clamp01(loadingOperation.progress / 0.9f);
		progressBar.value = Mathf.Clamp01(loadingOperation.progress / 0.9f);
		percentLoadedText.text = Mathf.Round(progressValue * 100).ToString();
		return progressValue;
	}

	private void FadeOut()
	{
		Debug.Log("fading out");
		faderAnimator.SetTrigger(FadeOutTrigger);
	}

	
}