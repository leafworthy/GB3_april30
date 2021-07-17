using UnityEngine;

public class BulletFX : MonoBehaviour
{
	private int shotTime = 2;
	private int shotCounter = 0;
	private Vector2 start;
	private Vector2 end;
	private bool isOn = true;
	private bool isGlock;

	private void Start()
	{
		GAME.OnGameEnd += CleanUp;
	}

	private void CleanUp()
	{
		isOn = false;
		//Destroy(gameObject);
	}

	public void Fire (Vector2 start, Vector2 end, bool isGlock)
	{
		this.isGlock = isGlock;
		this.start = start;
		this.end = end;
		shotCounter = shotTime;
		isOn = true;
	}

	void FixedUpdate ()
	{
		if (PAUSE.isPaused) return;
		if (isOn) {
			if (shotCounter > 0) {
				shotCounter--;
			} else {
				MAKER.Unmake (gameObject);
				isOn = false;
			}
		}
	}

	private void OnGUI ()
	{
		if (PAUSE.isPaused) return;
		if (!isOn) return;
		if (shotCounter <= 0) return;
		if (isGlock)
		{
			DRAW.line(DRAW.convertWorldToLinePoint(start), DRAW.convertWorldToLinePoint(end), Color.white, 25,
				ASSETS.FX.GlockBulletTexture2D);
		}
		else
		{
			DRAW.line(DRAW.convertWorldToLinePoint(start), DRAW.convertWorldToLinePoint(end), Color.white, 10,
				ASSETS.FX.AKbulletTexture2D);
		}
	}


}
