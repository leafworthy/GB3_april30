using UnityEngine;

namespace __SCRIPTS
{
	public class LevelLoadInteraction : TimedInteraction
	{
	
		[Tooltip("Display format used when prompting player (e.g. 'Go to {0}')"), SerializeField]
		private string promptFormat = "Go to {0}?";

		[SerializeField] private TravelPoint travelPoint;


		protected override void OnEnable()
		{
			base.OnEnable();
			OnTimeComplete += OnInteractionComplete;
			OnSelected += PlayerEnters;
			OnDeselected += PlayerExits;
			travelPoint = GetComponent<TravelPoint>();
		}

		protected override void OnDisable()
		{
			if(travelPoint == null) return;
			base.OnDisable();
			OnTimeComplete -= OnInteractionComplete;
			OnSelected -= PlayerEnters;
			OnDeselected -= PlayerExits;
		}
		private void PlayerEnters(Player player)
		{
			if (isFinished) return;
			// Make the player say where they're going
			if (player != null && player.SpawnedPlayerGO != null) player.Say(string.Format(promptFormat, travelPoint.destinationScene.sceneName));
		}

		private void PlayerExits(Player player)
		{
			if (isFinished) return;
			player.StopSaying();
		}

		private void OnInteractionComplete(Player player)
		{
			if (isFinished) return;
			Debug.Log($"LevelLoadInteraction: Transitioning from {SceneLoader.I.GetCurrentSceneDefinition().sceneName} to {travelPoint.destinationScene.sceneName}");

			if(travelPoint.destinationScene == null)
			{
				Debug.LogError("Destination scene is null");
				return;
			}
			// Load the destination scene
			FinishInteraction(player);
			LevelManager.I.StartNextLevel(travelPoint);
			if (travelPoint.destinationScene != null) SceneLoader.I.GoToScene(travelPoint.destinationScene);
		}
	
	}
}