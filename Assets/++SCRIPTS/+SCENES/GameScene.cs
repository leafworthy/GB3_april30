using UnityEngine;

public abstract class GameScene : MonoBehaviour
{
	public enum Type
	{
		MainMenu,
		CharacterSelect,
		Loading,
		InLevel,
		Paused,
		Endscreen,
		None,
		RestartLevel
	}
	protected bool isActive;



	protected void GoToScene(Type sceneType)
	{
		//Debug.Log("goto scene: " + sceneType);
		SceneLoader.I.SetDestinationScene(sceneType);
	}
}