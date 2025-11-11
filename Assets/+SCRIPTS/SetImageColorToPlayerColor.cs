using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;
using UnityEngine.UI;

public class SetImageColorToPlayerColor : MonoBehaviour,INeedPlayer
{
	public Image targetImage;

	public void SetPlayer(Player newPlayer)
	{
		targetImage.color = newPlayer.playerColor;
		Debug.LogWarning("HEYYYYYY");
	}
}
