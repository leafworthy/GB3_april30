using _PLUGINS;
using UnityEngine;

public class BulletFX : MonoBehaviour
{
	private int shotTime = 2;
	private int shotCounter;
	private bool isOn = true;
	private bool isGlock;
	private SpriteRenderer sprite;
	private  Vector3 centerPos;
	private  float originalSize;
	private float bulletWidth;
	private IsoSpriteSorting sorting;

	private void Awake()
	{
		sorting = GetComponent<IsoSpriteSorting>();
		sprite = GetComponent<SpriteRenderer>();
		originalSize = sprite.bounds.size.x;
		LEVELS.OnLevelStop += CleanUp;
	}

	private void CleanUp()
	{
		isOn = false;
		shotCounter = 0;
	}

	public void Fire (Vector2 start, Vector2 end, Vector3 hitPoint, float width = 10)
	{
		bulletWidth = width;
		shotCounter = shotTime;
		isOn = true;
		line(start,end);
		if (Vector3.Distance(sorting.SorterPositionOffset.transform.position, start) <
		    Vector3.Distance(sorting.SorterPositionOffset2.transform.position, start))
		{
			sorting.SorterPositionOffset2.transform.position = hitPoint;
		}
		else
		{

			sorting.SorterPositionOffset.transform.position = hitPoint;
		}

	}

	void FixedUpdate ()
	{
		if (Menu_Pause.isPaused) return;
		if (!isOn) return;
		if (shotCounter > 0) {
			shotCounter--;
		} else {
			MAKER.Unmake (gameObject);
			isOn = false;
		}
	}

	private void line(Vector2 start, Vector2 end)
	{
		if (Menu_Pause.isPaused) return;
		if (start == end) return;
		if (sprite == null)
		{
			sprite = GetComponent<SpriteRenderer>();

		}

		ScaleSprite(start, end);
		PositionSprite(start, end);
		RotateSprite(end);

	}

	private void RotateSprite(Vector2 end)
	{
		Vector3 angleVector =
			new Vector2(end.x - centerPos.x, end.y - centerPos.y);
		var angle = Mathf.Atan2(angleVector.y, angleVector.x) * Mathf.Rad2Deg;
		sprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
	}

	private void PositionSprite(Vector2 start, Vector2 end)
	{
		centerPos = new Vector3(start.x + end.x,
			start.y + end.y) / 2;

		sprite.transform.position = centerPos;
	}

	private void ScaleSprite(Vector2 start, Vector2 end)
	{
		sprite.transform.localScale = new Vector3(
			Vector2.Distance(end, start) / originalSize, bulletWidth, 1);
	}
}
