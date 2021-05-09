using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
	public enum ButtonType
	{
		Restart,
		Resume,
		MainMenu
	}
	private bool isHighlighted;
	public Sprite OnSprite;
	public Sprite OffSprite;
	public Image sprite;
	public ButtonType type;

	private void Awake()
	{
		sprite = GetComponent<Image>();
		UnHighlight();
	}

	public void Highlight()
	{
		sprite.sprite = OnSprite;
		isHighlighted = true;
	}

	public void UnHighlight()
	{
		sprite.sprite = OffSprite;
		isHighlighted = false;
	}
}
