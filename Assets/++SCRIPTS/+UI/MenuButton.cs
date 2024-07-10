using UnityEngine;
using UnityEngine.UI;

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
		Gas
	}
	public Sprite OnSprite;
	public Sprite OffSprite;
	public Image sprite;
	public ButtonType type;

	private void Awake()
	{
		if(sprite == null) sprite = GetComponentInChildren<Image>();
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