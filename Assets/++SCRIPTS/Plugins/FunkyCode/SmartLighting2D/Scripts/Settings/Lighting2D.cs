﻿using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Material;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings.Presets;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings
{
	public static class Lighting2D
	{
		public const int VERSION = 20211104;
		public const string VERSION_STRING = "2021.11.4";

		static public Lighting2DMaterials materials = new Lighting2DMaterials();

		// disable
		static public bool disable => false;

		// lightmaps
		static public LightmapPreset[] LightmapPresets => Profile.lightmapPresets.list;

		// quality
		static public QualitySettings QualitySettings => Profile.qualitySettings;
		
		// day lighting
		static public DayLightingSettings DayLightingSettings => Profile.dayLightingSettings;

		static public RenderingMode RenderingMode => ProjectSettings.renderingMode;

		static public CoreAxis CoreAxis => Profile.qualitySettings.coreAxis;

		// set & get
		static public Color DarknessColor => LightmapPresets[0].darknessColor;

		static public float Resolution => LightmapPresets[0].resolution;

		// methods
		static public void UpdateByProfile(Profile setProfile)
		{
			if (setProfile == null)
			{
				UnityEngine.Debug.Log("Light 2D: Update Profile is Missing");
				return;
			}
			
			// set profile also
			profile = setProfile;
		}

		static public void RemoveProfile()
		{
			profile = null;
		}

		// profile
		static private Profile profile = null;
		static public Profile Profile
		{
			get
			{
				if (profile != null)
				{
					return(profile);
				}

				if (ProjectSettings != null)
				{
					profile = ProjectSettings.Profile;
				}

				if (profile == null)
				{
					profile = Resources.Load("Profiles/Default Profile") as Profile;

					if (profile == null)
					{
						UnityEngine.Debug.LogError("Light 2D: Default Profile not found");
					}
				}

				return(profile);
			}
		}

		static private ProjectSettings projectSettings = null;
		static public ProjectSettings ProjectSettings
		{
			get
			{
				if (projectSettings != null)
				{
					return(projectSettings);
				}

				projectSettings = Resources.Load("Settings/Project Settings") as ProjectSettings;

				if (projectSettings == null)
				{
					UnityEngine.Debug.LogError("Light 2D: Project Settings not found");
					return(null);
				}
			
				return(projectSettings);
			}
		}
	}
}

//MyScriptableObjectClass asset = ScriptableObject.CreateInstance<MyScriptableObjectClass>();

//AssetDatabase.CreateAsset(asset, "Assets/NewScripableObject.asset");
//AssetDatabase.SaveAssets();

//EditorUtility.FocusProjectWindow();
