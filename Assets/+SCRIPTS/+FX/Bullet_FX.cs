using UnityEngine;

namespace __SCRIPTS
{
	public class Bullet_FX : MonoBehaviour
	{
		int shotTime = 1;
		int shotCounter;
		bool isOn = true;
		bool isGlock;
		SpriteRenderer sprite;
		Vector3 centerPos;
		float originalSize;
		float bulletWidth;
		bool hasFired;

		public void Fire(Attack attack)
		{
			sprite = GetComponent<SpriteRenderer>();
			if (!hasFired)
			{
				hasFired = true;
				originalSize = sprite.bounds.size.x;
			}

			float width = 10;
			bulletWidth = width;
			shotCounter = shotTime;
			isOn = true;
			line(attack.OriginWithHeight, attack.DestinationWithHeight);
		}

		void FixedUpdate()
		{
			if (Services.pauseManager.IsPaused) return;
			if (!isOn) return;
			if (shotCounter > 0)
				shotCounter--;
			else
			{
				Services.objectMaker.Unmake(gameObject);
				isOn = false;
			}
		}

		void line(Vector2 start, Vector2 end)
		{
			if (Services.pauseManager.IsPaused) return;
			if (start == end) return;
			if (sprite == null) sprite = GetComponent<SpriteRenderer>();

			ScaleSprite(start, end);
			PositionSprite(start, end);
			RotateSprite(end);
		}

		void RotateSprite(Vector2 end)
		{
			Vector3 angleVector = new Vector2(end.x - centerPos.x, end.y - centerPos.y);
			var angle = Mathf.Atan2(angleVector.y, angleVector.x) * Mathf.Rad2Deg;
			sprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
		}

		void PositionSprite(Vector2 start, Vector2 end)
		{
			centerPos = new Vector3(start.x + end.x, start.y + end.y) / 2;

			sprite.transform.position = centerPos;
		}

		void ScaleSprite(Vector2 start, Vector2 end)
		{
			sprite.transform.localScale = new Vector3(Vector2.Distance(end, start) / originalSize, bulletWidth, 1);
		}
	}
}
