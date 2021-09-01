using UnityEngine;

public class RoomActivator : MonoBehaviour
{
	[SerializeField] private RoomFader roomFader;
	[SerializeField] private CapturePlayerTrigger2D activationTrigger;

	private void Awake()
	{

		if (roomFader.ContainsPlayer())
		{
			roomFader.ShowUnFadedInterior();
		}
		else
		{
			roomFader.ShowUnFadedExterior();
		}
	}


	private void Update()
	{
		if (activationTrigger.ContainsPlayer())
		{
			Debug.Log("activation");
			if (roomFader.PlayerIsBehind())
			{
				roomFader.ShowFadedInterior();
			}
			else
			{
				roomFader.ShowUnFadedInterior();
			}

			return;
		}

		if (roomFader.PlayerIsBehind())
		{
			roomFader.ShowFadedExterior();
		}
		else
		{
			roomFader.ShowUnFadedExterior();
		}
	}
}
