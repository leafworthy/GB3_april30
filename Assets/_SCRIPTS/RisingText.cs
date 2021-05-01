using UnityEngine;
using UnityEngine.UI;

namespace _SCRIPTS
{
	internal class RisingText : MonoBehaviour
	{
		private Animator anim;
		public Text risingText;
		private static float textDisplayLength = 2;
		private static float fadeOutTime = 1;
		public void ShowText(string text)
		{
			anim = GetComponent<Animator>();
			risingText.text = text;
			anim.SetTrigger("rise");
			Invoke("FadeOut", textDisplayLength);
		}

		void FadeOut()
		{
			anim.SetTrigger("rise");
			MAKER.Destroy(gameObject, fadeOutTime);
		}
	}
}