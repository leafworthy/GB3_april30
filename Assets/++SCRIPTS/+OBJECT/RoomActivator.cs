using UnityEngine;

public class RoomActivator : MonoBehaviour
{
	[SerializeField] private Room room;
	[SerializeField] private CapturePlayerTrigger2D activationTrigger;

	private void Awake()
	{

		if (room.ContainsPlayer())
		{
			room.ShowUnFadedInterior();
		}
		else
		{
			room.ShowUnFadedExterior();
		}
	}


	private void Update()
	{
		if (activationTrigger.ContainsPlayer())
		{
			Debug.Log("activation");
			if (room.PlayerIsBehind())
			{
				room.ShowFadedInterior();
			}
			else
			{
				room.ShowUnFadedInterior();
			}

			return;
		}

		if (room.PlayerIsBehind())
		{
			room.ShowFadedExterior();
		}
		else
		{
			room.ShowUnFadedExterior();
		}
	}
}
