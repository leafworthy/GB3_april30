using TMPro;
using UnityEngine;

namespace GangstaBean.UI.RisingText
{
	public class RisingText : MonoBehaviour
	{
		private Animator animator;
		private TextMeshProUGUI text;
		private static readonly int WithText = Animator.StringToHash("RiseWithText");

	
	
		public void RiseWithText(string TextToRise, Color textColor)
		{
			animator = GetComponentInChildren<Animator>();
			text = GetComponentInChildren<TextMeshProUGUI>();
			text.text = TextToRise;
			text.color = textColor;
			animator.SetTrigger(WithText);
		}
	}
}