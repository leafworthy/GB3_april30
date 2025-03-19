using UnityEngine;
using UnityEngine.UI;

public class HUDAmmoChanger : MonoBehaviour, INeedPlayer
{
	public AmmoDisplay ammoDisplay;
	public Image AKIcon;
	public Image PistolIcon;
	private Player _player;
	private GunAttack gunAttack;
	private bool isGlocking;
	
	public void SetPlayer(Player player)
	{
		_player = player;
		if (player.CurrentCharacter != Character.Bean) return;
		gunAttack = player.SpawnedPlayerGO.GetComponent<GunAttack>();
	}

	private void Update()
	{
		if(_player == null) return;
		if (_player.CurrentCharacter != Character.Bean) return;
		gunAttack = _player.SpawnedPlayerGO.GetComponent<GunAttack>();
		if(isGlocking == gunAttack.isGlocking) return;
		isGlocking = gunAttack.isGlocking;
		ChangeAmmo(isGlocking);
	}

	private void ChangeAmmo(bool isGlock)
	{
		if (_player.CurrentCharacter != Character.Bean) return;
		if (isGlock)
		{
			AKIcon.enabled = false;
			PistolIcon.enabled = true;
			ammoDisplay.SetAmmo( _player.SpawnedPlayerGO.GetComponent<AmmoInventory>().unlimitedAmmo);
		}
		else
		{
			AKIcon.enabled = true;
			PistolIcon.enabled = false;
			ammoDisplay.SetAmmo(_player.SpawnedPlayerGO.GetComponent<AmmoInventory>().primaryAmmo);
		}
	}
}