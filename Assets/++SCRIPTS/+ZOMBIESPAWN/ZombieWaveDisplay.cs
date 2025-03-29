using TMPro;
using UnityEngine;

namespace __SCRIPTS._ZOMBIESPAWN
{
	public class ZombieWaveDisplay:MonoBehaviour
	{
		[SerializeField] private Animator animator;
		[SerializeField]private TextMeshProUGUI WaveText;
		private static readonly int DisplayTrigger = Animator.StringToHash("DisplayTrigger");

		public void DisplayText(string textToDisplay)
		{
			WaveText.text = textToDisplay;
			animator.SetTrigger(DisplayTrigger);
		}
	}
}