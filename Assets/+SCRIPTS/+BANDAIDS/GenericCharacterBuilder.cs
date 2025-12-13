using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


namespace __SCRIPTS
{
	[ExecuteAlways]
	public class GenericCharacterBuilder : MonoBehaviour
	{
		public string NPCName;
		public Color Tint = Color.white;
		public List< SpriteRenderer > toTint = new();
		public GenericCharacter currentCharacter;
		public bool autoRefresh = true;
		public SpriteRenderer sr;
		public GameObject leftArm;
		 public GameObject rightArm;
		 public GameObject leftLeg;
		 public GameObject rightLeg;
		 public GameObject face;

		public HideRevealObjects hideRevealObjects;

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void OnEnable()
		{
			Refresh();
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
            currentCharacter.tintColor = leftArm.GetComponentInChildren<SpriteRenderer>().color;
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
			Refresh();
		}

		void Update()
		{
			if(!Application.isPlaying && autoRefresh) Refresh();
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

		private const string DefaultFolder = "Assets/Resources/GenericCharacters";


		[Button]
		public void CreateCharacterData()
		{
			EnsureFolderExists(DefaultFolder);

			GenericCharacter asset = ScriptableObject.CreateInstance<GenericCharacter>();

			string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(DefaultFolder, $"{NPCName}.asset"));

			asset.displayName  = NPCName;
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

		private static void EnsureFolderExists(string path)
		{
			if (AssetDatabase.IsValidFolder(path))
				return;

			string parent = "Assets";
			string[] parts = path.Replace("Assets/", "").Split('/');

			foreach (string part in parts)
			{
				string current = Path.Combine(parent, part);
				if (!AssetDatabase.IsValidFolder(current))
					AssetDatabase.CreateFolder(parent, part);

				parent = current;
			}
		}
	}
}
