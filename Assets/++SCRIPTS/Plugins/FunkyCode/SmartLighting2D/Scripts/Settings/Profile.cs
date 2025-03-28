﻿using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings.Presets;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings
{
	[CreateAssetMenu(fileName = "Data", menuName = "Light 2D/Profile", order = 1)]

	public class Profile : ScriptableObject
	{
		public LightmapPresetList lightmapPresets;

		public LightPresetList lightPresets;

		public EventPresetList eventPresets;

		public QualitySettings qualitySettings;

		public DayLightingSettings dayLightingSettings;

		public Layers layers;

		public Color DarknessColor
		{
			get => lightmapPresets.list[0].darknessColor;

			set => lightmapPresets.list[0].darknessColor = value;
		}

		public Profile() {

			layers = new Layers();

			qualitySettings = new QualitySettings();

			lightmapPresets = new LightmapPresetList();
			lightmapPresets.list[0] = new LightmapPreset(0);
			lightmapPresets.list[0].darknessColor = new Color(0, 0, 0, 1);

			lightPresets = new LightPresetList();
			lightPresets.list[0] = new LightPreset(0);

			eventPresets = new EventPresetList();
			eventPresets.list[0] = new EventPreset(0);
			eventPresets.list[1] = new EventPreset(1);

			dayLightingSettings = new DayLightingSettings();
		}
	}
}
