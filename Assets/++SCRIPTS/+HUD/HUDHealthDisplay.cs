using __SCRIPTS._BANDAIDS;
using __SCRIPTS._PLAYER;
using __SCRIPTS._UNITS;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace __SCRIPTS._HUD
{
	[ExecuteInEditMode]
	public class HUDHealthDisplay : MonoBehaviour
	{

		[FormerlySerializedAs("healthBar"),FormerlySerializedAs("bar")] public AmmoLifeFX lifeFX;
		public TMP_Text healthText;
		private Life playerDefence;
		public GameObject shakeIcon;

		public void SetPlayer(Player player)
		{
			if (playerDefence != null)
			{
				playerDefence.OnFractionChanged -= UpdateDisplay;
			}
			playerDefence = player.SpawnedPlayerGO.GetComponent<Life>();
			if (lifeFX.slowBarImage != null)
			{
				lifeFX.slowBarImage.color = player.color;
			}

			if (lifeFX.capSpriteRenderer != null)
			{
				lifeFX.capSpriteRenderer.color = player.color;
			}

			playerDefence.OnFractionChanged += UpdateDisplay;
			UpdateDisplay(0);
		}

		private void UpdateDisplay(float f)
		{
			healthText.text = Mathf.Ceil(playerDefence.Health).ToString();
			lifeFX.UpdateBar(playerDefence.Health, playerDefence.HealthMax);
			var shaker = shakeIcon.gameObject.AddComponent<ObjectShaker>();
			shaker.Shake(ObjectShaker.ShakeIntensityType.medium);
		}
	}
}
