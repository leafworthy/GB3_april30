using __SCRIPTS.Plugins._ISOSORT;
using UnityEngine;

namespace __SCRIPTS
{
	public class Bullet_FX : MonoBehaviour
	{
		private int shotTime = 1;
		private int shotCounter;
		private bool isOn = true;
		private bool isGlock;
		private SpriteRenderer sprite;
		private Vector3 centerPos;
		private float originalSize;
		private float bulletWidth;
		private IsoSpriteSorting sorting;
		private bool hasFired;

		public void Fire(Attack attack, Vector2 attackStartPoint)
		{
			sprite = GetComponent<SpriteRenderer>();
			if(!hasFired)
			{
				hasFired = true;
				originalSize = sprite.bounds.size.x;
			
			}
			sorting = GetComponent<IsoSpriteSorting>();

			float width = 10;
			bulletWidth = width;
			shotCounter = shotTime;
			isOn = true;
			line(attackStartPoint, attack.DestinationWithHeight);

			sorting.SorterPositionOffset = attack.OriginFloorPoint -(Vector2)transform.position;
			sorting.SorterPositionOffset2 = attack.DestinationFloorPoint - (Vector2)transform.position;
			sorting.Setup();
			IsoSpriteSortingManager.UpdateSorting();
		}

		private void FixedUpdate()
		{
			if (PauseManager.I.IsPaused) return;
			if (!isOn) return;
			if (shotCounter > 0)
			{
			
				shotCounter--;
			}
			else
			{
				ObjectMaker.I.Unmake(gameObject);
				isOn = false;
			}
		}

		private void line(Vector2 start, Vector2 end)
		{
			if (PauseManager.I.IsPaused) return;
			if (start == end) return;
			if (sprite == null) sprite = GetComponent<SpriteRenderer>();

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
}