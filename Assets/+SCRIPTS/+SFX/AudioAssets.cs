using System;
using System.Collections.Generic;
using UnityEngine;

namespace __SCRIPTS
{
	[CreateAssetMenu(menuName = "My Assets/AudioAssets")]
	public class AudioAssets : ScriptableObject
	{

		public List<AudioClip> player_walk_sounds_concrete = new();
		public List<AudioClip> ak47_shoot_sounds = new();
		public List<AudioClip> ak47_empty_shoot_sounds = new();
		public List<AudioClip> jump_sound = new();
		public List<AudioClip> land_sound = new();
		public List<AudioClip> player_die_sounds = new();


		public List<AudioClip> brock_gethit_sounds = new();
		[Header("Brock Sounds")]
		public List<AudioClip> brock_charge_sounds = new();
		public List<AudioClip> brock_bat_swing_sounds = new();
		public List<AudioClip> brock_bathit_sounds = new();
		public List<AudioClip> brock_homerunhit_sounds = new();
		public List<AudioClip> brock_special_attack_sounds = new();
		public List<AudioClip> brock_teleport_sounds = new();

		[Header("Cone Sounds")]
		public List<AudioClip> cone_gethit_sounds = new();
		public List<AudioClip> cone_walk_sounds = new();
		public List<AudioClip> cone_roar_sounds = new();
		public List<AudioClip> cone_attack_sounds = new();
		public List<AudioClip> cone_splat_sounds = new();
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

		[Header("TMato Sounds")]
		public List<AudioClip> tmato_shoot_hit_sounds = new();
		public List<AudioClip> tmato_shoot_miss_sounds = new();
		public List<AudioClip> tmato_mine_throw_sounds = new();
		public List<AudioClip> tmato_shield_dash_sounds = new();
		public List<AudioClip> tmato_shield_hit_sounds = new();
		public List<AudioClip> tmato_chainsaw_start_sounds = new();
		public List<AudioClip> tmato_chainsaw_attack_start_sounds = new();
		public List<AudioClip> tmato_chainsaw_attack_idl_sounds = new();
		public List<AudioClip> tmato_chainsaw_attack_stop_sounds = new();
		public List<AudioClip> tmato_reload_sounds = new();

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
		public List<AudioClip> bloodSounds = new();

		[Header("Car Sounds")] public List<AudioClip> car_start_sound = new();

		[Header("Interaction Sounds")] public List<AudioClip> siphon_gas_sound = new();
		public List<AudioClip> chest_open_sound = new();
		public List<AudioClip> fridge_open_sound = new();
		public List<AudioClip> trash_open_sound = new();
		public List<AudioClip> drawer_open_sound = new();
		public List<AudioClip> door_open_sound = new();
		public List<AudioClip> door_close_sound = new();
		public List<AudioClip> door_repair_sound = new();
		public List<AudioClip> door_break_sound = new();
		public List<AudioClip> light_switch_sound = new();

		[Header("Donut Sounds")] public List<AudioClip> donut_roar_sounds = new();
		public List<AudioClip> donut_walk_sounds = new();
		public List<AudioClip> donut_hit_sounds = new();
		public List<AudioClip> donut_die_sounds = new();

		public List<AudioClip> bullet_hit_blood_sounds = new();
		public List<AudioClip> bullet_hit_glass_sounds = new();
		public List<AudioClip> bullet_hit_metal_sounds = new();
		public List<AudioClip> kunai_hit_sounds = new();


		public List<AudioClip> GetBulletHitSounds(DebrisType debris)
		{
			return debris switch
			       {
				       DebrisType.blood => bullet_hit_blood_sounds,
				       DebrisType.glass => bullet_hit_glass_sounds,
				       DebrisType.wood or DebrisType.metal => bullet_hit_metal_sounds,
				       DebrisType.wall => bean_gun_miss_sounds,
				       DebrisType.none => bean_gun_miss_sounds,
				       _ => throw new ArgumentOutOfRangeException(nameof(debris), debris, null)
			       };
		}
	}
}
