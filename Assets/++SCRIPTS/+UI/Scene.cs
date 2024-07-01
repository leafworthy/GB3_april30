using __SCRIPTS._SCENES;
using UnityEngine;

namespace __SCRIPTS._UI
{
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
}