using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _SCRIPTS
{
	public class DialogueBox : MonoBehaviour
	{
		public Button YesButton;
		public Button NoButton;
		public Text DescriptionText;
		UnityAction OnClickYes;
		UnityAction OnClickNo;
		public HorizontalLayoutGroup horizontalLayoutGroup;


		private void NoButtonClick()
		{
			Hide();
			OnClickNo();
		}


		private void YesButtonClick()
		{
			Hide();
			OnClickYes();
		}

		internal void Show(string description, UnityAction onClickYes, UnityAction onClickNo)
		{

			gameObject.SetActive(true);
			NoButton.gameObject.SetActive(true);

			DescriptionText.text = description;

			OnClickYes = onClickYes;
			OnClickNo = onClickNo;

			YesButton.onClick.AddListener(YesButtonClick);
			NoButton.onClick.AddListener(NoButtonClick);

		}

		internal void Hide()
		{
			YesButton.onClick.RemoveAllListeners();
			NoButton.onClick.RemoveAllListeners();
			DescriptionText.text = "";
			gameObject.SetActive(false);
		}

		internal void Show(string description, UnityAction onClickYes)
		{
			gameObject.SetActive(true);
			NoButton.gameObject.SetActive(false);
			DescriptionText.text = description;

			OnClickYes = onClickYes;

			YesButton.onClick.AddListener(YesButtonClick);
		}
	}
}
