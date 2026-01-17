using UnityEngine;

[ExecuteInEditMode]
public class PlayerHitboxFader : MonoBehaviour
{
	[SerializeField] float fadedOpacity = 0.3f;
	[SerializeField] LayerMask playerLayer;

	[SerializeField] SpriteRenderer spriteRenderer;
	Color originalColor;

	void Awake()
	{
		if (spriteRenderer != null) originalColor = spriteRenderer.color;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("trigger enter");
		if ((1 << other.gameObject.layer & playerLayer) != 0) spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, fadedOpacity);
	}

	void OnTriggerExit2D(Collider2D other)
	{
		Debug.Log("trigger exit");
		if ((1 << other.gameObject.layer & playerLayer) != 0) spriteRenderer.color = originalColor;
	}

	void OnTriggerStay2D(Collider2D other)
	{
		Debug.Log("trigger stay");
		//if ((1 << other.gameObject.layer & playerLayer) != 0) spriteRenderer.color = originalColor;
	}
}
