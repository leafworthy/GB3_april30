using System.Collections.Generic;
using System.Linq;
using GangstaBean.Core;
using UnityEngine;

namespace __SCRIPTS
{
	public class Tinter : MonoBehaviour, INeedPlayer
	{
		public List<Renderer> renderersToTint = new();
		Color materialTintColor;
		Color materialColor;
		const float tintFadeSpeed = 6f;
		IHaveUnitStats stats => _stats ??= GetComponent<IHaveUnitStats>();
		IHaveUnitStats _stats;
		bool IsFadingOutColor;

		static readonly int ColorReplaceColorA = Shader.PropertyToID("_NewColorA");
		static readonly int ColorReplaceColorB = Shader.PropertyToID("_NewColorB");
		static readonly int Tint = Shader.PropertyToID("_Tint");
		static readonly int Color = Shader.PropertyToID("_Color");

		void Update()
		{
			FadeOutTintAlpha();
			if (IsFadingOutColor) FadeOutColorAlpha();
		}

		public void StartTint(Color tintColor)
		{
			materialTintColor = tintColor;
			if (renderersToTint.Count == 0) renderersToTint = GetComponentsInChildren<Renderer>().ToList();
			if (renderersToTint == null) return;
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(Tint, materialTintColor);
			}
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

		public void StartFadeOut()
		{
			IsFadingOutColor = true;
			materialColor = new Color(1, 1, 1, 1);
		}

		void FadeOutColorAlpha()
		{
			if ((materialColor.a <= 0))
			{
				Services.objectMaker.Unmake(gameObject);
				return;
			}
			materialColor.a = Mathf.Clamp01(materialColor.a - tintFadeSpeed/4 * Time.deltaTime);
			foreach (var r in renderersToTint)
			{
				r.material.SetColor(Color, materialColor);
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
