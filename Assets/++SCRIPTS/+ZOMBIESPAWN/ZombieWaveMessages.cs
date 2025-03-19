using TMPro;
using UnityEngine;

public class ZombieWaveMessages : MonoBehaviour
{
	public TextMeshProUGUI NextWaveNumber;
	public TextMeshProUGUI NextWaveText;
	public TextMeshProUGUI CurrentWaveText;
	public ZombieWaveManager zombieWaveManager;
	public GameObject visible;
	private bool isOn;

	private void Start()
	{
		zombieWaveManager = FindFirstObjectByType<ZombieWaveManager>();
		LevelManager.OnStartLevel += LevelGameSceneOnStartLevel;
		LevelManager.OnStopLevel += LevelGameSceneOnStopLevel;
		visible.SetActive(isOn);
	}

	private void LevelGameSceneOnStopLevel(GameLevel gameLevel)
	{
		isOn = false;
		visible.SetActive(isOn);
	}

	private void LevelGameSceneOnStartLevel(GameLevel gameLevel)
	{
		isOn = true;
		visible.SetActive(isOn);
	}

	private void FixedUpdate()
	{
		if (!isOn) return;
if(zombieWaveManager == null) zombieWaveManager = FindFirstObjectByType<ZombieWaveManager>();
	
if(zombieWaveManager == null) return;
		if (zombieWaveManager.GetWavesRemaining() == 0)
		{
			CurrentWaveText.text = "Final Wave";
		}
		else
		{
			CurrentWaveText.text = "Wave: " + zombieWaveManager.GetCurrentWaveIndex();
		}

		if (zombieWaveManager.BetweenWaves)
		{
			NextWaveText.gameObject.SetActive(true);
			NextWaveNumber.gameObject.SetActive(true);
			NextWaveNumber.text = ":" + zombieWaveManager.GetTimeTillNextWave();
		}
		else
		{
			NextWaveNumber.gameObject.SetActive(false);
			NextWaveText.gameObject.SetActive(false);
		}
	}
}

