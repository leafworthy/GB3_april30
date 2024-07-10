using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/AudioAssets")]
public class AudioAssets : ScriptableObject
{
	
	public List<AudioClip> player_walk_sounds_concrete = new();
	public List<AudioClip> ak47_shoot_sounds = new();
	public List<AudioClip> jump_sound = new();
	public List<AudioClip> land_sound = new();

	[Header("Brock Sounds")]
	public List<AudioClip> brock_gethit_sounds = new();
	public List<AudioClip> brock_charge_sounds = new();
	public List<AudioClip> brock_bat_swing_sounds = new();
	public List<AudioClip> brock_bathit_sounds = new();
	public List<AudioClip> brock_homerunhit_sounds = new();
	public List<AudioClip> brock_teleport_sounds = new();

	[Header("Cone Sounds")]
	public List<AudioClip> cone_gethit_sounds = new();
	public List<AudioClip> cone_walk_sounds = new();
	public List<AudioClip> cone_roar_sounds = new();
	public List<AudioClip> cone_attack_sounds = new();
	public List<AudioClip> cone_die_sounds = new();

	[Header("Toast Sounds")]
	public List<AudioClip> toast_gethit_sounds = new();
	public List<AudioClip> toast_walk_sounds = new();
	public List<AudioClip> toast_roar_sounds = new();
	public List<AudioClip> toast_attack_sounds = new();
	public List<AudioClip> toast_die_sounds = new();

	[Header("Gangsta Bean Sounds")]
	public List<AudioClip> bean_roll_sounds = new();
	public List<AudioClip> bean_knifehit_sounds = new();
	public List<AudioClip> bean_reload_sounds = new();
	public List<AudioClip> bean_nade_throw_sounds = new();
	public List<AudioClip> bean_nade_bounce_sounds = new();
	public List<AudioClip> bean_nade_explosion_sounds = new();
	public List<AudioClip> bean_gun_miss_sounds = new();

	[Header("UI Sounds")]
	public List<AudioClip> pickup_pickup_sounds = new();

	public List<AudioClip> pickup_speed_sounds = new();
	public List<AudioClip> press_start_sounds = new();
	public List<AudioClip> pauseMenu_start_sounds = new();
	public List<AudioClip> pauseMenu_stop_sounds = new();
	public List<AudioClip> pauseMenu_select_sounds = new();
	public List<AudioClip> pauseMenu_move_sounds = new();
	public List<AudioClip> charSelect_move_sounds= new();
	public List<AudioClip> charSelect_select_sounds = new();
	public List<AudioClip> charSelect_deselect_sounds = new();
	public List<AudioClip> bloodSounds;

	public List<AudioClip> GetHitSounds(int debree)
	{
		switch (debree)
		{
			case 0:
				return bloodSounds;
			case 1:
			case 2:
			case 3:
			case 4:
				return null;
		}

		return null;
	}
}