using TMPro;
using UnityEngine;

namespace __SCRIPTS.RisingText
{
	public class RisingText : MonoBehaviour
	{
		Animator animator;
		TextMeshProUGUI text;
		static readonly int WithText = Animator.StringToHash("RiseWithText");

		public void RiseWithText(string TextToRise, Color textColor, float fontSize = 3)
		{
			animator = GetComponentInChildren<Animator>();
			text = GetComponentInChildren<TextMeshProUGUI>();
			text.text = TextToRise;
			text.color = textColor;
			text.fontSize = fontSize;
			animator.SetTrigger(WithText);
		}
	}
}
