﻿using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using UnityEngine.Events;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings
{
	public class LightEvent : UnityEvent <Light2D> {}

	public enum MaskLit {Lit, Unlit, LitAbove, Isometric, Custom}

	public enum LightLayerType {ShadowAndMask, ShadowOnly, MaskOnly}

	public enum LightLayerSorting {None, SortingLayerAndOrder, DistanceToLight, YDistanceToLight, YAxisLower, YAxisHigher, ZAxisLower, ZAxisHigher, Isometric};
	public enum LightLayerSortingIgnore {None, IgnoreAbove};

	public enum LightLayerShadowEffect {Default, Soft, LegacyCPU, LegacyGPU, PerpendicularProjection, SoftConvex, SoftVertex, SpriteProjection, Fast};
	public enum LightLayerMaskLit {AlwaysLit, AboveLit, NeverLit};

	public enum LayerSorting {None, ZAxisLower, ZAxisHigher, YAxisLower, YAxisHigher};
	public enum LayerType {ShadowsAndMask, ShadowsOnly, MaskOnly}

	public enum NormalMapTextureType
	{
		Texture,
		Sprite,
		SecondaryTexture
	}

	public enum NormalMapType
	{
		PixelToLight,
		ObjectToLight
	}
}
