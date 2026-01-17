using GangstaBean.Core;
using TMPro;
using UnityEngine;

namespace __SCRIPTS.HUD_Displays
{
	public class HUDHealthDisplay : MonoBehaviour, INeedPlayer
	{
		public LineBar barFX;
		public TMP_Text healthText;
		//public TMP_Text MaxHealthText;
		IGetAttacked playerDefence;
		Player player;

		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
			barFX = GetComponentInChildren<LineBar>();
			playerDefence = player.SpawnedPlayerGO.GetComponentInChildren<IGetAttacked>();
			if (playerDefence == null) return;
			playerDefence.OnFractionChanged -= UpdateDisplay;

			barFX.slowBarColor = newPlayer.playerColor;

			playerDefence.OnFractionChanged += UpdateDisplay;

			UpdateDisplay(playerDefence.GetFraction());
		}

		void UpdateDisplay(float fraction)
		{
			healthText.text = Mathf.Ceil(playerDefence.CurrentHealth).ToString();
			//MaxHealthText.text = "/" + Mathf.Ceil(playerDefence.MaxHealth);
			barFX.UpdateBar(fraction);
		}
	}
}
