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
	[SerializeField] private GameScene.Type StartingScene;

	public Animator faderAnimator;
	public static bool hasLoaded;
	private static readonly int IsFadedIn = Animator.StringToHash("IsFadedIn");

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
		Debug.Log("load scene");
	}


	protected override void Awake()
	{
		base.Awake();
		DontDestroyOnLoad(this);
		SceneManager.sceneLoaded += FirstFade;
		SceneManager.LoadScene(StartingScene.ToString());
	}

	private void FirstFade(Scene arg0, LoadSceneMode arg1)
	{
		FadeOut();
		hasLoaded = true;
		SceneManager.sceneLoaded -= FirstFade;
	}

	private void StartFadingIn()
	{
		loadingScreen.SetActive(true);
		faderAnimator.SetBool(IsFadedIn, true);
	}

	public void SetDestinationScene(GameScene.Type newScene)
	{
		DestinationScene = newScene;
		Debug.Log("destination scene set to " + DestinationScene);
		StartFadingIn();
	}

	private void LoadScene(GameScene.Type newScene)
	{
		loadingOperation = SceneManager.LoadSceneAsync(newScene.ToString());
		isLoading = true;
	}

	private void Update()
	{
		if (!isLoading) return;
		var progressValue = ShowLoadingProgress();
		if (progressValue < 1) return;
		isLoading = false;
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
		faderAnimator.SetBool(IsFadedIn, false);
	}
}