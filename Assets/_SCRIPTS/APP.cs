using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace _SCRIPTS
{
	[Serializable]
	public class APP : Singleton<APP>
	{
		private const string FilePath = "/savedUsers.leaf";
		private User currentUser;

		private List<ButtonAction> mainMenuButtons = new List<ButtonAction>();

		private void Start()
		{
			GAME.I.StartMainMenu();
		}

		private void DisplayMainMenuBox()
		{
			mainMenuButtons.Clear();
			if (SavedFileExists())
				mainMenuButtons.Add(new ButtonAction("Load", LoadSavedUserAndStartNewGame));
			else
			{
				Debug.Log("New User");
				currentUser = new User("New Default Player", new List<Player>());
			}

			mainMenuButtons.Add(new ButtonAction("Quit", QuitApplication));

			DISPLAY.ShowMenuBox("The Pirate Witch", "Main Menu", mainMenuButtons);
		}

		private bool SavedFileExists()
		{
			return File.Exists(Application.persistentDataPath + FilePath);
		}

		private void LoadSavedUserAndStartNewGame()
		{
			var loadedUser = LoadUser();
			if (loadedUser != null)
			{
				Debug.Log("user loaded" + loadedUser.userName);
				currentUser = loadedUser;
			}
			else
				Debug.Log("saved user invalid");
		}


		private void OnGameEnd()
		{
			SaveUser();
			DisplayMainMenuBox();
		}

		public static void SaveUser()
		{
			var bf = new BinaryFormatter();
			var file = File.Create(Application.persistentDataPath + FilePath);
			bf.Serialize(file, I.currentUser);
			file.Close();
		}

		public static User LoadUser()
		{
			var bf = new BinaryFormatter();
			var file = File.Open(Application.persistentDataPath + FilePath, FileMode.Open);
			var LoadedUser = (User) bf.Deserialize(file);
			file.Close();
			return LoadedUser;
		}

		public void QuitApplication()
		{
			Application.Quit();
		}
	}
}
