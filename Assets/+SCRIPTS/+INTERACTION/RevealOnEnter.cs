using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	public class RevealOnEnter : MonoBehaviour
	{
		// Start is called before the first frame update
		private HideRevealObjects hideRevealObjects;
	
		private List<Player> playersInside = new();

		protected void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject == gameObject) return;

			var life = other.GetComponent<Life>();
			if (life == null)
			{
				life = other.GetComponentInParent<Life>();
				if (life == null) return;
			}

			var player = life.Player;
			if (player == null)
			{
				Hide();
				return;
			}
			if(playersInside.Contains(player)) return;
			playersInside.Add(player);
			Reveal();
		}

		private void OnTriggerExit2D(Collider2D other)
		{
	

			var life = other.GetComponent<Life>();
			if (life == null)
			{
				life = other.GetComponentInParent<Life>();
				if (life == null) return;
			}

			var player = life.Player;
			if (player == null)
			{
				return;
			
			}

			if (!player.IsPlayer()) return;
			if (playersInside.Contains(player)) playersInside.Remove(player);
			if (playersInside.Count == 0) Hide();
		}

		protected void Start()
		{
			hideRevealObjects = GetComponent<HideRevealObjects>();
			hideRevealObjects.Set(1);
		}

		private void Hide()
		{

			hideRevealObjects.Set(1);
		}

		private void Reveal()
		{

			hideRevealObjects.Set(0);
		}

	}
}