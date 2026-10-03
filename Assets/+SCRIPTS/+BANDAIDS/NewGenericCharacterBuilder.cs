using System.Collections.Generic;
using System.IO;
using System.Linq;
using GangstaBean.Core;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS
{
	public class NewGenericBodyBuilder : SerializedMonoBehaviour, INeedPlayer
	{
		public GenericBody currentBody;
		public Color Tint = Color.white;
		public List<SpriteRenderer> ToTint;
		public Color BodyTint = Color.white;
		public bool autoRefresh = true;
		public SpriteRenderer bodySpriteRenderer;
		public Sprite bodySprite;
		public string displayName;
		public GameObject leftArm;
		public GameObject rightArm;
		public GameObject leftLeg;
		public GameObject rightLeg;
		public GameObject face;
		public SpriteRenderer faceSpriteRenderer;
		const string DefaultFolder = "Assets/Resources/Generic Bodies";
		List<Sprite> fruitFaces =>  Resources.LoadAll<Sprite>("Generic Faces").ToList();
		List<GenericBody> fruitBodies => Resources.LoadAll<GenericBody>("Generic Bodies").ToList();



#if UNITY_EDITOR
		[Button]
		public void SaveOverCurrentBody()
		{
			if (currentBody == null)
			{
				Debug.LogWarning("No GenericBody assigned to save over.");
				return;
			}

			// Record undo for safety
			Undo.RecordObject(currentBody, "Save GenericBody");

			currentBody.leftArmOffset = leftArm.transform.localPosition;
			currentBody.leftLegOffset = leftLeg.transform.localPosition;
			currentBody.rightArmOffset = rightArm.transform.localPosition;
			currentBody.rightLegOffset = rightLeg.transform.localPosition;
			currentBody.faceOffset = face.transform.localPosition;
			currentBody.bodySprite = bodySprite;
			currentBody.tintColor = Tint;

			// Mark dirty & save
			EditorUtility.SetDirty(currentBody);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Debug.Log($"Saved changes to {currentBody.name}");
		}
#endif

		[Button]
		public void ApplyCharacter()
		{
			Tint = currentBody.tintColor;
			leftArm.transform.localPosition = currentBody.leftArmOffset;
			rightArm.transform.localPosition = currentBody.rightArmOffset;
			leftLeg.transform.localPosition = currentBody.leftLegOffset;
			rightLeg.transform.localPosition = currentBody.rightLegOffset;
			face.transform.localPosition = currentBody.faceOffset;
			bodySpriteRenderer.sprite = currentBody.bodySprite;
			BodyTint = currentBody.bodyTintColor;
			bodySprite = currentBody.bodySprite;
			displayName = currentBody.displayName;
			bodySpriteRenderer.color = BodyTint;
			faceSpriteRenderer.sprite = GetRandomFaceSprite();
			Refresh();
		}

		[Button]
		public void ApplyRandomFace()
		{
			faceSpriteRenderer.sprite = GetRandomFaceSprite();
			Debug.Log(faceSpriteRenderer.sprite.name + " applied to " + gameObject.name);
			Refresh();
		}

		Sprite GetRandomFaceSprite() => fruitFaces.GetRandom();

		[Button]
		public void ApplyRandomCharacter()
		{
			currentBody = fruitBodies.GetRandom();
			//Debug.Log("apply random character " + currentBody.name);
			ApplyCharacter();
		}

		void Start()
		{
			Invoke(nameof(ApplyCharacter), 1);
			Invoke(nameof(ApplyCharacter), 2);
		}

		void Update()
		{
			if (!Application.isPlaying && autoRefresh) Refresh();
		}

		[Button]
		void Refresh()
		{
			foreach (var spriteRenderer in ToTint)
			{
				if (spriteRenderer.CompareTag("dontcolor"))
				{
					spriteRenderer.color = Color.white;
					continue;
				}

				spriteRenderer.color = Tint;

			}

			bodySpriteRenderer.color = BodyTint;
		}

		[Button]
		public void GatherRenderers()
		{
			ToTint.Clear();
			var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
			foreach (var spriteRenderer in spriteRenderers)
			{
				if (spriteRenderer.CompareTag("dontcolor"))
				{
					spriteRenderer.color = Color.white;
					continue;
				}

				spriteRenderer.color = Tint;
				ToTint.Add(spriteRenderer);
			}
		}



#if UNITY_EDITOR
		[Button]
		public void CreateBodyData()
		{
			MyEditorUtilities.EnsureFolderExists(DefaultFolder);

			var asset = ScriptableObject.CreateInstance<GenericBody>();

			var path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(DefaultFolder, $"{displayName}.asset"));

			asset.leftArmOffset = leftArm.transform.localPosition;
			asset.leftLegOffset = leftLeg.transform.localPosition;
			asset.rightArmOffset = rightArm.transform.localPosition;
			asset.rightLegOffset = rightLeg.transform.localPosition;
			asset.faceOffset = face.transform.localPosition;
			asset.tintColor = leftArm.GetComponentInChildren<SpriteRenderer>().color;
			asset.bodySprite = bodySpriteRenderer.sprite;
			asset.displayName = displayName;

			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
			currentBody = asset;
		}
#endif


		public void SetPlayer(Player newPlayer)
		{

			ApplyRandomCharacter();
		}
	}
}
