using UnityEngine;
using UnityEngine.UI;

namespace _SCRIPTS
{
	public class ZombieCounter : MonoBehaviour
	{
		private Text text;
		// Start is called before the first frame update
		void Start()
		{
			text = GetComponent<Text>();
		}

		// Update is called once per frame
		void Update()
		{
			var zombieCount = ENEMIES.GetNumberOfLivingEnemies();

			if (zombieCount <= 0)
			{
				text.text = "You Win!";
				GAME.I.EndGameMainMenu();
			}
			else
			{
				text.text = "Zombies Left: " + zombieCount.ToString();

			}

			var livingPlayerCount = PLAYERS.GetNumberOfLivingPlayers();

			if (livingPlayerCount <= 0)
			{
				text.text = "You Lose!";
				GAME.I.EndGameMainMenu();
			}

		}
	}
}
