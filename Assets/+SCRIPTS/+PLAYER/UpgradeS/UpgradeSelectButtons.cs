using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace __SCRIPTS.UpgradeS
{
	public class UpgradeSelectButtons : ServiceUser
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

			player.Controller.OnUIAxis_Left += OnLeft;
			player.Controller.OnUIAxis_Right += OnRight;
			player.Controller.OnSelect_Pressed += OnSelect;
			player.Controller.OnCancel_Pressed += OnCancel;
		}

		private void ResetButtons()
		{
			buttons[0] = DefaultButton;
			DeselectAllButtons();
			SetCurrentButton(DefaultButton);
			UpdateAllButtons();
		}

		private void UpdateAllButtons()
		{
			foreach (var button in buttons)
			{
				button.RefreshText();
			}
		}

		private void SetCurrentButton(UpgradeSelectButton defaultButton)
		{
			currentlySelectedButton = defaultButton;
			if (CanAfford(defaultButton))
			{
				currentlySelectedButton.RedHighlight();
			}
			else
			{
				currentlySelectedButton.Highlight();
			}

			upgradeDescription.text = currentlySelectedButton.upgrade.GetDescription;
		}



		private bool CanAfford(UpgradeSelectButton defaultButton) => defaultButton.upgrade.Cost > _player.GetComponent<PlayerStats>().GetStatValue(PlayerStat.StatType.TotalCash);

		private void OnDisable()
		{
			if (!hasInit) return;
			hasInit = false;
			_player.Controller.OnUIAxis_Left -= OnLeft;
			_player.Controller.OnUIAxis_Right -= OnRight;
			_player.Controller.OnSelect_Pressed -= OnSelect;
			_player.Controller.OnCancel_Pressed -= OnCancel;
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
			sfx.sounds.charSelect_deselect_sounds.PlayRandom();
			currentlySelectedButton.Deselect();

			OnExit?.Invoke();
		}

		private void OnSelect(NewControlButton obj)
		{
			ChooseUpgrade();
		}

		private void ChooseUpgrade()
		{
			OnUpgradeChosen?.Invoke(currentlySelectedButton.upgrade);
			currentlySelectedButton.transform.DOPunchScale(Vector3.one * .1f, 0.5f, 1, 0.2f);
			SetCurrentButton(currentlySelectedButton);
		}

		private void OnRight(NewInputAxis obj)
		{

			sfx.sounds.charSelect_move_sounds.PlayRandom();
			currentlySelectedButton.Unhighlight();
			SetCurrentButton(currentlySelectedButton.buttonToRight);
			currentlySelectedButton.transform.DOPunchScale(Vector3.one * .1f, 0.5f, 1, 0.1f);
		}

		private void OnLeft(NewInputAxis obj)
		{

			sfx.sounds.charSelect_move_sounds.PlayRandom();
			currentlySelectedButton.Unhighlight();
			SetCurrentButton(currentlySelectedButton.buttonToLeft);
		}
	}
}
