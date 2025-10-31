using System.Linq;
using __SCRIPTS;
using GangstaBean.Core;
using UnityEngine;

public class PlayerColorReplace : MonoBehaviour, INeedPlayer
{
	private Player player;
	private static readonly int ColorReplaceColorA = Shader.PropertyToID("_NewColorA");
	private static readonly int ColorReplaceColorB = Shader.PropertyToID("_NewColorB");
	private Color materialTintColor;

	public void SetPlayer(Player _player)
	{
		player = _player;
		PlayerTint(player.playerColor);
	}

	private void PlayerTint(Color color)
	{
		var renderersToTint = GetComponentsInChildren<Renderer>().ToList();
		foreach (var r in renderersToTint)
		{
			r.material.SetColor(ColorReplaceColorA, color);
			r.material.SetColor(ColorReplaceColorB, color);
		}
	}
}
