using System;
using UnityEngine;

public abstract class Scene : MonoBehaviour
{
	public enum Type
	{
		MainMenu,
		CharacterSelect,
		Loading,
		InLevel,
		Paused,
		Endscreen,
		None
	}
	protected bool isActive;



	protected void GoToScene(Type sceneType)
	{
		SceneLoader.I.SetDestinationScene(sceneType);
	}
}