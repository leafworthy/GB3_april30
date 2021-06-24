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
		private bool Up_Pressed;
		private bool Down_Pressed;

		public event Action<Player> MoveRight;
		public event Action<Player> MoveLeft;
		public event Action<Player> MoveUp;
		public event Action<Player> MoveDown;
		public event Action<Player> PressA;
		public event Action<Player> PressB;

		public GameObject SpawnedPlayerGO;
		private DefenceHandler spawnedPlayerDefence;
		private IAttackHandler attackHandler;
		public event Action<Player> OnDead;
		public event Action<Player> OnKillEnemy;

		public void Join(CharacterButton firstButton)
		{
			hasJoined = true;
			firstButton.HighlightButton(this);
		}

		public void SetSpawnedPlayerGO(GameObject newGO)
		{
			Debug.Log("spawned player");
			SpawnedPlayerGO = newGO;
			spawnedPlayerDefence = SpawnedPlayerGO.GetComponent<DefenceHandler>();
			spawnedPlayerDefence.OnDying += Die;
			attackHandler = SpawnedPlayerGO.GetComponent<IAttackHandler>();
			attackHandler.OnKillEnemy += KillEnemy;
		}

		private void KillEnemy()
		{
			OnKillEnemy?.Invoke(this);
		}

		private void Die()
		{
			OnDead?.Invoke(this);
		}

		private void Update()
		{

			if (GamePad.GetButton(CButton.B, playerIndex))
			{
				if (!B_Pressed)
				{
					B_Pressed = true;
					PressB?.Invoke(this);
				}
			}
			else
				B_Pressed = false;

			if (GamePad.GetButton(CButton.A, playerIndex))
			{
				if (!A_Pressed)
				{
					A_Pressed = true;
					PressA?.Invoke(this);
				}
			}
			else
				A_Pressed = false;

			HandleLeftRightMoves();
			HandleUpDownMoves();
		}

		private void HandleLeftRightMoves()
		{
			var dpadX = GamePad.GetAxis(CAxis.LX, playerIndex);
			if (dpadX > .5)
			{
				if (!Right_Pressed)
				{
					MoveRight?.Invoke(this);
					Right_Pressed = true;
				}
			}
			else
				Right_Pressed = false;

			if (dpadX < -.5)
			{
				if (!Left_Pressed)
				{
					MoveLeft?.Invoke(this);
					Left_Pressed = true;
				}
			}
			else
				Left_Pressed = false;
		}

		private void HandleUpDownMoves()
		{
			var dpadY = GamePad.GetAxis(CAxis.LY, playerIndex);
			if (dpadY > .5)
			{
				if (!Up_Pressed)
				{
					MoveUp?.Invoke(this);
					Up_Pressed = true;
				}
			}
			else
				Up_Pressed = false;

			if (dpadY < -.5)
			{
				if (!Down_Pressed)
				{
					MoveDown?.Invoke(this);
					Down_Pressed = true;
				}
			}
			else
				Down_Pressed = false;
		}


		public void CleanUp()
		{
			currentCharacter = Character.None;
			hasJoined = false;
			currentButton = null;
			hasSelectedCharacter = false;
			A_Pressed = false;
			B_Pressed = false;
			Left_Pressed = false;
			Right_Pressed = false;
			buttonIndex = 0;
			SpawnedPlayerGO = null;
		}

		public bool IsDead()
		{
			return spawnedPlayerDefence.IsDead();
		}
	}
}
