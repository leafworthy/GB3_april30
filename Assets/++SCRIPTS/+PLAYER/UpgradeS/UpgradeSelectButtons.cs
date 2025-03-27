using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UpgradeS
{
	public class UpgradeSelectButtons : MonoBehaviour
	{
		public UpgradeSelectButton DefaultButton;
		public event Action OnExit;
		public event Action<Upgrade> OnUpgradeChosen;

		private UpgradeSelectButton currentlySelectedButton;

		[SerializeField] private UpgradeSelectButton[] buttons;
		private Player _player;
		private bool hasInit;
		public TextMeshProUGUI upgradeDescription;

		public void Init(Player player)
		{
			_player = player;
			hasInit = true;
			ResetButtons();
			Debug.Log("buttons initialized");
			player.Controller.UIAxis.OnLeft += OnLeft;
			player.Controller.UIAxis.OnRight += OnRight;
			player.Controller.Select.OnPress += OnSelect;
			player.Controller.Cancel.OnPress += OnCancel;
		}

		private void ResetButtons()
		{
			buttons[0] = DefaultButton;
			DeselectAllButtons();
			SetCurrentButton(DefaultButton);
		}

		private void SetCurrentButton(UpgradeSelectButton defaultButton)
		{
			currentlySelectedButton = defaultButton;
			if(defaultButton.upgrade.GetCost() > _player.GetComponent<PlayerStats>().GetStatValue(PlayerStat.StatType.TotalCash))
			{
				currentlySelectedButton.RedHighlight();
			}
			else
			{
				currentlySelectedButton.Highlight();
			}
			
			upgradeDescription.text = currentlySelectedButton.upgrade.GetDescription;
		}

		private void OnDisable()
		{
			if (!hasInit) return;
			hasInit = false;
			_player.Controller.UIAxis.OnLeft -= OnLeft;
			_player.Controller.UIAxis.OnRight -= OnRight;
			_player.Controller.Select.OnPress -= OnSelect;
			_player.Controller.Cancel.OnPress -= OnCancel;
		}

		private void DeselectAllButtons()
		{
			foreach (var button in buttons)
			{
				button.Unhighlight();
				button.Deselect();
			}
		}

		private void OnCancel(NewControlButton obj)
		{
			SFX.sounds.charSelect_deselect_sounds.PlayRandom();
			currentlySelectedButton.Deselect();
			Debug.Log("Exit button pressed");
			OnExit?.Invoke();
		}

		private void OnSelect(NewControlButton obj)
		{
			ChooseUpgrade();
		}

		private void ChooseUpgrade()
		{
			currentlySelectedButton.Select();
			SFX.sounds.charSelect_select_sounds.PlayRandom();
			OnUpgradeChosen?.Invoke(currentlySelectedButton.upgrade);
			currentlySelectedButton.transform.DOPunchScale(Vector3.one * .1f, 0.5f, 1, 0.2f);
		}

		private void OnRight(NewInputAxis obj)
		{
			Debug.Log("on right");
			SFX.sounds.charSelect_move_sounds.PlayRandom();
			currentlySelectedButton.Unhighlight();
			currentlySelectedButton = currentlySelectedButton.buttonToRight;
			currentlySelectedButton.Highlight();
			upgradeDescription.text = currentlySelectedButton.upgrade.GetDescription;
		}

		private void OnLeft(NewInputAxis obj)
		{
			Debug.Log("on left");
			SFX.sounds.charSelect_move_sounds.PlayRandom();
			currentlySelectedButton.Unhighlight();
			currentlySelectedButton = currentlySelectedButton.buttonToLeft;
			currentlySelectedButton.Highlight();
			upgradeDescription.text = currentlySelectedButton.upgrade.GetDescription;
		}
	}
}