using System;
using UnityEngine;

public class BulletFX : MonoBehaviour
{
	private int shotTime = 2;
	private int shotCounter = 0;
	private Vector2 start;
	private Vector2 end;
	private bool isOn = true;

	private void Start()
	{
		GAME.OnGameEnd += CleanUp;
	}

	private void CleanUp()
	{
		isOn = false;
		//Destroy(gameObject);
	}

	public void Fire (Vector2 start, Vector2 end)
	{
		this.start = start;
		this.end = end;
		shotCounter = shotTime;
		isOn = true;
	}

	void FixedUpdate ()
	{
		if (isOn) {
			if (shotCounter > 0) {
				shotCounter--;
			} else {
				MAKER.Unmake (gameObject);
				isOn = false;
			}
		}
	}

	void OnGUI ()
	{
		if (isOn) {
			if (shotCounter > 0) {
				DRAW.line (DRAW.convertWorldToLinePoint (start), DRAW.convertWorldToLinePoint(end), Color.white);

			}
		}
	}


}
