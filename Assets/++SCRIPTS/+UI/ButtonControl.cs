using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
	public string Name = "Generic Button";

	private Button button;
	private float startTime = 0f;
	private float bounceTime = 0f;
	private float maxTime = .5f;

	private bool running = false;
	private bool outward = true;
	private float currentScore = 0f;
	private float timeOfLastPress = 0f;

	public System.Action OnPress;
	public System.Action<float> OnScore0_1;

	public AnimationCurve valueCurve;
	[FormerlySerializedAs("pressHealthBar"),FormerlySerializedAs("pressBar")] public LifeBar pressLifeBar;
	[FormerlySerializedAs("cooldownHealthBar"),FormerlySerializedAs("cooldownBar")] public LifeBar cooldownLifeBar;

	//DEFINED BY ABILITY
	public Text pushText;
	public bool disabled = false;
	public static bool allDisabled = false;
	private float coolDown = 0f;
	private bool cooledDown = true;



	// Use this for initialization
	void Start()
	{
		button = GetComponent<Button>();
		pressLifeBar.UpdateBar(0f, 1);
		cooldownLifeBar.UpdateBar(1f, 1);
	}

	void Update()
	{

		if (running)
		{
			float duration = Time.time - startTime;
			if (outward)
			{
				if (Time.time - startTime >= maxTime)
				{
					Bounce();
					pressLifeBar.UpdateBar(maxTime, maxTime);
				}
				else
				{
					pressLifeBar.UpdateBar(duration, maxTime);
					currentScore = duration / maxTime;
				}
			}
			else
			{
				float timeSinceBounce = Time.time - bounceTime;
				pressLifeBar.UpdateBar(maxTime - timeSinceBounce, maxTime);
				currentScore = (maxTime - timeSinceBounce) / maxTime;
			}
		}
		else if (!cooledDown)
		{
			float duration = Time.time - timeOfLastPress;
			if (duration < coolDown)
			{
				cooldownLifeBar.UpdateBar(duration, coolDown);
			}
			else
			{
				cooledDown = true;
				cooldownLifeBar.UpdateBar(1, 1);
				ShowButton();
			}
		}
	}

	public void DisableButton(bool isDisabled = true)
	{
		disabled = isDisabled;
		button.interactable = !isDisabled;
		pushText.enabled = isDisabled;
	}
	public static void DisableAllButtons(bool isDisabled = true)
	{
		allDisabled = isDisabled;
	}

	public void ShowButton()
	{
		DisableButton(false);
	}

	public void HideButton()
	{
		DisableButton(true);
	}

	public float GetScore0_1()
	{
		return Mathf.Clamp(valueCurve.Evaluate(currentScore), 0f, 1);
	}

	public float GetScore0_100()
	{
		return Mathf.Clamp(valueCurve.Evaluate(currentScore) * 100, 0f, 100);
	}


	private void Bounce()
	{
		outward = false;
		bounceTime = Time.time;
	}

	private void OnMouseDown()
	{
		if (!allDisabled && !disabled)
		{
			StartPress();
			OnPress?.Invoke();
		}
	}

	private void StartPress()
	{
		running = true;
		outward = true;
		cooledDown = false;
		currentScore = 0f;
		startTime = Time.time;
		ShowButton();
		pressLifeBar.UpdateBar(0f, 1);
	}


	private void OnMouseUp()
	{
		if (!allDisabled && running)
		{
			Score();

		}
	}

	private void Score()
	{
		running = false;
		timeOfLastPress = Time.time;
		//DISPLAY.DisplayRisingNumber(GetScore0_100(), transform.position);
		pressLifeBar.UpdateBar(0f, 1);
		OnScore0_1?.Invoke(GetScore0_1());
	}




}
