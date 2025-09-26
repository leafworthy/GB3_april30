using UnityEngine;
using UnityEngine.UI;

namespace __SCRIPTS
{
	public class MenuButton : MonoBehaviour
	{
		public enum ButtonType
		{
			Restart,
			Resume,
			MainMenu,
			Quit,
			Health,
			Ammo,
			Damage,
			Speed,
			Nades,
			Gas,
			Respawn,
			Unspawn
		}
		public Sprite OnSprite;
		public Sprite OffSprite;
		public Image sprite => GetComponentInChildren<Image>();
		public ButtonType type;

		private void Awake()
		{
			UnHighlight();
		}

		public virtual void Highlight()
		{
			sprite.sprite = OnSprite;
		}

		public virtual void UnHighlight()
		{
			sprite.sprite = OffSprite;
		}
	}
}
