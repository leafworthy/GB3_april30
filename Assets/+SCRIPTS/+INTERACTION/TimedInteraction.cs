using System;
using UnityEngine;
using UnityEngine.UI;

namespace __SCRIPTS
{
	public class TimedInteraction : PlayerInteractable
	{
		private float currentProgress;
		protected float totalTime = 1;
		private bool isActive;
		public event Action<Player> OnTimeComplete;
		private Image loadingBarImage;
		private Player currentPlayer;
		private GameObject loadingBar;

		private void Start()
		{
			Init();
		}

		protected virtual void OnEnable()
		{
			OnActionPress += InteractableOnActionPress;
			OnActionRelease += InteractableOnActionRelease;
			
		}

		protected virtual void OnDisable()
		{
			OnActionPress -= InteractableOnActionPress;
			OnActionRelease -= InteractableOnActionRelease;
		}
		
		private void OnDestroy()
		{
			// Clean up loading bar when this component is destroyed
			if (loadingBar != null)
			{
				if (Application.isPlaying)
				{
					Destroy(loadingBar);
				}
			}
		}

		private void Init()
		{
			if (loadingBar == null)
			{
				// Create loading bar slightly above the interaction object
				Vector3 barPosition = transform.position + Vector3.up * 1.5f;
				loadingBar = objectMaker.Make( assets.FX.loadingBarPrefab, barPosition);
				
				// Set up proper world space positioning
				var canvas = loadingBar.GetComponent<Canvas>();
				if (canvas != null)
				{
					canvas.renderMode = RenderMode.WorldSpace;
					canvas.sortingLayerName = "UI";
					canvas.sortingOrder = 100;
				}
				
				// Don't parent to transform to avoid rotation/scale issues
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
			
			// Keep loading bar positioned above the interaction object
			if (loadingBar != null)
			{
				loadingBar.transform.position = transform.position + Vector3.up * 1.5f;
			}
			
			currentProgress += Time.deltaTime;
			loadingBarImage.fillAmount = currentProgress / totalTime;
			if (!(currentProgress >= totalTime)) return;
			OnTimeComplete?.Invoke(currentPlayer);
			Stop();
		}

		private void Stop()
		{
			currentPlayer = null;
		

			loadingBar.SetActive(false);
			isActive = false;
		}
	}
}
