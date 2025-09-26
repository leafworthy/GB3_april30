using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace __SCRIPTS
{
	public class MenuButtons: MonoBehaviour
	{
		[SerializeField] protected List<MenuButton> menuButtons = new();
		public int CurrentButtonIndex;
		protected MenuButton CurrentlySelectedButton;

		[Button]
		public void ListButtonIndexes()
		{
			for (int i = 0; i < menuButtons.Count; i++)
			{
				Debug.Log($"Button {i}: {menuButtons[i].name}");
			}
		}
		[Button]
		public void InitButtons()
		{
			UnhighlightButtons();
			CurrentButtonIndex = 0;
			CurrentlySelectedButton = menuButtons[CurrentButtonIndex];
			CurrentlySelectedButton.Highlight();
		}

		public void UnhighlightButtons()
		{
			foreach (var button in menuButtons)
			{
				button.UnHighlight();
			}
		}

		protected void IncreaseButtonIndex()
		{
			if (CurrentButtonIndex >= menuButtons.Count - 1)
				CurrentButtonIndex = 0;
			else
				CurrentButtonIndex++;
		}


		protected void DecreaseButtonIndex()
		{
			if (CurrentButtonIndex <= 0)
				CurrentButtonIndex = menuButtons.Count - 1;
			else
				CurrentButtonIndex--;
		}

		protected void HighlightButton(int buttonIndex)
		{
			if (menuButtons[buttonIndex] is null) return;
			menuButtons[buttonIndex].Highlight();
		}

		protected void UnHighlightButton(int buttonIndex)
		{
			if (menuButtons[buttonIndex] is null) return;
			menuButtons[buttonIndex].UnHighlight();
		}

		[Button]
		public void Up()
		{
			UnHighlightButton(CurrentButtonIndex);
			IncreaseButtonIndex();
			HighlightButton(CurrentButtonIndex);
		}

		[Button]
		public void Down()
		{
			UnHighlightButton(CurrentButtonIndex);
			DecreaseButtonIndex();
			HighlightButton(CurrentButtonIndex);
		}

		public MenuButton GetCurrentButton() => menuButtons[CurrentButtonIndex];
	}
}
