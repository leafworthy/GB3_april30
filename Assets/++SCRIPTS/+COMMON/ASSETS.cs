using __SCRIPTS._UI;
using UnityEngine;

namespace __SCRIPTS._COMMON
{
	public class ASSETS : Singleton<ASSETS>
	{
		private void Start()
		{
	
			_levels = Resources.Load<LevelAssets>("Assets/Levels");
			_players = Resources.Load<CharacterPrefabAssets>("Assets/Players");
			_ui = Resources.Load<UIAssets>("Assets/UI");
		}





		public static LevelAssets LevelAssets => I._levels ? I._levels : Resources.Load<LevelAssets>("Assets/Levels");
		private LevelAssets _levels;

		public static CharacterPrefabAssets Players => I._players ? I._players : Resources.Load<CharacterPrefabAssets>("Assets/Players");
		private CharacterPrefabAssets _players;

		public static UIAssets ui => I._ui? I._ui:  Resources.Load<UIAssets>("Assets/UI");
		private UIAssets _ui;
	}
}
