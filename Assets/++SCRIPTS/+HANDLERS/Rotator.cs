using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class Rotator : MonoBehaviour
{
	public List<CarRotation> directionsStartingWithNorth = new List<CarRotation>();
	private float gap;
	public CarRotation currentCarRotation;
	private int originalCarRotationIndex;
	private int indexToSelect;
	private Vector2 originalPosition;

	private void Start()
	{
		originalPosition = transform.position;
		for (var index = 0; index < directionsStartingWithNorth.Count; index++)
		{
			var t = directionsStartingWithNorth[index];
			if (t == currentCarRotation)
			{
				originalCarRotationIndex = index;
				t.gameObject.SetActive(true);
			}
			else
			{
				t.gameObject.SetActive(false);
			}
		}
		
		
	}


	public void SetRotation(Vector2 dir)
	{
		if (directionsStartingWithNorth.Count <= 0) return;
		gap = 360 / (directionsStartingWithNorth.Count);
		float angle = Vector3.Angle(new Vector2(0.0f, 1.0f), dir.normalized);

		if (dir.x < 0.0f)
		{
			angle = 360.0f - angle;
		}

		var degrees = angle;
		indexToSelect = (int)Math.Round(degrees / gap);
		if (indexToSelect == 16) indexToSelect = 0;
		foreach (var t in directionsStartingWithNorth)
		{
			t.gameObject.SetActive(false);
		}

		currentCarRotation = directionsStartingWithNorth[indexToSelect];
		currentCarRotation.gameObject.SetActive(true);
		IsoSpriteSorting.UpdateSorters();
	}
}
