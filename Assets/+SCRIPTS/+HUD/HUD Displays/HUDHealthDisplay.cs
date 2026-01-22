using GangstaBean.Core;
using TMPro;
using UnityEngine;

namespace __SCRIPTS.HUD_Displays
{
	public class HUDHealthDisplay : MonoBehaviour, INeedPlayer
	{
		public LineBar barFX;
		public TMP_Text healthText;
		public TMP_Text MaxHealthText;
		IGetAttacked playerDefence;
		Player player;

		public void SetPlayer(Player newPlayer)
		{
			player = newPlayer;
			barFX = GetComponentInChildren<LineBar>();
			if (barFX != null) barFX.FastBar.color = newPlayer.playerColor;
			playerDefence = player.SpawnedPlayerGO.GetComponentInChildren<IGetAttacked>();
			if (playerDefence == null) return;
			playerDefence.OnFractionChanged -= UpdateDisplay;
			playerDefence.OnFractionChanged += UpdateDisplay;
			UpdateDisplay(playerDefence.GetFraction());
			if (barFX == null) return;
			barFX.useGradientColor = false;
			barFX.HideWhenAboveFraction = false;
		}

		void UpdateDisplay(float fraction)
		{
			if (healthText != null) healthText.text = Mathf.Ceil(playerDefence.CurrentHealth).ToString();
			if (MaxHealthText != null) MaxHealthText.text = "/" + Mathf.Ceil(playerDefence.MaxHealth);
			if (barFX != null) barFX.UpdateBar(fraction);
		}
	}
}
