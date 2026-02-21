using TMPro;
using UnityEngine;


namespace __SCRIPTS
{
	public class TextScrollEffect : MonoBehaviour
	{
		[Header("Scroll Settings")] public Texture2D scrollTexture;
		public float scrollSpeed = 0.5f;
		public Vector2 scrollDirection = new Vector2(0, -1);


		TextMeshProUGUI textComponent;
		Material material;


		void Awake()
		{
			textComponent = GetComponent<TextMeshProUGUI>();


			// Create a new material instance
			material = new Material(Shader.Find("TextMeshPro/Scrolling Fill"));


			// Set the initial material properties
			material.SetTexture("_ScrollTexture", scrollTexture);
			material.SetFloat("_ScrollSpeed", scrollSpeed);
			material.SetVector("_ScrollDirection", new Vector4(scrollDirection.x, scrollDirection.y, 0, 0));


			// Assign material to the TextMeshProUGUI component
			textComponent.fontMaterial = material;
			textComponent.UpdateMeshPadding();
		}


		void Update()
		{
			// Update scrolling parameters if needed at runtime
			if (material != null)
			{
				material.SetFloat("_ScrollSpeed", scrollSpeed);
				material.SetVector("_ScrollDirection", new Vector4(scrollDirection.x, scrollDirection.y, 0, 0));
			}
		}
	}
}
