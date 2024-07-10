using System;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class ThingWithHeight : MonoBehaviour
{
	public bool isOverLandable;

	private IsoSpriteSorting currentLandableSorting;
	private float DistanceToGround;
	private Landable currentLandable;

	public event Action<float> OnFallFromLandable;
	public GameObject ShadowObject;
	public GameObject JumpObject;
	public IsoSpriteSorting Sorting;
	public bool canLand;
	private bool isAboveAll;

	protected virtual void Start()
	{
		isAboveAll = Sorting.renderAboveAll;
		canLand = false;
		if (Sorting == null) Sorting = GetComponent<IsoSpriteSorting>();
	}

	protected virtual void FixedUpdate()
	{
		UpdateDistanceToCurrentLandable();
		UpdateGraphics();
	}

	private void UpdateGraphics()
	{
		SetDistanceToGround(DistanceToGround);
	}

	public void UpdateDistanceToCurrentLandable()
	{
		var oldLandable = currentLandable;
		currentLandable = GetCurrentLandable();
		if (Sorting == null) Sorting = GetComponent<IsoSpriteSorting>();
		if (currentLandable != null)
		{
			isOverLandable = true;
			if (canLand)
			{
				AddRenderersToCurrentLandable();
			}
		}
		else
		{
			if (!isOverLandable) return;
			isOverLandable = false;

			RemoveRenderersFromCurrentLandable();

			Sorting.enabled = true;
			currentLandableSorting = null;
			OnFallFromLandable?.Invoke(oldLandable.height);
		}
	}

	private void AddRenderersToCurrentLandable()
	{
		currentLandableSorting = currentLandable.sorting;
		Sorting.enabled = false;
		foreach (var renderer in Sorting.renderersToSort)
			if (!currentLandableSorting.renderersToSort.Contains(renderer))
				currentLandableSorting.renderersToSort.Add(renderer);
	}

	private void RemoveRenderersFromCurrentLandable()
	{
		Sorting.renderAboveAll = isAboveAll;
		if (currentLandableSorting == null) return;
		foreach (var renderer in Sorting.renderersToSort.Where(renderer => currentLandableSorting.renderersToSort.Contains(renderer)))
			currentLandableSorting.renderersToSort.Remove(renderer);
	}

	private Landable GetCurrentLandable()
	{
		var raycastHit = Physics2D.Raycast(transform.position, Vector3.down, 1, ASSETS.LevelAssets.LandableLayer);
		return raycastHit.collider != null ? raycastHit.collider.gameObject.GetComponentInChildren<Landable>() : null;
	}

	public Landable GetLandableAtPosition(Vector2 position)
	{
		var raycastHit = Physics2D.Raycast(position, Vector3.down, 1, ASSETS.LevelAssets.LandableLayer);
		return raycastHit.collider != null ? raycastHit.collider.gameObject.GetComponentInChildren<Landable>() : null;
	}
	public float GetCurrentLandableHeight()
	{
		if (currentLandable == null) return 0;
		return currentLandable.height;
	}

	public void SetDistanceToGround(float height)
	{
		DistanceToGround = height;
		JumpObject.transform.localPosition = new Vector3(0, DistanceToGround, 0);
		//Debug.Log("here");
		if (ShadowObject != null) ShadowObject.transform.localPosition = new Vector3(0, GetCurrentLandableHeight(), 0);
	}

	public float GetDistanceToGround()
	{
		return DistanceToGround;
	}
}