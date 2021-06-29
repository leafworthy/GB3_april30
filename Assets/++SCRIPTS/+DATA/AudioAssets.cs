using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/AudioAssets")]
public class AudioAssets : ScriptableObject
{
	public enum SoundType
	{
		shoot,
		knifehit
	}

	public List<AudioClip> player_walk_sounds_concrete = new List<AudioClip>();
	public List<AudioClip> ak47_shoot_sounds = new List<AudioClip>();

	[Header("Brock Sounds")]
	public List<AudioClip> brock_gethit_sounds = new List<AudioClip>();
	public List<AudioClip> brock_walk_sounds = new List<AudioClip>();
	public List<AudioClip> brock_bat_swing_sounds = new List<AudioClip>();
	public List<AudioClip> brock_bathit_sounds = new List<AudioClip>();
	public List<AudioClip> brock_homerunhit_sounds = new List<AudioClip>();
	public List<AudioClip> brock_teleport_sounds = new List<AudioClip>();

	[Header("Cone Sounds")]
	public List<AudioClip> cone_gethit_sounds = new List<AudioClip>();
	public List<AudioClip> cone_walk_sounds = new List<AudioClip>();
	public List<AudioClip> cone_roar_sounds = new List<AudioClip>();
	public List<AudioClip> cone_attack_sounds = new List<AudioClip>();
	public List<AudioClip> cone_die_sounds = new List<AudioClip>();

	[Header("Toast Sounds")]
	public List<AudioClip> toast_gethit_sounds = new List<AudioClip>();
	public List<AudioClip> toast_walk_sounds = new List<AudioClip>();
	public List<AudioClip> toast_roar_sounds = new List<AudioClip>();
	public List<AudioClip> toast_attack_sounds = new List<AudioClip>();
	public List<AudioClip> toast_die_sounds = new List<AudioClip>();

	[Header("Gangsta Bean Sounds")]
	public List<AudioClip> bean_roll_sounds = new List<AudioClip>();
	public List<AudioClip> bean_knifehit_sounds = new List<AudioClip>();
	public List<AudioClip> bean_reload_sounds = new List<AudioClip>();
	public List<AudioClip> bean_nade_throw_sounds = new List<AudioClip>();
	public List<AudioClip> bean_nade_bounce_sounds = new List<AudioClip>();
	public List<AudioClip> bean_nade_explosion_sounds = new List<AudioClip>();


}
