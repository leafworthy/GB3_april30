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
	
	private GameScene.Type DestinationScene;
	[SerializeField]private GameScene.Type StartingScene;
	
	public Animator faderAnimator;
	private static readonly int FadeOutTrigger = Animator.StringToHash("FadeOutTrigger");
	private static readonly int FadeInTrigger = Animator.StringToHash("FadeInTrigger");
	private bool isFadingIn;
	public bool autoLoadScene;
	public static bool hasLoaded;

	private void FixedUpdate()
	{
		Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
	}

	public void FadeOutComplete()
	{
		loadingScreen.SetActive(false);
	}

	public void FadeInComplete()
	{
		LoadScene(DestinationScene);
	}

	

	protected override void Awake()
	{
		base.Awake();
		DontDestroyOnLoad(this);
		SceneManager.LoadScene(StartingScene.ToString());
		FadeOut();
		hasLoaded = true;
		
		//SetDestinationScene(StartingScene);

	}

	private void StartFadingIn()
	{
		Debug.Log("fading in");
		loadingScreen.SetActive(true);
		faderAnimator.SetTrigger(FadeInTrigger);
	}

	public void SetDestinationScene(GameScene.Type newScene)
	{
		DestinationScene = newScene;
		Debug.Log("destination set: " + DestinationScene);
		StartFadingIn();
	}

	private void LoadScene(GameScene.Type newScene)
	{
		
		loadingOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(newScene.ToString());
		isLoading = true;

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
		faderAnimator.SetTrigger(FadeOutTrigger);
	}

	
}