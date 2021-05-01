using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _SCRIPTS
{
	public class DISPLAY:Singleton<DISPLAY>
	{
		private static DialogueBox dialogueBox;
		private static MenuBox menuBox;
		public GameObject DialogueBoxPrefab;
		public GameObject MenuBoxPrefab;
		public GameObject RisingTextPrefab;

		public static void Init()
		{
			dialogueBox = MAKER.Make(I.DialogueBoxPrefab, Vector3.zero).GetComponent<DialogueBox>();
			dialogueBox.Hide();
			menuBox = MAKER.Make(I.MenuBoxPrefab, Vector3.zero).GetComponentInChildren<MenuBox>();
			menuBox.Hide();
		}

		public static void ShowDialogueBox(string description, UnityAction OnClickYes, UnityAction OnClickNo)
		{
			dialogueBox.Show(description, OnClickYes, OnClickNo);
		}

		public static void ShowDialogueBox(string description, UnityAction OnClickYes)
		{
			dialogueBox.Show(description, OnClickYes);
		}

		public static void HideDialogueBox()
		{
			dialogueBox.Hide();
		}

		public static void ShowMenuBox(string title, string description, List<ButtonAction> buttons)
		{
			menuBox.Show(title, description, buttons);
		}

		public static void DisplayRisingText(string textToDisplay, Vector2 pos)
		{
			RisingText risingText = MAKER.Make(I.RisingTextPrefab, pos).GetComponent<RisingText>();
			risingText.ShowText(textToDisplay);
		}

		public static void DisplayRisingNumber(float number, Vector2 pos)
		{
			RisingText risingText = MAKER.Make(I.RisingTextPrefab, pos).GetComponent<RisingText>();
			risingText.ShowText(Mathf.Round(number).ToString());
		}
	}
}
