using __SCRIPTS;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenericCharacter), true)]
public class GenericCharacterEditor : Editor
{
	public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
	{
		var character = (GenericCharacter)target;
		if (character == null || character.sprite == null)
			return base.RenderStaticPreview(assetPath, subAssets, width, height);

		var preview = AssetPreview.GetAssetPreview(character.sprite);
		if (preview == null)
			return base.RenderStaticPreview(assetPath, subAssets, width, height);

		// Editor takes ownership of the returned texture, so hand back a copy
		var tex = new Texture2D(width, height);
		EditorUtility.CopySerialized(preview, tex);
		return tex;
	}
}
