using TMPro;
using UnityEngine;

namespace __SCRIPTS
{
	public class MenuTextButton : MenuButton
	{
		private TextMeshProUGUI buttonText =>  _buttonText ??= GetComponentInChildren<TextMeshProUGUI>();
		private TextMeshProUGUI _buttonText;

		private void Awake()
		{
			UnHighlight();
		}

		public override void Highlight()
		{
			buttonText.color = Color.white;
		}

		public override void UnHighlight()
		{
			buttonText.color = Color.red;
		}
	}
}
