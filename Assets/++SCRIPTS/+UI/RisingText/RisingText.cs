using TMPro;
using UnityEngine;

namespace RisingTextNamespace
{
	public class RisingText : MonoBehaviour
	{
		private Animator animator;
		private TextMeshProUGUI text;
		private static readonly int WithText = Animator.StringToHash("RiseWithText");

	
	
		public void RiseWithText(string TextToRise)
		{
			animator = GetComponentInChildren<Animator>();
			text = GetComponentInChildren<TextMeshProUGUI>();
			text.text = TextToRise;
			animator.SetTrigger(WithText);
		}
	}
}