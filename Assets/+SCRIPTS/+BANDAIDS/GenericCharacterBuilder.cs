using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace __SCRIPTS
{
	[ExecuteAlways]
	public class GenericCharacterBuilder : MonoBehaviour
	{
		public string NPCName;
		public Color Tint = Color.white;
		public Color BodyTint = Color.white;
		public List<SpriteRenderer> toTint;
		public GenericCharacter currentCharacter;
		public bool autoRefresh = true;
		public SpriteRenderer sr;
		public GameObject leftArm;
		public GameObject rightArm;
		public GameObject leftLeg;
		public GameObject rightLeg;
		public GameObject body;
		public GameObject face;

		public HideRevealObjects hideRevealObjects;

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void OnEnable()
		{
			Refresh();
		}

		void Start()
		{
			ApplyRandomCharacter();
		}

		[Button]
		public void SaveOverCurrentCharacter()
		{
			if (currentCharacter == null)
			{
				Debug.LogWarning("No GenericCharacter assigned to save over.");
				return;
			}

			// Record undo for safety
			Undo.RecordObject(currentCharacter, "Save GenericCharacter");

			currentCharacter.displayName = NPCName;
			currentCharacter.leftArmOffset = leftArm.transform.localPosition;
			currentCharacter.leftLegOffset = leftLeg.transform.localPosition;
			currentCharacter.rightArmOffset = rightArm.transform.localPosition;
			currentCharacter.rightLegOffset = rightLeg.transform.localPosition;
			currentCharacter.faceOffset = face.transform.localPosition;
			currentCharacter.faceIndex = hideRevealObjects.GetCurrentIndex();
			currentCharacter.tintColor = Tint;
			currentCharacter.bodyTintColor = BodyTint;
			currentCharacter.sprite = sr.sprite;


			// Mark dirty & save
			EditorUtility.SetDirty(currentCharacter);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Debug.Log($"Saved changes to {currentCharacter.name}");
		}

		[Button]
		public void ApplyCharacter()
		{
			Tint = currentCharacter.tintColor;
			leftArm.transform.localPosition = currentCharacter.leftArmOffset;
			rightArm.transform.localPosition = currentCharacter.rightArmOffset;
			leftLeg.transform.localPosition = currentCharacter.leftLegOffset;
			rightLeg.transform.localPosition = currentCharacter.rightLegOffset;
			face.transform.localPosition = currentCharacter.faceOffset;
			sr.sprite = currentCharacter.sprite;
			hideRevealObjects.Set(currentCharacter.faceIndex);
			BodyTint = currentCharacter.bodyTintColor;
			sr.color = BodyTint;
			NPCName = currentCharacter.displayName;
			Refresh();
		}

		[Button]
		public void ApplyRandomCharacter()
		{
			var fruitCharacters = Resources.LoadAll<GenericCharacter>("GenericCharacters").ToList();
			currentCharacter = fruitCharacters.GetRandom();
			ApplyCharacter();
			Refresh();
		}

		void Update()
		{
			if (!Application.isPlaying && autoRefresh) Refresh();
		}

		[Button]
		void Refresh()
		{
			foreach (var spriteRenderer in toTint)
			{
				if (spriteRenderer.CompareTag("dontcolor"))
				{
					spriteRenderer.color = Color.white;
					continue;
				}

				spriteRenderer.color = Tint;

			}

			sr.color = BodyTint;
		}

		[Button]
		public void GatherRenderers()
		{
			toTint.Clear();
			var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
			foreach (var spriteRenderer in spriteRenderers)
			{
				if (spriteRenderer.CompareTag("dontcolor"))
				{
					spriteRenderer.color = Color.white;
					continue;
				}

				spriteRenderer.color = Tint;
				toTint.Add(spriteRenderer);
			}
		}

		const string DefaultFolder = "Assets/Resources/GenericCharacters";

		[Button]
		public void CreateCharacterData()
		{
			MyEditorUtilities.EnsureFolderExists(DefaultFolder);

			var asset = ScriptableObject.CreateInstance<GenericCharacter>();

			var path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(DefaultFolder, $"{NPCName}.asset"));

			asset.displayName = NPCName;
			asset.leftArmOffset = leftArm.transform.localPosition;
			asset.leftLegOffset = leftLeg.transform.localPosition;
			asset.rightArmOffset = rightArm.transform.localPosition;
			asset.rightLegOffset = rightLeg.transform.localPosition;
			asset.faceOffset = face.transform.localPosition;
			asset.faceIndex = hideRevealObjects.GetCurrentIndex();
			asset.tintColor = leftArm.GetComponentInChildren<SpriteRenderer>().color;
			asset.sprite = sr.sprite;

			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
			currentCharacter = asset;
		}
	}
}
#endif
