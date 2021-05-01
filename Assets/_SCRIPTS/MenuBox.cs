using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _SCRIPTS
{
	public class MenuBox : MonoBehaviour
	{
		private List<Button> currentButtons = new List<Button>();
		public GameObject buttonPrefab;
		public VerticalLayoutGroup layoutGroup;
		public Text DescriptionText;
		public Text TitleText;


		internal void Show(string title, string description, List<ButtonAction> buttons)
		{
			gameObject.SetActive(true);
			TitleText.text = title;
			DescriptionText.text = description;
			buildButtons(buttons);
		}

		private void buildButtons(List<ButtonAction> buttons)
		{
			DestroyOldButtons();
			foreach (ButtonAction button in buttons)
			{
				Button newButton = MAKER.Make(buttonPrefab,Vector3.zero).GetComponent<Button>();
				if (newButton != null)
				{
					newButton.GetComponentInChildren<Text>().text = button.Name;
					newButton.onClick.AddListener(() => { Hide(); button.action(); });
					newButton.transform.SetParent(layoutGroup.transform);
					currentButtons.Add(newButton);
				}
			}
		}

		private void DestroyOldButtons()
		{
			var oldButtons = layoutGroup.gameObject.GetComponentsInChildren<Button>();
			foreach (Button button in oldButtons)
			{
				MAKER.Destroy(button.gameObject);
			}
		}

		internal void Hide()
		{
			DescriptionText.text = "";
			TitleText.text = "";
			foreach (Button button in currentButtons)
			{
				button.onClick.RemoveAllListeners();
				MAKER.Destroy(button.gameObject);
			}
			currentButtons.Clear();
			gameObject.SetActive(false);
		}

	}
}
