using System.Collections.Generic;
using System.Linq;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class Tinter : MonoBehaviour, INeedPlayer
	{
		List<Renderer> renderersToTint = new();
		Color materialTintColor;
		const float tintFadeSpeed = 6f;
		IHaveUnitStats stats => _stats ??= GetComponent<IHaveUnitStats>();
		IHaveUnitStats _stats;

		static readonly int ColorReplaceColorA = Shader.PropertyToID("_NewColorA");
		static readonly int ColorReplaceColorB = Shader.PropertyToID("_NewColorB");
		static readonly int Tint = Shader.PropertyToID("_Tint");

		public void StartTint(Color tintColor)
		{
			materialTintColor = tintColor;
			renderersToTint = GetComponentsInChildren<Renderer>().ToList();
			if (renderersToTint == null) return;
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(Tint, materialTintColor);
			}
		}

		void Update()
		{
			FadeOutTintAlpha();
		}

		void FadeOutTintAlpha()
		{
			if (!(materialTintColor.a > 0)) return;
			materialTintColor.a = Mathf.Clamp01(materialTintColor.a - tintFadeSpeed * Time.deltaTime);
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(Tint, materialTintColor);
			}
		}

		void RecolorSprite(Color color)
		{
			renderersToTint = GetComponentsInChildren<Renderer>().ToList();
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(ColorReplaceColorA, color);
				r.material.SetColor(ColorReplaceColorB, color);
			}
		}

		public void SetPlayer(Player newPlayer)
		{
			if (!newPlayer.IsHuman()) return;
			RecolorSprite(newPlayer.playerColor);
		}

		public void TintDebree(GameObject forwardDebree)
		{
			var spriteToTint = forwardDebree.GetComponentInChildren<SpriteRenderer>();
			if (spriteToTint != null) spriteToTint.color = stats.Stats.DebrisTint;
		}
	}
}
