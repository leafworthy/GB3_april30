using System;
using UnityEngine;

namespace _SCRIPTS
{
	public enum Character
	{
		None,
		Karrot,
		Bean,
		Brock,
		Tmato
	}

	public class Player : MonoBehaviour
	{
		public int Kills;
		public int Cash;
		public Character currentCharacter;
		public PlayerIndex playerIndex;
		public bool hasJoined;
		public CharacterButton currentButton;
		public bool hasSelectedCharacter;
		public bool A_Pressed;
		public bool B_Pressed;
		public bool Left_Pressed;
		public bool Right_Pressed;
		public int buttonIndex;
		public Color playerColor;
		public event Action<Player> OnJoin;

		public Player(PlayerIndex playerIndex)
		{
			this.playerIndex = playerIndex;
		}

		public void Join(CharacterButton firstButton)
		{
			hasJoined = true;
			firstButton.HighlightButton(this);
		}

		private void Update()
		{
				if (GamePad.GetButton(CButton.A, playerIndex))
				{
					if (!A_Pressed)
					{
						A_Pressed = true;
						Debug.Log("A_Pressed");
						OnJoin?.Invoke(this);

					}
				}
				else
				{
					A_Pressed = false;
				}

		}


		public void CleanUp()
		{
			Kills = 0;
			Cash = 0;
			currentCharacter = Character.None;
			hasJoined = false;
			currentButton = null;
			hasSelectedCharacter = false;
			A_Pressed = false;
			B_Pressed = false;
			Left_Pressed = false;
			Right_Pressed = false;
			buttonIndex = 0;
		}
	}
}
