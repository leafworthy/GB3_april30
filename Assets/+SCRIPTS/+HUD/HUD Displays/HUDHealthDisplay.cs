using GangstaBean.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace __SCRIPTS.HUD_Displays
{
	public class HUDHealthDisplay : MonoBehaviour, INeedPlayer {

		[FormerlySerializedAs("bar")] public Bar_FX barFX;
		public TMP_Text healthText;
		public TMP_Text MaxHealthText;
		private Life playerDefence;


		public void SetPlayer(Player _player)
		{
			barFX = GetComponentInChildren<Bar_FX>();
			if (playerDefence != null)
			{
				playerDefence.OnFractionChanged -= UpdateDisplay;
			}
			playerDefence = _player.SpawnedPlayerGO.GetComponentInChildren<Life>();


			if (barFX.fastBarImage != null)
			{
				barFX.fastBarImage.color = _player.playerColor;
			}

			playerDefence.OnFractionChanged += UpdateDisplay;

			UpdateDisplay(0);
		}



		private void UpdateDisplay(float f)
		{
			healthText.text = Mathf.Ceil(playerDefence.Health).ToString();
			MaxHealthText.text = "/" + Mathf.Ceil(playerDefence.MaxHealth).ToString();
			barFX.UpdateBar(playerDefence.Health, playerDefence.MaxHealth);
			//var shaker = shakeIcon.gameObject.AddComponent<ObjectShaker>();
			//shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
		}
	}
}
