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
		RestartLevel,
		GasStation
	}
	protected bool isActive;
	
	// Virtual property that derived classes can override to specify their scene type
	public virtual Type SceneType => Type.None;


	/// <summary>
	/// Navigate to a different scene
	/// </summary>
	/// <param name="sceneType">The type of scene to load</param>
	/// <param name="useLevelTransition">Whether to use the special level transition screen</param>
	protected void GoToScene(Type sceneType, bool useLevelTransition = false)
	{
		//Debug.Log("goto scene: " + sceneType);
		SceneLoader.I.SetDestinationScene(sceneType, useLevelTransition);
	}
}