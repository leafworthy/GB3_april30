using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDSlot : MonoBehaviour
{
	public CharacterHUD BeanHUD;
	public CharacterHUD BrockHUD;
	public CharacterHUD KarrotHUD;
	public CharacterHUD TmatoHUD;


	public void SetCharacter(Character newCharacter, Player player)
	{
		DisableAllHUDS();
		switch (newCharacter)
		{
			case Character.Karrot:
				KarrotHUD.gameObject.SetActive(true);
				KarrotHUD.gameObject.GetComponent<CharacterHUD>().SetPlayer(player);
				break;
			case Character.Bean:
				BeanHUD.gameObject.SetActive(true);
				BeanHUD.gameObject.GetComponent<CharacterHUD>().SetPlayer(player);
				break;
			case Character.Brock:
				BrockHUD.gameObject.SetActive(true);
				BrockHUD.gameObject.GetComponent<CharacterHUD>().SetPlayer(player);
				break;
			case Character.Tmato:
				TmatoHUD.gameObject.SetActive(true);
				TmatoHUD.gameObject.GetComponent<CharacterHUD>().SetPlayer(player);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(newCharacter), newCharacter, null);
		}

	}

	private void DisableAllHUDS()
	{

	 BeanHUD.gameObject.SetActive(false);
	 BrockHUD.gameObject.SetActive(false);
	 KarrotHUD.gameObject.SetActive(false);
	 TmatoHUD.gameObject.SetActive(false);
	}
}
