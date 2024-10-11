using System;
using UnityEngine;
using UnityEngine.UI;

public class TimedInteraction : PlayerInteractable
{
	private float currentProgress;
	protected float totalTime = 1;
	private bool isActive;
	public event Action<Player> OnTimeComplete;
	private Image loadingBarImage;
	private Player currentPlayer;
	private GameObject loadingBar;

	protected virtual void Start()
	{
		OnActionPress += InteractableOnActionPress;
		OnActionRelease += InteractableOnActionRelease;
		Init();
	}

	private void Init()
	{
		if (loadingBar == null)
		{
			loadingBar = Maker.Make(FX.Assets.loadingBarPrefab, transform.position);
			loadingBar.transform.SetParent(transform);
			loadingBarImage = loadingBar.GetComponentInChildren<Image>();
			loadingBarImage.enabled = true;
			loadingBar.SetActive(false);
		}

		currentProgress = 0;

	}

	protected virtual void InteractableOnActionPress(Player player)
	{
		if (isActive) return;
		currentPlayer = player;
		isActive = true;
		currentProgress = 0;
		loadingBar.SetActive(true);
		loadingBarImage.fillAmount = currentProgress / totalTime;
	}

	private void InteractableOnActionRelease(Player player)
	{
		if (!isActive) return;
		currentPlayer = null;
		Stop();
	}

	private void Update()
	{
		if (!isActive) return;
		currentProgress += Time.deltaTime;
		loadingBarImage.fillAmount = currentProgress / totalTime;
		if (!(currentProgress >= totalTime)) return;
		OnTimeComplete?.Invoke(currentPlayer);
		Stop();
	}

	private void Stop()
	{
		if (currentPlayer != null)
		{
			currentPlayer.StopSaying();
			currentPlayer = null;
		}

		loadingBar.SetActive(false);
		isActive = false;
	}
}