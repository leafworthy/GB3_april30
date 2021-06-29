using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

	[SerializeField]private CapturePlayerTrigger2D floorCollider;
	[SerializeField] private CapturePlayerTrigger2D behindCollider;

	[SerializeField] private AlphaSprites alphaSprites;

	[SerializeField] private List<GameObject> objectsHiddenOnActivation = new List<GameObject>();
	[SerializeField] private List<GameObject> objectsRevealedOnActivation = new List<GameObject>();
	[SerializeField] private List<GameObject> objectsRevealedOnBehind = new List<GameObject>();

	public void ShowFadedExterior()
	{
		Fade();
		ShowBehind();
	}

	public void ShowFadedInterior()
	{
		Fade();
		ShowInterior();
	}

	public void ShowUnFadedInterior()
	{
		UnFade();
		ShowInterior();
	}

	public void ShowUnFadedExterior()
	{
		UnFade();
		ShowExterior();
	}

	public bool ContainsPlayer()
	{
		return floorCollider.ContainsPlayer();
	}

	public bool PlayerIsBehind()
	{
		return behindCollider.ContainsPlayer();
	}

	private void Fade()
	{
		alphaSprites.SetAlpha(.2f);
	}

	private void UnFade()
	{
		alphaSprites.SetAlpha(1);
	}

	private void ShowInterior()
	{
		foreach (GameObject obj in objectsHiddenOnActivation)
		{
			obj.SetActive(false);
		}

		foreach (GameObject obj in objectsRevealedOnActivation)
		{
			obj.SetActive(true);
		}

		foreach (GameObject obj in objectsRevealedOnBehind)
		{
			obj.SetActive(false);
		}
	}

	private void ShowExterior()
	{
		foreach (GameObject obj in objectsHiddenOnActivation)
		{
			obj.SetActive(true);
		}

		foreach (GameObject obj in objectsRevealedOnActivation)
		{
			obj.SetActive(false);
		}

		foreach (GameObject obj in objectsRevealedOnBehind)
		{
			obj.SetActive(false);
		}
	}

	private void ShowBehind()
	{
		foreach (GameObject obj in objectsHiddenOnActivation)
		{
			obj.SetActive(false);
		}

		foreach (GameObject obj in objectsRevealedOnActivation)
		{
			obj.SetActive(false);
		}

		foreach (GameObject obj in objectsRevealedOnBehind)
		{
			obj.SetActive(true);
		}
	}


}
