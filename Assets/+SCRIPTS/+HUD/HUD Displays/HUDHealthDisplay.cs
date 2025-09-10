using GangstaBean.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS.HUD_Displays
{
	public class HUDHealthDisplay : MonoBehaviour, INeedPlayer {

		[FormerlySerializedAs("bar")] public Bar_FX barFX;
		public TMP_Text healthText;
		public TMP_Text MaxHealthText;
		private Life playerDefence;
		private Player player;


		public void SetPlayer(Player _player)
		{
			player = _player;
			barFX = GetComponentInChildren<Bar_FX>();
			playerDefence = player.SpawnedPlayerGO.GetComponentInChildren<Life>();
			if (playerDefence != null)
			{
				playerDefence.OnFractionChanged -= UpdateDisplay;
			}


			if (barFX.fastBarImage != null)
			{
				barFX.fastBarImage.color = _player.playerColor;
			}

			playerDefence.OnFractionChanged += UpdateDisplay;

			Debug.Log("set player fraction  " + playerDefence.GetFraction());
			UpdateDisplay(playerDefence.GetFraction());
		}

		private void Start()
		{
			playerDefence = player.SpawnedPlayerGO.GetComponentInChildren<Life>();
			Debug.Log("on enable fraction  " + playerDefence.GetFraction());
			UpdateDisplay(playerDefence.GetFraction());
		}

		private void UpdateDisplay(float fraction)
		{
			Debug.Log("display updated");
			healthText.text = Mathf.Ceil(playerDefence.CurrentHealth).ToString();
			MaxHealthText.text = "/" + Mathf.Ceil(playerDefence.MaxHealth).ToString();
			barFX.UpdateBar(fraction);
		}
	}
}
